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
			var info = new HookInfo { OriginalMethod = OriginalMethod, BackingMethod = BackingMethod, TargetObject = Item };
			
			var arguments = ParameterSurrogateHook(info, Params);
			var retVal = MethodSurrogateHook(info, arguments);
			retVal = ReturnSurrogateHook(info, retVal);

			return retVal;
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

	public class HookInfo
	{
		public MethodInfo BackingMethod, OriginalMethod;
		public object TargetObject;
	}
}