using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Statistics.Distributions;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    [Trait("Category", "Unit")]
    public class NormalTests
    {
        [Theory]
        [InlineData(0d, -1d, 1)]
        [InlineData(-1d, -2d, 1)]
        [InlineData(-1d, 1d, -1)]
        [InlineData(1d, 1d, 0)]
        public void BadValidation(double mean, double sd, int n)
        {
            Normal dist = new Statistics.Distributions.Normal(mean, sd, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 1d, 1)]
        [InlineData(-1d, 2d, 1)]
        public void GoodValidation(double mean, double sd, int n)
        {
            Normal dist = new Statistics.Distributions.Normal(mean, sd, n);
            dist.Validate();
            Assert.False(dist.HasErrors);
        }
        //https://en.wikipedia.org/wiki/Standard_normal_table
        [Theory]
        [InlineData(0d, 1,0, .99865, 3d)]
        [InlineData(0d, 1,0, .97725, 2d)]
        [InlineData(0d, 1,0, .84134, 1d)]
        [InlineData(0d, 1, 0, .5, 0d)]
        [InlineData(0d, 1,0, .15866, -1d)]
        [InlineData(0d, 1,0, .02275, -2d)]
        [InlineData(0d, 1,0, .00135, -3d)]
        public void StandardNormal_InverseCDF(double mean, double sd, int n, double p, double z)
        {
            var testObj = new Normal(mean, sd, n);
            Assert.Equal(z,testObj.InverseCDF(p),3);
        }

        [Theory]
        [InlineData(0d, 1, -2d,2d, 0, .99865, 1.97711642477487)]
        [InlineData(0d, 1, -2d, 2d, 0, .97725, 1.70144538422304)]
        [InlineData(0d, 1, -2d, 2d, 0, .84134, 0.937648811119616)]
        [InlineData(0d, 1, -2d, 2d, 0, .5, 0d)]
        [InlineData(0d, 1, -2d, 2d, 0, .15866, -0.937648811119617)]
        [InlineData(0d, 1, -2d, 2d, 0, .02275, -1.70144538422304)]
        [InlineData(0d, 1, -2d, 2d, 0, .00135, -1.97711642477487)]
        public void Truncated_StandardNormal_InverseCDF(double mean, double sd, double min, double max, int n, double p, double z)
        {
            var testObj = new TruncatedNormal(mean, sd, min, max, n);
            Assert.Equal(z, testObj.InverseCDF(p), 4);
        }
        //source: https://en.wikipedia.org/wiki/Standard_normal_table
        [Theory]
        [InlineData(1,2,2.36,.752)]
        [InlineData(2,3,5.6,.885)]
        [InlineData(3,4,12.6,.992)]
        [InlineData(4,5,14.5,.982)]
        [InlineData(5,6,16.4,.971)]
        public void Normal_CDF(double mean, double standardDeviation, double xValue, double expected)
        {
            Normal normal = new Normal(mean, standardDeviation);
            double actual = normal.CDF(xValue);
            Assert.Equal(expected, actual, 3);
        }
    }
}
