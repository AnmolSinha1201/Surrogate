using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.Helpers
{
	public static partial class AttributeFinder
	{
		public static Attribute[] FindAttribute(MethodInfo Method, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			

			var attributes = AttributeType == typeof(IReturnSurrogate) ?
				Method.ReturnTypeCustomAttributes.GetCustomAttributes(true).Cast<Attribute>() : Method.GetCustomAttributes();
			foreach (var attribute in attributes)
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal.ToArray();
		}

		public static void ILFindAttribute(this ILGenerator IL, MethodInfo Method, Type AttributeType)
		{
			IL.LoadExternalMethodInfo(Method);
			IL.LoadExternalType(AttributeType);
			IL.Emit(OpCodes.Call, typeof(AttributeFinder).GetMethod(nameof(AttributeFinder.FindAttribute), new[] { typeof(MethodInfo), typeof(Type) }));
		}
	}
}