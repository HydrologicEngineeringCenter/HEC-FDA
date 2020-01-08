using Functions;
using Functions.Coordinates;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
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
            CoordinateConstants coord = new CoordinateConstants(new Constant(x), new Constant(y));
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
            CoordinateConstants coord = new CoordinateConstants(new Constant(x), new Constant(y));
            Assert.True(coord.X.Value() == x);
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
            CoordinateConstants coord = new CoordinateConstants(new Constant(x), new Constant(y));
            Assert.True(coord.Y.Value() == y);
        }

        ///// <summary>
        ///// Tests that the Sample method returns the CoordinatConstants object.
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        //[Theory]
        //[InlineData(0, 0)]
        //[InlineData(-1, 5)]
        //[InlineData(99, 105)]
        //[InlineData(Double.MaxValue, Double.MaxValue)]
        //[InlineData(Double.MinValue, Double.MaxValue)]
        //public void Sample_GoodInput_Returns_CoordinateConstants(double x, double y)
        //{
        //    CoordinateConstants coord = new CoordinateConstants(new Constant(x), new Constant(y));
        //    Assert.True(coord.Sample() == coord);
        //}

        /// <summary>
        /// Tests that the equals method returns true when checking two coordinates with the same values.
        /// </summary>
        [Fact]
        public void Equals_GoodInput_Returns_Bool()
        {
            CoordinateConstants coord1 = new CoordinateConstants(new Constant(1), new Constant(2));
            CoordinateConstants coord2 = new CoordinateConstants(new Constant(1), new Constant(2));

            Assert.True(coord1.Equals(coord2));
        }

        /// <summary>
        /// Tests that the write to xml method can be read back in and turned into the same coordinate.
        /// </summary>
        [Fact]
        public void WriteToXML_GoodInput_Returns_Bool()
        {
            CoordinateConstants coord1 = new CoordinateConstants(new Constant(1), new Constant(2));
            XElement xOrdXml = coord1.X.WriteToXML();
            XElement yOrdXml = coord1.Y.WriteToXML();

            ICoordinate returnedCoord = ICoordinateFactory.Factory(xOrdXml, yOrdXml);
            Assert.True(coord1.Equals(returnedCoord));
        }
    }
}
