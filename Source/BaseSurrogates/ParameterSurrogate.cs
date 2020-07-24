using System;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParameterSurrogate : Attribute, IParameterSurrogate
	{
		public object InterceptParameter(object Argument)
		=> Argument;
	}
}