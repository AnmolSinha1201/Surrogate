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

		private static List<T> Order<T>(this List<T> AttributeList)
		{
			var groups = AttributeList.ToLookup(item => item is IOrderOfExecution);
			var retVal = groups[true].OrderBy(i => ((IOrderOfExecution)i).OrderOfExecution).Concat(groups[false]).ToList();

			return retVal;
		}

		private static object Default(this Type type)
		{
			if(type.IsValueType)
				return Activator.CreateInstance(type);
			
			return null;
		}
	}
}