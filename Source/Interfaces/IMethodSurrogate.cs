using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IMethodSurrogate
	{
		void InterceptMethod(object Item, MethodInfo Member, object[] Arguments);
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
	}
}