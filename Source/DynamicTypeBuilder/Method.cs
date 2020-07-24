using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static void OverrideMethod(this TypeBuilder Builder, MethodInfo OriginalMethod, Action<ILGenerator> BodyAction)
		{
			var parameterTypes = OriginalMethod.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);
			
			ILGenerator il = methodBuilder.GetILGenerator();
			BodyAction(il);

			Builder.DefineMethodOverride(methodBuilder, OriginalMethod);
		}
	}
}