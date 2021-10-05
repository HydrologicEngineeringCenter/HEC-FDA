using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Statistics.Histograms;
using Xunit;

namespace StatisticsTests.Histograms
{


    [ExcludeFromCodeCoverage]
    public class HistogramTests
    {
        [Theory]
        [InlineData(1000000, .001, .025, -1.96)]
        [InlineData(1000000, .001, .975, 1.96)]
        public void NormallyDistributed_Histogram_InvCDF(int n, double binWidth, double prob, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            double[] data = new double[n];
            var rand = new Random();
            for (Int64 i = 0; i<n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            IHistogram histogram = new HistogramBinnedData(obs, 0, 1, binWidth);
            double actual = histogram.InverseCDF(prob);
            double errTol = Math.Abs((expected - actual) / expected);
            Assert.True(errTol < .01);
        }

        [Theory]
        [InlineData(1000000, .001, -1.96, .025)]
        [InlineData(1000000, .001, 1.96, .975)]
        public void NormallyDistributed_Histogram_CDF(int n, double binWidth, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            double[] data = new double[n];
            var rand = new Random();
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            IHistogram histogram = new HistogramBinnedData(obs, 0, 1, binWidth);
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(err < errTol);
        }

        [Theory]
        [InlineData(1000000, .001, 2d, 1d, 2d, 2d)]
        public void NormallyDistributed_Histogram_CentralTendency(int n, double binWidth, double mean, double standardDeviation, double expectedMean, double expectedMedian)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(mean, standardDeviation);
            double[] data = new double[n];
            var rand = new Random();
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            IHistogram histogram = new HistogramBinnedData(obs, 0, 1, binWidth);
            
            double actualMean = histogram.Mean;
            double meanErr = Math.Abs((expectedMean - actualMean) / actualMean);

            double actualMedian = histogram.Median;
            double medianErr = Math.Abs((expectedMedian - actualMedian) / actualMedian);

            double errTol = 0.01;
            Assert.True(meanErr < errTol);
            Assert.True(medianErr < errTol);
    
        }



    }
}
