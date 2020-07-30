using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		/// <summary>
		/// <para>Adds a field to TypeBuilder.</para>
		/// <para>Default access level is Public, i.e. field is public and instance level.</para>
		/// </summary>
		public static FieldBuilder AddField(this TypeBuilder Builder, string MemberName, Type MemberType, FieldAttributes Attributes = FieldAttributes.Public)
		=> Builder.DefineField(MemberName, MemberType, Attributes);

		public static FieldBuilder AddField(this TypeBuilder Builder, FieldInfo Field)
		{
			var field = Builder.AddField(Field.Name, Field.MemberType(), Field.Attributes);

			foreach (var attribute in Field.GetCustomAttributesData())
				field.AddAttribute(attribute);

			return field;
		}

		public static void AddAttribute(this FieldBuilder Builder, CustomAttributeData CustomAttribute)
		=> Builder.SetCustomAttribute(CustomAttribute.ToCustomAttributeBuilder());

		public static void AddAttribute(this FieldBuilder Builder, CustomAttributeBuilder CustomAttribute)
		=> Builder.SetCustomAttribute(CustomAttribute);
	}
}