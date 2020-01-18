using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.Internal.Helpers
{
	public static partial class AttributeFinder
	{
		public static Attribute[] FindAttributes(ParameterInfo Parameter, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			var attributes = Parameter.GetCustomAttributes();
			
			foreach (var attribute in attributes)
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal.ToArray();
		}

		public static Attribute[] FindAttributes(ParameterInfo[] Parameters, Type AttributeType)
		{
			var retVal = new List<Attribute>();

			foreach (var parameter in Parameters)
				retVal.AddRange(FindAttributes(parameter, AttributeType));

			return retVal.ToArray();
		}
	}
}