using System;
using System.Reflection;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodSurrogate : Attribute, IMethodSurrogate
	{
		public bool InterceptMethod(object Item, MethodInfo Member, ref object[] Arguments)
		=> true;
	}
}