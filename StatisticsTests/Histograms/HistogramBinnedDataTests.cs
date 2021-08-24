using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using Xunit;

using Statistics;
using Statistics.Histograms;
using Utilities;
using System.Diagnostics.CodeAnalysis;

namespace StatisticsTests.Histograms
{
    /// <summary>
    /// This class only exists to provide details on the simple test data cases used by the <see cref="HistogramBinnedDataTests"/> test class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class HistogramBinnedDataTestData
    {
        /// <summary>
        /// A <see cref="HistogramBinnedData"/> with a <see cref="Histogram.Minimum"/>: 0, <see cref="Histogram.Maximum"/>: 2, constant <see cref="IBin"/> width of 1 and <see cref="IData.Elements"/>: { 1 }./>
        /// </summary>
        internal static object[] SimpleTestData1 => new object[4] { new double[] { 1d }, 0d, 2d, 1d };
        internal static object[] SimpleTestData2 => new object[2] { new double[] { 1d, 2d, 3d }, 1d };
    }

    /// <summary>
    /// Tests the <see cref="HistogramBinnedData"/> class. Simple test data cases are described in <seealso cref="HistogramBinnedDataTestData"/>./>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HistogramBinnedDataTests
    {
        #region Constructor Tests

        #region Histogram Range Tests
        #endregion

        #region Binned Data
        /// <summary>
        /// Tests that simple test data cases described in <seealso cref="HistogramBinnedDataTestData"/> return the expected number of <see cref="Histogram.Bins"/>.
        /// </summary>
        /// <param name="data"> Array of doubles used to generate <see cref="IData"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="min"> The <see cref="Histogram.Minimum"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="max"> The <see cref="Histogram.Maximum"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="width"> The constant <see cref="IBin"/> width used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="expectedValue"> The expected number of <see cref="Histogram.Bins"/>. </param>
        [Theory]
        [InlineData(new object[5] { new double[1] { 1d }, 0d, 2d, 1d, 2 })]
        public void SampleSize_SimpleTestData_Returns_ExpectedNumberOfBins(double[] data, double min, double max, double width, int expectedValue)
        {
            var testObj = new HistogramBinnedData(IDataFactory.Factory(data), min, max, width);
            Assert.Equal(expectedValue, testObj.Bins.Length);
        }
        /// <summary>
        /// Tests that simple test data cases described in <seealso cref="HistogramBinnedDataTestData"/> return the expected <see cref="Histogram.SampleSize"/>.
        /// </summary>
        /// <param name="data"> Array of doubles used to generate <see cref="IData"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="min"> The <see cref="Histogram.Minimum"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="max"> The <see cref="Histogram.Maximum"/> used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="width"> The constant <see cref="IBin"/> width used to construct the <see cref="HistogramBinnedData"/> test data object. </param>
        /// <param name="expectedValue"> The expected <see cref="Histogram.SampleSize"/>. </param>
        [Theory]
        [InlineData(new object[5] { new double[1] { 1d }, 0d, 2d, 1d, 1 })]
        public void SampleSize_SimpleTestData_Returns_ExpectedMinimum(double[] data, double min, double max, double width, double expectedMin)
        {
            var testObj = new HistogramBinnedData(IDataFactory.Factory(data), min, max, width);
            Assert.Equal(expectedMin, testObj.SampleSize);
        }
        #endregion

        #region InvalidConstructorArgumentsExceptions
        /// <summary>
        /// Tests the <see cref="HistogramBinnedData.HistogramBinnedData(IData, double, double, double)"/> constructor to ensure that when the requested <see cref="Histogram.Bins"/> width is greater than the specified range an <see cref="InvalidConstructorArgumentsException"/> is thrown.
        /// </summary>
        /// <param name="data"> 'Good' data to be binned in the histogram if an exception in not thrown. </param>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 0.5 }, 0d, 1d, 2d)]
        [InlineData(new double[] { 0.0 }, -1d, 1d, 10d)]
        [InlineData(new double[] { .15 }, 0.1d, 0.2d, 1d)]
        public void IsConstructable_BinWidthGreaterThanRange_Throws_InvalidConstructorArgumentsException(double[] data, double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramBinnedData(IDataFactory.Factory(data), min, max, width));
        }
        /// <summary>
        /// Tests the <see cref="HistogramBinnedData.HistogramBinnedData(IData, double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramBinnedDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/>  when the requested <see cref="Histogram.Bins"/> width is negative or not finite.
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
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramBinnedData(IDataFactory.Factory(data), min, max, width));
        }
        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramBinnedDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Minimum"/> is not finite.
        /// </summary>
        /// <param name="min"> The requested <see cref="Histogram.Minimum"/>. </param>
        /// <param name="max"> The requested <see cref="Histogram.Maximum"/>. </param>
        /// <param name="width"> The requested constant <see cref="Histogram.Bins"/> width. </param>
        [Theory]
        [InlineData(new double[] { 1d }, double.NaN, 2d, 1d)]
        [InlineData(new double[] { 1d }, double.NegativeInfinity, 2d, 1d)]
        [InlineData(new double[] { 1d }, double.PositiveInfinity, 2d, 1d)]
        public void IsConstructable_NonFiniteMin_Throws_InvalidConstructorArgumentsException(double[] data,  double min, double max, double width)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramBinnedData(IDataFactory.Factory(data), min, max, width));
        }
        /// <summary>
        /// Tests the <see cref="HistogramNoData.HistogramNoData(double, double, double)"/> constructor to ensure the simple data case described in <see cref="HistogramBinnedDataTestData"/> throws an <see cref="InvalidConstructorArgumentsException"/> when the requested <see cref="Histogram.Maximum"/> is not finite.
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
            Assert.Throws<InvalidConstructorArgumentsException>(() => new HistogramBinnedData(IDataFactory.Factory(data), min, max, width));
        }
        #endregion

        #region IValidate Tests
        /// <summary>
        /// Tests that for standard good data ( but empty histogram example, constructed using min, max and nBins arguements: (min: 0, max: 2, nBins: 2) that the <see cref="Histogram.IsValid"/> property is set to <see langword="false"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="nBins"></param>
        [Theory]
        [InlineData(0, 2, 2)]
        public void IsValid_GoodDataEmptyHistogramNBins_Returns_False(double min, double max, int nBins)
        {
            //var testObj = new Histogram(min, max, nBins);
            //Assert.False(testObj.IsValid);
        }
        #endregion
        #endregion
    }
}
