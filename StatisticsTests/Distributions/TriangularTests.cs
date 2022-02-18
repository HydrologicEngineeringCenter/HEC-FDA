using Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    public class TriangularTests
    {
        [Theory]
        [InlineData(0d,5d, 10d, 0.0, 0.0)]
        [InlineData(0d,5d, 10d, 0.75, 6.464466)]
        [InlineData(0d,5d, 10d, 0.25, 3.535534)]
        [InlineData(0d,5d, 10d, 0.5, 5.0)]
        [InlineData(0d,5d, 10d, 1.0, 10.0)]
        public void Triangular_INVCDF(double min, double mostlikely, double max, double prob, double expected)
        {
            var testObj = new Statistics.Distributions.Triangular(min, mostlikely, max);
            double result = testObj.InverseCDF(prob);
            Assert.Equal(result, expected, 5);
        }
        [Theory]
        [InlineData(0d, -1d, -2d, 1)]
        [InlineData(-1d, -2d, 3d, 1)]
        [InlineData(-1d, 4d, 3d, 1)]
        [InlineData(-1d, 1d, 2d, -1)]
        public void BadValidation(double min, double mostlikely, double max, int n)
        {
            Statistics.Distributions.Triangular dist = new Statistics.Distributions.Triangular(min, mostlikely, max, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
        }
        [Theory]
        [InlineData(0d, 0d, 0d, 1)]
        [InlineData(0d, 0d, 3d, 1)]
        [InlineData(-1d, 3d, 3d, 1)]
        public void MinorValidation(double min, double mostlikely, double max, int n)
        {
            Statistics.Distributions.Triangular dist = new Statistics.Distributions.Triangular(min, mostlikely, max, n);
            dist.Validate();
            Assert.True(dist.HasErrors);
            Assert.True(dist.ErrorLevel == HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Minor);
        }
        [Theory]
        [InlineData(0d, 1d, 2d, 1)]
        [InlineData(-1d, 2d, 3d, 1)]
        public void GoodValidation(double min,double mostlikely, double max, int n)
        {
            Statistics.Distributions.Triangular dist = new Statistics.Distributions.Triangular(min, mostlikely, max, n);
            dist.Validate();
            Assert.False(dist.HasErrors);
        }
    }
}
