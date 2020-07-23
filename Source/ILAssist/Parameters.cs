using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Internal.ILConstructs;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		private static readonly OpCode[] LoadArgsOpCodes =
		{
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};
		public static void LoadArgument(this ILGenerator Generator, int Index)
		{
			if (Index <= LoadArgsOpCodes.Length)
				Generator.Emit(LoadArgsOpCodes[Index]);
			else
				Generator.Emit(OpCodes.Ldarg, Index);
		}
		
		internal static void ApplyParameters(this ConstructorBuilder Constructor, ParameterInfo[] OriginalParameters)
		{
			for (var i = 0; i < OriginalParameters.Length; i++) 
			{
				var parameter = OriginalParameters[i];
				var parameterBuilder = Constructor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);

				if ((parameter.Attributes & ParameterAttributes.HasDefault) != 0) 
					parameterBuilder.SetConstant(parameter.RawDefaultValue);

				foreach (var attribute in parameter.GetCustomAttributesData().ToCustomAttributeBuilder())
					parameterBuilder.SetCustomAttribute(attribute);
			}
		}

		internal static void ApplyParameters(this MethodBuilder Builder, ParameterInfo[] OriginalParameters)
		{
			for (var i = 0; i < OriginalParameters.Length; i++) 
				Builder.ApplyParameterAt(OriginalParameters[i], i + 1);
		}

		/// <summary>
		/// <para>Index 0 is for return</para>
		/// <para>Index 1+ is for parameters</para>
		/// </summary>
		internal static void ApplyParameterAt(this MethodBuilder Builder, ParameterInfo Parameter, int Index)
		{
			var parameterBuilder = Builder.DefineParameter(Index, Parameter.Attributes, Parameter.Name);

			if ((Parameter.Attributes & ParameterAttributes.HasDefault) != 0) 
				parameterBuilder.SetConstant(Parameter.RawDefaultValue);

			foreach (var attribute in Parameter.GetCustomAttributesData().ToCustomAttributeBuilder())
				parameterBuilder.SetCustomAttribute(attribute);
		}


		internal static ILArray ArgumentsToArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILArguments = IL.CreateArray<object>(parameters.Count(), (i) =>
			{
				IL.LoadArgument(i + 1); // 0 not used here
				if (parameters[i].IsByRefOrOut())
					IL.LoadIndirect(parameters[i].ParameterType);

				IL.Emit(OpCodes.Box, parameters[i].ActualParameterType());
			});
			
			return ILArguments;
		}

		internal static void ArrayToArguments(this ILGenerator IL, ILArray Array, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].IsByRefOrOut())
					continue;

				IL.LoadArgument(i + 1);
				Array.LoadElementAt(i);
				IL.Emit(OpCodes.Unbox_Any, parameters[i].ActualParameterType());
				IL.StoreIndirect(parameters[i].ActualParameterType());
			}
		}

		internal static bool IsByRefOrOut(this ParameterInfo Info)
		=> Info.IsOut || Info.ParameterType.IsByRef;

		internal static Type ActualParameterType(this ParameterInfo Info)
		{
			if (Info.IsByRefOrOut())
				return Info.ParameterType.GetElementType();
			return Info.ParameterType;
		}
	}
}