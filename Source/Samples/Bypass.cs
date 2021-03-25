using System;
using System.Reflection;
using Surrogate.ILAssist;
using Surrogate.Interfaces;

namespace Surrogate.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Bypass : Attribute, IMethodSurrogate, IOrderOfExecution
	{
		public int OrderOfExecution { get; set; } = 0;

		public Bypass(int OrderOfExecution = 0)
		{
			this.OrderOfExecution = OrderOfExecution;
		}
	}
}