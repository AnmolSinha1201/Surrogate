using System;
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
			var itemAttribute = (MethodSurrogate)OriginalMethod.GetCustomAttribute(typeof(MethodSurrogate));
			if (itemAttribute == null)
				return;

			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				Type.EmptyTypes
			);

			ILGenerator il = methodBuilder.GetILGenerator();


			// this.OriginalMethod()
			// il.LoadThis();
			// il.Call(OriginalMethod);

			il.Emit(OpCodes.Ldtoken, OriginalMethod);
			il.Emit(OpCodes.Call, Method.Of(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));
			il.Emit(OpCodes.Ldtoken, typeof(MethodSurrogate));
			il.Emit(OpCodes.Call, Method.Of(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle))));
			il.Emit(OpCodes.Call, Method.Of(() => Attribute.GetCustomAttribute(default(MemberInfo), default(Type))));
			

			il.Emit(OpCodes.Ldtoken, typeof(MethodSurrogateInfo));
			il.Emit(OpCodes.Call, Method.Of(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle))));
			il.Emit(OpCodes.Call, Method.Of(() => Activator.CreateInstance(default(Type))));


			il.Emit(OpCodes.Callvirt, Method.Of((MethodSurrogate a) => a.Process(default(MethodSurrogateInfo))));

			
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, OriginalMethod);


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