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
            Statistics.Distributions.Uniform bu = (Statistics.Distributions.Uniform)bootstrap;
            Assert.Equal(.5, (bu.Min+bu.Max)/2);
            Assert.Equal(IDistributionEnum.Uniform, bootstrap.Type);
        }

        [Theory]
        [InlineData(3.537,.438,.075,125,1234, 3.537)]
        public void BootstrapWorksForLP3_Test(double mean, double standardDeviation, double skew, int sampleSize,int seed, double expected)
        {
            IDistribution distributionLP3 = IDistributionFactory.FactoryLogPearsonIII(mean, standardDeviation, skew, sampleSize);
            double[] randomNumberArray = new double[distributionLP3.SampleSize];
            Random random = new Random(seed);
            for (int i = 0; i<distributionLP3.SampleSize; i++)
            {
                randomNumberArray[i] = random.NextDouble();
            }
            IDistribution testBootstrapDistribution = distributionLP3.Sample(randomNumberArray);
            Statistics.Distributions.LogPearson3 tbd = (Statistics.Distributions.LogPearson3)testBootstrapDistribution;
            double actual = tbd.Mean;
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .05);
        }
    }
}
