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
		public static Attribute[] LoadAttributes(MethodInfo Method, Type AttributeType)
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

		public static ILArray ILLoadAttributes<TAttribute>(this ILGenerator IL, MethodInfo Method)
		{
			IL.LoadExternalMethodInfo(Method);
			IL.LoadExternalType(typeof(TAttribute));

			var array = IL.CreateArray<TAttribute>(() =>
			{
				IL.Emit(OpCodes.Call, typeof(AttributeFinder).GetMethod(nameof(AttributeFinder.LoadAttributes), new[] { typeof(MethodInfo), typeof(Type) }));
			});

			return array;
		}
	}
}