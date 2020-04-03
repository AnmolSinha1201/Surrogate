using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;
using Surrogate.Interfaces;
using Surrogate.Internal.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
		
		public static object Build(Type ItemType, params object[] Arguments)
		{
			if (Cache.ContainsKey(ItemType))
				return Activator.CreateInstance(Cache[ItemType]);

			var builder = ItemType.CreateTypeBuilder();
			// var method = ItemType.GetMethod(nameof(Foo.ActualMethod));
			var methods = ItemType.GetMethods();
			foreach (var method in methods)
			{
				if (!method.IsVirtual)
					continue;

				MethodWorkflowDispatch(builder, method);
			}

			var type = builder.CreateType();
			Cache.Add(ItemType, type);
			return Activator.CreateInstance(type, Arguments);
		}

		public static object Build<T>(params object[] Arguments)
		{
			return Build(typeof(T), Arguments);
		}

		private static void MethodWorkflowDispatch(TypeBuilder Builder, MethodInfo Method)
		{
			var methodAttributes = AttributeFinder.FindAttributes(Method, typeof(IMethodSurrogate));
			var parameterAttributes = AttributeFinder.FindAttributes(Method.GetParameters(), typeof(IParameterSurrogate));
			var returnAttributes = AttributeFinder.FindAttributes(Method, typeof(IReturnSurrogate));

			if (methodAttributes.Length == 0 && parameterAttributes.Length == 0 && returnAttributes.Length == 0)
				return;
			
			Builder.OverrideMethod(Method);
		}

		private static LocalBuilder OverrideMethod(this TypeBuilder Builder, MethodInfo Method)
		{
			var parameterTypes = Method.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				Method.Name,
				Method.Attributes,
				CallingConventions.HasThis,
				Method.ReturnType,
				parameterTypes
			);
			var backingMethod = Builder.CreateBackingMethod(Method);
			ILGenerator il = methodBuilder.GetILGenerator();
			var args = il.CreateParameterProxy(Method);
			
			var returnValue = il.CreateMethodInterceptor(Method, backingMethod, args);

			if (Method.ReturnType != null && Method.ReturnType != typeof(void))
			{
				il.CreateReturnProxy(Method, returnValue);

				il.Emit(OpCodes.Ldloc, returnValue);	
				il.Emit(OpCodes.Unbox_Any, Method.ReturnType);
			}
			il.Emit(OpCodes.Ret);

			Builder.DefineMethodOverride(methodBuilder, Method);
			return returnValue;
		}
	}
}