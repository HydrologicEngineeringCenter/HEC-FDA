using Xunit;
using HEC.FDA.Model.structures;
using HEC.FDA.Statistics.Distributions;

namespace fda_model_test.unittests.structures
{
    public class ValueRatioWithUncertaintyShould
    {
        [Theory]
        [InlineData(IDistributionEnum.Normal, .1, .6, 0, .95,.764485)]
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
    }
}
