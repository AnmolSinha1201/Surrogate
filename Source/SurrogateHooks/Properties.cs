using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		public static object SurrogateHookPropertyGet(object Item, PropertyInfo NewProperty, MethodInfo BackingMethod)
		{
			var attributes = NewProperty.FindAttributes<IPropertySurrogate>().Order();
			var retVal = BackingMethod.Invoke(Item, new object[] { });

			foreach (var attribute in attributes)
				retVal = attribute.InterceptPropertyGet(retVal);

			return retVal;
		}

		public static void SurrogateHookPropertySet(object Item, PropertyInfo NewProperty, MethodInfo BackingMethod, object Value)
		{
			var attributes = NewProperty.FindAttributes<IPropertySurrogate>().Order();
			var retVal = Value;

			foreach (var attribute in attributes)
				retVal = attribute.InterceptPropertySet(retVal);

			BackingMethod.Invoke(Item, new [] { retVal });
		}
	}
}