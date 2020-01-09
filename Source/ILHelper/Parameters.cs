using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.Helpers
{
	internal static partial class ILHelpers
	{
		public static bool IsByRefOrOut(this ParameterInfo Info)
		=> Info.IsOut || Info.ParameterType.IsByRef;

		public static Type ActualParameterType(this ParameterInfo Info)
		{
			if (Info.IsByRefOrOut())
				return Info.ParameterType.GetElementType();
			return Info.ParameterType;
		}
	}
}