using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.IOrderOfExecutionTest
{
    public class Cases
    {
		public Setup TestObject;
		public Cases()
		{
			TestObject = (Setup)SurrogateBuilder.Build<Setup>();
		}

		[Fact]
        public void OrderTest1()
        {
			var retVal = TestObject.Method(123);
			Assert.Equal(970, retVal);
        }

		[Fact]
        public void OrderTest2()
        {
			var retVal = TestObject.Method2(123);
			Assert.Equal(977, retVal);
        }
    }
}
