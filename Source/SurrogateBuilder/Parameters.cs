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
				IL.ResolveArguments(parameters[i], ILParameters, ILArguments, i);
			}

			return ILArguments;
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

		private static void ResolveArguments(this ILGenerator IL, ParameterInfo PInfo, LocalBuilder ILParameterVariable, LocalBuilder ILArgumentsVariable, int Index)
		{
			if (!PInfo.EligibleParameterProxy())
				return;

			var ILAttributes = IL.LoadAttributesFromParameter(ILParameterVariable, Index);
			var attributes = Attribute.GetCustomAttributes(PInfo);

			for (int i = 0; i < attributes.Count(); i++)
			{
				if (!typeof(IParameterSurrogate).IsAssignableFrom(attributes[i].GetType()))
					continue;
				
				// Attribute.InterceptParameter(ParameterSurrogateInfo)
				IL.LoadValueAtArrayIndex(ILAttributes, i);
				var info = IL.CreateParameterSurrogateInfo(PInfo, ILParameterVariable, ILArgumentsVariable, Index);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Call, attributes[i].GetType().GetMethod(nameof(IParameterSurrogate.InterceptParameter), new[] { typeof(ParameterSurrogateInfo) }));
				
				IL.Emit(OpCodes.Ldloc, ILArgumentsVariable);
				IL.LoadConstantInt32(Index);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Ldfld, typeof(ParameterSurrogateInfo).GetField(nameof(ParameterSurrogateInfo.ParamValue)));
				// IL.Emit(OpCodes.Box, PInfo.ActualParameterType());
				IL.Emit(OpCodes.Stelem_Ref);
			}
		}

		private static LocalBuilder CreateParametersArray(this ILGenerator IL, MethodInfo Method)
		{
			var ILParameters = IL.DeclareLocal(typeof(ParameterInfo[]));

			IL.LoadExternalMethodInfo(Method);
			IL.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod(nameof(MethodInfo.GetParameters)));
			IL.Emit(OpCodes.Stloc, ILParameters);

			return ILParameters;
		}

		private static LocalBuilder CreateArgumentsArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILArguments = IL.CreateArray(typeof(object), parameters.Count());

			for (int i = 0; i < parameters.Count(); i++)
			{
				IL.Emit(OpCodes.Ldloc, ILArguments);
				IL.LoadConstantInt32(i);
				IL.LoadArgument(i, parameters[i]);
				
				IL.Emit(OpCodes.Box, parameters[i].ActualParameterType());
				IL.Emit(OpCodes.Stelem_Ref);
			}
			

			return ILArguments;
		}

		private static LocalBuilder LoadAttributesFromParameter(this ILGenerator IL, LocalBuilder ParameterAddress, int ParameterIndex)
		{
			IL.LoadValueAtArrayIndex(ParameterAddress, ParameterIndex);

			IL.Emit(OpCodes.Call, typeof(Attribute).GetMethod(nameof(Attribute.GetCustomAttributes), new [] { typeof(ParameterInfo) }));
			var localAttributes = IL.DeclareLocal(typeof(IParameterSurrogate));
			IL.Emit(OpCodes.Stloc, localAttributes);

			return localAttributes;
		}

		private static LocalBuilder CreateParameterSurrogateInfo(this ILGenerator IL, ParameterInfo PInfo, LocalBuilder ILParameterVariable, LocalBuilder ILArgumentsVariable, int Index)
		{
			IL.LoadValueAtArrayIndex(ILParameterVariable, Index);
			IL.LoadValueAtArrayIndex(ILArgumentsVariable, Index);

			var info = IL.CreateExternalType(typeof(ParameterSurrogateInfo), new[] { typeof(ParameterInfo), typeof(object) });
			return info;
		}
	}
}