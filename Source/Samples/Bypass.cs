using System;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Bypass : Attribute
	{
		public void InterceptMethod(MethodSurrogateInfo Info)
		{
			Console.WriteLine($"Bypassing {Info.Member.Name}");
			// Info.ReturnValue = Info.Member.ReturnType.Default();
		}
	}
}