using Xunit;
using Statistics;
using HEC.FDA.Model.structures;

namespace HEC.FDA.ModelTest.unittests.structures
{
    public class ValueUncertaintyShould
    {
        //test deterministic and test a negative value 
        [Theory]
        [InlineData(IDistributionEnum.Normal, .10, 0, 100, .2, 91.58379)]
        [InlineData(IDistributionEnum.LogNormal, .10, 0, 2, .8, 8.74362)]
        [InlineData(IDistributionEnum.Triangular, .1, .2, 100, .95, 114.5228)]
        [InlineData(IDistributionEnum.Uniform, .2, .3, 200, .05, 165)]
        [InlineData(IDistributionEnum.Deterministic, .98349, .2343, 10, .593497, 10)] //deterministic should only return the inventory value
        [InlineData(IDistributionEnum.Normal, 1.5, 0, 100, .01, 0)] //if value sampled is negative, return 0 
        public void ValueUncertaintyShouldSampleCorrectly(IDistributionEnum distributionEnum, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax, double inventoryValue, double probability, double expected)
        {
            ValueUncertainty valueUncertainty = new ValueUncertainty(distributionEnum, percentOfInventoryValueStandardDeviationOrMin, percentOfInventoryMax);
            double actual = valueUncertainty.Sample(inventoryValue, probability);
            Assert.Equal(expected, actual, 1);
        }
    }
}
