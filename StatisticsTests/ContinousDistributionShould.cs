using Statistics;
using System;
using Xunit;

namespace StatisticsTests
{
    public class ContinousDistributionShould
    {
        [Fact]
        public void ComputeBootstrap_LP3()
        {
            Statistics.Distributions.LogPearson3 lp3 = new Statistics.Distributions.LogPearson3(1, 1, 1, 100);
            double[] probs = NextRandomSequence(lp3.SampleSize, 1234);
            IDistribution bootstrap = lp3.Sample(probs);
            double value = bootstrap.InverseCDF(.5);
            Assert.Equal(7.09299165828198, value,6);
        }
        [Fact]
        public void ComputeMeanBootstrap_LP3()
        {
            Statistics.Distributions.LogPearson3 lp3 = new Statistics.Distributions.LogPearson3(1, 1, 1, 100);
            double[] probs = NextNonRandomSequence(lp3.SampleSize);
            IDistribution bootstrap = lp3.Sample(probs);
            double value = bootstrap.InverseCDF(.5);
            Assert.Equal(7.04066080278019, value, 6);
        }
        private double[] NextRandomSequence(int size, int seed)
        {
            Random _rng = new Random(seed);
            double[] randyPacket = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randyPacket[i] = _rng.NextDouble();
            }
            return randyPacket;
        }
        private double[] NextNonRandomSequence(int size)
        {
            double[] randyPacket = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randyPacket[i] = (((double)i)+.5) / (double)size;
            }
            return randyPacket;
        }
    }
}
