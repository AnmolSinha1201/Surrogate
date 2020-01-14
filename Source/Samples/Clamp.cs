using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class Clamp : Attribute, IParameterSurrogate, IReturnSurrogate
	{
		public int LowerBound;
		public int UpperBound;

		public Clamp(int LowerBound, int UpperBound)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
		}

		public void InterceptParameter(ParameterSurrogateInfo Info)
		{
			if ((int)Info.Value < LowerBound || (int)Info.Value > UpperBound)
				Info.Value = Math.Clamp((int)Info.Value, LowerBound, UpperBound);
		}

		public void InterceptReturn(ReturnSurrogateInfo Info)
		{
			if ((int)Info.Value < LowerBound || (int)Info.Value > UpperBound)
				Info.Value = Math.Clamp((int)Info.Value, LowerBound, UpperBound);
		}
	}
}