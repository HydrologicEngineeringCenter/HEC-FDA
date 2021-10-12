using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Statistics.Histograms;
using Xunit;
using Utilities;

namespace StatisticsTests.Histograms
{


    [ExcludeFromCodeCoverage]
    public class HistogramTests
    {
        [Theory]
        [InlineData(.001, .025, -1.96, .975, .01, 1000)]
        [InlineData(.001, .975, 1.96, .975, .01, 1000)]
        public void NormallyDistributed_Histogram_InvCDF(double binWidth, double prob, double expected, double quantile, double tolerance, int minNewObservations)
        {
            double min = -1, max = 1;
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            IRange<int> testRange = IRangeFactory.Factory(1000, 100000000);
            IConvergenceCriteria criteria = new ConvergeCriteria(quantile, tolerance, minNewObservations, testRange);

            var converged = IConvergenceResultFactory.Factory();
            // instantiate an arbitrary histogram 
            Histogram histogram = new Histogram(binWidth, min, max);
            var rand = new Random();
            
            while (!converged.Passed)
            {

                // add sampled data to histogram using factory, existing histogram, sampled data in IData object
                double[] data = new double[minNewObservations];

                double oldQuantile = histogram.InverseCDF(quantile);
                for (Int64 i = 0; i < minNewObservations; i++)
                {
                    var randProb = rand.NextDouble();
                    data[i] = stdNormal.InverseCDF(randProb);
                }

                IData obs = new Data(data);
                Histogram.AddObservationsToHistogram(histogram, obs);
                double newQuantile = histogram.InverseCDF(quantile);
                // test convergence 
                converged = criteria.Test(oldQuantile, newQuantile, minNewObservations, histogram.SampleSize);

            }

            double actual = histogram.InverseCDF(prob);
            double errTol = Math.Abs((expected - actual) / expected);
            Assert.True(errTol < .01);
        }

        [Theory]
        [InlineData(1000000, .001, -1.96, .025)]
        [InlineData(1000000, .001, 1.96, .975)]
        public void NormallyDistributed_Histogram_CDF(int n, double binWidth, double value, double expected)
        {
            double min = -1, max = 1;
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            Histogram histogram = new Histogram(binWidth, min, max);
            double[] data = new double[n];
            var rand = new Random();
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            Histogram.AddObservationsToHistogram(histogram, obs);
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(err < errTol);
        }

        [Theory]
        [InlineData(1000000, .001, 2d, 1d, 2d, 2d)]
        public void NormallyDistributed_Histogram_CentralTendency(int n, double binWidth, double mean, double standardDeviation, double expectedMean, double expectedMedian)
        {
            double min = -1, max = 1;
            Histogram histogram = new Histogram(binWidth, min, max);
            IDistribution stdNormal = new Statistics.Distributions.Normal(mean, standardDeviation);
            double[] data = new double[n];
            var rand = new Random();
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            Histogram.AddObservationsToHistogram(histogram, obs);            
            double actualMean = histogram.Mean;
            double meanErr = Math.Abs((expectedMean - actualMean) / actualMean);

            double actualMedian = histogram.Median;
            double medianErr = Math.Abs((expectedMedian - actualMedian) / actualMedian);

            double errTol = 0.01;
            Assert.True(meanErr < errTol);
            Assert.True(medianErr < errTol);
    
        }

        [Theory]
        [InlineData(0,6,1)]
        public void Fit_ExpandsHistogram_WithDataOutOfRange(double min, double max, double binWidth)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(binWidth, min, max);
            Histogram.AddObservationsToHistogram(histogram, obs);
            double[] newData = new double[2] { 7, 9 };
            IData newObs = new Data(newData);
            Histogram.AddObservationsToHistogram(histogram, newObs);
            double expected = 9.5;
            double actual = histogram.Range.Max;
            Assert.Equal(expected, actual);
        }



    }
}
