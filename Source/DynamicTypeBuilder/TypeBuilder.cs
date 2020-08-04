using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static TypeBuilder ToTypeBuilder(this Type BaseType)
		=> DynamicTypeBuilder.ToTypeBuilder(BaseType, BaseType.Name);

		public static TypeBuilder ToTypeBuilder(this Type BaseType, string TypeName)
		{
			AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
			var typeBuilder = moduleBuilder.DefineType(TypeName, TypeAttributes.Public, BaseType);

			if (BaseType == null)
				return typeBuilder;
			
			foreach (var attribute in BaseType.GetCustomAttributesData())
				typeBuilder.SetCustomAttribute(attribute.ToCustomAttributeBuilder());
			foreach (var constructor in BaseType.GetConstructors()) 
				typeBuilder.CreatePassThroughConstructor(constructor);

			return typeBuilder;
		}

		public static void AddAttributes(this TypeBuilder Builder, List<CustomAttributeData> CustomAttributes)
		{
			foreach (var attribute in CustomAttributes)
				Builder.SetCustomAttribute(attribute.ToCustomAttributeBuilder());
		}
	}
}