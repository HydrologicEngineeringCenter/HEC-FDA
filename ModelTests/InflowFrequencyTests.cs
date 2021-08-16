using Functions;
using MathNet.Numerics.Distributions;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ModelTests
{
    public class InflowFrequencyTests
    {
        [Theory]
        [InlineData(1d, 1d, 1d, 100)]
        public void GoodData_InflowFrequency_HasTruncatedFlows(double mean, double sd, double skew, int n)
        {
            Statistics.IDistribution dist = IDistributionFactory.FactoryLogPearsonIII(mean, sd, skew, n);
            IFunction fx = IFunctionFactory.Factory(dist);
            Model.IFrequencyFunction freqFx = IFrequencyFunctionFactory.Factory(fx, IParameterEnum.InflowFrequency);
            var testObj = Utilities.IRangeFactory.Factory(
                freqFx.F(IOrdinateFactory.Factory(.01)).Value(),
                freqFx.F(IOrdinateFactory.Factory(0.999)).Value());
            Assert.True(testObj.Equals(freqFx.Range));
        }
    }
}
