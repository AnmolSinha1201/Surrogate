using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParameterSurrogate : Attribute, IParameterSurrogate
	{
		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
		}
	}
}