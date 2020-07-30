using System;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNull : Attribute, IParameterSurrogate, IReturnSurrogate, IOrderOfExecution, IErrorAction
	{
		private static Action<string> DefaultErrorAction = ErrorText => throw new Exception(ErrorText);
		public Action<string> ErrorAction 
		{ 
			get { return DefaultErrorAction; }
			set { DefaultErrorAction = value; }
		}

		public int OrderOfExecution { get; set; } = 0;

		public NotNull(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}


		public object InterceptParameter(object Argument)
		{
			if (Argument == null)
				ErrorAction($"{nameof(NotNull)} : Argument is a null");

			return Argument;
		}

		public object InterceptReturn(object Argument)
		{
			if (Argument == null)
				ErrorAction($"{nameof(NotNull)} : Return value is a null");

			return Argument;
		}
	}
}