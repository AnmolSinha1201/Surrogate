using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IMethodSurrogate
	{
		void InterceptMethod(MethodSurrogateInfo Info);
	}

	public class MethodSurrogateInfo
	{
		public object Item;
		public MethodInfo Member;
		public object[] Arguments;
		public object ReturnValue;

		public MethodSurrogateInfo(object Item, MethodInfo Member, object[] Arguments)
		{
			this.Item = Item;
			this.Member = Member;
			this.Arguments = Arguments;
		}

		public MethodSurrogateInfo(object Item, MethodInfo Member, object[] Arguments, object ReturnValue)
		{
			this.Item = Item;
			this.Member = Member;
			this.Arguments = Arguments;
			this.ReturnValue = ReturnValue;
		}

	}
}