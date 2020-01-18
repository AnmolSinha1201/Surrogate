using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.BetweenTest
{
    public class Setup
    {
		// String is passed. Show throw exception.
		public virtual string Method([Between(5, 10)]string Input)
		{
			return Input;
		}

		// Returns null, should throw exception
		[return : Between(5, 10)]
		public virtual object Method2(int Input)
		{
			return null;
		}

		// Return value should be Betweened.
		[return : Between(5, 10)]
		public virtual int Method3(int Input)
		{
			return Input;
		}

		// Input value should be Betweened.
		public virtual int Method4([Between(5, 10)]int Input)
		{
			return Input;
		}
    }
}
