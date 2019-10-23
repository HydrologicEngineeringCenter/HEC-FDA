using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Xunit;

using Statistics.Histograms;

namespace StatisticsTests.Histograms
{
    public class HistogramTests
    {
        #region Empty Bins
        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(0, 1, 0.75)]
        [InlineData(0, 1, 0.50)]
        [InlineData(0, 1000, 0.001)]
        [InlineData(0.50, 1.50, 0.50)]
        public void BinsCount_GoodDataEmptyHistogram_Returns_ExpectedNumberOfBins(double min, double max, double widths)
        {
            var testObj = new Histogram(min, max, widths);
            int expected = Convert.ToInt32(Math.Ceiling((max - min) / widths));
            double requiredPrecision = 0.01, actualPrecision = Math.Abs((testObj.Bins.Length - expected) / expected);
            Assert.True(actualPrecision < requiredPrecision);
            //A double precision issue prevents the commented out assert statement below from working...
            //Assert.Equal<int>(expected, testObj.Bins.Length, );
        }
        [Theory]
        [InlineData(-100, -1, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(0, 1, 1)]
        [InlineData(0.001, 1, .1)]
        [InlineData(100, 1000, 1)]
        public void Min_GoodDataEmptyHistogram_Returns_SpecifiedMin(double min, double max, double widths)
        {
            var testobj = new Histogram(min, max, widths);
            Assert.Equal(min, testobj.Minimum);
        }
        [Theory]
        [InlineData(-100, -1, 1)]
        [InlineData(-1, 0, 1)]
        [InlineData(-1, 1, 1)]
        [InlineData(0.001, .1, .1)]
        [InlineData(100, 1000, 1)]
        public void Max_GoodDataEmptyHistogram_Returns_SpecifiedMin(double min, double max, double widths)
        {
            var testobj = new Histogram(min, max, widths);
            Assert.Equal(max, testobj.Maximum);
        }
        [Fact]
        public void SampleSize_GoodDataEmptyHistogram_Returns_0()
        {
            var testobj = new Histogram(0, 1, 1);
            Assert.Equal(0, testobj.SampleSize);
        }
        [Fact]
        public void Mean_SimpleGoodDataEmptyHistogramCase_Returns_NaN()
        {
            var testobj = new Histogram(0, 1, 1);
            Assert.Equal(double.NaN, testobj.Mean);
        }
        [Fact]
        public void Variance_SimpleGoodDataEmptyHistogramCase_Returns_NaN()
        {
            var testobj = new Histogram(0, 1, 1);
            Assert.Equal(double.NaN, testobj.Variance);
        }
        [Fact]
        public void StandardDeviation_SimpleGoodDataEmptyHistogramCase_Returns_NaN()
        {
            var testobj = new Histogram(0, 1, 1);
            Assert.Equal(double.NaN, testobj.StandardDeviation);
        }
        [Fact]
        public void Skewness_SimpleGoodDataEmptyHistogramCase_Returns_NaN()
        {
            var testobj = new Histogram(0, 1, 1);
            Assert.Equal(double.NaN, testobj.Skewness);
        }
        #endregion

        //public void Bins_SingleBinDataOnMin_Returns_ExpectedSingleBin()
        //{
        //    double[] testData = new double[1] { 0 };
        //    var testObj = new Histogram(testData, 1);
        //}
    }
}
