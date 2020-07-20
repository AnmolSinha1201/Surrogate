using System;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParameterSurrogate : Attribute
	{
		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
		}
	}
}