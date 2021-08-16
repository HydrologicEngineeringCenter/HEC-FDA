using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Xunit;

using Statistics.Histograms;
using Utilities;
using Statistics;

namespace StatisticsTests.Histograms
{
      
    [ExcludeFromCodeCoverage]
    public class BinTests
    {
        #region InvalidConstructorArguments Tests
        [Theory]
        [InlineData(0d, 0d, 0)]
        [InlineData(1d, 0d, 0)]
        [InlineData(0d, 1d, -1)]
        [InlineData(double.NaN, 0d, 0)]
        [InlineData(0d, double.NaN, 0)]
        [InlineData(double.NegativeInfinity, 0d, 0)]
        [InlineData(0d, double.PositiveInfinity, 0)]
        public void Bin_NewBinBadData_Throws_InvalidConstructorArgumentsException(double min, double max, int n)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new Bin(min, max, n));
        }
        [Theory]
        [InlineData(null, 0)]
        public void Bin_OldBinNull_Throws_InvalidConstructorArgumentsException(IBin bin, int addN)
        {
            Assert.Throws<InvalidConstructorArgumentsException>(() => new Bin(bin, addN));
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Bin_OldBinAddNegativeObservations_Throws_InvalidConstructorArgumentsException(int addN)
        {
            var oldBin = new Bin(0, 2, 0);
            Assert.Throws<InvalidConstructorArgumentsException>(() => new Bin(oldBin, addN));
        }
        #endregion

        #region Property Tests
        /// <summary>
        /// Tests that the requested minimum is provided as the <see cref="Bin.Minimum"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="min"> The requested bin minimum. </param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Minimum_GoodOrBad_Returns_InputValue(double min)
        {
            Assert.Equal(min, new Bin(min, 2, 0).Range.Min);
        }
        /// <summary>
        /// Tests that the requested maximum is provided as the <see cref="Bin.Maximum"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="max"> The requested bin maximum. </param>
        [Theory]
        [InlineData(1)]
        public void Maximum_GoodOrBad_Returns_InputValue(double max)
        {
            Assert.Equal(max, new Bin(0, max, 0).Range.Max);
        }
        /// <summary>
        /// Tests that the requested bin count is provided as the <see cref="Bin.Count"/> property regardless of the validity of the provided argument.
        /// </summary>
        /// <param name="count"> The requested bin count. </param>
        [Theory]
        [InlineData(0)]
        [InlineData(100)]
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
        [InlineData(-1, 1)]
        [InlineData(-3, -1)]
        [InlineData(-1.2, -1.1)]
        public void MidPoint_GoodOrBad_Returns_InputValue(double min, double max)
        {
            double expected = min + (max - min) / 2;
            var testObj = new Bin(min, max, 0);
            Assert.Equal(expected, testObj.MidPoint);
        }
        #endregion

        #region IValidate Tests
        #region State
        /// <summary>
        /// Tests that new <see cref="Bin"/> standard good data case: Bin(min: 0, max: 2, n: 0) sets <see cref="IBin.State"/> property to <see cref="IMessageLevels.NoErrors"/>.
        /// </summary>
        [Fact]
        public void State_NewBin_GoodData_Returns_NoErrors()
        {
            Assert.Equal(IMessageLevels.NoErrors, new Bin(0, 2, 0).State);
        }
        // Any way to get an error?

        /// <summary>
        /// Tests that standard good data case <see cref="Bin"/>: <see cref="Bin"/>(<see cref="Bin.Minimum"/>: 0, <see cref="Bin.Maximum"/>: 2, <see cref="Bin.Count"/>: 0) incremented with a positive number of new observations sets new <see cref="Bin"/> object <see cref="Bin.State"/> property to <see cref="IMessageLevels.NoErrors"/>.
        /// </summary>
        /// <param name="addN"></param>
        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void State_OldBin_GoodDataPlusPositiveValues_Returns_NoErrors(int addN)
        {
            Bin oldBin = new Bin(0, 2, 0);
            Assert.Equal(IMessageLevels.NoErrors, new Bin(oldBin, addN).State);
        }
        #endregion
        #region Errors
        /// <summary>
        /// Tests that new bin standard good data case: Bin(min: 0, max: 2, n: 0) returns no string errors in the <see cref="Bin.Messages"/> property.
        /// </summary>
        [Fact]
        public void Messages_NewBin_GoodData_Returns_True()
        {
            Assert.True(!new Bin(0, 2, 0).Messages.Any());
        }
        // Any way to get a message?
        #endregion
        #endregion  
    }
}
