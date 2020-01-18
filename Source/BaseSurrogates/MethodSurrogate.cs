using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodSurrogate : Attribute, IMethodSurrogate
	{
		public void InterceptMethod(MethodSurrogateInfo Info)
		{
			var retVal = Info.Execute();
			Info.ReturnValue = retVal;
		}
	}
}