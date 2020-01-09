using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
        public static void Box(this ILGenerator IL, Type ItemType)
		=> IL.Emit(OpCodes.Box, ItemType);

		public static void Unbox(this ILGenerator IL, Type ItemType)
		=> IL.Emit(OpCodes.Unbox_Any, ItemType);
	}
}