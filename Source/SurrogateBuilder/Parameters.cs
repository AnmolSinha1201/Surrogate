using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static LocalBuilder CreateParameterProxy(this ILGenerator IL, MethodInfo OriginalMethod)
		{
			var parameters = OriginalMethod.GetParameters();
			var ILParameters = IL.CreateParametersArray(OriginalMethod);
			var ILArguments = IL.CreateArgumentsArray(OriginalMethod);

			for (int i = 0; i < parameters.Count(); i++)
			{
				if (!parameters[i].EligibleParameterProxy())
					continue;

				IL.ResolveArguments(parameters[i], ILParameters, ILArguments, i);
			}

			return ILArguments.Address;
		}

		private static bool EligibleParameterProxy(this ParameterInfo PInfo)
		{
			var attributes = PInfo.GetCustomAttributes();
			foreach (var attribute in attributes)
			{
				if (typeof(IParameterSurrogate).IsAssignableFrom(attribute.GetType()))
					return true;
			}

			return false;
		}

		private static void ResolveArguments(this ILGenerator IL, ParameterInfo PInfo, ILArray ILParameters, ILArray ILArguments, int Index)
		{
			var ILAttributes = IL.ILLoadAttributes<IParameterSurrogate>(ILParameters.ElementAtIL(Index));
			var attributes = Attribute.GetCustomAttributes(PInfo);

			for (int i = 0; i < attributes.Count(); i++)
			{
				if (!typeof(IParameterSurrogate).IsAssignableFrom(attributes[i].GetType()))
					continue;
				
				// Attribute.InterceptParameter(ParameterSurrogateInfo)
				ILAttributes.LoadElementAt(i);
				var info = IL.CreateParameterSurrogateInfo(ILParameters, ILArguments, Index);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Call, attributes[i].GetType().GetMethod(nameof(IParameterSurrogate.InterceptParameter), new[] { typeof(ParameterSurrogateInfo) }));
				
				ILArguments.StoreElementAt(Index, () =>
				{
					IL.Emit(OpCodes.Ldloc, info);
					IL.Emit(OpCodes.Ldfld, typeof(ParameterSurrogateInfo).GetField(nameof(ParameterSurrogateInfo.ParamValue)));
				});
			}
		}

		private static ILArray CreateParametersArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILParameters = IL.CreateArray<ParameterInfo>(() =>
			{
				IL.LoadExternalMethodInfo(Method);
				IL.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod(nameof(MethodInfo.GetParameters)));
			});

			return ILParameters;
		}

		private static ILArray CreateArgumentsArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILArguments = IL.CreateArray<object>(parameters.Count(), (i) =>
			{
				IL.LoadArgument(i, parameters[i]);
				IL.Emit(OpCodes.Box, parameters[i].ActualParameterType());
			});
			
			return ILArguments;
		}

		private static LocalBuilder CreateParameterSurrogateInfo(this ILGenerator IL, ILArray ILParameters, ILArray ILArguments, int Index)
		{
			ILParameters.LoadElementAt(Index);
			ILArguments.LoadElementAt(Index);

			var info = IL.CreateExternalType(typeof(ParameterSurrogateInfo), new[] { typeof(ParameterInfo), typeof(object) });
			return info;
		}
	}
}