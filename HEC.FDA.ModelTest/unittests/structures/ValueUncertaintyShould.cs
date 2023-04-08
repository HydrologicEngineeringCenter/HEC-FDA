using Xunit;
using Statistics;
using HEC.FDA.Model.structures;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class ValueUncertaintyShould
    {
        //test deterministic and test a negative value 
        [Theory]
        [InlineData(IDistributionEnum.Normal, 10, 0, 100, .2, 91.58379)]
        [InlineData(IDistributionEnum.LogNormal, 10, 0, 2, .8, 8.74362)]
        [InlineData(IDistributionEnum.Triangular, 90, 120, 100, .95, 114.5228)]
        [InlineData(IDistributionEnum.Uniform, 80, 130, 200, .05, 165)]
        [InlineData(IDistributionEnum.Deterministic, 98.349, 23.43, 10, .593497, 10)] //deterministic should only return the inventory value
        [InlineData(IDistributionEnum.Normal, 150, 0, 100, .01, 0)] //if value sampled is negative, return 0 
        public void ValueUncertaintyShouldSampleCorrectly(IDistributionEnum distributionEnum, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax, double inventoryValue, double probability, double expected)
        {
            ValueUncertainty valueUncertainty = new ValueUncertainty(distributionEnum, percentOfInventoryValueStandardDeviationOrMin, percentOfInventoryMax);
            double valueOffset = valueUncertainty.Sample(probability, computeIsDeterministic: false);
            double actual = inventoryValue*valueOffset;
            Assert.Equal(expected, actual, 1);
        }
    }
}
