using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
        public static void Box(this ILGenerator IL, ParameterInfo Info)
        {
			if (Info.ParameterType.IsByRef || Info.IsOut)
				IL.Emit(OpCodes.Box, Info.ParameterType.GetElementType());
			else
				IL.Emit(OpCodes.Box, Info.ParameterType);
        }

		public static void Unbox(this ILGenerator IL, ParameterInfo Info)
        {
			if (Info.ParameterType.IsByRef || Info.IsOut)
				IL.Emit(OpCodes.Unbox_Any, Info.ParameterType.GetElementType());
			else
				IL.Emit(OpCodes.Box, Info.ParameterType);
        }
	}
}