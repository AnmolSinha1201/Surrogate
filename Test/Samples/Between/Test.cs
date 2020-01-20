using System;
using AutoFixture.Xunit2;
using Surrogate.Samples;
using Xunit;

namespace Surrogate.Tests.BetweenTest
{
    public class Cases
    {
		public Setup TestObject;
		public Random RGen;
		public Cases()
		{
			TestObject = (Setup)SurrogateBuilder.Build<Setup>();
			RGen = new Random();
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

		[Theory]
		[InlineAutoData(11)]
		[InlineAutoData(4)]
        public void Input_OutputOutOfRange(int Input)
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Method3(Input));
        }

		[Theory]
		[InlineAutoData(5)]
		[InlineAutoData(10)]
		[InlineAutoData(8)]
        public void Input_OutputInRange(int Input)
        {
			var retVal = TestObject.Method3(Input);
			Assert.Equal(Input, retVal);
        }


		[Theory]
		[InlineAutoData(11)]
		[InlineAutoData(4)]
        public void InputOutOfRange_Output(int Input)
        {
			Assert.ThrowsAny<Exception>(() => TestObject.Method4(Input));
        }

		[Theory]
		[InlineAutoData(5)]
		[InlineAutoData(10)]
		[InlineAutoData(8)]
        public void InputInRange_Output(int Input)
        {
			var retVal = TestObject.Method4(Input);
			Assert.Equal(Input, retVal);
        }
    }
}
