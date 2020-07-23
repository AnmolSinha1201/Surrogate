using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static ConcurrentDictionary<Type, Type> Cache = new ConcurrentDictionary<Type, Type>();

		public static T Build<T>(params object[] Params)
		=> (T)typeof(T).Build(Params);
		public static object Build(this Type BaseType, params object[] Params)
		{
			if (Cache.ContainsKey(BaseType))
				return Activator.CreateInstance(Cache[BaseType], Params);


			var builder = BaseType.ToTypeBuilder();
			var methods = BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			foreach (var method in methods)
				builder.OverrideMethod(method);

			var generatedType = builder.CreateType();
			
			// To make it truly thread safe, we check need to check if it already exists, and if it doesn't
			// add or update it, since multiple threads can add it at the same time, and it doesn't make sense
			// to throw exceptions in such cases.
			if (!Cache.ContainsKey(BaseType))
				Cache.AddOrUpdate(BaseType, generatedType, (key, oldValue) => generatedType);

			return Activator.CreateInstance(generatedType, Params);
		}

		private static void OverrideMethod(this TypeBuilder Builder, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			Builder.OverrideMethod(Method, il =>
			{
				var backingMethod = Builder.CreateBackingMethod(Method);

				il.LoadExternalMethodInfo(backingMethod);
				il.Emit(OpCodes.Ldarg_0);
				il.LoadExternalMethodInfo(Method);

				var array = il.ArgumentsToArray(Method);
				il.Emit(OpCodes.Ldloc, array.Address);

				il.Emit(OpCodes.Call, typeof(Extensions).GetMethod(nameof(Extensions.SurrogateHook)));
				var returnValue = il.DeclareLocal(Method.ReturnType);
				il.Emit(OpCodes.Stloc, returnValue);


				il.ArrayToArguments(array, Method);


				il.Emit(OpCodes.Ldloc, returnValue);
				il.Emit(OpCodes.Unbox_Any, Method.ReturnType);
				il.Emit(OpCodes.Ret);
			});
		}
	}
}