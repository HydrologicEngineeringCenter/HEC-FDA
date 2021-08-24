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
            CoordinatesFunctionRowItemBuilder builder = new CoordinatesFunctionRowItemBuilder(x, true)
                .WithConstantDist(y, InterpolationEnum.None);
            CoordinatesFunctionRowItem row = builder.Build();

            ICoordinate coord = row.CreateCoordinateFromRow();
            Assert.True(coord.X.Value() == x && coord.Y.Value() == y);

        }
    
    }
}
