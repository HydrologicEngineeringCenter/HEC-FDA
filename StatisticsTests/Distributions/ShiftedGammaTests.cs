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
        [InlineData(3.370, 614.880, 1.2, 0.998, 6806.79323611756262835115)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.995, 6098.80793105717748403549)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.98, 4985.97495879006964969449)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.95, 4209.20458921972476673545)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.5, 1872.38437289608009450603)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.1, 821.04217088459745355067)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.05, 623.93076495493585298391)]//USGS-R SMWR
        [InlineData(3.370, 614.880, 1.2, 0.01, 351.50666218164383280964)]//USGS-R SMWR
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
