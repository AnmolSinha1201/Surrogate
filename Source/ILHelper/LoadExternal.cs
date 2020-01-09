using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
		public static void LoadExternalType(this ILGenerator IL, Type ItemType)
		{
			IL.Emit(OpCodes.Ldtoken, ItemType);
			IL.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) }));
		}

		public static void LoadExternalMethodInfo(this ILGenerator IL, MethodInfo Method)
		{
			IL.Emit(OpCodes.Ldtoken, Method);
			IL.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) }));
		}
	}
}