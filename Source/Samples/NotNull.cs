using System;
using System.Reflection;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property)]
	public class NotNull : Attribute, IParameterSurrogate, IReturnSurrogate, IPropertySurrogate, IOrderOfExecution, IErrorAction
	{
		private static Action<string> DefaultErrorAction = ErrorText => throw new Exception($"{nameof(NotNull)} : {MethodBase.GetCurrentMethod().Name} {ErrorText}");
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
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		public object InterceptReturn(object Argument)
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		public object InterceptPropertyGet(object Argument)
		=> Argument;

		public object InterceptPropertySet(object Argument)
		=> DefaultAction(MethodBase.GetCurrentMethod(), Argument);

		private object DefaultAction(MethodBase Method, object Argument)
		{
			if (Argument == null)
				ErrorAction($"{nameof(NotNull)} : {Method.Name} Argument is null");

			return Argument;
		}
	}
}