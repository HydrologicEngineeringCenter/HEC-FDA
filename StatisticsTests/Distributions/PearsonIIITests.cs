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
        [InlineData(4434.739, 1681.290, 1.323, 1, .998, 11941.46931887674872996286)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .995, 10763.40989302623711409979)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .98, 8933.28373736705179908313)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .95, 7676.20361345045967027545)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .5, 4075.17124630542957675061)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .1, 2654.30189278692023435724)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .05, 2421.96631832806406237069)] //USGS-R SMWR
        [InlineData(4434.739, 1681.290, 1.323, 1, .01, 2135.36487671968916401966)] //USGS-R SMWR
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
