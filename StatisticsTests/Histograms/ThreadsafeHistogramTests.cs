using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Statistics.Histograms;
using Xunit;
using Utilities;
using System.Threading.Tasks;

namespace StatisticsTests.Histograms
{
    public class ThreadsafeHistogramTests
    {
        [Theory]
        [InlineData(1, 1)]
        public void HistogramStatistics_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach(double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(1, 0)]
        public void HistogramStatistics_AddedData_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            histogram.SetIterationSize(1);
            histogram.AddObservationToHistogram(0,0);
            histogram.ForceDeQueue();
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(1, 6)]
        public void HistogramStatistics_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(1, 7)]
        public void HistogramStatistics_AddedData_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            histogram.SetIterationSize(1);
            histogram.AddObservationToHistogram(6,0);
            histogram.ForceDeQueue();
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(1, 3.5)]
        public void HistogramStatistics_Mean(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double actual = histogram.HistogramMean();
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(1, 1.67705)]
        public void HistogramStatistics_HistogramStandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double actual = histogram.HistogramStandardDeviation();
            Assert.Equal(expected, actual, 3);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 1.414214)]//verified in excel with =STDEV.P(data) incidentally =SQRT(VAR(data)) yeilds 1.581139 which can be achived by not backing out n-1/n when calcuating variance.
        public void HistogramStatistics_StandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double actual = histogram.StandardDeviation;
            Assert.Equal(expected, actual, 5);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 0.4, 2.25)]
        public void Histogram_InvCDF(double binWidth, double prob, double expected)
        {
            double[] data = new double[14] { 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4 };
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(14);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
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
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
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
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
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
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(5);
            int i = 0;
            foreach (double observation in data)
            {
                histogram.AddObservationToHistogram(observation, i);
                i++;
            }
            histogram.ForceDeQueue();
            double[] newData = new double[2] { 7, 9 };
            histogram.SetIterationSize(2);
            int j = 0;
            foreach (double observation in newData)
            {
                histogram.AddObservationToHistogram(observation, j);
                j++;
            }
            histogram.ForceDeQueue();
            double expected = 10;//computed by test.
            double actual = histogram.Max;
            Assert.Equal(expected, actual,5);
        }
        [Theory]
        [InlineData(10000, .1, .80, 1.96, .975)]
        public void NormallyDistributed_Histogram_Convergence(Int64 maxiter, double binWidth, double quantile, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: .1, zAlpha: z);
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, convergencecriteria);
            int iter = 0;
            histogram.SetIterationSize(maxiter+1);
            while (!histogram.IsConverged)
            {
                histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()), iter);
                iter++;
                if (iter % 10000 == 0)
                {
                    histogram.TestForConvergence(quantile, 1 - quantile);
                }
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);

            Assert.True(histogram.ConvergedIteration >= maxiter);
            Assert.True(err<.05);
        }
        [Theory]
        [InlineData(10000, .1, .80, 1.96, .975)]
        public void Parallel_Histogram_Convergence(Int64 maxiter, double binWidth, double quantile, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: .01, zAlpha: z);
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, convergencecriteria);
            while (!histogram.IsConverged)
            {
                histogram.SetIterationSize(10000);
                Parallel.For(0, 10000, index =>
                 {
                    histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()), index);
                 });
                histogram.TestForConvergence(quantile, 1 - quantile);
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            Assert.True(histogram.ConvergedIteration >= maxiter);
            double tolerance = .01;
            Assert.True(err<tolerance);
        }
        [Theory]
        [InlineData(10000000, .80, 1.96, .975)]
        public void Parallel_Histogram_Convergence_automatic(Int64 maxiter, double quantile, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: .1, zAlpha: z);
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(convergencecriteria);
            Int64 iterations = convergencecriteria.MinIterations;
            object whilelock = new object();
            while (!histogram.IsConverged)
            {
                histogram.SetIterationSize(iterations);
                lock (whilelock)
                {
                    Parallel.For(0, iterations, index =>
                    {
                        histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()), index);
                    });
                }
                histogram.TestForConvergence(quantile, 1 - quantile);
                iterations = histogram.EstimateIterationsRemaining(quantile, 1 - quantile);
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            Assert.Equal(expected, actual, 2);
        }
        [Theory]
        [InlineData(.1, 1)]
        public void IsHistogramConstructableWithNullData(double binWidth, double expected)
        {
            ThreadsafeInlineHistogram histogram = new ThreadsafeInlineHistogram(binWidth, new ConvergenceCriteria());
            histogram.SetIterationSize(1);
            histogram.AddObservationToHistogram(.05,0);
            histogram.ForceDeQueue();
            double actual = histogram.BinCounts[0];
            Assert.Equal(expected, actual);
        }

    }
}
