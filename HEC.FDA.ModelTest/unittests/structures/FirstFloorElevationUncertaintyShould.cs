using Xunit;
using Statistics;
using HEC.FDA.Model.structures;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class FirstFloorElevationUncertaintyShould
    {
        //Demonstration of use of log normal accessible at https://docs.google.com/spreadsheets/d/1suqLnNJEF2Gq4du3yvXcaupYzIGzUbaJ/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
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
                actual = inventoriedFirstFloorElevation * firstFloorElevationUncertainty.Sample(probability);
            }
            else
            {
                actual = inventoriedFirstFloorElevation + firstFloorElevationUncertainty.Sample(probability);
            }
            Assert.Equal(expected, actual, 1);
        }

        [Fact]
        public void ValidationShould()
        {
            IDistributionEnum distributionEnum = IDistributionEnum.LogPearsonIII;
            double standardDeviation = -1;
            FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(distributionEnum, standardDeviation);
            firstFloorElevationUncertainty.Validate();
            foreach (PropertyRule rule in firstFloorElevationUncertainty.RuleMap.Values)
            {
                Assert.Single(rule.Errors);
                Assert.Equal(HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal, rule.ErrorLevel);
            }
        }
    }
}
