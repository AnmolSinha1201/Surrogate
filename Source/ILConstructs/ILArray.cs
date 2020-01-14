using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Surrogate.ILConstructs;

namespace Surrogate.Helpers
{
	public class ILArray
	{
		public ILGenerator IL;
		public Type BaseType, ArrayType;
		public LocalBuilder Address;

		public void LoadElementAt(int Index)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.LoadConstantInt32(Index);
			IL.Emit(OpCodes.Ldelem_Ref);
		}

		public void LoadElementAt(ILVariable Local)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			Local.Load();
			IL.Emit(OpCodes.Ldelem_Ref);
		}

		public void StoreElementAt(int Index, Action ILAction)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.LoadConstantInt32(Index);
			ILAction();
			IL.Emit(OpCodes.Stelem_Ref);
		}

		public void LoadLength()
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.Emit(OpCodes.Ldlen);
			// IL.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(int) }));
		}

		public Action ElementAtIL(int Index)
		{
			return () => LoadElementAt(Index);
		}

	}

	internal static partial class ILHelpers
	{
		public  static ILArray CreateArray<T>(this ILGenerator IL, int Size = 0)
		{
			var array = new ILArray();

			array.IL = IL;
			array.BaseType = typeof(T);
			array.ArrayType = array.BaseType.MakeArrayType();

			array.Address = IL.DeclareLocal(array.ArrayType);
			
			if (Size != 0)
			{
				IL.LoadConstantInt32(Size);
				IL.Emit(OpCodes.Newarr, array.BaseType);
				IL.Emit(OpCodes.Stloc, array.Address);
			}

			return array;
		}

		public static ILArray CreateArray<T>(this ILGenerator IL, Action AddressAction)
		{
			var array = IL.CreateArray<T>();

			AddressAction();
			IL.Emit(OpCodes.Stloc, array.Address);

			return array;
		}

		public static ILArray CreateArray<T>(this ILGenerator IL, int Size, Action<int> AddressAction)
		{
			var array = IL.CreateArray<T>(Size);
			
			for (int i = 0; i < Size; i++)
			{
				IL.Emit(OpCodes.Ldloc, array.Address);
				IL.LoadConstantInt32(i);
				AddressAction(i);
				IL.Emit(OpCodes.Stelem_Ref);
			}

			return array;
		}

		public static ILArray CreateArray<T>(this ILGenerator IL, int Size, Func<int, LocalBuilder> AddressAction)
		{
			return IL.CreateArray<T>(Size, (i) =>
			{
				var local = AddressAction(i);
				IL.Emit(OpCodes.Ldloc, local);
			});
		}
	}
}