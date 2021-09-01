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
        [InlineData(.33d, 2d, 1d, 0.0000001d, 0.00023424651174493445)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .01d, 0.00142314347635817923)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .05d, 0.00496956524453536289)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .25d, 0.07333650550108276878)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .5d, 1.00475632515959278912)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .75d, 27.50600294310048710145)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .95d, 12124.29592557328032853547)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, .99d, 2372015.45751278521493077278)]//USGS-R SMWR
        [InlineData(.33d, 2d, 1d, 0.9999999d, 207124976093093232640d)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, 0.0000001d, 3.66134726526910103672)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .01d, 6.05563695368426380128)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .05d, 6.94053218004162530974)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .25d, 8.52821888415644657755)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .5d, 9.92358626175317404261)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .75d, 11.62815802852201230166)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .95d, 14.79010797131094889778)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, .99d, 17.66955511306299086982)]//USGS-R SMWR
        [InlineData(1d, .1d, .2d, 0.9999999d, 40.67518055224932282954)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .998d, 18878.87515053270180942491)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .995d, 14246.58825980164874636102)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .99d, 11408.83966308754315832630)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .98d, 9043.72657283687294693664)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .95d, 6511.95816420457322237780)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .90d, 4961.12702987368902540766)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .8d, 3656.87315507261564562214)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .5d, 2191.79779904862152761780)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .2d, 1435.93911608508096833248)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .1d, 1189.92079576230275961279)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .05d, 1035.43101823480742496031)]//USGS-R SMWR
        [InlineData(3.368d, .246d, .668d, .01d, 827.66401592971760692308)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .998d, 68797.08324508435907773674)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .995d, 49664.22500837691040942445)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .99d, 38021.49704154192295391113)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .98d, 28448.56144728167782886885)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .95d, 18478.30725197442734497599)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .90d, 12639.18321421466134779621)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .8d, 8015.72587415261750720674)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .5d, 3400.36436700901276708464)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .2d, 1468.46588154357164057728)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .1d, 953.48702080914085854602)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .05d, 669.90326710880833616102)]//USGS-R SMWR
        [InlineData(3.537d, .438d, .075d, .01d, 348.55777089714820249355)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .998d, 32506.82384690305480035022)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .995d, 24602.16720938941944041289)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .99d, 19284.35566198145897942595)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .98d, 14566.43229235407852684148)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .95d, 9288.03872218876131228171)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .90d, 6041.78073029362531087827)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .8d, 3451.04415806516362863476)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .5d, 1043.47618679165134381037)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .2d, 265.80156381679307742161)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .1d, 121.13255664011704482164)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .05d, 60.95393539477918665170)]//USGS-R SMWR
        [InlineData(2.966d, .668d, -.473d, .01d, 15.29554262000421083201)]//USGS-R SMWR
        public void LPIII_InverseCDF(double mean, double sd, double skew, double rv, double output)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.InverseCDF(rv);
            double fraction = Math.Abs((output - result) / output);
            Assert.True(fraction < .01);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d, 2.071250e+20)] //USGS-R SMWR
        [InlineData(1d, .1d, .2d, 4.067518e+01)] //USGS-R SMWR
        [InlineData(5d, 3d, 8d, 7.163528e+136)] //USGS-R SMWR
        [InlineData(9d, 5d, 5d, 2.751981e+159)] //USGS-R SMWR
        [InlineData(9d, 5d, .5d, 3.571023e+46)] //USGS-R SMWR
        [InlineData(0d, 1d, 2d, 1.312489e+15)] //USGS-R SMWR
        public void LPIII_Maximums(double mean, double sd, double skew, double output)
        {
            //https://github.com/xunit/xunit/issues/1293
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Range.Max;
            double fraction = Math.Abs((output - result) / output);

            Assert.True(fraction<.01);
        }
        [Theory]
        [InlineData(.33d, 2d, 1d, 0.00023424651174493445)] //USGS-R SMWR
        [InlineData(1d, .1d, .2d, 3.66134726526910103672)] //USGS-R SMWR
        [InlineData(5d, 3d, 8d, 17782.79410038922651438043)]//USGS-R SMWR
        [InlineData(9d, 5d, 5d, 10000000)]//USGS-R SMWR
        [InlineData(9d, 5d, .5d, 0.00000004890397239656)]//USGS-R SMWR
        [InlineData(0d, 1d, 2d, 0.10000002302585474234)]//USGS-R SMWR
        public void LPIII_Minimums(double mean, double sd, double skew, double output)
        {
            var testObj = new Statistics.Distributions.LogPearson3(mean, sd, skew);
            double result = testObj.Range.Min;
            double fraction = Math.Abs((output - result) / output);
    
                Assert.True(fraction < .001);
     
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
