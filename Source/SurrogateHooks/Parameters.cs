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
		public static object[] ParameterSurrogateHook(HookInfo Info, object[] Params)
		{
			var parameters = Info.OriginalMethod.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				var parameterAttributes = parameters[i].FindAttributes<IParameterSurrogate>().Order();
				if (parameterAttributes.Count == 0)
					continue;

				foreach (var attribute in parameterAttributes)
					Params[i] = attribute.InterceptParameter(Params[i]);
			}

			return Params;
		}
	}
}