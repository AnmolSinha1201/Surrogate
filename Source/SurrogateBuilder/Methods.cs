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

			var backingMethod = Builder.CreateBackingMethod(OriginalMethod);

			// il.Emit(OpCodes.Ldarg_2);
			// il.Emit(OpCodes.Ldind_I4);
			// var writeLine = typeof(System.Console).GetMethod(nameof(Console.WriteLine), new Type[] { typeof(int) });
			// il.Emit(OpCodes.Call, writeLine);

			il.LoadAttribute(OriginalMethod, AttributeInfo);
			

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldtoken, backingMethod);
			il.Emit(OpCodes.Call, Method.Of(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));

			var args = il.DeclareLocal(typeof(object[]));
			il.Emit(OpCodes.Ldc_I4, parameterTypes.Count());
			il.Emit(OpCodes.Newarr, typeof(object));
			il.Emit(OpCodes.Stloc, args);
			for (int i = 0; i < parameterTypes.Count(); i++)
			{
				il.Emit(OpCodes.Ldloc, args);
				il.Emit(OpCodes.Ldc_I4, i);
				
				if (parameterTypes[i].IsByRef || OriginalMethod.GetParameters()[i].IsOut)
				{
					il.Emit(OpCodes.Ldarg, i + 1);
					il.Emit(OpCodes.Ldind_I4);
				}
				else
					il.Emit(OpCodes.Ldarg, i + 1);
				if (parameterTypes[i].IsByRef || OriginalMethod.GetParameters()[i].IsOut)
				{
					il.Emit(OpCodes.Box, parameterTypes[i].GetElementType());
				}
				else
					il.Emit(OpCodes.Box, parameterTypes[i]);
				il.Emit(OpCodes.Stelem_Ref);
			}
			il.Emit(OpCodes.Ldloc, args);

			// il.Emit(OpCodes.Ldloc, args);
			// il.Emit(OpCodes.Ldc_I4_1);
			// il.Emit(OpCodes.Ldelem_Ref);
			// il.Emit(OpCodes.Unbox_Any, typeof(int));
			// il.Emit(OpCodes.Call, writeLine);

			var info = il.DeclareLocal(typeof(MethodSurrogateInfo));
			il.Emit(OpCodes.Newobj, typeof(MethodSurrogateInfo).GetConstructor(new Type[] { typeof(object), typeof(MethodInfo), typeof(object[]) }));
			il.Emit(OpCodes.Stloc, info);
			il.Emit(OpCodes.Ldloc, info);
			var interceptMethodInfo = AttributeInfo.GetType().GetMethod(nameof(IMethodSurrogate.InterceptMethod), new Type[] { typeof(MethodSurrogateInfo) });
			il.Emit(OpCodes.Call, interceptMethodInfo);


			for (int i = 0; i < parameterTypes.Count(); i++)
			{
				if (parameterTypes[i].IsByRef || OriginalMethod.GetParameters()[i].IsOut)
				{
					
					il.Emit(OpCodes.Ldarg, i + 1);
					il.Emit(OpCodes.Ldloc, args);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem_Ref);
					il.Emit(OpCodes.Unbox_Any, parameterTypes[i].GetElementType());
					il.Emit(OpCodes.Stind_I4);
				}

			}

			il.Emit(OpCodes.Ldloc, info);
			var retValInfo = typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue));
			il.Emit(OpCodes.Ldfld, retValInfo);
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

		private static void LoadAttribute(this ILGenerator IL, MethodInfo OriginalMethod, Attribute AttributeInfo)
		{
			IL.Emit(OpCodes.Ldtoken, OriginalMethod);
			IL.Emit(OpCodes.Call, Method.Of(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle))));
			IL.Emit(OpCodes.Ldtoken, AttributeInfo.GetType());
			IL.Emit(OpCodes.Call, Method.Of(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle))));
			IL.Emit(OpCodes.Call, Method.Of(() => Attribute.GetCustomAttribute(default(MemberInfo), default(Type))));
		}
	}

	static class Method {
    public static MethodInfo Of<TResult>(Expression<Func<TResult>> f) => ((MethodCallExpression) f.Body).Method;
    public static MethodInfo Of<T>(Expression<Action<T>> f) => ((MethodCallExpression) f.Body).Method;
    public static MethodInfo Of(Expression<Action> f) => ((MethodCallExpression) f.Body).Method;
}
}