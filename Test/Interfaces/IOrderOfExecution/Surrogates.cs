using System;
using System.Reflection;
using Surrogate.Interfaces;

namespace Surrogate.Tests.IOrderOfExecutionTest
{
	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class ReturnSurrogate1 : Attribute, IReturnSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public ReturnSurrogate1(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public object InterceptReturn(dynamic Argument)
		=> Argument - 1;
	}

	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class ReturnSurrogate2 : Attribute, IReturnSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public ReturnSurrogate2(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public object InterceptReturn(dynamic Argument)
		=> Argument * 2;
	}



	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class ParameterSurrogate1 : Attribute, IParameterSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public ParameterSurrogate1(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public object InterceptParameter(dynamic Argument)
		=> Argument - 1;
	}

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class ParameterSurrogate2 : Attribute, IParameterSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public ParameterSurrogate2(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public object InterceptParameter(dynamic Argument)
		=> Argument * 2;
	}



	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MethodSurrogate1 : Attribute, IMethodSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public MethodSurrogate1(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public bool InterceptMethod(object Item, MethodInfo Member, ref dynamic[] Arguments)
		{
			Arguments[0] = Arguments[0] - 1;
			return true;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MethodSurrogate2 : Attribute, IMethodSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public MethodSurrogate2(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}

		public bool InterceptMethod(object Item, MethodInfo Member, ref dynamic[] Arguments)
		{
			Arguments[0] = Arguments[0] * 2;
			return true;
		}
	}
}