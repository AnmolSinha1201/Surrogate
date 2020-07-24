using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.BypassTest
{
	public class Setup
	{
		public int Value = 5;

		// Should not execute
		[Bypass]
		public virtual void Method()
		{
			Value = 10;
		}
	}
}
