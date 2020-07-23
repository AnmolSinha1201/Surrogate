using System;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class Between : Attribute
	{
		public double LowerBound;
		public double UpperBound;

		public Between(double LowerBound, double UpperBound)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
		}

		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			// if (Info.Value == null)
			// 	throw new Exception(Info.ParamInfo.ParameterError("Between"));

			// // if (!Info.Value.IsNumber())
			// // 	throw new Exception(Info.ParamInfo.ParameterError("Between", "number"));
				
			// var value = Convert.ToDouble(Info.Value);
			// if (value < LowerBound || value > UpperBound)
			// 	throw new Exception(Info.ParamInfo.ParameterError("Between", $"within bounds [{LowerBound}, {UpperBound}]"));
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			// if (Info.Value == null)
			// 	throw new Exception(Info.Member.ReturnError("Between"));

			// // if (!Info.Value.IsNumber())
			// // 	throw new Exception(Info.Member.ReturnError("Between", "number"));
				
			// var value = Convert.ToDouble(Info.Value);
			// if (value < LowerBound || value > UpperBound)
			// 	throw new Exception(Info.Member.ReturnError("Between", $"within bounds [{LowerBound}, {UpperBound}]"));
		}
	}
}