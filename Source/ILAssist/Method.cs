using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class ILMethod : BaseILMethodBase
	{
		public ILMethod(MethodBuilder Builder) : base(Builder)
		{ }
	}
}