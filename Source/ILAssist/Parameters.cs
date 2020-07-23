using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;
using Surrogate.Internal.ILConstructs;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
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

		internal static ILArray CreateArgumentsArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILArguments = IL.CreateArray<object>(parameters.Count(), (i) =>
			{
				IL.LoadArgument(i + 1, parameters[i]);
				IL.Emit(OpCodes.Box, parameters[i].ActualParameterType());
			});
			
			return ILArguments;
		}

		internal static bool IsByRefOrOut(this ParameterInfo Info)
		=> Info.IsOut || Info.ParameterType.IsByRef;

		internal static Type ActualParameterType(this ParameterInfo Info)
		{
			if (Info.IsByRefOrOut())
				return Info.ParameterType.GetElementType();
			return Info.ParameterType;
		}

		public static void LoadArgument(this ILGenerator IL, int Index, ParameterInfo Info)
		{
			IL.LoadArgument(Index);
			if (Info.IsByRefOrOut())
				IL.LoadFromAddress(Info.ParameterType);
		}
	}
}