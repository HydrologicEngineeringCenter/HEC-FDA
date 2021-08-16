//using Statistics.Histograms;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;
//using Utilities;
//using Xunit;

//namespace StatisticsTests.Histograms
//{
//    [ExcludeFromCodeCoverage]
//    public class HistogramBuilderTests
//    {
//        #region InitializeWithBinsCountAndRange Tests
//        #region ArgumentOutOfRangeException (Not Enough Bins or Bad Range)
//        /// <summary>
//        /// Tests that <see cref="IHistogramBuilder.InitializeWithBinsCountAndRange(int, double, double)"/> throws an <see cref="ArgumentOutOfRangeException"/> if:
//        /// <ul>
//        /// <li> The number of requested bins is not a positive integer, </li>
//        /// <li> The requested histogram minimum or maximum value is not a finite numerical value </li>
//        /// </ul>
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (exclusive). </param>
//        [Theory]
//        [InlineData(-1, 1d, 2d)]
//        [InlineData(0, 1d, 2d)]
//        [InlineData(1, 1d, 1d)]
//        [InlineData(1, 2d, 1d)]
//        [InlineData(1, double.NaN, 2d)]
//        [InlineData(1, 1d, double.NaN)]
//        [InlineData(1, double.NegativeInfinity, 2d)]
//        [InlineData(1, 1d, double.PositiveInfinity)]
//        public void InitializeWithBinsCountAndRange_BadData_Throws_ArgumentsException(int nBins, double min, double max)
//        {
//            HistogramBuilder testObj = new HistogramBuilder();
//            Assert.Throws<ArgumentOutOfRangeException>(() => testObj.InitializeWithBinsCountAndRange(nBins, min, max));
//        }
//        #endregion
//        #region Expected BinCount
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountAndRange(int, double, double)"/> function and provided good data inputs the expected number of <see cref="Statistics.IHistogram.Bins"/> are created.
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (exclusive). </param>
//        /// <param name="expectedBinCount"> The expected result. </param>
//        [Theory]
//        [InlineData(1, 1d, 2d, 1)]
//        [InlineData(2, 1d, 2d, 2)]
//        [InlineData(4, 1d, 2d, 4)]
//        [InlineData(100, 0d, 100d, 100)]
//        public void InitializeWithBinsCountAndRange_GoodData_Returns_ExpectedBinCount(int nBins, double min, double max, int expectedBinCount)
//        {
//            HistogramBuilder testObj = new HistogramBuilder();
//            testObj.InitializeWithBinsCountAndRange(nBins, min, max);
//            Assert.Equal(expectedBinCount, testObj.BinCount);
//        }
//        #endregion
//        #region Expected BinWidth
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountAndRange(int, double, double)"/> function and provided good data inputs the expected <see cref="Bin.Width"/> is generated for the <see cref="Statistics.IHistogram"/>. Included tests:
//        /// <ul>
//        /// <li> Values divisible by whole numbers, </li>
//        /// <li> Fractional values, </li>
//        /// </ul>
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (exclusive). </param>
//        /// <param name="expectedWidth"> The expected result. </param>
//        [Theory]
//        [InlineData(1, 1d, 2d, 1d)]
//        [InlineData(2, 1d, 2d, 0.5)]
//        [InlineData(4, 1d, 2d, 0.25)]
//        [InlineData(100, 0d, 100d, 1d)]
//        public void InitializeWithBinsCountAndRange_GoodData_Returns_ExpectedBinWidth(int nBins, double min, double max, double expectedWidth)
//        {
//            HistogramBuilder testObj = new HistogramBuilder();
//            testObj.InitializeWithBinsCountAndRange(nBins, min, max);
//            Assert.Equal(expectedWidth, testObj.BinWidth);
//        }
//        #endregion
//        #region Expected Range
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountAndRange(int, double, double)"/> function and provided good data inputs the expected <see cref="IRange{T}"/> is generated. 
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (inclusive). </param>
//        /// <param name="expectedMin"> The expected histogram minimum. </param>
//        /// <param name="expectedMax"> The expected histogram maximum. </param>
//        [Theory]
//        [InlineData(1, 1d, 2d, 1d, 2d)]
//        [InlineData(2, 1d, 2d, 1d, 2d)]
//        [InlineData(4, 1d, 2d, 1d, 2d)]
//        [InlineData(100, 0d, 100d, 0d, 100d)]
//        public void InitializeWithBinsCountAndRange_GoodData_Returns_ExpectedRange(int nBins, double min, double max, double expectedMin, double expectedMax)
//        {
//            HistogramBuilder testObj = new HistogramBuilder();
//            testObj.InitializeWithBinsCountAndRange(nBins, min, max);
//            IRange<double> expectedRange = IRangeFactory.Factory(expectedMin, expectedMax, true, false, true, true);
//            Assert.True(testObj.Range.Equals<double>(expectedRange));
//        }
//        #endregion
//        #region Expected Bins
//        /// <summary>
//        /// Tests that when the <see cref="IHistogramBuilder"/> is initialized with the <see cref="IHistogramBuilder.InitializeWithBinsCountAndRange(int, double, double)"/> function, the expected <see cref="Statistics.IHistogram.Bins"/>s are created. The expected <see cref="Statistics.IHistogram.Bins"/> are tested one <see cref="Statistics.IBin"/> at a time.
//        /// </summary>
//        /// <param name="nBins"> The requested number of <see cref="Statistics.IHistogram.Bins"/>. </param>
//        /// <param name="min"> The histogram minimum. </param>
//        /// <param name="max"> The histogram maximum. </param>
//        /// <param name="binIndex"> The index of the <see cref="Statistics.Ibin"/> in the array of <see cref="Statistics.IHistogram.Bins"/>. </param>
//        /// <param name="binMin"> The expected minimum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param>
//        /// <param name="binMax"> The expected maximum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param>
//        [Theory]
//        [InlineData(1, 1d, 2d, 0, 1d, 2d)]
//        [InlineData(2, 1d, 2d, 0, 1d, 1.5)]
//        [InlineData(2, 1d, 2d, 1, 1.5, 2d)]
//        [InlineData(4, 1d, 2d, 0, 1d, 1.25)]
//        [InlineData(4, 1d, 2d, 1, 1.25, 1.5)]
//        [InlineData(4, 1d, 2d, 2, 1.5, 1.75)]
//        [InlineData(4, 1d, 2d, 3, 1.75, 2d)]
//        [InlineData(100, 0d, 100d, 0, 0d, 1d)]
//        [InlineData(100, 0d, 100d, 49, 49d, 50d)]
//        [InlineData(100, 0d, 100d, 99, 99d, 100d)]
//        public void InitializeWithBinsCountAndRange_GoodData_Returns_ExpectedBin(int nBins, double min, double max, int binIndex, double binMin, double binMax)
//        {
//            HistogramBuilder testObj = new HistogramBuilder();
//            testObj.InitializeWithBinsCountAndRange(nBins, min, max);
//            Assert.True(testObj.Bins[binIndex].Equals(new Bin(binMin, binMax, 0)));
//        }
//        #endregion
//        #endregion
//        #region InitializeWithBinsCountWidthAndMin Tests
//        #region ArgumentOutOfRangeException (Not Enough Bins, Bad Min or Width)
//        /// <summary>
//        /// Tests that <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> throws an <see cref="ArgumentOutOfRangeException"/> if:
//        /// <ul>
//        /// <li> The number of requested bins is not a positive integer, </li>
//        /// <li> The specified histogram minimum value is not a finite numerical value, </li>
//        /// <li> The specified bin width is not a positive finite numerical value </li>
//        /// </ul>
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="width"> The requested (constant) histogram bin width. </param>
//        [Theory]
//        [InlineData(-1, 1d, 1d)]
//        [InlineData(0, 1d, 1d)]
//        [InlineData(1, double.NaN, 1d)]
//        [InlineData(1, double.NegativeInfinity, 1d)]
//        [InlineData(1, double.PositiveInfinity, 1d)]
//        [InlineData(1, 1d, -1d)]
//        [InlineData(1, 1d, 0d)]
//        [InlineData(1, 1d, double.NaN)]
//        [InlineData(1, 1d, double.NegativeInfinity)]
//        [InlineData(1, 1d, double.PositiveInfinity)]
//        public void InitialzeWithBinsCountWidthAndMin_BadData_Throws_ArgumentOutOfRangeException(int nBins, double min, double width)
//        {
//            Assert.Throws<ArgumentOutOfRangeException>(() => new HistogramBuilder().InitializeWithBinsCountWidthAndMin(nBins, min, width));
//        }
//        #endregion
//        #region Expected BinCount
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> function and provided good data inputs the expected number of <see cref="Statistics.IHistogram.Bins"/> are created.
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="width"> The requested (constant) histogram bin width. </param>
//        /// <param name="expectedBinCount"> The expected result. </param>
//        [Theory]
//        [InlineData(1, 1d, 1d, 1)]
//        [InlineData(2, 1d, 1d, 2)]
//        [InlineData(2, 1d, 0.5d, 2)]
//        [InlineData(100, 1d, 1d, 100)]
//        public void InitializeWithBinsCountWidthAndMin_GoodData_Returns_ExpectedBinCount(int nBins, double min, double width, int expectedBinCount)
//        {
//            IHistogramBuilder testObj = new HistogramBuilder().InitializeWithBinsCountWidthAndMin(nBins, min, width);
//            Assert.Equal(expectedBinCount, testObj.BinCount);
//        }
//        #endregion
//        #region Expected BinWidth
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> function and provided good data inputs the expected <see cref="Bin.Width"/> is generated for the <see cref="Statistics.IHistogram"/>. Included tests:
//        /// <ul>
//        /// <li> Values divisible by whole numbers, </li>
//        /// <li> Fractional values, </li>
//        /// </ul>
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="width"> The requested (constant) histogram bin width. </param>
//        /// <param name="expectedWidth"> The expected result. </param>
//        [Theory]
//        [InlineData(1, 1d, 1d, 1d)]
//        [InlineData(2, 1d, 1d, 1d)]
//        [InlineData(2, 1d, 0.5d, 0.5d)]
//        [InlineData(4, 1d, 0.25d, 0.25d)]
//        [InlineData(100, 1d, 1d, 1d)]
//        public void InitializeWithBinsCountWidthAndMin_GoodData_Returns_ExpectedBinWidth(int nBins, double min, double width, double expectedBinWidth)
//        {
//            IHistogramBuilder testObj = new HistogramBuilder().InitializeWithBinsCountWidthAndMin(nBins, min, width);
//            Assert.Equal(expectedBinWidth, testObj.BinWidth);
//        }
//        #endregion
//        #region Expected Range
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> function and provided good data inputs the expected <see cref="IRange{T}"/> is generated. 
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (inclusive). </param>
//        /// <param name="expectedMin"> The expected histogram minimum. </param>
//        /// <param name="expectedMax"> The expected histogram maximum. </param>
//        [Theory]
//        [InlineData(1, 1d, 1d, 1d, 2d)]
//        [InlineData(2, 1d, 1d, 1d, 3d)]
//        [InlineData(2, 1d, 0.5d, 1d, 2d)]
//        [InlineData(4, 1d, 0.25d, 1d, 2d)]
//        [InlineData(100, 0d, 1d, 0d, 100d)]
//        public void InitializeWithBinsCountWidthAndMin_GoodData_Returns_ExpectedRange(int nBins, double min, double width, double expectedMin, double expectedMax)
//        {
//            IHistogramBuilder testObj = new HistogramBuilder().InitializeWithBinsCountWidthAndMin(nBins, min, width);
//            IRange<double> expectedRange = IRangeFactory.Factory(expectedMin, expectedMax, true, false, true, true);
//            Assert.True(testObj.Range.Equals<double>(expectedRange));
//        }
//        #endregion
//        #region Expected Bins
//        /// <summary>
//        /// Tests that when the <see cref="IHistogramBuilder"/> is initialized with the <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> function, the expected <see cref="Statistics.IHistogram.Bins"/>s are created. The expected <see cref="Statistics.IHistogram.Bins"/> are tested one <see cref="Statistics.IBin"/> at a time.
//        /// </summary>
//        /// <param name="nBins"> The requested number of bins. </param>
//        /// <param name="min"> The requested histogram minimum (inclusive). </param>
//        /// <param name="max"> The requested histogram maximum (inclusive). </param>
//        /// <param name="binIndex"> The index of the <see cref="Statistics.IBin"/> in the array of <see cref="Statistics.IHistogram.Bins"/>. </param>
//        /// <param name="binMin"> The expected minimum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param>
//        /// <param name="binMax"> The expected maximum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param> 
//        [Theory]
//        [InlineData(1, 1d, 1d, 0, 1d, 2d)]
//        [InlineData(2, 1d, 0.5, 0, 1d, 1.5)]
//        [InlineData(2, 1d, 0.5, 1, 1.5, 2d)]
//        [InlineData(4, 1d, 0.25, 0, 1d, 1.25)]
//        [InlineData(4, 1d, 0.25, 1, 1.25, 1.5)]
//        [InlineData(4, 1d, 0.25, 2, 1.5, 1.75)]
//        [InlineData(4, 1d, 0.25, 3, 1.75, 2d)]
//        [InlineData(100, 0d, 1d, 0, 0d, 1d)]
//        [InlineData(100, 0d, 1d, 49, 49d, 50d)]
//        [InlineData(100, 0d, 1d, 99, 99d, 100d)]
//        [InlineData(2, 0d, double.Epsilon, 0, 0d, double.Epsilon)]
//        [InlineData(2, 0d, double.Epsilon, 1, double.Epsilon, (double.Epsilon * 2))]
//        public void InitializeWithBinsCountWidthAndMin_GoodData_Returns_ExpectedBin(int nBins, double min, double width, int binIndex, double binMin, double binMax)
//        {
//            Assert.True(new HistogramBuilder().InitializeWithBinsCountWidthAndMin(nBins, min, width).Bins[binIndex].Equals(new Bin(binMin, binMax, 0)));
//        }
//        #endregion
//        #endregion
//        #region InitializeWithDataAndWidth Tests
//        #region ArgumentOutOfRangeException (Null, Empty, Non-finite and numeric, or single good value data; or bad Widths)
//        [Theory]
//        [InlineData(null, 1d)]
//        [InlineData(new double[0] { }, 1d)]
//        [InlineData(new double[1] { double.NaN }, 1d)]
//        [InlineData(new double[2] { 1d, 2d }, -1d)]
//        [InlineData(new double[2] { 1d, 2d }, 0d)]
//        [InlineData(new double[2] { 1d, 2d }, double.NaN)]
//        [InlineData(new double[2] { 1d, 2d }, double.PositiveInfinity)]
//        public void InitializeWithDataAndWidth_BadData_Throws_ArgumentOutOfRangeException(IEnumerable<double> data, double width)
//        {
//            Assert.Throws<ArgumentOutOfRangeException>(() => new HistogramBuilder().InitializeWithDataAndWidth(data, width));
//        }
//        #endregion
//        #region Expected BinCount
//        /// <summary>
//        /// Tests that when the histogram is initialized using the <see cref="IHistogramBuilder.InitializeWithDataAndWidth(IEnumerable{double}, double)"/> function and provided good data inputs the expected number of <see cref="Statistics.IHistogram.Bins"/> are created. For instance,
//        /// <ul>
//        /// <li> One data value creates one min </li>
//        /// <li> Data values of bin max (exclusive) generates an additional bin </li>
//        /// </ul>
//        /// </summary>
//        /// <param name="data"> The data to be binned in the histogram. </param>
//        /// <param name="width"> The requested (constant) histogram bin width. </param>
//        /// <param name="expectedBinCount"> The expected result. </param>
//        [Theory]
//        [InlineData(new double[1] { 1d }, 1d, 1)]
//        [InlineData(new double[2] { 1d, 2d}, 1d, 2)]
//        [InlineData(new double[3] { 1d, 1.5d, 2d }, 1d, 2)]
//        [InlineData(new double[3] { 1d, 2d, 2.5d}, 1d, 3)]
//        public void InitializeWithDataAndWidth_GoodData_Returns_ExpectedBinCount(IEnumerable<double> data, double width, int expectedBinCount)
//        {
//            Assert.Equal(new HistogramBuilder().InitializeWithDataAndWidth(data, width).BinCount, expectedBinCount);
//        }
//        #endregion


