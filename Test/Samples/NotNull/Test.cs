using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.NotNullTest
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
        public void InputNotNull_OutputNull(string InputText)
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Method2(InputText));
        }

		[Theory, AutoData]
        public void InputNotNull_OutputNotNull_CheckCorrectReturnVal(string InputText)
        {
			var retVal = TestObject.Method(InputText);
			Assert.Equal(InputText, retVal);
        }

		[Fact]
        public void PropertyInput_Null()
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Property = null);
        }

		[Theory, AutoData]
        public void PropertyInput_Null_And_NotNull(string InputText)
        {
			TestObject.Property = InputText;
        }
    }
}
