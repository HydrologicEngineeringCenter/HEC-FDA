using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Distributions
{
    /// <summary>
    /// This class tests the LogPearsonIII Distribution in the Statistics Library <see cref="Statistics.Distributions.LogPearsonIII"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogPearsonIIITests
    {
        /// <summary>
        /// Tests that invalid parameter values cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. The <see cref="Statistics.Distributions.LogPearsonIII.RequiredParameterization(bool)"/> function prints a list of the parameterization requirements.
        /// </summary>
        /// <param name="mean"> <see cref="Statistics.Distributions.LogPearsonIII.Mean"/> parameter for the <see cref="Statistics.Distributions.LogPearsonIII"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="sd"> <see cref="Statistics.Distributions.LogPearsonIII.StandardDeviation"/> parameter for the <see cref="Statistics.Distributions.LogPearsonIII"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="skew"> <see cref="Statistics.Distributions.LogPearsonIII.Skewness"/> parameter for the <see cref="Statistics.Distributions.LogPearsonIII"/> distribution, which is a log base 10 representation of a random number. Only non-finite or non-numerical values are expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="n"> <see cref="Statistics.Distributions.LogPearsonIII.SampleSize"/> parameter. Any non-positive value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        [Theory]
        [InlineData(0d, 1d, 1d, 1)]
        [InlineData(1d, 0d, 1d, 1)]
        [InlineData(1d, 1d, 1d, 0)]
        [InlineData(-1d, 1d, 1d, 1)]
        [InlineData(1d, -1d, 1d, 1)]
        [InlineData(1d, 1d, 1d, -1)]
        [InlineData(double.NaN, 1d, 1d, 1)]
        [InlineData(1d, double.NaN, 1d, 1)]
        [InlineData(1d, 1d, double.NaN, 1)]
        [InlineData(double.NegativeInfinity, 1d, 1d, 1)]
        [InlineData(1d, double.NegativeInfinity, 1d, 1)]
        [InlineData(1d, 1d, double.NegativeInfinity, 1)]
        [InlineData(double.PositiveInfinity, 1d, 1d, 1)]
        [InlineData(1d, double.PositiveInfinity, 1d, 1)]
        [InlineData(1d, 1d, double.PositiveInfinity, 1)]
        public void InvalidParameterValues_Throw_InvalidConstructorArgumentsException(double mean, double sd, double skew, int n)
        {
            Assert.Throws<Utilities.InvalidConstructorArgumentsException>(() => new Statistics.Distributions.LogPearsonIII(mean: mean, standardDeviation: sd, skew: skew, sampleSize: n));
        }
        [Fact]
        public void GoodData_Returns_ValidFiniteMin()
        {
            var testObj = new Statistics.Distributions.LogPearsonIII(mean: 1, standardDeviation: 0.01, skew: -2);
            var min = testObj.Range.Min;
            Assert.True(min.IsFinite());
        }

    }
}
