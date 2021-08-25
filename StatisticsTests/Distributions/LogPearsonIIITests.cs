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
           // testObj.CDF();
            Assert.True(testObj.Range.IsFinite());
        }
        [Theory]
        [InlineData(.33d, 2d, 1d, 0.000000001, 0.000216134889500715)]
        [InlineData(.33d, 2d, 1d, .01d, 0.00134414494171666)]
        [InlineData(.33d, 2d, 1d, .05d, 0.0049022148945051)]
        [InlineData(.33d, 2d, 1d, .25d, 0.0746377090617247)]
        [InlineData(.33d, 2d, 1d, .5d, 1.01353836396177)]
        [InlineData(.33d, 2d, 1d, .75d, 27.1222370320434)]
        [InlineData(.33d, 2d, 1d, .95d, 11902.6602438014)]
        [InlineData(.33d, 2d, 1d, .99d, 2467533.03270371)]
        [InlineData(.33d, 2d, 1d, .999999999d, 1.57287146151741E+21)]
        [InlineData(1d, .1d, .2d, 0.000000001, 3.6535355664783)]
        [InlineData(1d, .1d, .2d, .01d, 6.05436271802718)]
        [InlineData(1d, .1d, .2d, .05d, 6.9400415588869)]
        [InlineData(1d, .1d, .2d, .25d, 8.52910108926983)]
        [InlineData(1d, .1d, .2d, .5d, 9.92362555921716)]
        [InlineData(1d, .1d, .2d, .75d, 11.6269101385278)]
        [InlineData(1d, .1d, .2d, .95d, 14.7912004150301)]
        [InlineData(1d, .1d, .2d, .99d, 17.6736017808928)]
        [InlineData(1d, .1d, .2d, .999999999d, 40.7971842209507)]
        public void LPIII_InverseCDF(double mean, double sd, double skew, double rv, double output)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.InverseCDF(rv);
            double percent = Math.Abs((output - result) / output);
            //Assert.Equal(output, result, 9);
            Assert.True(percent < .00000000001);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d, 1.57287146151741E+21)]
        [InlineData(1d, .1d, .2d, 40.79718422)]
        [InlineData(5d, 3d, 8d, 1.1889E+179)]
        [InlineData(9d, 5d, 5d, 3.568E+206)]
        [InlineData(9d, 5d, .5d, 1.07859E+47)]
        [InlineData(0d, 1d, 2d, 1.05735E+17)]
        public void LPIII_Maximums(double mean, double sd, double skew, double output)
        {
            //https://github.com/xunit/xunit/issues/1293
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Range.Max;
            double percent = Math.Abs((output - result) / output);
            Assert.True(percent<.00001);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d, 0.000216135)]
        [InlineData(1d, .1d, .2d, 3.653535566)]
        [InlineData(5d, 3d, 8d, 0)]
        [InlineData(9d, 5d, 5d, 2.4504E-124)]
        [InlineData(9d, 5d, .5d, 3.22503E-08)]
        [InlineData(0d, 1d, 2d, 0.025031829)]
        public void LPIII_Minimums(double mean, double sd, double skew, double output)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Range.Min;
            Assert.Equal(output, result, 9);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d)]
        [InlineData(1d, .1d, .2d)]
        [InlineData(5d, 3d, 8d)]
        [InlineData(9d, 5d, 5d)]
        [InlineData(9d, 5d, .5d)]
        [InlineData(0d, 1d, 2d)]
        public void LPIII_Means(double mean, double sd, double skew)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Mean;
            Assert.Equal(mean, result, 9);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d)]
        [InlineData(1d, .1d, .2d)]
        [InlineData(5d, 3d, 8d)]
        [InlineData(9d, 5d, 5d)]
        [InlineData(9d, 5d, .5d)]
        [InlineData(0d, 1d, 2d)]
        public void LPIII_StandardDeviation(double mean, double sd, double skew)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.StandardDeviation;
            Assert.Equal(sd, result, 9);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d)]
        [InlineData(1d, .1d, .2d)]
        [InlineData(5d, 3d, 8d)]
        [InlineData(9d, 5d, 5d)]
        [InlineData(9d, 5d, .5d)]
        [InlineData(0d, 1d, 2d)]
        public void LPIII_Skew(double mean, double sd, double skew)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Skewness;
            Assert.Equal(skew, result, 9);
        }
    }
}
