using System;
using System.Reflection;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodSurrogate : Attribute, IMethodSurrogate
	{ }
}