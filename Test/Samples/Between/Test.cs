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

		[Fact]
        public void Input_OutputGreater()
        {
			var Input = RGen.Next(11, 100);
			Assert.ThrowsAny<Exception>(() => TestObject.Method3(Input));
        }

		[Fact]
        public void Input_OutputGreaterEqual()
        {
			var Input = 10;
			var retVal = TestObject.Method3(Input);
			Assert.Equal(Input, retVal);
        }

		[Fact]
        public void Input_OutputLesser()
        {
			var Input = RGen.Next(4);
			Assert.ThrowsAny<Exception>(() => TestObject.Method3(Input));
        }

		[Fact]
        public void Input_OutputLesserEqual()
        {
			var Input = 5;
			var retVal = TestObject.Method3(Input);
			Assert.Equal(Input, retVal);
        }

		[Fact]
        public void Input_OutputRange()
        {
			var Input = RGen.Next(5, 10);
			var retVal = TestObject.Method3(Input);
			Assert.Equal(Input, retVal);
        }

		[Fact]
        public void InputGreater_Output()
        {
			var Input = RGen.Next(11, 100);
			Assert.ThrowsAny<Exception>(() => TestObject.Method4(Input));
        }

		[Fact]
        public void InputGreaterEqual_Output()
        {
			var Input = 10;
			var retVal = TestObject.Method4(Input);
			Assert.Equal(Input, retVal);
        }

		[Fact]
        public void InputLesser_Output()
        {
			var Input = RGen.Next(4);
			Assert.ThrowsAny<Exception>(() => TestObject.Method4(Input));
        }

		[Fact]
        public void InputLesserEqual_Output()
        {
			var Input = 5;
			var retVal = TestObject.Method4(Input);
			Assert.Equal(Input, retVal);
        }

		[Fact]
        public void InputRange_Output()
        {
			var Input = RGen.Next(5, 10);
			var retVal = TestObject.Method4(Input);
			Assert.Equal(Input, retVal);
        }
    }
}
