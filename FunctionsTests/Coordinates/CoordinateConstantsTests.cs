using Functions.Coordinates;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.Coordinates
{
    [ExcludeFromCodeCoverage]

    public class CoordinateConstantsTests
    {
        /// <summary>
        /// Tests the CoordinateConstants constructor with double values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(99, 105)]
        [InlineData(Double.MaxValue, Double.MaxValue)]
        [InlineData(Double.MinValue, Double.MaxValue)]
        public void CoordinateConstants_GoodInput_Returns_CoordinateConstants(double x, double y)
        {
            CoordinateConstants coord = new CoordinateConstants(x, y);
            Assert.NotNull(coord);
        }


        /// <summary>
        /// Tests that the X value property returns correctly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(99, 105)]
        [InlineData(Double.MaxValue, Double.MaxValue)]
        [InlineData(Double.MinValue, Double.MaxValue)]
        public void X_GoodInput_Returns_Double(double x, double y)
        {
            CoordinateConstants coord = new CoordinateConstants(x, y);
            Assert.True(coord.X == x);
        }

        /// <summary>
        /// Tests that the Y value property returns correctly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(99, 105)]
        [InlineData(Double.MaxValue, Double.MaxValue)]
        [InlineData(Double.MinValue, Double.MaxValue)]
        public void Y_GoodInput_Returns_Double(double x, double y)
        {
            CoordinateConstants coord = new CoordinateConstants(x, y);
            Assert.True(coord.Y == y);
        }

        /// <summary>
        /// Tests that the Sample method returns the CoordinatConstants object.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 5)]
        [InlineData(99, 105)]
        [InlineData(Double.MaxValue, Double.MaxValue)]
        [InlineData(Double.MinValue, Double.MaxValue)]
        public void Sample_GoodInput_Returns_CoordinateConstants(double x, double y)
        {
            CoordinateConstants coord = new CoordinateConstants(x, y);
            Assert.True(coord.Sample() == coord);
        }

    }
}
