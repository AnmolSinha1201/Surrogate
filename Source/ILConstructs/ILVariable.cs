using System;
using System.Reflection.Emit;

namespace Surrogate.ILConstructs
{
	public class ILVariable
	{
		public LocalBuilder Address;
		public ILGenerator IL;

		public ILVariable(ILGenerator IL, Type VariableType)
		{
			this.IL = IL;
			this.Address = IL.DeclareLocal(VariableType);
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
	}

	internal static partial class ILHelpers
	{
		public static ILVariable NewVariable(this ILGenerator IL, Type VariableType, Action StoreAction)
		{
			var variable = new ILVariable(IL, VariableType);
			variable.Store(StoreAction);

			return variable;
		}
	}
}