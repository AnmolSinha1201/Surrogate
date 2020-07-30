using System;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.IPropertySurrogateTest
{
	public class Setup
	{
		[PropertySurrogateSet]
		public virtual int PropertySet { get; set; }

		[PropertySurrogateGet]
		public virtual int PropertyGet { get; set; }
	}
}
