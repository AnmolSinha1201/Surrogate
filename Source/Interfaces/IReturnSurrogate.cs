using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IReturnSurrogate
	{
		object InterceptReturn(object Argument);
	}

	public class ReturnSurrogateInfo
	{
		public MethodInfo Member;
		public object Value;

		public ReturnSurrogateInfo(MethodInfo Member, object Value)
		{
			this.Member = Member;
			this.Value = Value;
		}
	}
}