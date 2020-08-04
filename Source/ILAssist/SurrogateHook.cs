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
		public static object SurrogateHook(MethodInfo BackingMethod, object Item, MethodInfo OriginalMethod, object[] Params)
		{
			var parameters = OriginalMethod.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameterAttributes = parameters[i].FindAttributes<IParameterSurrogate>().Order();
				if (parameterAttributes.Count == 0)
					continue;

				foreach (var attribute in parameterAttributes)
					Params[i] = attribute.InterceptParameter(Params[i]);
			}
			
			var methodAttributes = OriginalMethod.FindAttributes<IMethodSurrogate>().Order();
			var continueExecution = true;
			foreach (var attribute in methodAttributes)
			{
				continueExecution = attribute.InterceptMethod(Item, OriginalMethod, ref Params);
				if (!continueExecution)
					break;
			}

			object retVal = OriginalMethod.ReturnType.Default();
			if (continueExecution)
				retVal = BackingMethod.Invoke(Item, Params);
			
			var returnAttributes = OriginalMethod.FindAttributes<IReturnSurrogate>().Order();
			foreach (var attribute in returnAttributes)
				retVal = attribute.InterceptReturn(retVal);

			return retVal;
		}

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

		private static List<T> Order<T>(this List<T> AttributeList)
		{
			var groups = AttributeList.ToLookup(item => item is IOrderOfExecution);
			var retVal = groups[true].OrderBy(i => ((IOrderOfExecution)i).OrderOfExecution).Concat(groups[false]).ToList();

			return retVal;
		}

		private static object Default(this Type ItemType)
		{
			if (ItemType == typeof(void))
				return null;

			if(ItemType.IsValueType)
				return Activator.CreateInstance(ItemType);
			
			return null;
		}
	}
}