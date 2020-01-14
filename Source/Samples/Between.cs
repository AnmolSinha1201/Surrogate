using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class Between : Attribute, IParameterSurrogate, IReturnSurrogate
	{
		public int LowerBound;
		public int UpperBound;

		public Between(int LowerBound, int UpperBound)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
		}

		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			if ((int)Info.Value < LowerBound || (int)Info.Value > UpperBound)
				throw new ArgumentNullException($"{Info.ParamInfo.Name} for ${Info.ParamInfo.Member.Name}exceeds bounds");
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			if ((int)Info.Value < LowerBound || (int)Info.Value > UpperBound)
				throw new ArgumentNullException($"return value of {Info.Member.Name} exceeds bounds");
		}
	}
}