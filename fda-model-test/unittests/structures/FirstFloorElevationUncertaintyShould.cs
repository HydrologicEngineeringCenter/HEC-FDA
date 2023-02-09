using Xunit;
using Statistics;
using HEC.FDA.Model.structures;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("Category", "Remote")]
    public class FirstFloorElevationUncertaintyShould
    {
        [Theory]
        [InlineData(IDistributionEnum.Normal, .5, 0, 5, .4, 4.873326)]
        [InlineData(IDistributionEnum.LogNormal, .2, 0, 2, .8, 8.74362)]
        [InlineData(IDistributionEnum.Triangular, .5, 1, 5, .75, 5.387628)]
        [InlineData(IDistributionEnum.Uniform, .5, 1, 5, .45, 5.175)]
        [InlineData(IDistributionEnum.Deterministic, 203958, 20935, 10, .456, 10)]
        public void FirstFloorElevationShouldSampleCorrectly(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum, double inventoriedFirstFloorElevation, double probability, double expected)
        {
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(distributionEnum, standardDeviationOrMinimum, maximum);
            double actual = firstFloorElevationUncertainty.Sample(inventoriedFirstFloorElevation, probability, computeIsDeterministic: false);
            Assert.Equal(expected, actual, 1);
        }
    }
}
