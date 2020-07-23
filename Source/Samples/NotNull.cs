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

		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
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