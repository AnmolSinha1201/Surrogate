using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ReturnSurrogate : Attribute, IReturnSurrogate
	{
		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
		}
	}
}