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

			if (!BaseType.IsEligibleForSurrogate())
			{
				if (!Cache.ContainsKey(BaseType))
					Cache.AddOrUpdate(BaseType, BaseType, (key, oldValue) => BaseType);

				return Activator.CreateInstance(BaseType, Params);
			}


			var builder = BaseType.ToTypeBuilder();
			var methods = BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			foreach (var method in methods)
				builder.OverrideMethod(method);

			var generatedType = builder.CreateType();
			
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
				il.ArrayToArguments(array, Method);

				// Stack still has either null (for void) or default(ReturnType)
				if (Method.ReturnType != typeof(void))
					il.Emit(OpCodes.Unbox_Any, Method.ReturnType);
				else
					il.Emit(OpCodes.Pop);
					
				il.Emit(OpCodes.Ret);
			});
		}

		public static void WriteAssemblyToDisk(this Type BaseType, string FileName)
		{
			var generator = new Lokad.ILPack.AssemblyGenerator();
			generator.GenerateAssembly(Assembly.GetAssembly(BaseType), FileName);
		}
	}
}