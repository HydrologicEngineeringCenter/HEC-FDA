using Functions;
using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace FunctionsTests.Coordinates
{
    [ExcludeFromCodeCoverage]

    public class CoordinateVariableYTests
    {

        public static TheoryData<double[]> GoodData_Normal =>
           new TheoryData<double[]>
           {
                {  new double[] {0,1 } },
                {  new double[] {.5,.5 } },
                {  new double[] {10,20 } },

           };

        /// <summary>
        /// Tests the CoordinateConstants constructor with double values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [MemberData(nameof(GoodData_Normal))]

        public void CoordinateVariableY_GoodInput_Returns_CoordinateConstants(double[] value)
        {
            Distribution dist = new Distribution(new Normal(value[0], value[1]));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(value[0]), dist);
            Assert.NotNull(coord);
        }
        /// <summary>
        /// Tests that the X value property returns correctly.
        /// </summary>
        [Theory]
        [MemberData(nameof(GoodData_Normal))]
        public void X_GoodInput_Returns_Double(double[] value)
        {
            Distribution dist = new Distribution(new Normal(value[0], value[1]));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(value[0]), dist);
            Assert.True(coord.X.Value() == value[0]);
        }

        /// <summary>
        /// Tests that the Y value property returns correctly.
        /// </summary>
        [Theory]
        [MemberData(nameof(GoodData_Normal))]
        public void Y_GoodInput_Returns_Double(double[] value)
        {
            Distribution dist = new Distribution(new Normal(value[0], value[1]));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(value[0]), dist);
            Assert.True(coord.Y.Value() == value[0]);
        }

        /// <summary>
        /// Tests that the equals method returns true when checking two coordinates with the same values.
        /// </summary>
        [Fact]
        public void Equals_GoodInput_Returns_Bool()
        {
            Distribution dist = new Distribution(new Normal(1, 1));
            CoordinateVariableY coord1 = new CoordinateVariableY(new Constant(1), dist);

            IDistributedOrdinate dist2 = new Distribution(new Normal(1, 1));
            CoordinateVariableY coord2 = new CoordinateVariableY(new Constant(1), dist);
            Assert.True(coord1.Equals(coord2));
        }

        /// <summary>
        /// Tests that the write to xml method can be read back in and turned into the same coordinate.
        /// </summary>
        [Fact]
        public void WriteToXML_GoodInput_Returns_Bool()
        {
            Distribution dist = new Distribution(new Normal(1, 1));
            CoordinateVariableY coord1 = new CoordinateVariableY(new Constant(1), dist);
            XElement xOrdXml = coord1.X.WriteToXML();
            XElement yOrdXml = coord1.Y.WriteToXML();

            ICoordinate returnedCoord = ICoordinateFactory.Factory(xOrdXml, yOrdXml);
            Assert.True(coord1.Equals(returnedCoord));
        }
    }
}
