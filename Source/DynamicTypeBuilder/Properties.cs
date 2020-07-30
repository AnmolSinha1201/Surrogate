using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		/// <summary>
		/// <para>Adds a field to TypeBuilder.</para>
		/// <para>Default access level is Public, i.e. field is public and instance level.</para>
		/// <para>
		/// By default, property getters and setters use <c>Public | HideBySig | SpecialName</c>.
		/// For inheritance, use <c>Public | Virtual | NewSlot | SpecialName | HideBySig</c>
		/// </para>
		/// </summary>
		public static PropertyBuilder AddProperty(this TypeBuilder Builder, string MemberName, Type MemberType, MethodAttributes MAttributes = DefaultPropertyAttributes)
		{
			FieldBuilder backingField = Builder.AddField(BackingFieldNameByConvention(MemberName), MemberType, FieldAttributes.Private);
			PropertyBuilder property = Builder.DefineProperty(MemberName, PropertyAttributes.HasDefault, MemberType, null);

			MethodBuilder getter = Builder.DefineMethod($"get_{MemberName}", MAttributes, MemberType, Type.EmptyTypes);
			ILGenerator getterIL = getter.GetILGenerator();
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Ldfld, backingField);
			getterIL.Emit(OpCodes.Ret);

			MethodBuilder setter =  Builder.DefineMethod($"set_{MemberName}", MAttributes, null, new Type[] { MemberType });
			ILGenerator setterIL = setter.GetILGenerator();
			setterIL.Emit(OpCodes.Ldarg_0);
			setterIL.Emit(OpCodes.Ldarg_1);
			setterIL.Emit(OpCodes.Stfld, backingField);
			setterIL.Emit(OpCodes.Ret);

			property.SetGetMethod(getter);
			property.SetSetMethod(setter);

			return property;
		}

		public static PropertyBuilder AddProperty<T>(this TypeBuilder Builder, string MemberName, MethodAttributes MAttributes = DefaultPropertyAttributes)
		=> Builder.AddProperty(MemberName, typeof(T), MAttributes);

		public const MethodAttributes DefaultPropertyAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
		public const MethodAttributes InheritedFromInterfacePropertyAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.HideBySig;


		public static PropertyBuilder AddProperty(this TypeBuilder Builder, MemberInfo Member, MethodAttributes MAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName)
		{
			var property = Builder.AddProperty(Member.Name, Member.MemberType(), MAttributes);

			foreach (var attribute in Member.GetCustomAttributesData())
				property.AddAttribute(attribute);

			return property;
		}

		public static void AddAttribute(this PropertyBuilder Builder, CustomAttributeData CustomAttribute)
		=> Builder.SetCustomAttribute(CustomAttribute.ToCustomAttributeBuilder());

		public static void AddAttribute(this PropertyBuilder Builder, CustomAttributeBuilder CustomAttribute)
		=> Builder.SetCustomAttribute(CustomAttribute);


		static string BackingFieldNameByConvention(string FieldName)
		=> $"<{FieldName}>k__BackingField";
	}
}