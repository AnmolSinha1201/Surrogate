using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.IPropertySurrogateTest
{
    public class Cases
    {
		public Setup TestObject;
		public Cases()
		{
			TestObject = SurrogateBuilder.Build<Setup>();
		}

		[Theory, AutoData]
        public void SetTest(int InputNum)
        {
			Assert.Equal(0, TestObject.PropertySet);
			TestObject.PropertySet = InputNum;
			Assert.Equal(789, TestObject.PropertySet);
        }

		[Fact]
        public void GetTest()
        {
			Assert.Equal(123, TestObject.PropertyGet);
        }
    }
}
