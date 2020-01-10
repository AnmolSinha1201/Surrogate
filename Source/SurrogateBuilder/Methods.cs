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
		private static void CreateMethodProxy(this TypeBuilder Builder, MethodInfo OriginalMethod, Attribute AttributeInfo)
		{
			var parameterTypes = OriginalMethod.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);
			var backingMethod = Builder.CreateBackingMethod(OriginalMethod);
			ILGenerator il = methodBuilder.GetILGenerator();


			il.LoadExternalAttribute(OriginalMethod, AttributeInfo.GetType());
			il.Emit(OpCodes.Ldarg_0);
			il.LoadExternalMethodInfo(backingMethod);
			var args = methodBuilder.CreateParameterProxy(OriginalMethod);
			// var args = il.CreateArrayFromArgs(OriginalMethod);
			il.Emit(OpCodes.Ldloc, args);
			var info = il.CreateExternalType(typeof(MethodSurrogateInfo), new [] { typeof(object), typeof(MethodInfo), typeof(object[]) });
			
			il.Emit(OpCodes.Ldloc, info);
			il.Emit(OpCodes.Call, AttributeInfo.GetType().GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) }));

			il.CopyArrayToArgs(OriginalMethod, args);

			il.Emit(OpCodes.Ldloc, info);
			il.Emit(OpCodes.Ldfld, typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue)));
			il.Emit(OpCodes.Unbox_Any, OriginalMethod.ReturnType);
			il.Emit(OpCodes.Ret);

			Builder.DefineMethodOverride(methodBuilder, OriginalMethod);
		}

		private static MethodBuilder CreateBackingMethod(this TypeBuilder Builder, MethodInfo OriginalMethod)
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