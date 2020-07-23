using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IMethodSurrogate
	{
		/// <summary>
		/// Return true to continue execution, false to cancel
		/// </summary>
		bool InterceptMethod(object Item, MethodInfo Member, ref object[] Arguments);
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