using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
        public static Type MemberType(this MemberInfo Member)
        => Member.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)Member).FieldType,
            MemberTypes.Property => ((PropertyInfo)Member).PropertyType,
            _ => null
        };
	}
}