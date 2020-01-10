using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
		private static readonly OpCode[] LoadArgsOpCodes =
		{
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};

		/// <summary>
		/// <param name="Index">Base 0 Index. Indexes are automatically converted to Base 1 since they are the original index types.</para>
		/// </summary>
		public static void LoadArgument(this ILGenerator IL, int Index)
		{
			if (Index < LoadArgsOpCodes.Length)
				IL.Emit(LoadArgsOpCodes[Index]);
			else
				IL.Emit(OpCodes.Ldarg, Index + 1);
		}
	}
}