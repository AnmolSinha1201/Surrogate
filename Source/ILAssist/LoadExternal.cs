using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	internal static partial class ILHelpers
	{
		public static void LoadExternalMethodInfo(this ILGenerator IL, MethodInfo Method)
		{
			IL.Emit(OpCodes.Ldtoken, Method);
			IL.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) }));
		}
	}
}