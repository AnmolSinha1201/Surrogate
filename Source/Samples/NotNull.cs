using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNull : Attribute
	{
		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			// if (Info.Value == null)
			// 	throw new Exception(Info.ParamInfo.ParameterError("NotNull"));
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			// if (Info.Value == null)
			// 	throw new Exception(Info.Member.ReturnError("NotNull"));
		}
	}
}