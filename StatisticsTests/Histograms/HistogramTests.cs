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
        [InlineData(1, 1)]
        public void HistogramStatistics_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        public void HistogramStatistics_AddedData_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] additionalData = new double[1] { 0 };
            IData additionalObs = new Data(additionalData);
            histogram.AddObservationsToHistogram(additionalObs);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 6)]
        public void HistogramStatistics_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 7)]
        public void HistogramStatistics_AddedData_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] additionalData = new double[1] { 6 };
            IData additionalObs = new Data(additionalData);
            histogram.AddObservationsToHistogram(additionalObs);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 3.5)]
        public void HistogramStatistics_Mean(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Mean;
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 1.58113883)]
        public void HistogramStatistics_StandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.StandardDeviation;
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1, 0.4, 2.25)]
        public void Histogram_InvCDF(double binWidth, double prob, double expected)
        {
            double[] data = new double[14] {0,0,1,1,1,2,2,2,2,3,3,3,4,4};
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.InverseCDF(prob);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1, 2, .4)]
        public void Histogram_CDF(double binWidth, double val, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.CDF(val);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
            
        }

        [Theory]
        [InlineData(1, 2, .2)]
        public void Histogram_PDF(double binWidth, double val, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.PDF(val);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1)]
        public void Fit_ExpandsHistogram_WithDataOutOfRange(double binWidth)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] newData = new double[2] { 7, 9 };
            IData newObs = new Data(newData);
            histogram.AddObservationsToHistogram(newObs);
            double expected = 10;
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1000000, .001, -1.96, .025)]
        [InlineData(1000000, .001, 1.96, .975)]
        public void NormallyDistributed_Histogram_CDF(int n, double binWidth, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random();
            double[] initialObs = new double[1] { stdNormal.InverseCDF(rand.NextDouble()) };
            IData initialData = new Data(initialObs);
            Histogram histogram = new Histogram(initialData, binWidth);
            double[] data = new double[n];
            
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            histogram.AddObservationsToHistogram(obs);
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(err < errTol);
        }

        [Theory]
        [InlineData(1000000, .1, 2d, 1d, 2d, 2d)]
        public void NormallyDistributed_Histogram_CentralTendency(int n, double binWidth, double mean, double standardDeviation, double expectedMean, double expectedMedian)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(mean, standardDeviation);
            var rand = new Random();
            double[] initialObs = new double[1] { stdNormal.InverseCDF(rand.NextDouble()) };
            IData initialData = new Data(initialObs);
            Histogram histogram = new Histogram(initialData, binWidth);
            double[] data = new double[n];
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            IData obs = new Data(data);
            histogram.AddObservationsToHistogram(obs);
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
