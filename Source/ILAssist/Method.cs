using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static MethodBuilder CreateBackingMethod(this TypeBuilder Builder, MethodInfo OriginalMethod)
		{
			var parameters = OriginalMethod.GetParameters();

			MethodBuilder methodBuilder = Builder.DefineMethod(
				$"<{OriginalMethod.Name}>k__BackingMethod",
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameters.Select(i => i.ParameterType).ToArray()
			);

			// To copy attributes to backing method
			// methodBuilder.ApplyParameters(parameters);
			// foreach (var attribute in OriginalMethod.GetCustomAttributesData().ToCustomAttributeBuilder())
			// 	methodBuilder.SetCustomAttribute(attribute);
			// methodBuilder.ApplyParameterAt(OriginalMethod.ReturnParameter, 0);
			

			// base.OriginalMethod(args);
			ILGenerator il = methodBuilder.GetILGenerator();
			for (int i = 0; i <= parameters.Count(); i++)
				il.LoadArgument(i);
			il.Emit(OpCodes.Call, OriginalMethod);
			il.Emit(OpCodes.Ret);

			return methodBuilder;
		}
	}
}