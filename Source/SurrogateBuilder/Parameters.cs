using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.ILConstructs;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static ILArray CreateParameterProxy(this ILGenerator IL, MethodInfo OriginalMethod)
		{
			var parameters = OriginalMethod.GetParameters();
			var ILParameters = IL.CreateParametersArray(OriginalMethod);
			var ILArguments = IL.CreateArgumentsArray(OriginalMethod);

			for (int i = 0; i < parameters.Count(); i++)
			{
				if (!parameters[i].EligibleForParameterInterceptor())
					continue;

				var ILParameter = ILParameters.ElementAt(i);
				var ILArgument = ILArguments.ElementAt(i);
				IL.ResolveArgument(ILParameter, ILArgument);
				ILArguments.StoreElementAt(i, ILArgument);
			}

			return ILArguments;
		}

		private static bool EligibleForParameterInterceptor(this ParameterInfo PInfo)
		{
			var attributes = PInfo.GetCustomAttributes();
			foreach (var attribute in attributes)
			{
				if (typeof(IParameterSurrogate).IsAssignableFrom(attribute.GetType()))
					return true;
			}

			return false;
		}

		private static void ResolveArgument(this ILGenerator IL, ILVariable ILParameter, ILVariable ILArgument)
		{
			var ILAttributes = IL.ILLoadAttributes<IParameterSurrogate>(ILParameter);

			ILAttributes.ForEach((attribute) =>
			{
				// Attribute.InterceptParameter(ParameterSurrogateInfo)
				attribute.Load();
				var info = IL.CreateParameterSurrogateInfo(ILParameter, ILArgument);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Callvirt, typeof(IParameterSurrogate).GetMethod(nameof(IParameterSurrogate.InterceptParameter), new[] { typeof(ParameterSurrogateInfo) }));
				
				ILArgument.Store(() =>
				{
					IL.Emit(OpCodes.Ldloc, info);
					IL.Emit(OpCodes.Ldfld, typeof(ParameterSurrogateInfo).GetField(nameof(ParameterSurrogateInfo.Value)));
				});
			});
		}

		private static ILArray CreateParametersArray(this ILGenerator IL, MethodInfo Method)
		{
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

		private static LocalBuilder CreateParameterSurrogateInfo(this ILGenerator IL, ILVariable Parameter, ILVariable Argument)
		{
			Parameter.Load();
			Argument.Load();

			var info = IL.CreateExternalType(typeof(ParameterSurrogateInfo), new[] { typeof(ParameterInfo), typeof(object) });
			return info;
		}
	}
}