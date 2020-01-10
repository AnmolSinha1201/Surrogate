using System;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
        /// <summary>
		/// <param name="ArrayType">Must be base type like typeof(object) instead of typeof(object[])</para>
		/// </summary>
		public static LocalBuilder CreateArray(this ILGenerator IL, Type ArrayType, int ArraySize)
		{
			var array = IL.DeclareLocal(ArrayType.MakeArrayType());

			IL.LoadConstantInt32(ArraySize);
			IL.Emit(OpCodes.Newarr, ArrayType);
			IL.Emit(OpCodes.Stloc, array);

			return array;
		}

		public static void LoadValueAtArrayIndex(this ILGenerator IL, LocalBuilder LocalArray, int Index)
		{
			IL.Emit(OpCodes.Ldloc, LocalArray);
			IL.LoadConstantInt32(Index);
			IL.Emit(OpCodes.Ldelem_Ref);
		}
	}
}