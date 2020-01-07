using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Base;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static void CreateMethodProxy(this TypeBuilder Builder, MethodInfo OriginalMethod)
		{
			var parameterTypes = OriginalMethod.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);

			ILGenerator il = methodBuilder.GetILGenerator();

			MethodBuilder methodBuilder2 = Builder.DefineMethod(
				"Hidden",
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);

			ILGenerator il2 = methodBuilder2.GetILGenerator();
			il2.Emit(OpCodes.Ldarg_0);
			il2.Emit(OpCodes.Ldarg_1);
			il2.Emit(OpCodes.Call, OriginalMethod);
			il2.Emit(OpCodes.Ret);


			il.Emit(OpCodes.Ldtoken, OriginalMethod);
			il.Emit(OpCodes.Call, Method.Of(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));
			il.Emit(OpCodes.Ldtoken, typeof(MethodSurrogate));
			il.Emit(OpCodes.Call, Method.Of(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle))));
			il.Emit(OpCodes.Call, Method.Of(() => Attribute.GetCustomAttribute(default(MemberInfo), default(Type))));
			

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldtoken, methodBuilder2);
			il.Emit(OpCodes.Call, Method.Of(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));

			var args = il.DeclareLocal(typeof(object[]));
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Newarr, typeof(object));
			il.Emit(OpCodes.Stloc, args);
			il.Emit(OpCodes.Ldloc, args);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Box, typeof(int));
			il.Emit(OpCodes.Stelem_Ref);
			il.Emit(OpCodes.Ldloc, args);

			var info = il.DeclareLocal(typeof(MethodSurrogateInfo));
			il.Emit(OpCodes.Newobj, typeof(MethodSurrogateInfo).GetConstructor(new Type[] { typeof(object), typeof(MethodInfo), typeof(object[]) }));
			il.Emit(OpCodes.Stloc, info);
			il.Emit(OpCodes.Ldloc, info);
			il.Emit(OpCodes.Call, Method.Of((MethodSurrogate a) => a.Process(default(MethodSurrogateInfo))));
			// il.Emit(OpCodes.Unbox_Any, OriginalMethod.ReturnType);
			il.Emit(OpCodes.Ldloc, info);
			var retValInfo = typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue));
			il.Emit(OpCodes.Ldfld, retValInfo);
			il.Emit(OpCodes.Unbox_Any, OriginalMethod.ReturnType);


			
			// il.Emit(OpCodes.Ldarg_0);
			// il.Emit(OpCodes.Call, OriginalMethod);


			// return to terminate
			il.Emit(OpCodes.Ret);

			Builder.DefineMethodOverride(methodBuilder, OriginalMethod);
			
		}
	}

	static class Method {
    public static MethodInfo Of<TResult>(Expression<Func<TResult>> f) => ((MethodCallExpression) f.Body).Method;
    public static MethodInfo Of<T>(Expression<Action<T>> f) => ((MethodCallExpression) f.Body).Method;
    public static MethodInfo Of(Expression<Action> f) => ((MethodCallExpression) f.Body).Method;
}
}