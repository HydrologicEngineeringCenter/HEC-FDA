using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    public class UniformTests
    {
        [Theory]
        [InlineData(0d, 1d)]
        public void GoodDataParameters_Return_ExpectedMean(double min, double max)
        {
            var testObj = new Statistics.Distributions.Uniform(0, 1);
            Assert.Equal(0.5d, testObj.Mean, 2);
        }

    }
}
