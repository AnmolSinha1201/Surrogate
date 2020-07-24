using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static CustomAttributeBuilder[] ToCustomAttributeBuilder(this IEnumerable<CustomAttributeData> CustomAttributes)
		=> CustomAttributes.Select(attribute => attribute.ToCustomAttributeBuilder()).ToArray();

		public static CustomAttributeBuilder ToCustomAttributeBuilder(this CustomAttributeData CustomAttribute)
		{
			var attributeArgs = CustomAttribute.ConstructorArguments.Select(a => a.Value).ToArray();

			var propertyArgs = CustomAttribute.NamedArguments.Where(i => i.MemberInfo is PropertyInfo);
			var propertyInfos = propertyArgs.Select(a => (PropertyInfo)a.MemberInfo).ToArray();
			var propertyValues = propertyArgs.Select(a => a.TypedValue.Value).ToArray();
			
			var fieldArgs = CustomAttribute.NamedArguments.Where(i => i.MemberInfo is FieldInfo);
			var fieldInfos = fieldArgs.Select(a => (FieldInfo)a.MemberInfo).ToArray();
			var fieldValues = fieldArgs.Select(a => a.TypedValue.Value).ToArray();
			
			return new CustomAttributeBuilder(CustomAttribute.Constructor, attributeArgs, propertyInfos, propertyValues, fieldInfos, fieldValues);
		}
	}
}