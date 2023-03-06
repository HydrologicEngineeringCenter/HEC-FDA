using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities;
using Xunit;
using Statistics.Distributions;



namespace StatisticsTests.Distributions
{
  [Trait("RunsOn", "Remote")]
  public class EmpiricalTests
    {
        [Theory]
        [InlineData(1234, 1000000, 0, 1)]
        [InlineData(4321, 1000000, 0, 1)]
        [InlineData(2345, 1000000, 0, 1)]
        [InlineData(5432, 1000000, 0, 1)]
        public void EmpiricalSummaryStatisticsMonotonic_Tests(int seed, int sampleSize, double expectedMean, double expectedStandardDeviation)
        {
            Random random = new Random(seed);
            Normal normal = new Normal(0, 1);
            double[] cumulativeProbabilities = new double[sampleSize];
            double[] observationValues = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                double randomProbability = random.NextDouble();
                cumulativeProbabilities[i] = randomProbability;
                observationValues[i] = normal.InverseCDF(randomProbability);
            }
            //sorted so must be monotonic
            Array.Sort(cumulativeProbabilities);
            Array.Sort(observationValues);
            Empirical empirical = new Empirical(cumulativeProbabilities, observationValues);
            double actualMean = empirical.Mean;
            double actualStandardDeviation = empirical.StandardDeviation;
            Assert.True(Math.Abs(expectedMean - actualMean) < 0.001);
            Assert.True(Math.Abs(expectedStandardDeviation - actualStandardDeviation) < 0.001);
        }

