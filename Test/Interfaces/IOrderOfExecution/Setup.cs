using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.IOrderOfExecutionTest
{
	public class Setup
	{
		public int Value = 5;

		// Should not execute
		[MethodSurrogate1(0)][MethodSurrogate2(1)]
		[return: ReturnSurrogate1(0)][return: ReturnSurrogate2(1)]
		public virtual int Method([ParameterSurrogate1(0)][ParameterSurrogate2(1)] int Foo)
		{
			return Foo;
		}

		[MethodSurrogate1(1)][MethodSurrogate2(0)]
		[return: ReturnSurrogate1(1)][return: ReturnSurrogate2(0)]
		public virtual int Method2([ParameterSurrogate1(1)][ParameterSurrogate2(0)] int Foo)
		{
			return Foo;
		}
	}
}
