using System;
using Surrogate.Helpers;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Bypass : Attribute, IMethodSurrogate
	{
		public void InterceptMethod(MethodSurrogateInfo Info)
		{
			Console.WriteLine($"Bypassing {Info.Member.Name}");
			// Info.ReturnValue = Info.Member.ReturnType.Default();
		}
	}
}