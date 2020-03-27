using Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;

namespace StatisticsTests
{
    /// <summary>
    /// Tests the <see cref="Data"/> class that implements the <see cref="IData"/> interface.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataTests
    {
        /// <summary>
        /// Test that the <see cref="Data"/> constructor when called with: 
        /// <ul> 
        /// <li> <see langword="null"/> </li> 
        /// <li> empty </li> 
        /// <li> only non-finite {<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>} </li> 
        /// <li> only non-numeric (<see cref="double.NaN"/>) </li>   
        /// </ul>
        /// data elements and the usable data requirement is set to <see langword="true"/> an <see cref="Utilities.InvalidConstructorArgumentsException"/> is thrown.
        /// <see cref="IEnumerable{double}"/> an <see cref="InvalidConstructorArgumentsException"/> is thrown.
        /// </summary>
        /// <param name="data"> The <see cref="IEnumerable{double}"/> data to be processed. </param>
        [Theory]
        [InlineData(null)]
        [InlineData(new double[] { })]
        [InlineData(new double[1] { double.NaN})]
        [InlineData(new double[1] { double.NegativeInfinity })]
        [InlineData(new double[1] { double.PositiveInfinity })]
        public void Data_NullEmptyOrNonFiniteDataInput_Throws_InvalidConstructorArgumentsException(IEnumerable<double> data)
        {
            Assert.Throws<Utilities.InvalidConstructorArgumentsException>(() => new Data(data, true));
        }
        /// <summary>
        /// Test that the expected <see cref="IData.Range"/> is returned after construction with good data.
        /// </summary>
        /// <param name="data"> The provided <see cref="IEnumerable{data}"/> to be processed by the constructor. </param>
        /// <param name="expectedMin"> The expected <see cref="IData.Range.Min"/> value. </param>
        /// <param name="expectedMax"> The expected <see cref="IData.Range.Max"/> value. </param>
        [Theory]
        [InlineData(new object[3] { new double[1] { 1d }, 1d, 1d })]
        [InlineData(new object[3] { new double[2] { 1d, 2d}, 1d, 2d })]
        [InlineData(new object[3] { new double[3] { 3d, 1d, 2d }, 1d, 3d })]
        [InlineData(new object[3] { new double[3] { 3d, double.NaN, 1d }, 1d, 3d })]
        public void Range_GoodData_Returns_ExpectedIRange(IEnumerable<double> data, double expectedMin, double expectedMax)
        {
            var testObj = new Data(data);
            IRange<double> expectedRange = IRangeFactory.Factory(expectedMin, expectedMax, true, true, true, false);
            Assert.True(expectedRange.Equals<double>(testObj.Range));
        }
        /// <summary>
        /// Tests that the expected <see cref="IData.Elements"/> array is returned after construction with good data.
        /// </summary>
        /// <param name="data"> The provided <see cref="IEnumerable{double}"/> to be processed by the constructor. </param>
        /// <param name="expectedElements"> The expected array of finite numeric <see cref="IEnumerable{double}"/> data elements sorted in ascending order. </param>
        [Theory]
        [InlineData(new object[2] { new double[1] { 1d }, new double[1] { 1d }})]
        [InlineData(new object[2] { new double[2] { 1d, 2d }, new double[2] { 1d, 2d }})]
        [InlineData(new object[2] { new double[3] { 3d, 1d, 2d }, new double[3] { 1d, 2d, 3d }})]
        [InlineData(new object[2] { new double[3] { 3d, double.NaN, 1d }, new double[2] { 1d, 3d }})]
        public void Elements_GoodData_Returns_ExpectedElementsArray(IEnumerable<double> data, IEnumerable<double> expectedElements)
        {
            Assert.Equal(new Data(data).Elements, expectedElements);
        }
        /// <summary>
        /// Tests that the expected <see cref="Data.InvalidElements"/> array is returned after construction with good data.
        /// </summary>
        /// <param name="data"> The provided <see cref="IEnumerable{double}"/> to be processed by the constructor. </param>
        /// <param name="expectedInvalidElements"> The expected array of <see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/> and <see cref="double.PositiveInfinity"/> values. </param>
        /// <remarks> The <see cref="Data.InvalidElements"/> property is not part of the <see cref="IData"/> interface. </remarks>
        [Theory]
        [InlineData(new object[2] { new double[1] { 1d }, new double[0] { } })]
        [InlineData(new object[2] { new double[2] { 1d, 2d }, new double[] { } })]
        [InlineData(new object[2] { new double[3] { 3d, double.NaN, 1d }, new double[1] { double.NaN } })]
        [InlineData(new object[2] { new double[2] { double.NaN, 1d }, new double[1] { double.NaN }})]
        [InlineData(new object[2] { new double[3] { double.NaN, 1d, double.NaN }, new double[2] { double.NaN, double.NaN } })]
        [InlineData(new object[2] { new double[4] { double.PositiveInfinity, double.NaN, 1d, double.NegativeInfinity }, new double[3] { double.PositiveInfinity, double.NaN, double.NegativeInfinity } })]
        public void InvalidElements_GoodData_Returns_ExpectedInvalidElementsArray(IEnumerable<double> data, IEnumerable<double> expectedInvalidElements)
        {
            var testObj = new Data(data);
            Assert.Equal(testObj.InvalidElements, expectedInvalidElements);
        }
        /// <summary>
        /// Tests that the expected <see cref="IData.SampleSize"/> is returned after construction with good data.
        /// </summary>
        /// <param name="data"> The provided <see cref="IEnumerable{double}"/> to be processed by the constructor. </param>
        /// <param name="expectedSampleSize"> The <see cref="IData.SampleSize"/> number of finite numeric data elements. </param>
        [Theory]
        [InlineData(new double[1] { 1d }, 1)]
        [InlineData(new double[2] { 1d, 2d }, 2)]
        [InlineData(new double[3] { 3d, 1d, 2d }, 3)]
        [InlineData(new double[3] { 3d, double.NaN, 1d }, 2)]
        [InlineData(new double[3] { double.NaN, 1d, double.NaN }, 1)]
        public void SampleSize_GoodData_Returns_ExpectedSampleSize(IEnumerable<double> data, int expectedSampleSize)
        {
            Assert.Equal(new Data(data).SampleSize, expectedSampleSize);
        }
    }
}
