using System;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public class ReturnSurrogate : Attribute
	{
		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
		}
	}
}