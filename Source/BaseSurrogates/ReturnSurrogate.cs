using System;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public class ReturnSurrogate : Attribute, IReturnSurrogate
	{
		public object InterceptReturn(object Argument)
		=> Argument;
	}
}