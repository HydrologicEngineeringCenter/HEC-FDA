using Xunit;
using Statistics;
using HEC.FDA.Model.structures;
using System;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class ValueUncertaintyShould
    {
        //test deterministic and test a negative value 
        //A demontration of the use of log normal can be found here: https://docs.google.com/spreadsheets/d/1oZMH9EUAe5J3WnCWD-7xZJkcR6z_pYu2/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        [Theory]
        [InlineData(IDistributionEnum.Normal, 10, 0, 100, .2, 91.58379)]
        [InlineData(IDistributionEnum.LogNormal, 20.3852, 0, 42.15755, .8, 80.10293)]
        [InlineData(IDistributionEnum.Triangular, 90, 120, 100, .95, 114.5228)]
        [InlineData(IDistributionEnum.Uniform, 80, 130, 200, .05, 165)]
        [InlineData(IDistributionEnum.Deterministic, 98.349, 23.43, 10, .593497, 10)] //deterministic should only return the inventory value
        [InlineData(IDistributionEnum.Normal, 150, 0, 100, .01, 0)] //if value sampled is negative, return 0 
        public void ValueUncertaintyShouldSampleCorrectly(IDistributionEnum distributionEnum, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax, double inventoryValue, double probability, double expected)
        {
            ValueUncertainty valueUncertainty = new ValueUncertainty(distributionEnum, percentOfInventoryValueStandardDeviationOrMin, percentOfInventoryMax);
            double valueOffset = valueUncertainty.Sample(probability, computeIsDeterministic: false);
            double actual = 0;
            if (distributionEnum.Equals(IDistributionEnum.LogNormal))
            {
                actual = Math.Pow(valueOffset, Math.Log(inventoryValue)) * inventoryValue;
            } 
            else
            {
                actual = inventoryValue*valueOffset;
            }
            Assert.Equal(expected, actual, 1);
        }
    }
}
