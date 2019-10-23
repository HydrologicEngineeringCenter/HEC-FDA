using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionsTests.Ordinates
{
    public class ConstantTests
    {

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99)]
        [InlineData(Double.MaxValue)]
        [InlineData(Double.MinValue)]
        public void Constant_GoodInput_Returns_Constant(double value)
        {
            Constant constant = new Constant(value);
            Assert.NotNull(constant);
        }


    }
}
