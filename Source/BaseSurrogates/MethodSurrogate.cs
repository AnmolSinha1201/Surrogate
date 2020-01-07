using System;
using System.Reflection;

namespace Surrogate.Base
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodSurrogate : Attribute
	{
		public virtual object Process(MethodSurrogateInfo Info)
		{
			Console.WriteLine("Inside Surrogate");
			var retVal = Info.Execute();
			return retVal;
		}
	}

	public class MethodSurrogateInfo
	{
		public object Item;
		public MethodInfo Member;

		public MethodSurrogateInfo(object Item, MethodInfo Member)
		{
			this.Item = Item;
			this.Member = Member;
		}

		public object Execute()
		{
			return Member.Invoke(Item, null);
		}
	}
}