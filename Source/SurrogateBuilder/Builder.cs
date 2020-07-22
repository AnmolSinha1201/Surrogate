using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;
using Surrogate.ILAssist;
using Surrogate.Interfaces;
using Surrogate.Internal.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();

		public static object Build(Type BaseType, params object[] Params)
		{
			var builder = BaseType.ToTypeBuilder();
			var methods = BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			foreach (var method in methods)
				builder.OverrideMethod(method);

			return Activator.CreateInstance(builder.CreateType(), Params);
		}

		internal static void OverrideMethod(this TypeBuilder Builder, MethodInfo Method)
		{
			Builder.OverrideMethod(Method, il =>
			{
				var backingMethod = Builder.CreateBackingMethod(Method);

				il.LoadExternalMethodInfo(backingMethod);
				il.Emit(OpCodes.Ldarg_0);
				il.LoadExternalMethodInfo(Method);

				var array = il.CreateArgumentsArray(Method);
				il.Emit(OpCodes.Ldloc, array.Address);

				il.Emit(OpCodes.Call, typeof(Extensions).GetMethod(nameof(Extensions.SurrogateHook)));
				il.Emit(OpCodes.Unbox_Any, Method.ReturnType);
				il.Emit(OpCodes.Ret);
			});
		}
	}
}