//        #region Expected Bins
//        /// <summary>
//        /// Tests that when the <see cref="IHistogramBuilder"/> is initialized with the <see cref="IHistogramBuilder.InitializeWithBinsCountWidthAndMin(int, double, double)"/> function, the expected <see cref="Statistics.IHistogram.Bins"/>s are created. The expected <see cref="Statistics.IHistogram.Bins"/> are tested one <see cref="Statistics.IBin"/> at a time.
//        /// </summary>
//        /// <param name="data"> The data to be binned in the histogram. </param>
//        /// <param name="width"> The requested (constant) histogram bin width. </param>
//        /// <param name="binIndex"> The index of the <see cref="Statistics.IBin"/> in the array of <see cref="Statistics.IHistogram.Bins"/>. </param>
//        /// <param name="binIndex"> The index of the <see cref="Statistics.IBin"/> in the array of <see cref="Statistics.IHistogram.Bins"/>. </param>
//        /// <param name="binMin"> The expected minimum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param>
//        /// <param name="binMax"> The expected maximum of the <see cref="Statistics.IBin"/> in the <see cref="Statistics.IHistogram.Bins"/> being tested. </param> 
//        [Theory]
//        [InlineData(new double[2] { 1d, 2d }, 1d, 0, 0.5d, 1.5d, 1)]
//        [InlineData(new double[2] { 1d, 1.5d}, 1d, 0, 0.5d, 1.5d, 1)]
//        [InlineData(new double[2] { 1d, 1.5d}, 1d, 1, 1.5d, 2.5d, 1)]
//        [InlineData(new double[2] { 1d, 2d }, 1d, 1, 1.5d, 2.5d, 1)]
//        [InlineData(new double[3] { 1d, 1.5d, 2d}, 1d, 1, 1.5d, 2.5d, 2)]
//        public void InitializeWithDataAndWidth_GoodData_Returns_ExpectedBins(IEnumerable<double> data, double width, int binIndex, double expectedBinMin, double expectedBinMax, int expectedBinCount)
//        {
//            Assert.True(new HistogramBuilder().InitializeWithDataAndWidth(data, width).Bins[binIndex].Equals(new Bin(expectedBinMin, expectedBinMax, expectedBinCount)));
//        }
//        #endregion
//        #endregion

//    }
//}
