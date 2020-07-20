using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;

namespace Surrogate.ILAssist
{
	public class ILMethod
	{
		MethodInfo Base;

		public ILMethod(MethodInfo Info)
		{ 
			this.Base = Info;
		}
	}

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
			
			// base.OriginalMethod(args);
			ILGenerator il = methodBuilder.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			for (int i = 0; i < parameters.Count(); i++)
				il.LoadArgument(i);
			il.Emit(OpCodes.Call, OriginalMethod);
			il.Emit(OpCodes.Ret);

			return methodBuilder;
		}
	}
}