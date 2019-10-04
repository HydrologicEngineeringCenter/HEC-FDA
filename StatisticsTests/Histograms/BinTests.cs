using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Xunit;

using Statistics.Histograms;
using Statistics.Validation;

namespace StatisticsTests.Histograms
{
    [ExcludeFromCodeCoverage]
    public class BinTests
    {
        /// <summary>
        /// Test that non finite <see cref="Bin.Minimum"/> values return <see langword="false"/> <see cref="Bin.IsValid"/> value.
        /// </summary>
        /// <param name="min"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void IsValid_NotFiniteMinimumValue_Returns_False(double min)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.False(new Bin(min, max:2, n:0).IsValid);
        }
        /// <summary>
        /// Test that non finite <see cref="Bin.Maximum"/> values return <see langword="false"/> <see cref="Bin.IsValid"/> value.
        /// </summary>
        /// <param name="max"></param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void IsValid_NotFiniteMaximumValue_Returns_False(double max)
        {
            //ValidationRegistry.Register(new BinValidator());
            Assert.False(new Bin(min:0, max, n: 0).IsValid);
        }
    }
}
