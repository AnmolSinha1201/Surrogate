using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.NotNullTest
{
    public class Setup
    {
		[return : NotNull]
		public virtual string Method([NotNull]string InputText)
		{
			return InputText;
		}

		[return : NotNull]
		public virtual string Method2(string InputText)
		{
			return null;
		}

		[NotNull]
		public virtual string Property { get; set; }
    }
}
