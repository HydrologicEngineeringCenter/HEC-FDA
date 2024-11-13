using Xunit;
using Statistics;
using HEC.FDA.Model.structures;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    public class ValueRatioWithUncertaintyShould
    {
        [Theory]
        [InlineData(IDistributionEnum.Normal, .1, .6, 0, .95, .764485)]
        [InlineData(IDistributionEnum.LogNormal, .57, -.85, 0, .05, .167367)]
        [InlineData(IDistributionEnum.Triangular, .7, .85, .9, .88, .865359)]
        [InlineData(IDistributionEnum.Uniform, .6, .8, .95, .40, .74)]
        [InlineData(IDistributionEnum.Deterministic, 394802, .8, 20442, 333, .8)]
        [InlineData(IDistributionEnum.Normal, .5, .1, 0, .01, 0)]
        public void ValueRatioWithUncertaintyShouldSampleCorrectly(IDistributionEnum distributionEnum, double standardDeviationOrMin, double centralTendency, double max, double probability, double expected)
        {
            ValueRatioWithUncertainty valueRatioWithUncertainty = new ValueRatioWithUncertainty(distributionEnum, standardDeviationOrMin, centralTendency, max);
            double actual = valueRatioWithUncertainty.Sample(probability);
            Assert.Equal(expected, actual, 1);
        }


        [Fact]
        public void ValidationShould()
        {
            IDistributionEnum distributionEnum = IDistributionEnum.LogPearsonIII;
            double min = -1;
            double centralTendancy = 0;
            double max = -2;
            ValueRatioWithUncertainty valueRatioWithUncertainty = new ValueRatioWithUncertainty(distributionEnum, min, centralTendancy, max);
            valueRatioWithUncertainty.Validate();
            foreach (PropertyRule rule in valueRatioWithUncertainty.RuleMap.Values)
            {
                Assert.Equal(HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal, rule.ErrorLevel);
            }

        }
    }
}
