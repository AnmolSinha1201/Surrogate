using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.ClampTest
{
    public class Cases
    {
		public Setup TestObject;
		public Cases()
		{
			TestObject = (Setup)SurrogateBuilder.Build<Setup>();
		}

		[Fact]
        public void InputNull()
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Method(null));
        }

		[Theory, AutoData]
        public void InputNotNull_OutputNull(int Input)
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Method2(Input));
        }

		[Theory, AutoData]
        public void InputNotNull_OutputNotNull_OutputClamped(int Input)
        {
			var retVal = TestObject.Method3(Input);
			Assert.Equal(retVal, Math.Clamp(Input, 5, 10));
        }

		[Theory, AutoData]
        public void InputNotNull_OutputNotNull_InputClamped(int Input)
        {
			var retVal = TestObject.Method4(Input);
			Assert.Equal(retVal, Math.Clamp(Input, 5, 10));
        }
    }
}
