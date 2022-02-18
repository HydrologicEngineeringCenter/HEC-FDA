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
    public class HistogramTests
    {
        [Theory]
        [InlineData(1, 1)]
        public void HistogramStatistics_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        public void HistogramStatistics_AddedData_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            histogram.AddObservationToHistogram(0);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 6)]
        public void HistogramStatistics_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 7)]
        public void HistogramStatistics_AddedData_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            histogram.AddObservationToHistogram(6);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 3.5)]
        public void HistogramStatistics_Mean(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.HistogramMean();
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 1.67705)]
        public void HistogramStatistics_HistogramStandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.HistogramStandardDeviation();
            //double err = Math.Abs((expected - actual) / expected);
            //double tol = 0.01;
            //Assert.True(err < tol);//this gives meaningless error reporting in stacktraces
            Assert.Equal(expected, actual,3);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 1.414214)]//verified in excel with =STDEV.P(data) incidentally =SQRT(VAR(data)) yeilds 1.581139 which can be achived by not backing out n-1/n when calcuating variance.
        public void HistogramStatistics_StandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.StandardDeviation;
            Assert.Equal(expected,actual,5);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 0.4, 2.25)]
        public void Histogram_InvCDF(double binWidth, double prob, double expected)
        {
            double[] data = new double[14] {0,0,1,1,1,2,2,2,2,3,3,3,4,4};
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
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
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
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
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
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
            Histogram histogram = new Histogram(binWidth);
            histogram.AddObservationsToHistogram(data);
            double[] newData = new double[2] { 7, 9 };
            histogram.AddObservationsToHistogram(newData);
            double expected = 10;
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }
        /*
        [Theory]
        [InlineData(10000000, .001, -1.96, .025)]
        [InlineData(10000000, .001, 1.96, .975)]
        public void NormallyDistributed_Histogram_CDF(int n, double binWidth, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            Histogram histogram = new Histogram(null, binWidth);
            double[] data = new double[n];
            
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(err < errTol);
        }
        */
        [Theory]
        [InlineData(10000, .1, .80, 1.96, .975)]
        public void NormallyDistributed_Histogram_Convergence(Int64 maxiter, double binWidth, double quantile, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: 1, zAlpha: z);
            Histogram histogram = new Histogram(0, binWidth, convergencecriteria);
            while(!histogram.IsConverged)
            {
                histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()));
                if (histogram.SampleSize%1000 == 0){
                    histogram.TestForConvergence(quantile,1-quantile);
                }
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(histogram.ConvergedIteration < maxiter);
            Assert.True(err < errTol);
        }
        /*
        [Theory]
        [InlineData(1000000, .1, 2d, 1d, 2d, 2d)]
        public void NormallyDistributed_Histogram_CentralTendency(int n, double binWidth, double mean, double standardDeviation, double expectedMean, double expectedMedian)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(mean, standardDeviation);
            var rand = new Random();
            Histogram histogram = new Histogram(null, binWidth);
            double[] data = new double[n];
            for (Int64 i = 0; i < n; i++)
            {
                var randProb = rand.NextDouble();
                data[i] = stdNormal.InverseCDF(randProb);
            }
            histogram.AddObservationsToHistogram(data);
            double actualMean = histogram.Mean;
            double meanErr = Math.Abs((expectedMean - actualMean) / actualMean);

            double actualMedian = histogram.InverseCDF(.5);
            double medianErr = Math.Abs((expectedMedian - actualMedian) / actualMedian);

            double errTol = 0.01;
            Assert.True(meanErr < errTol);
            Assert.True(medianErr < errTol);

        }
        */
    }
}
