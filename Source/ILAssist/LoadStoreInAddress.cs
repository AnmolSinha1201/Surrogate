using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	internal static partial class ILHelpers
	{
		// TODO : Find Ldind for U8
        public static void LoadFromAddress(this ILGenerator IL, Type LoadType)
        {
			var opcode = 
				LoadType == typeof(sbyte) ? OpCodes.Ldind_I1 :
				LoadType == typeof(Int16) ? OpCodes.Ldind_I2 :
				LoadType == typeof(Int32) ? OpCodes.Ldind_I4 :
				LoadType == typeof(Int64) ? OpCodes.Ldind_I8 :

				LoadType == typeof(float) ? OpCodes.Ldind_R4 :
				LoadType == typeof(double) ? OpCodes.Ldind_R8 :

				LoadType == typeof(byte) ? OpCodes.Ldind_U1 :
				LoadType == typeof(UInt16) ? OpCodes.Ldind_U2 :
				LoadType == typeof(UInt32) ? OpCodes.Ldind_U4 :
				
				OpCodes.Ldind_Ref;

			IL.Emit(opcode);
        }

		// TODO : Find Stind's for U1, U2, U4, U8
		public static void StoreIntoAddress(this ILGenerator IL, Type StoreType)
        {
			var opcode = 
				StoreType == typeof(sbyte) ? OpCodes.Stind_I1 :
				StoreType == typeof(Int16) ? OpCodes.Stind_I2 :
				StoreType == typeof(Int32) ? OpCodes.Stind_I4 :
				StoreType == typeof(Int64) ? OpCodes.Stind_I8 :

				StoreType == typeof(float) ? OpCodes.Stind_R4 :
				StoreType == typeof(double) ? OpCodes.Stind_R8 :
				
				OpCodes.Stind_Ref;

			IL.Emit(opcode);
        }
	}
}