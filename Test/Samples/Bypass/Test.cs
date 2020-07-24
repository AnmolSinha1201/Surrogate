using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.BypassTest
{
    public class Cases
    {
		public Setup TestObject;
		public Cases()
		{
			TestObject = (Setup)SurrogateBuilder.Build<Setup>();
		}

		[Fact]
        public void ShouldNotExecute()
        {
			TestObject.Method();

			var freshObject = new Setup();
			Assert.Equal(freshObject.Value, TestObject.Value);
        }
    }
}
