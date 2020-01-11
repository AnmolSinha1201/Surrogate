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

		public static ILArray Create<T>(ILGenerator IL, T[] CorrespondingObject)
		{
			var array = new ILArray();

			array.IL = IL;
			array.BaseType = typeof(T);
			array.ArrayType = array.BaseType.MakeArrayType();
			array.Size = CorrespondingObject.Length;
			array.CorrespondingObject = CorrespondingObject;

			array.Address = IL.DeclareLocal(array.ArrayType);
			IL.LoadConstantInt32(array.Size);
			IL.Emit(OpCodes.Newarr, array.BaseType);
			IL.Emit(OpCodes.Stloc, array.Address);

			return array;
		}

		public static ILArray Create<T>(ILGenerator IL, T[] CorrespondingObject, Action AddressAction)
		{
			var array = new ILArray();

			array.IL = IL;
			array.BaseType = typeof(T);
			array.ArrayType = array.BaseType.MakeArrayType();
			array.Size = CorrespondingObject.Length;
			array.CorrespondingObject = CorrespondingObject;

			array.Address = IL.DeclareLocal(array.ArrayType);
			AddressAction();
			IL.Emit(OpCodes.Stloc, array.Address);

			return array;
		}

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
		public static ILArray NewILArray<T>(this ILGenerator IL, T[] CorrespondingObject)
		{
			return ILArray.Create(IL, CorrespondingObject);
		}
	}
}