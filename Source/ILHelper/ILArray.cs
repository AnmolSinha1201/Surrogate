using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	public class ILArray
	{
		public ILGenerator IL;
		public Type BaseType, ArrayType;
		public int Size;
		public LocalBuilder Address;
		public object CorrespondingObject;

		public void LoadElementAt(int Index)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.LoadConstantInt32(Index);
			IL.Emit(OpCodes.Ldelem_Ref);
		}

		public void StoreElementAt(int Index, Action ILAction)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.LoadConstantInt32(Index);
			ILAction();
			IL.Emit(OpCodes.Stelem_Ref);
		}
	}

	internal static partial class ILHelpers
	{
		public  static ILArray CreateArray<T>(this ILGenerator IL, T[] CorrespondingObject, bool Initialize = false)
		{
			var array = new ILArray();

			array.IL = IL;
			array.BaseType = typeof(T);
			array.ArrayType = array.BaseType.MakeArrayType();
			array.Size = CorrespondingObject.Length;
			array.CorrespondingObject = CorrespondingObject;

			array.Address = IL.DeclareLocal(array.ArrayType);
			
			if (Initialize)
			{
				IL.LoadConstantInt32(array.Size);
				IL.Emit(OpCodes.Newarr, array.BaseType);
				IL.Emit(OpCodes.Stloc, array.Address);
			}

			return array;
		}

		public static ILArray CreateArray<T>(this ILGenerator IL, T[] CorrespondingObject, Action AddressAction)
		{
			var array = IL.CreateArray<T>(CorrespondingObject, false);

			AddressAction();
			IL.Emit(OpCodes.Stloc, array.Address);

			return array;
		}

		public static ILArray CreateArray<T>(this ILGenerator IL, T[] CorrespondingObject, Action<int> AddressAction)
		{
			var array = IL.CreateArray<T>(CorrespondingObject, true);
			
			for (int i = 0; i < array.Size; i++)
			{
				IL.Emit(OpCodes.Ldloc, array.Address);
				IL.LoadConstantInt32(i);
				AddressAction(i);
				IL.Emit(OpCodes.Stelem_Ref);
			}

			return array;
		}
	}
}