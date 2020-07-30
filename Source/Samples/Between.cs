using System;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class Between : Attribute, IParameterSurrogate, IReturnSurrogate, IErrorAction, IOrderOfExecution
	{
		public double LowerBound, UpperBound;

		private static Action<string> DefaultErrorAction = ErrorText => throw new Exception(ErrorText);
		public Action<string> ErrorAction 
		{ 
			get { return DefaultErrorAction; }
			set { DefaultErrorAction = value; }
		}

		public int OrderOfExecution { get; set; } = 0;

		public Between(double LowerBound, double UpperBound, int OrderOfExecution = 0)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
			this.OrderOfExecution = OrderOfExecution;
		}

		public object InterceptParameter(object Argument)
		{
			var arg = Convert.ToDouble(Argument);

			if (arg > UpperBound || arg < LowerBound)
				ErrorAction($"{nameof(Between)} : Argument out of bounds");

			return Argument;
		}

		public object InterceptReturn(dynamic Argument)
		{
			var arg = Convert.ToDouble(Argument);

			if (arg > UpperBound || arg < LowerBound)
				ErrorAction($"{nameof(Between)} : Return value out of bounds");

			return Argument;
		}
	}
}