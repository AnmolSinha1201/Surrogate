using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static List<PropertyBuilder> ImplementInterface(this TypeBuilder typeBuilder, Type Interface)
		{
			var retList = new List<PropertyBuilder>();
			var methodAttributesForInterface = InheritedFromInterfacePropertyAttributes;

			var members = Interface.GetProperties();
			typeBuilder.AddInterfaceImplementation(Interface);

			foreach(var member in members)
				retList.Add(typeBuilder.AddProperty(member, methodAttributesForInterface));

			return retList;
		}
	}
}