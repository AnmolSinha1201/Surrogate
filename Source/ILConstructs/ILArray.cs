using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Surrogate.ILAssist;
using Surrogate.Internal.ILConstructs;

namespace Surrogate.Internal.ILConstructs
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

		public void StoreElementAt(int Index, ILVariable Variable)
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.LoadConstantInt32(Index);
			Variable.Load();
			IL.Emit(OpCodes.Stelem_Ref);
		}

		public void LoadLength()
		{
			IL.Emit(OpCodes.Ldloc, Address);
			IL.Emit(OpCodes.Ldlen);
			// IL.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(int) }));
		}

		public void ForEach(Action<ILVariable> ForEachAction)
		{
			var currentIndex = IL.NewVariable(0);
			var currentItem = IL.NewVariable(BaseType);

			var loopStart = IL.DefineLabel();
			var end = IL.DefineLabel();
			var validation = IL.DefineLabel();


			IL.MarkLabel(validation);
			currentIndex.Load();
			LoadLength();
			IL.Emit(OpCodes.Bge, end);

			currentItem.Store(() => LoadElementAt(currentIndex));
			ForEachAction(currentItem);
			currentIndex.Add(1);

			IL.Emit(OpCodes.Br, validation);
			IL.MarkLabel(end);
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
	}
}