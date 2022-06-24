using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Statistics.Histograms;
using Xunit;
using Utilities;
using Statistics.Distributions;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;

namespace StatisticsTests.Histograms
{
    [Trait("Category", "Unit")]
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
            
            for (int i = 0; i < n; i++)
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
        public void NormallyDistributed_Histogram_Convergence(int maxiter, double binWidth, double quantile, double value, double expected)
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
                    histogram.IsHistogramConverged(quantile,1-quantile);
                }
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(histogram.ConvergedIteration < maxiter);
            Assert.True(err < errTol);
        }
        //TODO: This test does not pass for distributions which have a non-negligible share below zero
        [Fact]
        public void HistogramsShouldAddCorrectly()
        {
                Normal normal1 = new Normal(300, 10);
                Normal normal2 = new Normal(400, 20);
                Normal normal3 = new Normal(200, 9);
                double[] probabilities = new double[] {.25, .5, .75};
                double[] expected = new double[probabilities.Length];
                for (int i = 0; i < probabilities.Length; i++)
                {
                    expected[i] = normal1.InverseCDF(probabilities[i]);
                    expected[i] += normal2.InverseCDF(probabilities[i]);
                    expected[i] += normal3.InverseCDF(probabilities[i]);
                }

                double reallySmallProbability = 0.0001;
                ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
                double binWidth = 0.01;
                double min1 = normal1.InverseCDF(reallySmallProbability);
                double min2 = normal2.InverseCDF(reallySmallProbability);
                double min3 = normal3.InverseCDF(reallySmallProbability);

                Histogram histogram1 = new Histogram(min1, binWidth, convergenceCriteria);
                Histogram histogram2 = new Histogram(min2, binWidth, convergenceCriteria);
                Histogram histogram3 = new Histogram(min3, binWidth, convergenceCriteria);

                int seed = 8305;
                Random random = new Random(seed);
                int iterations = 10000;
                for (int i = 0; i < iterations; i++)
                {
                    histogram1.AddObservationToHistogram(normal1.InverseCDF(random.NextDouble()));
                    histogram2.AddObservationToHistogram(normal2.InverseCDF(random.NextDouble()));
                    histogram3.AddObservationToHistogram(normal3.InverseCDF(random.NextDouble()));
                }

                    List<IHistogram> histograms = new List<IHistogram>();
                    histograms.Add(histogram1);
                    histograms.Add(histogram2);
                    histograms.Add(histogram3);
                    IHistogram histogramAddedUp = Histogram.AddHistograms(histograms);

                    double[] actual = new double[probabilities.Length];
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        actual[i] = histogramAddedUp.InverseCDF(probabilities[i]);
                    }

                    double tolerance = 0.05;
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        double error = Math.Abs((actual[i] - expected[i]) / expected[i]);
                        Assert.True(error < tolerance);
                    }
         
        }
        [Theory]
        [InlineData(10000, .1, .80)]
        public void HistogramReadsTheSameThingItWrites(int maxiter, double binWidth, double quantile)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: 1, zAlpha: z);
            Histogram histogram = new Histogram(0, binWidth, convergencecriteria);
            while (!histogram.IsConverged)
            {
                histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()));
                if (histogram.SampleSize % 1000 == 0)
                {
                    histogram.IsHistogramConverged(quantile, 1 - quantile);
                }
            }
            XElement xElement = histogram.WriteToXML();
            Histogram histogramFromXML = Histogram.ReadFromXML(xElement);
            bool histogramsAreTheSame = histogram.Equals(histogramFromXML);
            Assert.True(histogramsAreTheSame);
        }


        /*
         * TODO this test is left commented out because it takes a long time to run. 
         * We should move this test to integration tests
        [Theory]
        [InlineData(1000000, .1, 2d, 1d, 2d, 2d)]
        public void NormallyDistributed_Histogram_CentralTendency(int n, double binWidth, double mean, double standardDeviation, double expectedMean, double expectedMedian)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(mean, standardDeviation);
            var rand = new Random();
            Histogram histogram = new Histogram(null, binWidth);
            double[] data = new double[n];
            for (int i = 0; i < n; i++)
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

        [Fact]
        public void AddingHistograms()
        {
            Normal normal1 = new Normal(3, 1);
            Normal normal2 = new Normal(4, 2);
            Normal normal3 = new Normal(2, 5);
            double[] probabilities = new double[] { .25, .5, .75 };
            double[] expected = new double[probabilities.Length];
            for (int i = 0; i < probabilities.Length; i++)
            {
                expected[i] = normal1.InverseCDF(probabilities[i]);
                expected[i] += normal2.InverseCDF(probabilities[i]);
                expected[i] += normal3.InverseCDF(probabilities[i]);
            }

            double reallySmallProbability = 0.0001;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            double binWidth = 0.01;
            double min1 = normal1.InverseCDF(reallySmallProbability);
            double min2 = normal2.InverseCDF(reallySmallProbability);
            double min3 = normal3.InverseCDF(reallySmallProbability);

            Histogram histogram1 = new Histogram(min1, binWidth, convergenceCriteria);
            Histogram histogram2 = new Histogram(min2, binWidth, convergenceCriteria);
            Histogram histogram3 = new Histogram(min3, binWidth, convergenceCriteria);

            int seed = 8305;
            Random random = new Random(seed);
            int iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                histogram1.AddObservationToHistogram(normal1.InverseCDF(random.NextDouble()));
                histogram2.AddObservationToHistogram(normal2.InverseCDF(random.NextDouble()));
                histogram3.AddObservationToHistogram(normal3.InverseCDF(random.NextDouble()));
            }

            List<IHistogram> histograms = new List<IHistogram>();
            histograms.Add(histogram1);
            histograms.Add(histogram2);
            histograms.Add(histogram3);





            //IHistogram histogramAddedUp = Histogram.AddHistograms(histograms);

            double min = 0;
            double max = 0;
            int sampleSize = 0;
            foreach (IHistogram histogramToAdd in histograms)
            {
                min += histogramToAdd.Min;
                max += histogramToAdd.Max;
                sampleSize += histogramToAdd.SampleSize;
            }
            double range = max - min;
            double binQuantity = 1 + 3.322 * Math.Log(sampleSize); //sturges rule 
            double aggregatedbinWidth = range / binQuantity;

            Histogram aggregatedHistogram = new Histogram(min, aggregatedbinWidth, convergenceCriteria);

            for (int i = 0; i < iterations; i++)
            {
                double probabilityStep = (i + 0.5) / iterations;
                double summedValue = 0;
                int summedBinCount = 0;

                foreach (IHistogram histogramToSample in histograms)
                {
                    double sampledValue = histogramToSample.InverseCDF(probabilityStep);
                    summedValue += sampledValue;
                    summedBinCount += histogramToSample.FindBinCount(sampledValue, false);
                }
                for (int j = 0; j < summedBinCount; j++)
                {
                    aggregatedHistogram.AddObservationToHistogram(summedValue, j);
                }
            }

            Histogram aggregatedHistogramAltStyle = new Histogram(min, aggregatedbinWidth, convergenceCriteria);
            for (int i = 0; i < iterations; i++)
            {
                double obs1 = histogram1.InverseCDF(random.NextDouble());
                double obs2 = histogram2.InverseCDF(random.NextDouble());
                double obs3 = histogram3.InverseCDF(random.NextDouble());
                double summedObservation = obs1 + obs2 + obs3;
                aggregatedHistogramAltStyle.AddObservationToHistogram(summedObservation);
            }

            double[] actualFromAddingMethod = new double[probabilities.Length];
            for (int i = 0; i < probabilities.Length; i++)
            {
                actualFromAddingMethod[i] = aggregatedHistogram.InverseCDF(probabilities[i]);
            }

            double[] actualFromAltMethod = new double[probabilities.Length];
            for (int i = 0; i < probabilities.Length; i++)
            {
                actualFromAltMethod[i] = aggregatedHistogramAltStyle.InverseCDF(probabilities[i]);
            }
            double tolerance = 0.05;
            for (int i = 0; i < probabilities.Length; i++)
            {
                double error = Math.Abs((actualFromAddingMethod[i] - actualFromAltMethod[i]) / actualFromAltMethod[i]);
                Assert.True(error < tolerance);
            }

        }
    }
}
