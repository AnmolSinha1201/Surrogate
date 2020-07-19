using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static void ApplyParameters(this ConstructorBuilder Constructor, ParameterInfo[] OriginalParameters)
		{
			for (var i = 0; i < OriginalParameters.Length; ++i) 
			{
				var parameter = OriginalParameters[i];
				var parameterBuilder = Constructor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);

				if ((parameter.Attributes & ParameterAttributes.HasDefault) != 0) 
					parameterBuilder.SetConstant(parameter.RawDefaultValue);

				foreach (var attribute in parameter.GetCustomAttributesData().ToCustomAttributeBuilder())
					parameterBuilder.SetCustomAttribute(attribute);
			}
		}
	}
}