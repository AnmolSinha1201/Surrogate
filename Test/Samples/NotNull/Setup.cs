using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.NotNull
{
    public class Setup
    {
		[return : Samples.NotNull]
		public virtual string Method([Samples.NotNull]string InputText)
		{
			return InputText;
		}

		[return : Samples.NotNull]
		public virtual string Method2(string InputText)
		{
			return null;
		}
    }
}
