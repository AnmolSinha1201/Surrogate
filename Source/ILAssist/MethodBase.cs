using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class BaseILMethodBase
	{
		public ILGenerator Generator;
		public MethodBase Base;
		public BaseILMethodBase(MethodBase Builder)
		{
			Base = Builder;
			Generator = ((dynamic)Builder).GetILGenerator();
		}


		private readonly OpCode[] LoadArgsOpCodes =
		{
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};
		public void LoadArgument(int Index)
		{
			if (Index <= LoadArgsOpCodes.Length)
				Generator.Emit(LoadArgsOpCodes[Index]);
			else
				Generator.Emit(OpCodes.Ldarg, Index);
		}

		public void CallBase() // Also loads this
		{
			for (int i = 0; i <= Base.GetParameters().Length; i++)
				LoadArgument(i);
			
			Generator.Emit(OpCodes.Call, (dynamic)Base);
		}
	}
}