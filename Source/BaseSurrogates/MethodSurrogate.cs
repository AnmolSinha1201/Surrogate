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
		public object[] Arguments;

		public MethodSurrogateInfo(object Item, MethodInfo Member, object[] Arguments)
		{
			this.Item = Item;
			this.Member = Member;
			this.Arguments = Arguments;
		}

		public object Execute()
		{
			return Member.Invoke(Item, new object[] { 67890 });
		}
	}
}