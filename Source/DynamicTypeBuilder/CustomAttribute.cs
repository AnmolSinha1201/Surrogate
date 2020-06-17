using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static CustomAttributeBuilder ToCustomAttributeBuilder(this CustomAttributeData AttributeData)
		{
			var constructorArgs = AttributeData.ConstructorArguments.Select(i => i.Value);

			var fieldArgs = AttributeData.NamedArguments.Where(i => i.MemberInfo is FieldInfo);
			var fieldArgsInfo = fieldArgs.Select(i => (FieldInfo)i.MemberInfo);
			var fieldArgsValues = fieldArgs.Select(i => i.TypedValue.Value);

			var propertyArgs = AttributeData.NamedArguments.Where(i => i.MemberInfo is PropertyInfo);
			var propertyArgsInfo = propertyArgs.Select(i => (PropertyInfo)i.MemberInfo);
			var propertyArgsValues = propertyArgs.Select(i => i.TypedValue.Value);

			return new CustomAttributeBuilder(
				AttributeData.Constructor, constructorArgs.ToArray(),
				propertyArgsInfo.ToArray(), propertyArgsValues.ToArray(),
				fieldArgsInfo.ToArray(), fieldArgsValues.ToArray()
			);
		} 

		public static CustomAttributeBuilder ToCustomAttributeBuilder(this Type CustomAttributeType, object[] ConstructorArguments = null)
		{
			var constructorSignature = ConstructorArguments == null ? new Type[] {} : ConstructorArguments.Select(i => i.GetType()).ToArray();
			var constructcorInfo = CustomAttributeType.GetConstructor(constructorSignature);
			return new CustomAttributeBuilder(constructcorInfo, ConstructorArguments ?? new object[] {});
		}
	}
}