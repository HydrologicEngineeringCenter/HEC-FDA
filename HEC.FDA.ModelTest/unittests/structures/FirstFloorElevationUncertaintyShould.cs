using Xunit;
using Statistics;
using HEC.FDA.Model.structures;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class FirstFloorElevationUncertaintyShould
    {
        //Demonstration of use of log normal accessible at 
        [Theory]
        [InlineData(IDistributionEnum.Normal, .5, 0, 5, .4, 4.873326)]
        [InlineData(IDistributionEnum.LogNormal, .050636, 0, 10, .7, 10.26909)]
        [InlineData(IDistributionEnum.Triangular, .5, 1, 5, .75, 5.387628)]
        [InlineData(IDistributionEnum.Uniform, .5, 1, 5, .45, 5.175)]
        [InlineData(IDistributionEnum.Deterministic, 203958, 20935, 10, .456, 10)]
        public void FirstFloorElevationShouldSampleCorrectly(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum, double inventoriedFirstFloorElevation, double probability, double expected)
        {
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(distributionEnum, standardDeviationOrMinimum, maximum);
            double actual = 0;
            if (distributionEnum.Equals(IDistributionEnum.LogNormal))
            {
                actual = inventoriedFirstFloorElevation * firstFloorElevationUncertainty.Sample(probability, computeIsDeterministic: false);
            }
            else
            {
                actual = inventoriedFirstFloorElevation + firstFloorElevationUncertainty.Sample(probability, computeIsDeterministic: false);
            }
            Assert.Equal(expected, actual, 1);
        }
    }
}