        //In this test, we show that our empirical distribution mimics a parametric distribution
        //only by sampling 2500 probability steps 
        [Theory]
        [InlineData(1234, 2500, 0, 1)]
        [InlineData(4321, 2500, 1, 2)]
        [InlineData(2345, 2500, 2, 3)]
        [InlineData(5432, 2500, 3, 4)]
        public void EmpiricalSummaryStatisticsNonMonotonic_Tests(int seed, int sampleSize, double inputMean, double inputStandardDeviation)
        {
            Random random = new Random(seed);
            Normal normal = new Normal(inputMean, inputStandardDeviation);
            double[] cumulativeProbabilities = new double[sampleSize];
            double[] observationValues = new double[sampleSize];
            int probabilitySteps = sampleSize;
            for (int i = 0; i < sampleSize; i++)
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;
                cumulativeProbabilities[i] = probabilityStep;
                observationValues[i] = normal.InverseCDF(probabilityStep);
            }
            Empirical empirical = new Empirical(cumulativeProbabilities, observationValues);
            double actualMean = empirical.Mean;
            double actualStandardDeviation = empirical.StandardDeviation;
            Assert.True(Math.Abs(inputMean - actualMean) < 0.001);
            Assert.True(Math.Abs(inputStandardDeviation - actualStandardDeviation) < 0.01);
        }

        [Theory]
        [InlineData(1234, 1000000, 0.0080, -2.41)]
        [InlineData(4321, 1000000, 0.1230, -1.161)]
        [InlineData(2345, 1000000, 0.8770, 1.16)]
        [InlineData(5432, 1000000, 0.9970, 2.75)]
        public void EmpiricalInvCDF_Test(int seed, int sampleSize, double cumulativeProbability, double expected)
        {
            Random random = new Random(seed);
            Normal normal = new Normal(0, 1);
            double[] cumulativeProbabilities = new double[sampleSize];
            double[] observationValues = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                double randomProbability = random.NextDouble();
                cumulativeProbabilities[i] = randomProbability;
                observationValues[i] = normal.InverseCDF(randomProbability);
            }
            Array.Sort(cumulativeProbabilities);
            Array.Sort(observationValues);
            Empirical empirical = new Empirical(cumulativeProbabilities, observationValues);
            double actual = empirical.InverseCDF(cumulativeProbability);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1234, 1000000, 0.0080, -2.41)]
        [InlineData(4321, 1000000, 0.1230, -1.161)]
        [InlineData(2345, 1000000, 0.8770, 1.16)]
        [InlineData(5432, 1000000, 0.9970, 2.75)]
        public void EmpiricalCDF_Test(int seed, int sampleSize, double expected, double observationValue)
        {
            Random random = new Random(seed);
            Normal normal = new Normal(0, 1);
            double[] cumulativeProbabilities = new double[sampleSize];
            double[] observationValues = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                double randomProbability = random.NextDouble();
                cumulativeProbabilities[i] = randomProbability;
                observationValues[i] = normal.InverseCDF(randomProbability);
            }
            Array.Sort(cumulativeProbabilities);
            Array.Sort(observationValues);
            Empirical empirical = new Empirical(cumulativeProbabilities, observationValues);
            double actual = empirical.CDF(observationValue);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }
        /// <summary>
        /// Prove that stacking distributions stacks the means 
        /// </summary>
        [Theory]
        [InlineData(1234, 2500, 0,1,2,3,2, -2)]
        [InlineData(1234, 2500, 4, 5, 2,3, 6, 2)]
        public void EmpiricalMeans_Add(int seed, int probabilitySteps, double mean1, double sd1, double mean2, double sd2, double expectedMeanAdded, double expectedMeanSubtracted)
        {
            Random random = new Random(seed);
            Normal normal1 = new Normal(mean1, sd1);
            Normal normal2 = new Normal(mean2, sd2);
            double[] cumulativeProbabilities = new double[probabilitySteps];
            double[] observationValues1 = new double[probabilitySteps];
            double[] observationValues2 = new double[probabilitySteps];
            for (int i = 0; i < probabilitySteps; i++)
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;
                cumulativeProbabilities[i] = probabilityStep;
                observationValues1[i] = normal1.InverseCDF(probabilityStep);
                observationValues2[i] =  normal2.InverseCDF(probabilityStep);

            }
            Empirical empirical1 = new Empirical(cumulativeProbabilities, observationValues1);
            Empirical empirical2 = new Empirical(cumulativeProbabilities, observationValues2);
            List<Empirical> empiricalToStack = new List<Empirical>() { empirical1, empirical2};

            Empirical stackedEmpiricalAdded = Empirical.StackEmpiricalDistributions(empiricalToStack, Empirical.Sum);
            Empirical stackedEmpiricalSubtracted = Empirical.StackEmpiricalDistributions(empiricalToStack, Empirical.Subtract);

            double actualAddedMean = stackedEmpiricalAdded.Mean;
            double differenceAddedMean = Math.Abs(actualAddedMean - expectedMeanAdded);
            double relativeDifferenceAddedMeans = differenceAddedMean / expectedMeanAdded;

            double actualSubtractedMean = stackedEmpiricalSubtracted.Mean;
            double differenceSubtractedMean = Math.Abs(actualSubtractedMean - expectedMeanSubtracted);
            double relativeDifferenceSubtractedMeans = differenceSubtractedMean / expectedMeanSubtracted;

            double tolerance = 0.01;

            Assert.True(relativeDifferenceAddedMeans < tolerance);
            Assert.True(relativeDifferenceSubtractedMeans < tolerance);
        }

        [Fact]
        public void StacksCorrectlyWithDummyData()
        {
            Empirical empirical0 = new Empirical();
            Normal normal = new Normal(1,2);

            int probabilitySteps = 2500;
            double[] probabilities = new double[probabilitySteps];
            double[] invCDFs = new double[probabilitySteps];
            for (int i = 0; i < probabilitySteps; i++)
            {
                probabilities[i] = (i + 0.5) / probabilitySteps;
                invCDFs[i] = normal.InverseCDF(probabilities[i]);
            }

            Empirical empirical1 = new Empirical(probabilities, invCDFs);

            List<Empirical> zeroThen1 = new List<Empirical>() { empirical0, empirical1 };
            List<Empirical> oneThen0 = new List<Empirical>() { empirical1, empirical0 };

            Empirical zeroMinusOne = Empirical.StackEmpiricalDistributions(zeroThen1, Empirical.Subtract);
            Empirical zeroPlusOne = Empirical.StackEmpiricalDistributions(zeroThen1, Empirical.Sum);
            Empirical oneMinusZero = Empirical.StackEmpiricalDistributions(oneThen0, Empirical.Subtract);
            Empirical onePlusZero = Empirical.StackEmpiricalDistributions(oneThen0, Empirical.Sum);

            Assert.Equal(-1, zeroMinusOne.Mean, 2);
            Assert.Equal(1, zeroPlusOne.Mean, 2);
            Assert.Equal(1, oneMinusZero.Mean, 2);
            Assert.Equal(1, onePlusZero.Mean, 2);


        }
    }
}