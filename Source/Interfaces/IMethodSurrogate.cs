using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IMethodSurrogate
	{
		/// <summary>
		/// <para>Return : True - if you want to continue execution, False if you want to stop</para>
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