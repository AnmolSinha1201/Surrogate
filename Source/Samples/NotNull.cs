using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNull : Attribute, IParameterSurrogate, IReturnSurrogate
	{
		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			if (Info.ParamValue == null)
				throw new ArgumentNullException($"{Info.ParamInfo.Name} is null");
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			if (Info.Value == null)
				throw new ArgumentNullException($"return value of {Info.Member.Name} is null");
		}
	}
}