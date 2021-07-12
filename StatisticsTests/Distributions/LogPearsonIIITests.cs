using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Distributions
{
    /// <summary>
    /// This class tests the LogPearsonIII Distribution in the Statistics Library <see cref="Statistics.Distributions.LogPearson3"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogPearsonIIITests
    {
        /// <summary>
        /// Tests that invalid parameter values cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. The <see cref="Statistics.Distributions.LogPearson3.RequiredParameterization(bool)"/> function prints a list of the parameterization requirements.
        /// </summary>
        /// <param name="mean"> <see cref="Statistics.Distributions.LogPearson3.Mean"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="sd"> <see cref="Statistics.Distributions.LogPearson3.StandardDeviation"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="skew"> <see cref="Statistics.Distributions.LogPearson3.Skewness"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Only non-finite or non-numerical values are expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="n"> <see cref="Statistics.Distributions.LogPearson3.SampleSize"/> parameter. Any non-positive value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        [Theory]
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
            Assert.Throws<Utilities.InvalidConstructorArgumentsException>(() => new Statistics.Distributions.LogPearson3(mean: mean, standardDeviation: sd, skew: skew, sampleSize: n));
        }
        [Theory]
        [InlineData(11d, 1d, 1d)]
        [InlineData(1d, 11d, 1d)]
        [InlineData(1d, 1d, 11d)]
        public void TooBigParameterValues_Throw_InvalidConstructorArguementsException(double mean, double sd, double skew)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new Statistics.Distributions.LogPearson3(mean, sd, skew));
        }

        /// <summary>
        /// Tests that valid parameters return a finite <see cref="IRange{T}"/> through the <see cref="Statistics.Distributions.LogPearson3._ProbabilityRange"/> field which is generated to restrit the unbounded distribution to a finite range.
        /// </summary>
        /// <param name="mean"> <see cref="Statistics.Distributions.LogPearson3.Mean"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="sd"> <see cref="Statistics.Distributions.LogPearson3.StandardDeviation"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="skew"> <see cref="Statistics.Distributions.LogPearson3.Skewness"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Only non-finite or non-numerical values are expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        [Fact]
        public void GoodData_Returns_ValidFiniteRange()
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean: 1, standardDeviation: 0.01, skew: -2);
            var min = testObj.Range.Min;
            Assert.True(min.IsFinite());
        }
        [Theory]
        [InlineData(1d, 2d, 2d)]
        [InlineData(1d, 3d, 3d)]
        [InlineData(1d, 4d, 4d)]
        [InlineData(1d, 5d, 5d)]
        [InlineData(1d, 2d, -2d)]
        [InlineData(1d, 3d, -3d)]
        [InlineData(1d, 4d, -4d)]
        [InlineData(1d, 5d, -5d)]
        [InlineData(1d, 1d, 1d)]
        [InlineData(2d, 2d, 2d)]
        [InlineData(3d, 3d, 3d)]
        [InlineData(4d, 4d, 4d)]
        [InlineData(5d, 5d, 5d)]
        [InlineData(6d, 6d, 6d)]
        public void GoodData_Returns_FiniteRange(double mean, double sd, double skew)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            Assert.True(testObj.Range.IsFinite());
        }
        /// <summary>
        /// Tests that valid parameters return the <see cref="Statistics.Distributions.LogPearson3"/> in a non-error state. A <see cref="Statistics.Distributions.LogPearson3.State"/> should be <see cref="IMessageLevels.Message"/> since a message is added describing the finite range of the object.
        /// </summary>
        /// <param name="mean"> <see cref="Statistics.Distributions.LogPearson3.Mean"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="sd"> <see cref="Statistics.Distributions.LogPearson3.StandardDeviation"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Any non-positive, non-finite or non-numerical value is expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="skew"> <see cref="Statistics.Distributions.LogPearson3.Skewness"/> parameter for the <see cref="Statistics.Distributions.LogPearson3"/> distribution, which is a log base 10 representation of a random number. Only non-finite or non-numerical values are expected to cause an <see cref="Utilities.InvalidConstructorArgumentsException"/> to be thrown. </param>
        /// <param name="n"></param>
        [Theory]
        [InlineData(1d, 1d, 1d, 100)]
        [InlineData(2d, 2d, 2d, 100)]
        [InlineData(3d, 3d, 3d, 100)]
        [InlineData(4d, 4d, 4d, 100)]
        [InlineData(5d, 5d, 5d, 100)]
        [InlineData(6d, 6d, 6d, 100)]
        public void GoodData_Returns_NoErrorState(double mean, double sd, double skew, int n)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew, n);
            Assert.True(testObj.State < IMessageLevels.Error);
        }

        [Theory]
        [InlineData(2d, 2d, 2d)]
        public void GoodData_Returns_FiniteRange2(double mean, double sd, double skew)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            testObj.CDF()
            Assert.True(testObj.Range.IsFinite());
        }

    }
}
