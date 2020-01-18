using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.NotNullTest
{
    public class Setup
    {
		[return : NotNull]
		public virtual string Method([Samples.NotNull]string InputText)
		{
			return InputText;
		}

		[return : NotNull]
		public virtual string Method2(string InputText)
		{
			return null;
		}
    }
}
