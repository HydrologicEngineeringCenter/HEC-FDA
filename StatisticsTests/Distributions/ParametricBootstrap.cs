using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Statistics;

namespace StatisticsTests.Distributions
{
    public class ParametricBootstrap
    {
        [Theory]
        [InlineData(0, 1d, 100)]
        public void Bootstrap_Works_with_Reasonable_RandomRange(double min, double max, int n)
        {
            Statistics.Distributions.Uniform dist = new Statistics.Distributions.Uniform(min, max, n);
            double[] randyPacket = new double[dist.SampleSize];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < dist.SampleSize; i++)
            {
                randyPacket[i] = ((double)i +0.5) / (double)dist.SampleSize;
            }
            Statistics.IDistribution bootstrap = dist.Sample(randyPacket);
            Assert.Equal(bootstrap.Mean, .5);
            Assert.Equal(bootstrap.Type, IDistributionEnum.Uniform);
        }
    }
}
