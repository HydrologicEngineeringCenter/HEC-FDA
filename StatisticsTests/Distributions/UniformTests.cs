using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.Distributions
{
    public class UniformTests
    {
        [Theory]
        [InlineData(0d, 1d)]
        public void GoodDataParameters_Set_Properly(double min, double max)
        {
            var testObj = new Statistics.Distributions.Uniform(min, max);
            Assert.Equal(min, testObj.Min, 2);
            Assert.Equal(max, testObj.Max, 2);
        }
        [Theory]
        [InlineData(0d, -1d, 1)]
        [InlineData(-1d, -2d, 1)]
        [InlineData(-1d, 1d, -1)]
        public void BadValidation(double min, double max, int n)
        {
            Statistics.Distributions.Uniform dist = new Statistics.Distributions.Uniform(min, max, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 0d, 1)]
        public void MinorValidation(double min, double max, int n)
        {
            Statistics.Distributions.Uniform dist = new Statistics.Distributions.Uniform(min, max, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
            Assert.True(dist.ErrorLevel==HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Minor);
        }
        [Theory]
        [InlineData(0d, 1d, 1)]
        [InlineData(-1d, 2d, 1)]
        public void GoodValidation(double min, double max, int n)
        {
            Statistics.Distributions.Uniform dist = new Statistics.Distributions.Uniform(min, max, n);
            dist.Validate();
            Assert.False(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 1d, 0.5, 0.5)]
        [InlineData(0d, 1d, 0.25, 0.25)]
        [InlineData(0d, 1d, 0.75, 0.75)]
        [InlineData(0d, 1d, 0.95, 0.95)]
        [InlineData(1d, 2d, 0.5, 1.5)]
        [InlineData(1d, 2d, 0.25, 1.25)]
        [InlineData(1d, 2d, 0.75, 1.75)]
        [InlineData(1d, 2d, 0.95, 1.95)]
        [InlineData(1d, 3d, 0.5, 2)]
        [InlineData(1d, 3d, 0.25, 1.5)]
        [InlineData(1d, 3d, 0.75, 2.5)]
        [InlineData(1d, 3d, 0.95, 2.90)]
        public void Uniform_INVCDF(double min, double max, double prob, double expected)
        {
            var testObj = new Statistics.Distributions.Uniform(min, max);
            double result = testObj.InverseCDF(prob);
            Assert.Equal(result, expected, 5);
        }
    }
}
