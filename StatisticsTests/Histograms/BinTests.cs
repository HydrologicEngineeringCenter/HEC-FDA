using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Xunit;

using Statistics.Histograms;
using Utilities;

namespace StatisticsTests.Histograms
{
    [ExcludeFromCodeCoverage]
    public class BinTests
    {
        #region Property Tests
        /// <summary>
        /// Tests that the requested minimum is provided as the <see cref="Bin.Minimum"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="min"> The requested bin minimum. </param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void Minimum_GoodOrBad_Returns_InputValue(double min)
        {
            var testObj = new Bin(min, 2, 0);
            Assert.Equal(min, testObj.Minimum);
        }
        /// <summary>
        /// Tests that the requested maximum is provided as the <see cref="Bin.Maximum"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="max"> The requested bin maximum. </param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void Maximum_GoodOrBad_Returns_InputValue(double max)
        {
            var testObj = new Bin(0, max, 0);
            Assert.Equal(max, testObj.Maximum);
        }
        /// <summary>
        /// Tests that the requested bin count is provided as the <see cref="Bin.Count"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="count"> The requested bin count. </param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void Count_GoodOrBad_Returns_InputValue(int count)
        {
            var testObj = new Bin(0, 2, count);
            Assert.Equal(count, testObj.Count);
        }
        /// <summary>
        /// Tests that the mid point between the <see cref="Bin.Minimum"/> and the <see cref="Bin.Maximum"/> is is provided as the <see cref="Bin.MidPoint"/> property regardless of the validity of the value.
        /// </summary>
        /// <param name="count"> The requested bin count. </param>
        [Theory]
        [InlineData(0, 2)]
        [InlineData(2, 0)]
        [InlineData(double.NegativeInfinity, double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity, 2)]
        [InlineData(0, double.PositiveInfinity)]
        [InlineData(double.NaN, double.NaN)]
        public void MidPoint_GoodOrBad_Returns_InputValue(double min, double max)
        {
            double expected = min + (max - min) / 2;
            var testObj = new Bin(min, max, 0);
            Assert.Equal(expected, testObj.MidPoint);
        }
        #endregion


