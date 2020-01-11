using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IReturnSurrogate
	{
		void InterceptReturn(ReturnSurrogateInfo Info);
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