using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Distributions
{

    /// <summary>
    /// This class tests the LogPearsonIII Distribution in the Statistics Library <see cref="Statistics.Distributions.ShiftedGamma"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ShiftedGammaTests
    {
        [Theory]
        [InlineData(3.370, 1/614.880, 0, 0.5, 1868.6)]
        public void ShiftedGamma_InverseCDF(double alpha, double beta, double shift, double p, double output)
        {
            var testObj = new Statistics.Distributions.ShiftedGamma(alpha, beta, shift);
            double result = testObj.InverseCDF(p);
            double percent = Math.Abs((output - result) / output);
            //Assert.Equal(output, result, 9);
            Assert.True(percent < .01);
        }
    }
}