        #region IValidate Tests
        #region IsValid (NewBin)
        /// <summary>
        /// Tests that new bin standard good data case: Bin(min: 0, max: 2, n: 0) sets <see cref="Bin.IsValid"/> property to <see langword="true"/>.
        /// </summary>
        [Fact]
        public void IsValid_NewBin_GoodData_Returns_True()
        {
            Assert.True(new Bin(0, 2, 0).IsValid);
        }
        /// <summary>
        /// Test that new bin non-finite <see cref="Bin.Minimum"/> values: (<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>, <see cref="double.NaN"/>) sets <see cref="Bin.IsValid"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="min"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void IsValid_NewBinNotFiniteMinimumValue_Returns_False(double min)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.False(new Bin(min, max: 2, n: 0).IsValid);
        }
        /// <summary>
        /// Test that new bin non-finite <see cref="Bin.Maximum"/> values: (<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>, <see cref="double.NaN"/>) sets <see cref="Bin.IsValid"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="max"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void IsValid_NewBinNotFiniteMaximumValue_Returns_False(double max)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.False(new Bin(min: 0, max, n: 0).IsValid);
        }
        /// <summary>
        /// Tests that new <see cref="Bin"/> with bad range [<see cref="Bin.Minimum"/>, <see cref="Bin.Maximum"/>) including: <see cref="Bin.Minimum"/> &gt;= <see cref="Bin.Maximum"/>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [Theory]
        [InlineData(0,0)]
        [InlineData(1,0)]
        [InlineData(0,-1)]
        public void IsValid_BadRange_Returns_False(double min, double max)
        {
            Assert.False(new Bin(min, max, n: 0).IsValid);
        }

        /// <summary>
        /// Test that new bin negative <see cref="Bin.Count"/> values: (-1, <see cref="int.MinValue"/>) sets <see cref="Bin.IsValid"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="count"></param>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void IsValid_NewBinNegativeCountValue_Returns_False(int count)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.False(new Bin(min: 0, max: 2, n: count).IsValid);
        }

        #endregion

        #region Errors (NewBin)
        /// <summary>
        /// Tests that new bin standard good data case: Bin(min: 0, max: 2, n: 0) returns no string errors in the <see cref="Bin.Errors"/> property.
        /// </summary>
        [Fact]
        public void Errors_NewBin_GoodData_Returns_True()
        {
            Assert.True(!new Bin(0, 2, 0).Messages.Any());
        }
        /// <summary>
        /// Test that new bin non finite <see cref="Bin.Minimum"/> values: (<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>, <see cref="double.NaN"/>) return a two errors(not finite range, not finite midpoint) in the <see cref="Bin.Errors"/> property.
        /// </summary>
        /// <param name="min"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void Errors_NewBinNotFiniteMinimumValue_Returns_TwoErrorString(double min)
        {
            //ValidationRegistry.Register(new BinValidator());
            if (min == double.PositiveInfinity) Assert.True(new Bin(min, max: 2, n: 0).Messages.Count() == 3);
            else Assert.True(new Bin(min, max: 2, n: 0).Messages.Count() == 2);
        }
        /// <summary>
        /// Test that new bin non finite <see cref="Bin.Maximum"/> values: (<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>, <see cref="double.NaN"/>) return two errors (not finite range, not finite midpoint) in the <see cref="Bin.Errors"/> property.
        /// </summary>
        /// <param name="max"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void Errors_NewBinNotFiniteMaximumValue_Returns_TwoErrorString(double max)
        {
            //ValidationRegistry.Register(new BinValidator());
            var testObj = new Bin(min: 0, max: max, n: 0);
            // Messages: 1 - non-finite range, 2 - not a range (for double.NegativeInfinity and .NaN only), 3 - non finite mid-point
            Assert.True(testObj.Messages.Count() > 1 && testObj.Messages.Count() < 4);
        }
        /// <summary>
        /// Test that new bin negative <see cref="Bin.Count"/> values: (-1, <see cref="int.MinValue"/>) return a single string in the <see cref="Bin.Errors"/> property.
        /// </summary>
        /// <param name="count"></param>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void Errors_NewBinNegativeCountValue_Returns_SingleErrorString(int count)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.True(new Bin(min: 0, max: 2, n: count).Messages.Count() == 1);
        }
        #endregion

        #region IsValid (OldBin)
        /// <summary>
        /// Tests that standard good data case <see cref="Bin"/>: <see cref="Bin"/>(<see cref="Bin.Minimum"/>: 0, <see cref="Bin.Maximum"/>: 2, <see cref="Bin.Count"/>: 0) incremented with a positive number of new observations sets new <see cref="Bin"/> object <see cref="Bin.IsValid"/> property to <see langword="true"/>.
        /// </summary>
        /// <param name="addN"></param>
        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void IsValid_OldBin_GoodDataPlusPositiveValues_Returns_True(int addN)
        {
            Bin oldBin = new Bin(0, 2, 0);
            Assert.True(new Bin(oldBin, addN).IsValid);
        }
        /// <summary>
        /// Tests that when additional observations are added to an existing <see cref="Bin"/> with non finite <see cref="Bin.Minimum"/> or <see cref="Bin.Maximum"/> property values, the <see cref="Bin.IsValid"/> property reamins <see langword="false"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        [Theory]
        [InlineData(double.NaN, 2)]
        [InlineData(double.NegativeInfinity, 2)]
        [InlineData(double.PositiveInfinity, 2)]
        [InlineData(0, double.NaN)]
        [InlineData(0, double.NegativeInfinity)]
        [InlineData(0, double.PositiveInfinity)]
        public void IsValid_OldBin_NotFiniteBounds_Returns_False(double min, double max)
        {
            Bin oldBin = new Bin(min, max, n: 0);
            Assert.False(new Bin(oldBin, 0).IsValid);
        }
        /// <summary>
        /// Tests that when additional observations are added to an existing <see cref="Bin"/> that causes the <see cref="Bin.Count"/> property to return a negative value the <see cref="Bin.IsValid"/> property is set to <see langword="false"/>.
        /// </summary>
        /// <param name="addN"></param>
        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void IsValid_OldBin_GoodDataPlusNegativeCount_Returns_False(int addN)
        {
            Bin oldBin = new Bin(0, 2, n: 0);
            Assert.False(new Bin(oldBin, addN).IsValid);
        }

        #endregion
        #endregion

        
    }
}
