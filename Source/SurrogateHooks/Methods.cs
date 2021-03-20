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
		public static object MethodSurrogateHook(HookInfo Info, object[] Params)
		{
			var methodAttributes = Info.OriginalMethod.FindAttributes<IMethodSurrogate>().Order();
			var continueExecution = true;
			foreach (var attribute in methodAttributes)
			{
				continueExecution = attribute.InterceptMethod(Info.TargetObject, Info.OriginalMethod, ref Params);
				if (!continueExecution)
					break;
			}

			object retVal = Info.OriginalMethod.ReturnType.Default();
			if (continueExecution)
				retVal = Info.BackingMethod.Invoke(Info.TargetObject, Params);

			return retVal;
		}
	}
}