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
using System.Diagnostics.Metrics;

namespace StatisticsTests.Histograms
{
    [Trait("RunsOn", "Remote")]
    public class HistogramTests
    {
        [Theory]
        [InlineData(1, 1)]
        public void HistogramStatistics_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        public void HistogramStatistics_AddedData_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 7)]
        public void HistogramStatistics_AddedData_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.HistogramMean();
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 1.67705)]
        public void HistogramStatistics_HistogramStandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.HistogramStandardDeviation();
            //double err = Math.Abs((expected - actual) / expected);
            //double tol = 0.01;
            //Assert.True(err < tol);//this gives meaningless error reporting in stacktraces
            Assert.Equal(expected, actual, 3);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 1.414214)]//verified in excel with =STDEV.P(data) incidentally =SQRT(VAR(data)) yeilds 1.581139 which can be achived by not backing out n-1/n when calcuating variance.
        public void HistogramStatistics_StandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
            histogram.AddObservationsToHistogram(data);
            double actual = histogram.StandardDeviation;
            Assert.Equal(expected, actual, 5);//this gives much more meaningful error reporting
        }
        [Theory]
        [InlineData(1, 0.4, 2.25)]
        public void Histogram_InvCDF(double binWidth, double prob, double expected)
        {
            double[] data = new double[14] { 0, 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4 };
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
            DynamicHistogram histogram = new DynamicHistogram(binWidth, new ConvergenceCriteria());
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
        [InlineData(1000, 10000, .1, .80, 1.96, .975)]
        public void NormallyDistributed_Histogram_Convergence(int minIter, int maxiter, double binWidth, double quantile, double value, double expected)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(minIterations: minIter, maxIterations: maxiter, tolerance: 1, zAlpha: z);
            DynamicHistogram histogram = new DynamicHistogram(0, binWidth, convergencecriteria);
            while (!histogram.IsConverged)
            {
                histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()));
                if (histogram.SampleSize % 1000 == 0)
                {
                    histogram.IsHistogramConverged(quantile, 1 - quantile);
                }
            }
            double actual = histogram.CDF(value);
            double err = Math.Abs((expected - actual) / expected);
            double errTol = 0.01;
            Assert.True(histogram.ConvergedIteration <= maxiter);
            Assert.True(err < errTol);
        }

        [Theory]
        [InlineData(10000, .1, .80)]
        public void HistogramReadsTheSameThingItWrites(int maxiter, double binWidth, double quantile)
        {
            IDistribution stdNormal = new Statistics.Distributions.Normal(0, 1);
            var rand = new Random(1234);
            double z = stdNormal.InverseCDF(.5 + .5 * .85);
            var convergencecriteria = new ConvergenceCriteria(maxIterations: maxiter, tolerance: 1, zAlpha: z);
            DynamicHistogram histogram = new DynamicHistogram(0, binWidth, convergencecriteria);
            while (!histogram.IsConverged)
            {
                histogram.AddObservationToHistogram(stdNormal.InverseCDF(rand.NextDouble()));
                if (histogram.SampleSize % 1000 == 0)
                {
                    histogram.IsHistogramConverged(quantile, 1 - quantile);
                }
            }
            XElement xElement = histogram.ToXML();
            DynamicHistogram histogramFromXML = DynamicHistogram.ReadFromXML(xElement);
            bool histogramsAreTheSame = histogram.Equals(histogramFromXML);
            Assert.True(histogramsAreTheSame);
        }

        [Theory]
        //normal 2, 3 and 3, 4
        [InlineData(2, 3, -999, 3, 4, -999)]
        //Tri 
        [InlineData(3, 4, 7, 6, 6.5, 15)]
        //Tri
        [InlineData(3, 4, 7, 6, 15, 15.5)]

        public void HistgramToEmpiricalAreEquivalentDistributions(double mean, double standardDeviationOrMin, double max, double mean2, double standardDeviationOrMin2, double max2)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            int sampleSize = 5000;

            Normal normal = new Normal(mean, standardDeviationOrMin);
            Normal normal2 = new Normal(mean2, standardDeviationOrMin2);
            Triangular triangular = new Triangular(mean, standardDeviationOrMin, max);
            Triangular triangular2 = new Triangular(mean2, standardDeviationOrMin2, max2);
                      
                        
            List<double> resultCollection = new List<double>();
            Random random = new Random(Seed: 1234);
            for (int i = 0; i < sampleSize; i++)
            {
                resultCollection.Add(normal.InverseCDF(random.NextDouble()));
            }

            List<double> resultCollection2 = new List<double>();
            Random random2 = new Random(Seed: 1234);
            for (int i = 0; i < sampleSize; i++)
            {
                resultCollection2.Add(normal2.InverseCDF(random2.NextDouble()));
            }

            List<double> resultCollection3 = new List<double>();
            Random random3 = new Random(Seed: 1234);
            for (int i = 0; i < sampleSize; i++)
            {
                resultCollection3.Add(triangular.InverseCDF(random3.NextDouble()));
            }

            List<double> resultCollection4 = new List<double>();
            Random random4 = new Random(Seed: 1234);
            for (int i = 0; i < sampleSize; i++)
            {
                resultCollection4.Add(triangular2.InverseCDF(random4.NextDouble()));
            }
                        
            
            DynamicHistogram histogram = new DynamicHistogram(resultCollection, convergenceCriteria);
            DynamicHistogram histogram2 = new DynamicHistogram(resultCollection2, convergenceCriteria);
            DynamicHistogram histogram3 = new DynamicHistogram(resultCollection3, convergenceCriteria);
            DynamicHistogram histogram4 = new DynamicHistogram(resultCollection4, convergenceCriteria);
                        
            
            Empirical empirical = DynamicHistogram.ConvertToEmpiricalDistribution(histogram);
            Empirical empirical2 = DynamicHistogram.ConvertToEmpiricalDistribution(histogram2);
            Empirical empirical3 = DynamicHistogram.ConvertToEmpiricalDistribution(histogram3);
            Empirical empirical4 = DynamicHistogram.ConvertToEmpiricalDistribution(histogram4);
                       
            
            double meanDifference = Math.Abs(empirical.Mean - mean);
            double meanRelativeDifference = meanDifference / mean;
            double standardDeviationDifference = Math.Abs(empirical.StandardDeviation - standardDeviationOrMin);
            double standardDeviationRelativeDifference = standardDeviationDifference / standardDeviationOrMin;
            double tolerance = 0.03;
            Assert.True(meanRelativeDifference < tolerance);
            Assert.True(standardDeviationRelativeDifference < tolerance);

            double meanDifference2 = Math.Abs(empirical2.Mean - mean2);
            double meanRelativeDifference2 = meanDifference2 / mean2;
            double standardDeviationDifference2 = Math.Abs(empirical2.StandardDeviation - standardDeviationOrMin2);
            double standardDeviationRelativeDifference2 = standardDeviationDifference2 / standardDeviationOrMin2;
            double tolerance2 = 0.03;
            Assert.True(meanRelativeDifference2 < tolerance2);
            Assert.True(standardDeviationRelativeDifference2 < tolerance2);

            double meanDifference3 = Math.Abs(empirical3.Mean - mean);
            double meanRelativeDifference3 = meanDifference3 / mean;                    
            double standardDeviationDifference3 = Math.Abs(empirical3.StandardDeviation - standardDeviationOrMin);
            double standardDeviationRelativeDifference3 = standardDeviationDifference3 / standardDeviationOrMin;
            double maxDifference = Math.Abs(empirical3.Max - max);
            double maxRelativeDifference = maxDifference / max;
            double tolerance3 = 0.03;
            Assert.True(meanRelativeDifference3 < tolerance3);
            Assert.True(standardDeviationRelativeDifference3 < tolerance3);
            Assert.True(maxRelativeDifference < tolerance3);

            double meanDifference4 = Math.Abs(empirical4.Mean - mean2);
            double meanRelativeDifference4 = meanDifference4 / mean2;
            double standardDeviationDifference4 = Math.Abs(empirical4.StandardDeviation - standardDeviationOrMin2);
            double standardDeviationRelativeDifference4 = standardDeviationDifference4 / standardDeviationOrMin2;
            double maxDifference2 = Math.Abs(empirical4.Max - max2);
            double maxRelativeDifference2 = maxDifference2 / max2;
            double tolerance4 = 0.03;
            Assert.True(meanRelativeDifference4 < tolerance4);
            Assert.True(standardDeviationRelativeDifference4 < tolerance4);
            Assert.True(maxRelativeDifference2 < tolerance4);

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

            DynamicHistogram histogram1 = new DynamicHistogram(min1, binWidth, convergenceCriteria);
            DynamicHistogram histogram2 = new DynamicHistogram(min2, binWidth, convergenceCriteria);
            DynamicHistogram histogram3 = new DynamicHistogram(min3, binWidth, convergenceCriteria);

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
            Int64 sampleSize = 0;
            foreach (IHistogram histogramToAdd in histograms)
            {
                min += histogramToAdd.Min;
                max += histogramToAdd.Max;
                sampleSize += histogramToAdd.SampleSize;
            }
            double range = max - min;
            double binQuantity = 1 + 3.322 * Math.Log(sampleSize); //sturges rule 
            double aggregatedbinWidth = range / binQuantity;

            DynamicHistogram aggregatedHistogram = new DynamicHistogram(min, aggregatedbinWidth, convergenceCriteria);

            for (int i = 0; i < iterations; i++)
            {
                double probabilityStep = (i + 0.5) / iterations;
                double summedValue = 0;
                Int64 summedBinCount = 0;

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

            DynamicHistogram aggregatedHistogramAltStyle = new DynamicHistogram(min, aggregatedbinWidth, convergenceCriteria);
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

        [Theory]
        [InlineData(1.96)]
        public void HistogramConstructsCorrectly(double expected)
        {
            int iterations = 10000;
            Normal normal = new Normal();
            Random random = new Random(1234);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            List<double> data = new List<double>();
            for (int i = 0; i < iterations; i++)
            {
                double randomObservation = normal.InverseCDF(random.NextDouble());
                data.Add(randomObservation);
            }

            DynamicHistogram histogram = new DynamicHistogram(data, convergenceCriteria);
            double actual = histogram.InverseCDF(.975);

            Assert.Equal(expected, actual, 1);

        }

        [Fact]
        public void RecreateDistributionsWithEnoughSamples()
        {
            int sampleSize = 5000;
            double binWidth = .01;
            // Triangular distribution
            double min = 90;
            double max = 110;
            double mode = 100;
            Triangular triangularDistribution = new Triangular(min, mode, max);
            List<double> samples = new List<double>();
            Random random = new Random();
            for (int i = 0; i < sampleSize; i++)
            {
                double sample = triangularDistribution.InverseCDF(random.NextDouble());
                samples.Add(sample);
            }
            DynamicHistogram Trianglehistogram = new DynamicHistogram(min, binWidth, new ConvergenceCriteria());
            Trianglehistogram.AddObservationsToHistogram(samples.ToArray());

            // Normal distribution
            Normal normalDistribution = new Normal(500, 4);
            List<double> normalSamples = new List<double>();
            Random normalRandom = new Random();
            for (int i = 0; i < sampleSize; i++)
            {
                double sample = normalDistribution.InverseCDF(normalRandom.NextDouble());
                normalSamples.Add(sample);
            }
            DynamicHistogram normalHistogram = new DynamicHistogram(min, binWidth, new ConvergenceCriteria());
            normalHistogram.AddObservationsToHistogram(normalSamples.ToArray());


            // Uniform distribution
            Uniform uniformDistribution = new Uniform(100, 130);
            List<double> uniformSamples = new List<double>();
            Random uniformRandom = new Random();
            for (int i = 0; i < sampleSize; i++)
            {
                double sample = uniformDistribution.InverseCDF(uniformRandom.NextDouble());
                uniformSamples.Add(sample);
            }
            DynamicHistogram uniformHistogram = new DynamicHistogram(min, binWidth, new ConvergenceCriteria());
            uniformHistogram.AddObservationsToHistogram(uniformSamples.ToArray());

            //Test
            double[] probabilities = new double[] { .025, 0.25, 0.5, 0.75, .975 };
            double tolerance = 0.025; // Define a tolerance for comparison

            foreach (double probability in probabilities)
            {
                double triangleoriginalValue = triangularDistribution.InverseCDF(probability);
                double trianglehistogramValue = Trianglehistogram.InverseCDF(probability);
                double error = Math.Abs((triangleoriginalValue - trianglehistogramValue) / triangleoriginalValue);
                Assert.True(error < tolerance);

                double uniformoriginalValue = uniformDistribution.InverseCDF(probability);
                double uniformhistogramValue = uniformHistogram.InverseCDF(probability);
                error = Math.Abs((uniformoriginalValue - uniformhistogramValue) / uniformoriginalValue);
                Assert.True(error < tolerance);

                double normaloriginalValue = normalDistribution.InverseCDF(probability);
                double normalhistogramValue = normalHistogram.InverseCDF(probability);
                error = Math.Abs((normaloriginalValue - normalhistogramValue) / normaloriginalValue);
                Assert.True(error < tolerance);
            }

            //Triangle Moments - Here we assume median == mode because this is a symmetric distribution.
            double errorCentral = Math.Abs((triangularDistribution.MostLikely - Trianglehistogram.InverseCDF(0.5)))/triangularDistribution.MostLikely;
            Assert.True(errorCentral < tolerance);

            //Normal Moments
            errorCentral = Math.Abs(normalDistribution.Mean - normalHistogram.Mean) / normalDistribution.Mean;
            double errorStd = Math.Abs((normalDistribution.StandardDeviation - normalHistogram.StandardDeviation)) / normalDistribution.StandardDeviation;
            Assert.True(errorCentral < tolerance);
            Assert.True(errorStd < tolerance);
        }

        [Fact]
        public void HistogramResetToZeroPreservesDistribution()
        {
            Triangular triangular = new Triangular(min: 0.0001, mode: 100, max: 1000);
            DynamicHistogram histogram = new DynamicHistogram(min: triangular.Min, binWidth: 2, new ConvergenceCriteria());
            Random random = new Random(Seed: 1234);
            int sampleSize = 50000;
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                sample[i] = (triangular.InverseCDF(random.NextDouble()));
            }
            histogram.AddObservationsToHistogram(sample);
            double zeroVal = 0;
            histogram.AddObservationToHistogram(zeroVal);

            
            Assert.Equal(zeroVal, histogram.Min);

            double tolerance = 0.06;
            int steps = 100;
            for (int i = 0; i < steps; i++)
            {
                double probabilityStep = i + 1;
                double probability = (probabilityStep) / steps;
                double histogramQuantile = histogram.InverseCDF(probability);
                double triangularQuantile = triangular.InverseCDF(probability);
                double relativeDifference = Math.Abs(( histogramQuantile - triangularQuantile ) / triangularQuantile);
                Assert.True(relativeDifference < tolerance);
            }
        }
    }


}



