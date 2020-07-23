using System;
using System.Reflection;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Bypass : Attribute, IMethodSurrogate, IOrderOfExecution
	{
		private int DefaultOrderOfExecution = 0;
		public int OrderOfExecution 
		{
			get { return DefaultOrderOfExecution; }
			set { DefaultOrderOfExecution = value; }
		}

		public Bypass(int OrderOfExecution = 0)
		{
			this.DefaultOrderOfExecution = OrderOfExecution;
		}

		public bool InterceptMethod(object Item, MethodInfo Member, ref object[] Arguments)
		{
			throw new NotImplementedException();
		}
	}
}