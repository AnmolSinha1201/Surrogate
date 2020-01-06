using System;

namespace Surrogate.Base
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodSurrogate : Attribute
	{
		public virtual void Process(MethodSurrogateInfo Info)
		{
			Console.WriteLine("Inside Surrogate");
		}
	}

	public class MethodSurrogateInfo
	{
		public object qwe;
		public MethodSurrogateInfo(object qwe)
		{
			this.qwe = qwe;
		}

		public MethodSurrogateInfo()
		{
		}
	}
}