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
		internal static bool IsEligibleForSurrogate(this MethodInfo Method)
		{
			var parameterAttributes = Method.GetParameters().SelectMany(i => i.FindAttributes<IParameterSurrogate>());
			if (parameterAttributes.Count() > 0)
				return true;

			var methodAttributes = Method.FindAttributes<IMethodSurrogate>();
			if (methodAttributes.Count > 0)
				return true;

			var returnAttributes = Method.FindAttributes<IReturnSurrogate>();
			if (returnAttributes.Count > 0)
				return true;

			return false;
		}
		
		// BUG : Derived type's methods are not being considered
		internal static bool IsEligibleForSurrogate(this Type BaseType)
		{
			var methods = BaseType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			return methods.Select(i => i.IsEligibleForSurrogate()).Aggregate((current, next) => current || next);
		}
	}
}