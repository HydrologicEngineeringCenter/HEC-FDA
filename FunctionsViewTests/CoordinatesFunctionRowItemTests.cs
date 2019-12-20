using Functions;
using Functions.CoordinatesFunctions;
using FunctionsView.ViewModel;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace FunctionsViewTests
{
    public class CoordinatesFunctionRowItemTests
    {
        [ExcludeFromCodeCoverage]

       
        /// <summary>
        /// Tests that a row item with constant values can create an ICoordinate with the correct values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(100, 200)]
        [InlineData(200, 100)]
        public void CoordinatesFunctionRowItem_CreateCoordinateFromRow_ConstantValues_Returns_ICoordinate(double x, double y)
        {
            CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(x)
                .WithConstantDist(y, InterpolationEnum.None);
            CoordinatesFunctionRowItem row = builder.Build();

            ICoordinate coord = row.CreateCoordinateFromRow();
            Assert.True(coord.X.Value() == x && coord.Y.Value() == y);

        }

        /// <summary>
        /// Tests that a row item with normal values can create an ICoordinate with the correct values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 1,0)]
        [InlineData(-1, -1,1)]
        [InlineData(5,100, 200)]
        [InlineData(99, 200, 100)]
        public void CoordinatesFunctionRowItem_CreateCoordinateFromRow_NormalValues_Returns_ICoordinate(double x,double mean, double stDev)
        {
            CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(x)
                .WithNormalDist(mean,stDev, InterpolationEnum.None);
            CoordinatesFunctionRowItem row = builder.Build();

            IDistributedValue distValue = DistributedValueFactory.FactoryNormal(mean, stDev);
            ICoordinate testCoord = ICoordinateFactory.Factory(x, distValue);

            ICoordinate coord = row.CreateCoordinateFromRow();
            Assert.True(coord.Equals(testCoord));
        }

        /// <summary>
        /// Tests that a row item with uniform values can create an ICoordinate with the correct values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(-1, -1, 1)]
        [InlineData(5, 100, 200)]
        [InlineData(99, 100, 200)]
        public void CoordinatesFunctionRowItem_CreateCoordinateFromRow_UniformValues_Returns_ICoordinate(double x, double min, double max)
        {
            CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(x)
                .WithUniformDist(min,max, InterpolationEnum.None);
            CoordinatesFunctionRowItem row = builder.Build();

            IDistributedValue distValue = DistributedValueFactory.FactoryUniform(min,max);
            ICoordinate testCoord = ICoordinateFactory.Factory(x, distValue);

            ICoordinate coord = row.CreateCoordinateFromRow();
            Assert.True(coord.Equals(testCoord));
        }
    }
}
