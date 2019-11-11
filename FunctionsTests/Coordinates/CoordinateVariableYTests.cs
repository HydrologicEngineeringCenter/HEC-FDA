using Functions;
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
     
        public void CoordinateVariableY_GoodInput_Returns_CoordinateConstants()
        {
            IDistributedValue dist = new DistributedValue(new Normal(1, 1));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(1), new Distribution(dist));
            Assert.NotNull(coord);
        }

        [Fact]
        public void X_GoodInput_Returns_Double()
        {
            IDistributedValue dist = new DistributedValue(new Normal(1, 1));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(1), new Distribution(dist));
            Assert.True(coord.X.Value() == 1);
        }

        [Fact]
        public void Y_GoodInput_Returns_Double()
        {
            IDistributedValue dist = new DistributedValue(new Normal(1, 1));
            CoordinateVariableY coord = new CoordinateVariableY(new Constant(1), new Distribution(dist));
            Assert.True(coord.Y.Value() == 1);
        }
    }
}
