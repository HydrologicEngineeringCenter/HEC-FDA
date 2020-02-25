using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    public class NormalTests
    {
        [Theory]
        [InlineData(double.NaN, 1d, 1)]
        [InlineData(0d, double.NaN, 1)]
        [InlineData(double.NegativeInfinity, 1d, 1)]
        [InlineData(0d, double.NegativeInfinity, 1)]
        [InlineData(double.PositiveInfinity, 1d, 1)]
        [InlineData(0d, double.PositiveInfinity, 1)]
        public void InvalidParameters_Throw_InvalidConstructorArguments(double mean, double sd, int n)
        {
            Assert.Throws<Utilities.InvalidConstructorArgumentsException>(() => new Statistics.Distributions.Normal(mean, sd, n));
        }
        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(0, 1, -1)]
        public void NotPositiveSampleSize_Returns_IsValid_Equals_False(double mean, double sd, int n)
        {
            var testObj = new Statistics.Distributions.Normal(mean, sd, n);
            Assert.True(testObj.State == Utilities.IMessageLevels.Error);
        }
    }
}
