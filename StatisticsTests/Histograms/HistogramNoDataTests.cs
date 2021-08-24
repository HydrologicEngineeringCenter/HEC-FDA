using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Histograms
{
    /// <summary>
    /// This class only exists to provide details on the simple test data cases used by the <see cref="HistogramNoDataTests"/> test class.
    /// </summary>
    public static class HistogramNoDataTestData
    {
        /// <summary>
        /// A <see cref="HistogramNoData"/> with a <see cref="Histogram.Minimum"/>: 0, <see cref="Histogram.Maximum"/>: 2 and constant <see cref="IBin"/> width of 1./>
        /// </summary>
        internal static object[] SimpleTestData1 => new object[3] { 0d, 2d, 1d };
    }

    /// <summary>
    /// Test the <see cref="HistogramNoData"/> class. Simple test data cases are described in <seealso cref="HistogramNoDataTestData"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HistogramNoDataTests
    {
        #region InvalidConstructorArgumentsException Tests
        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure that when the requested <see cref="Histogram.Bins"/> width is greater than the specified range an <see cref="InvalidConstructorArgumentsException"/> is thrown.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, 1d, 2d)]
        [InlineData(-1d, 1d, 10d)]
        [InlineData(0.1d, 0.2d, 1d)]
        public void IsConstructable_BinWidthGreaterThanRange_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramNoData(min, max, width));
        }

        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramNoDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Bins"/> width is negative or not finite .
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, 2d, double.NaN)]
        [InlineData(0d, 2d, double.NegativeInfinity)]
        [InlineData(0d, 2d, double.PositiveInfinity)]
        [InlineData(0d, 2d, -1d)]
        public void IsConstructable_NonFiniteOrNegativeBinWidths_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramNoData(min, max, width));
        }

        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramNoDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Minimum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(double.NaN, 2d, 1d)]
        [InlineData(double.NegativeInfinity, 2d, 1d)]
        [InlineData(double.PositiveInfinity, 2d, 1d)]
        public void IsConstructable_NonFiniteMin_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramNoData(min, max, width));
        }

        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramNoDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Maximum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, double.NaN, 1d)]
        [InlineData(0d, double.NegativeInfinity, 1d)]
        [InlineData(0d, double.PositiveInfinity, 1d)]
        public void IsConstructable_NonFiniteMax_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramNoData(min, max, width));
        }
        #endregion

        #region Number of Bins Tests
        /// <summary>
        /// Tests that for a variety of good data examples the expected number of <see cref="Histogram.Bins"/> is returned, where the expected number of bins is equal to the <see cref="Histogram.Maximum"/> minus the <see cref="Histogram.Minimum"/> divided by the provided <paramref name="widths"/> rounded up to the nearest integer value.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="widths"></param>
        [Theory]
        [InlineData(0d, 1d, 1d)]
        [InlineData(-1d, 1d, 1d)]
        [InlineData(0d, 1d, 0.75)]
        [InlineData(0d, 1d, 0.50)]
        [InlineData(0d, 1000d, 0.001)]
        [InlineData(0.50, 1.50, 0.50)]
        public void BinsCount_GoodDataEmptyHistogram_Returns_ExpectedNumberOfBins(double min, double max, double widths)
        {
            var testObj = new HistogramNoData(min, max, widths);
            int expected = Convert.ToInt32(Math.Ceiling((max - min) / widths));
            double requiredPrecision = 0.01, actualPrecision = Math.Abs((testObj.Bins.Length - expected) / expected);
            Assert.True(actualPrecision < requiredPrecision);
        }
        #endregion

        #region Properties Tests
        
        /// <summary>
        /// Tests that the <see cref="HistogramNoData"/> <see cref="Histogram.SampleSize"/> property is set to 0 for simple data test {min: 0, max: 1, width: 1}.
        /// </summary>
        [Fact]
        public void SampleSize_GoodDataEmptyHistogram_Returns_0()
        {
            var testobj = new HistogramNoData(0d, 1d, 1d);
            Assert.Equal(0, testobj.SampleSize);
        }

        /// <summary>
        /// Tests that the <see cref="HistogramNoData"/> <see cref="Histogram.Mean"/> property is set to <see cref="Double.NaN"/> for simple data test {min: 0, max: 1, width: 1}.
        /// </summary>
        [Fact]
        public void Mean_SimpleGoodDataEmptyHistogramCase_Returns_NaN()
        {
            var testobj = new HistogramNoData(0d, 1d, 1d);
            Assert.Equal(double.NaN, testobj.Mean);
        }
        
        #endregion
    }
}
