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

		private static LocalBuilder CreateParameterProxy(this MethodBuilder Builder, MethodInfo OriginalMethod)
		{
			var il = Builder.GetILGenerator();
			var parameters = OriginalMethod.GetParameters();

			var argsArray = il.CreateArray(typeof(object), parameters.Count());
			var ILParameters = il.CreateParametersArray(OriginalMethod);

			// argsArray[i] = Args[i]
			for (int i = 0; i < parameters.Count(); i++)
			{
				il.Emit(OpCodes.Ldloc, argsArray);
				il.LoadConstantInt32(i);
				
				if (parameters[i].EligibleParameterProxy())
				{
					var localAttributes = il.LoadAttributesFromParameter(ILParameters, i);
					var attributes = Attribute.GetCustomAttributes(parameters[i]);

					for (int o = 0; o < attributes.Count(); o++)
					{
						if (!typeof(IParameterSurrogate).IsAssignableFrom(attributes[o].GetType()))
							continue;
						
						il.LoadValueAtArrayIndex(localAttributes, o);
						il.LoadValueAtArrayIndex(ILParameters, i);

						il.LoadArgument(i);
						if (parameters[i].IsByRefOrOut())
							il.LoadFromAddress(parameters[i].ParameterType);
						il.Emit(OpCodes.Box, parameters[i].ActualParameterType());
						
						var info = il.CreateExternalType(typeof(ParameterSurrogateInfo), new[] { typeof(ParameterInfo), typeof(object) });
						il.Emit(OpCodes.Ldloc, info);

						il.Emit(OpCodes.Call, attributes[o].GetType().GetMethod(nameof(IParameterSurrogate.InterceptParameter), new[] { typeof(ParameterSurrogateInfo) }));
						
						il.Emit(OpCodes.Ldloc, info);
						il.Emit(OpCodes.Ldfld, typeof(ParameterSurrogateInfo).GetField(nameof(ParameterSurrogateInfo.ParamValue)));
					}
				}
				else
				{
					il.LoadArgument(i);
					if (parameters[i].IsByRefOrOut())
						il.LoadFromAddress(parameters[i].ParameterType);
				}
				
				il.Emit(OpCodes.Box, parameters[i].ActualParameterType());
				il.Emit(OpCodes.Stelem_Ref);
			}

			return argsArray;
		}

		private static LocalBuilder CreateParametersArray(this ILGenerator IL, MethodInfo Method)
		{
			var ILParameters = IL.DeclareLocal(typeof(ParameterInfo[]));

			IL.LoadExternalMethodInfo(Method);
			IL.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod(nameof(MethodInfo.GetParameters)));
			IL.Emit(OpCodes.Stloc, ILParameters);

			return ILParameters;
		}

		private static LocalBuilder LoadAttributesFromParameter(this ILGenerator IL, LocalBuilder ParameterAddress, int ParameterIndex)
		{
			IL.LoadValueAtArrayIndex(ParameterAddress, ParameterIndex);

			IL.Emit(OpCodes.Call, typeof(Attribute).GetMethod(nameof(Attribute.GetCustomAttributes), new [] { typeof(ParameterInfo) }));
			var localAttributes = IL.DeclareLocal(typeof(IParameterSurrogate));
			IL.Emit(OpCodes.Stloc, localAttributes);

			return localAttributes;
		}

		/// <summary>
		/// <param name="ArrayType">Must be base type like typeof(object) instead of typeof(object[])</para>
		/// </summary>
		private static LocalBuilder CreateArray(this ILGenerator IL, Type ArrayType, int ArraySize)
		{
			var array = IL.DeclareLocal(ArrayType.MakeArrayType());

			IL.LoadConstantInt32(ArraySize);
			IL.Emit(OpCodes.Newarr, ArrayType);
			IL.Emit(OpCodes.Stloc, array);

			return array;
		}

		private static void LoadValueAtArrayIndex(this ILGenerator IL, LocalBuilder LocalArray, int Index)
		{
			IL.Emit(OpCodes.Ldloc, LocalArray);
			IL.LoadConstantInt32(Index);
			IL.Emit(OpCodes.Ldelem_Ref);
		}
	}
}