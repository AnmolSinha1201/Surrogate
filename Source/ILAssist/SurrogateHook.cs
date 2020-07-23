using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{		public static object SurrogateHook(MethodInfo BackingMethod, object Item, MethodInfo OriginalMethod, object[] Params)
		{
			var parameters = OriginalMethod.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameterAttributes = parameters[i].FindAttributes<IParameterSurrogate>();
				if (parameterAttributes.Length == 0)
					continue;

				foreach (var attribute in parameterAttributes)
				{
					if (i < Params.Length)
						attribute.InterceptParameter(ref Params[i]);
					else
					{
						var value = parameters[i].RawDefaultValue;
						attribute.InterceptParameter(ref value);
					}
				}
			}
			
			var methodAttributes = OriginalMethod.FindAttributes<IMethodSurrogate>();
			foreach (var attribute in methodAttributes)
				attribute.InterceptMethod(Item, OriginalMethod, Params);

			var returnAttributes = OriginalMethod.FindAttributes<IReturnSurrogate>();
			var retVal = BackingMethod.Invoke(Item, Params);
			foreach (var attribute in returnAttributes)
				attribute.InterceptReturn(ref retVal);

			return retVal;
		}
	}
}