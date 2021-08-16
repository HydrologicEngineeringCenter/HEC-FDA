using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests.Histograms
{
    public class IHistogramFactoryTests
    {
        #region InvalidConstructorArgumentsException Tests
        /// <summary>
        /// Tests <see cref="Statistics.IHistogramFactory.Factory(double, double, int)"/> to ensure simple test data cases described in <seealso cref="HistogramNoDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the number of requested <see cref="Histogram.Bins"/> is not set to a positive value.
        /// </summary>
        /// <param name="n"> The number of requested <see cref="Histogram.Bins"/>. </param>
        [Theory]
        [InlineData(0d, 2d, 0d)]
        [InlineData(0d, 2d, -1d)]
        public void IsConstructable_NoDataLessThanOneBinRequested_Throws_InvalidConstructorArgumentsException(double min, double max, int n)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(min, max, n));
        }
        
        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(IData, double, double, int)"/> to ensure simple test data cases described in <see cref="HistogramBinnedDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the number of requested <see cref="Histogram.Bins"/> is not set to a positive value.
        /// </summary>
        /// <param name="n"> The number of requested <see cref="Histogram.Bins"/>. </param>
        [Theory]
        [InlineData(new double[] { 1 }, 0d, 2d, 0d)]
        [InlineData(new double[] { 1 }, 0d, 2d, -1d)]
        public void IsConstructable_LessThanOneBinRequested_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, int n)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(IDataFactory.Factory(data), min, max, n));
        }

        /// <summary>
        /// Tests <see cref="Statistics.IHistogramFactory.Factory(double, double, double)"/> to ensure an <see cref="InvalidConstructorArgumentsException"/> is thrown when the requested <see cref="Histogram.Bins"/> width is greater than the specified range.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, 1d, 2d)]
        [InlineData(-1d, 1d, 10d)]
        [InlineData(0.1d, 0.2d, 1d)]
        public void IsConstructable_NoDataBinWidthGreaterThanRange_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(double, double, double)"/> to ensure simple test data cases described in <seealso cref="HistogramNoDataTestData"/> throw an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Bins"/> width is negative or not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, 2d, double.NaN)]
        [InlineData(0d, 2d, double.NegativeInfinity)]
        [InlineData(0d, 2d, double.PositiveInfinity)]
        [InlineData(0d, 2d, -1d)]
        public void IsConstructable_NoDataNonFiniteOrNegativeBinWidths_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(IData, double, double, double)"/> to ensure an <see cref="Utilities.InvalidConstructorArgumentsException"/> is  thrown when the requested <see cref="Histogram.Bins"/> width is greater than the specified range.
        /// </summary>
        /// <param name="data"> 'Good' data to be binned in the histogram if an expection in not thrown. </param>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 0.5d }, 0d, 1d, 2d)]
        [InlineData(new double[] { 0.0d }, -1d, 1d, 10d)]
        [InlineData(new double[] { .15d }, 0.1d, 0.2d, 1d)]
        public void IsConstructable_BinWidthGreaterThanRange_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(IDataFactory.Factory(data), min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(IData, double, double, double)"/> to ensure that simple test data cases described in <see cref="HistogramBinnedDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Bins"/> width is negative or not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 1d }, 0d, 2d, double.NaN)]
        [InlineData(new double[] { 1d }, 0d, 2d, double.NegativeInfinity)]
        [InlineData(new double[] { 1d }, 0d, 2d, double.PositiveInfinity)]
        [InlineData(new double[] { 1d }, 0d, 2d, -1d)]
        public void IsConstructable_NonFiniteOrNegativeBinWidths_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(IDataFactory.Factory(data), min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(double, double, double)"/> to ensure simple test data cases described in <seealso cref="HistogramNoDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Minimum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(double.NaN, 2d, 1d)]
        [InlineData(double.NegativeInfinity, 2d, 1d)]
        [InlineData(double.PositiveInfinity, 2d, 1d)]
        public void IsConstructable_NoDataNonFiniteMin_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(IData, double, double, double)"/> to ensure simple test data cases described in <seealso cref="HistogramBinnedDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Minimum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 1d }, double.NaN, 2d, 1d)]
        [InlineData(new double[] { 1d }, double.NegativeInfinity, 2d, 1d)]
        [InlineData(new double[] { 1d }, double.PositiveInfinity, 2d, 1d)]
        public void IsConstructable_NonFiniteMin_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(IDataFactory.Factory(data), min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(double, double, double)"/> to ensure simple test data cases described in <seealso cref="HistogramNoDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Maximum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(0d, double.NaN, 1d)]
        [InlineData(0d, double.NegativeInfinity, 1d)]
        [InlineData(0d, double.PositiveInfinity, 1d)]
        public void IsConstructable_NoDataNonFiniteMax_Throws_InvalidConstructorArgumentsException(double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(min, max, width));
        }

        /// <summary>
        /// Tests <see cref="IHistogramFactory.Factory(IData, double, double, double)"/> to ensure simple test data cases described in <seealso cref="HistogramBinnedDataTestData"/> throw an <see cref="Utilities.InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Maximum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 1d }, 0d, double.NaN, 1d)]
        [InlineData(new double[] { 1d }, 0d, double.NegativeInfinity, 1d)]
        [InlineData(new double[] { 1d }, 0d, double.PositiveInfinity, 1d)]
        public void IsConstructable_NonFiniteMax_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => IHistogramFactory.Factory(IDataFactory.Factory(data), min, max, width));
        }
        #endregion
    }
}
