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
    public class PearsonIIITests
    {
        [Theory]
        [InlineData(4434.739, 1681.290, 1.323, 1, .5, 4075.2)]
        public void PearsonIII_InverseCDF(double mean, double standardDeviation, double skew, int sampleSize, double p, double output)
        {
            var testObj = new Statistics.Distributions.PearsonIII(mean, standardDeviation, skew, sampleSize);
            double result = testObj.InverseCDF(p);
            double percent = Math.Abs((output - result) / output);
            //Assert.Equal(output, result, 9);
            Assert.True(percent < .01);
        }
    }
}
