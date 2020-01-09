using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
        public static void Box(this ILGenerator IL, ParameterInfo Info)
        {
			if (Info.IsByRefOrOut())
				IL.Emit(OpCodes.Box, Info.ParameterType.GetElementType());
			else
				IL.Emit(OpCodes.Box, Info.ParameterType);
        }

		public static void Unbox(this ILGenerator IL, ParameterInfo Info)
        {
			if (Info.IsByRefOrOut())
				IL.Unbox(Info.ParameterType.GetElementType());
			else
				IL.Unbox(Info.ParameterType);
        }

		public static void Unbox(this ILGenerator IL, Type ItemType)
		=> IL.Emit(OpCodes.Unbox_Any, ItemType);
	}
}