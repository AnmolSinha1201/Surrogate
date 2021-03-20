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
		public static object ReturnSurrogateHook(HookInfo Info, object DefaultReturnValue)
		{
			var retVal = DefaultReturnValue;
			
			var returnAttributes = Info.OriginalMethod.FindAttributes<IReturnSurrogate>().Order();
			foreach (var attribute in returnAttributes)
				retVal = attribute.InterceptReturn(retVal);

			return retVal;
		}
	}
}