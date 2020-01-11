using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public class ReturnSurrogate : Attribute, IReturnSurrogate
	{
		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
		}
	}
}