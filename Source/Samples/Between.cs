using System;
using System.Reflection;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	/// <summary>
	/// UpperBounds and LowerBounds are inclusive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
	public class Between : Attribute, IParameterSurrogate, IReturnSurrogate, IPropertySurrogate, IErrorAction, IOrderOfExecution
	{
		public double LowerBound, UpperBound;

		private static Action<string> DefaultErrorAction = ErrorText => throw new Exception(ErrorText);
		public Action<string> ErrorAction 
		{ 
			get { return DefaultErrorAction; }
			set { DefaultErrorAction = value; }
		}

		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public Between(double LowerBound, double UpperBound, int OrderOfExecution = 0)
		{
			this.LowerBound = LowerBound;
			this.UpperBound = UpperBound;
			this.DefaultOrderOfExecution = OrderOfExecution;
		}

		public object InterceptParameter(object Argument)
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		public object InterceptReturn(dynamic Argument)
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		public object InterceptPropertyGet(object Argument)
		=> Argument;

		public object InterceptPropertySet(object Argument)
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		private object DefaultAction(MethodBase Method, object Argument)
		{
			var arg = Convert.ToDouble(Argument);

			if (arg > UpperBound || arg < LowerBound)
				ErrorAction($"{nameof(NotNull)} : {Method.Name} Argument is out of bounds");

			return Argument;
		}
	}
}