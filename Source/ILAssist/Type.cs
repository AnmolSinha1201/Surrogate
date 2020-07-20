using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static TypeBuilder ToTypeBuilder(this Type BaseType)
		=> DynamicTypeBuilder.BuildType(BaseType.Name, BaseType);
	}
}