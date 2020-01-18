using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.ClampTest
{
    public class Setup
    {
		// String is passed. Show throw exception.
		public virtual string Method([Clamp(15, 20)]string Input)
		{
			return Input;
		}

		// Returns null, should throw exception
		[return : Clamp(5, 10)]
		public virtual object Method2(int Input)
		{
			return null;
		}

		// Return value should be clamped.
		[return : Clamp(5, 10)]
		public virtual int Method3(int Input)
		{
			return Input;
		}

		// Input value should be clamped.
		public virtual int Method4([Clamp(5, 10)]int Input)
		{
			return Input;
		}
    }
}
