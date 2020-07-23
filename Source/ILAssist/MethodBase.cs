using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		private static readonly OpCode[] LoadArgsOpCodes =
		{
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};
		public static void LoadArgument(this ILGenerator Generator, int Index)
		{
			if (Index <= LoadArgsOpCodes.Length)
				Generator.Emit(LoadArgsOpCodes[Index]);
			else
				Generator.Emit(OpCodes.Ldarg, Index);
		}

		public static void EmitCallBaseAndReturn(this ILGenerator Generator, MethodBase Base) // Also loads this
		{
			for (int i = 0; i <= Base.GetParameters().Length; i++)
				Generator.LoadArgument(i);
			
			Generator.Emit(OpCodes.Call, (dynamic)Base);
			Generator.Emit(OpCodes.Ret);
		}
	}
}