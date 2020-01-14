using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MethodSurrogateStub : Attribute, IMethodSurrogate
	{
		public void InterceptMethod(MethodSurrogateInfo Info)
		{
			var retVal = Info.Execute();
			Info.ReturnValue = retVal;
		}
	}
}