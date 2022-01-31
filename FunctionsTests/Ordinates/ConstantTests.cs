using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace FunctionsTests.Ordinates
{
    [ExcludeFromCodeCoverage]
    public class ConstantTests
    {
        /// <summary>
        /// Tests the Constant constructor with double value.
        /// </summary>
        /// <param name="value"></param>
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

        /// <summary>
        /// Tests the range property returns correct value.
        /// </summary>
        /// <param name="value"></param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99)]
        [InlineData(Double.MaxValue)]
        [InlineData(Double.MinValue)]
        public void Range_GoodInput_Returns_RangeEqualToValue(double value)
        {
            Constant constant = new Constant(value);
            Assert.True(constant.Range.Min == constant.Range.Max && constant.Range.Min == value);
        }

        /// <summary>
        /// Tests the Equals method returns true for Constants that have been given the same value.
        /// </summary>
        /// <param name="valForConstant1"></param>
        /// <param name="valForConstant2"></param>
        [Theory]
        [InlineData(.33333,.33333)]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        [InlineData(99.7857, 99.7857)]
        [InlineData(Double.MaxValue, Double.MaxValue)]
        [InlineData(Double.MinValue, Double.MinValue)]
        public void Equals_GoodInput_Returns_Bool(double valForConstant1, double valForConstant2)
        {
            Constant constant1 = new Constant(valForConstant1);
            Constant constant2 = new Constant(valForConstant2);

            Assert.True(constant1.Equals(constant2));
        }

        /// <summary>
        /// Tests the Equals method returns false for Constants that have been given the different value.
        /// </summary>
        /// <param name="valForConstant1"></param>
        /// <param name="valForConstant2"></param>
        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 25)]
        public void Equals_BadInput_Returns_Bool(double valForConstant1, double valForConstant2)
        {
            Constant constant1 = new Constant(valForConstant1);
            Constant constant2 = new Constant(valForConstant2);

            Assert.False(constant1.Equals(constant2));
        }

        /// <summary>
        /// Tests that the Value property returns the value passed to the Constant.
        /// </summary>
        /// <param name="value"></param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99.7857)]
        [InlineData(Double.MaxValue)]
        [InlineData(Double.MinValue)]
        public void Value_GoodInput_Returns_Double(double value)
        {
            Constant constant = new Constant(value);
            Assert.True(constant.Value() == value);
        }
    }
}
