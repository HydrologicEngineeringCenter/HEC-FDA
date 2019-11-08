using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.Coordinates
{
    [ExcludeFromCodeCoverage]

    public class CoordinateVariableYTests
    {
        /// <summary>
        /// Tests the CoordinateConstants constructor with double values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Fact]
     
        public void CoordinateConstants_GoodInput_Returns_CoordinateConstants()
        {
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(1), new Distribution(new Normal(1,1)));
            Assert.NotNull(coord);
        }
    }
}
