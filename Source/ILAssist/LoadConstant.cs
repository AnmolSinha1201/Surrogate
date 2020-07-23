using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	internal static partial class ILHelpers
	{    
        private static readonly OpCode[] LoadConstantInt32OpCodes =
        {
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4_1,
            OpCodes.Ldc_I4_2,
            OpCodes.Ldc_I4_3,
            OpCodes.Ldc_I4_4,
            OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6,
            OpCodes.Ldc_I4_7,
            OpCodes.Ldc_I4_8
        };

		public static void LoadConstantInt32(this ILGenerator IL, int Number)
        {
            if (Number >= 0 && Number <= 8)
                IL.Emit(LoadConstantInt32OpCodes[Number]);
            else
                IL.Emit(OpCodes.Ldc_I4, Number);
        }   
	}
}