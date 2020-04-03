using System;
using System.Reflection.Emit;
using Surrogate.Helpers;

namespace Surrogate.Internal.ILConstructs
{
	public class ILVariable
	{
		public LocalBuilder Address;
		public ILGenerator IL;
		public Type VariableType;

		public ILVariable(ILGenerator IL, Type VariableType)
		{
			this.IL = IL;
			this.VariableType = VariableType;
			this.Address = IL.DeclareLocal(VariableType);
		}

		public ILVariable(ILGenerator IL, int Value)
		{
			this.IL = IL;
			this.Address = IL.DeclareLocal(typeof(int));
			this.VariableType = typeof(int);

			IL.LoadConstantInt32(Value);
			IL.Emit(OpCodes.Stloc, Address);
		}

		public void Load()
		{
			IL.Emit(OpCodes.Ldloc, Address);
		}

		public void Store(Action StoreAction)
		{
			StoreAction();
			IL.Emit(OpCodes.Stloc, Address);
		}

		public void Add(int Number)
		{
			Store(() =>
			{
				Load();
				IL.LoadConstantInt32(Number);
				IL.Emit(OpCodes.Add);
			});
		}
	}

	internal static partial class ILHelpers
	{
		public static ILVariable NewVariable(this ILGenerator IL, Type VariableType, Action StoreAction = null)
		{
			var variable = new ILVariable(IL, VariableType);

			if (StoreAction != null)
				variable.Store(StoreAction);

			return variable;
		}

		public static ILVariable NewVariable(this ILGenerator IL, int Value)
		{
			return new ILVariable(IL, Value);
		}
	}
}