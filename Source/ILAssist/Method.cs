using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class ILMethod
	{
		MethodInfo Base;

		public ILMethod(MethodInfo Info)
		{ 
			this.Base = Info;
		}
	}
}