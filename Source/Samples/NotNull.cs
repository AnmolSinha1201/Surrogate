using System;
using Surrogate.Interfaces;
using Surrogate.Helpers;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNull : Attribute, IParameterSurrogate, IReturnSurrogate
	{
		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			if (Info.Value == null)
				throw new Exception($"[NotNull] : {Info.ParamInfo.Name} for {Info.ParamInfo.FullMemberName()} cannot be null");
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			if (Info.Value == null)
				throw new Exception($"[NotNull] : Return value of {Info.Member.FullMemberName()} cannot be null");
		}
	}
}