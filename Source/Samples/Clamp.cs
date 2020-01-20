using System;
using Surrogate.Helpers;
using Surrogate.Interfaces;
using Surrogate.Internal.Helpers;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class Clamp : Attribute, IParameterSurrogate, IReturnSurrogate
	{
		public double LowerBound;
		public double UpperBound;

		public Clamp(double LowerBound, double UpperBound)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
		}

		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			if (Info.Value == null)
				throw new Exception(Info.ParamInfo.ParameterError("Clamp"));

			if (!Info.Value.IsNumber())
				throw new Exception(Info.ParamInfo.ParameterError("Clamp", "number"));

			var retVal = Math.Clamp(Convert.ToDouble(Info.Value), LowerBound, UpperBound);
			Info.Value = Convert.ChangeType(retVal, Info.ParamInfo.ActualParameterType());
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			if (Info.Value == null)
				throw new Exception(Info.Member.ReturnError("Clamp"));

			if (!Info.Value.IsNumber())
				throw new Exception(Info.Member.ReturnError("Clamp", "number"));

			var retVal = Math.Clamp(Convert.ToDouble(Info.Value), LowerBound, UpperBound);
			Info.Value = Convert.ChangeType(retVal, Info.Member.ReturnType);
		}
	}
}