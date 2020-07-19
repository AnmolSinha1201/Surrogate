using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static CustomAttributeBuilder ToCustomAttributeBuilder(this Type CustomAttributeType, object[] ConstructorArguments = null)
		{
			var constructorSignature = ConstructorArguments == null ? new Type[] {} : ConstructorArguments.Select(i => i.GetType()).ToArray();
			var constructcorInfo = CustomAttributeType.GetConstructor(constructorSignature);
			return new CustomAttributeBuilder(constructcorInfo, ConstructorArguments ?? new object[] {});
		}
	}
}