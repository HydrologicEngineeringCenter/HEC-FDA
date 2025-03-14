using System;
using Xunit;
using HEC.FDA.Model.extensions;
using Statistics.Distributions;
using HEC.FDA.ModelTest.unittests.extensions;

namespace HEC.FDA.ModelTest.unittests.extensions
{
    [Trait("RunsOn", "Remote")]
    public class GraphicalTests
    {
        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 10)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
            new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
            20)]
        public void ReturnDistributionsForInputProbabilities(double[] probs, double[] flows, int erl)
        {
            GraphicalDistribution graphical = new GraphicalDistribution(probs, flows, erl);
            double[] outputProbs = graphical.ExceedanceProbabilities;
            foreach (double value in probs)
            {
                Assert.Contains(value, outputProbs);
            }
        }
        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 5)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
            new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
            20)]
        public void ReturnDistributionsWithInputMeanValues(double[] probs, double[] flows, int erl)
        {
            GraphicalDistribution graphical = new GraphicalDistribution(probs, flows, erl);
            Statistics.ContinuousDistribution[] dists = graphical.StageOrLogFlowDistributions;
            double[] means = new double[dists.Length];
            for (int i = 0; i < dists.Length; i++)
            {
                means[i] = ((Normal)dists[i]).Mean;
            }
            foreach (double value in flows)
            {
                Assert.Contains(value, means);
            }
        }
        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 5)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
            new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
            20)]
        public void ReturnSameNumberOfProbabilitesAsDistributions(double[] probs, double[] flows, int erl)
        {
            GraphicalDistribution graphical = new GraphicalDistribution(probs, flows, erl);
            Statistics.ContinuousDistribution[] dists = graphical.StageOrLogFlowDistributions;
            double[] prob = graphical.ExceedanceProbabilities;
            Assert.Equal(dists.Length, prob.Length);
        }

        /// <summary>
        /// Make it clear what assumptions take place in the development of the test case/expected values
        /// </summary>
        [Theory]
        [InlineData(new double[] {.5, .2, .1, .04, .02, .01, .005, .002}, new double[] {1, 1.1, 4.93, 4.98, 5.02, 5.04, 5.18, 5.3}, 40, true, new double[] {.2, .08, .05, .04, .015, .002 }, new double[] {1.149, .031, .036, .044, .038, .088})] //Algiers
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .005, .002 }, new double[] { 0, 1340.37, 1899.36, 2279.29, 2731.60, 3000.05, 3416.28, 3767.17, 4207.08 }, 20, false, new double[] { .2, .1, .06, .02, .005 }, new double[] { 271.8, 363.61, 478.99, 765.9, 1489.73})] //Tafuna
        public void ReturnsCorrectStandardDeviations(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, bool usingStagesNotFlows, double[] frequenciesAtWhichToCheck, double[] expectedSD)
        {
            GraphicalDistribution graphical = new GraphicalDistribution(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows);
            Statistics.ContinuousDistribution[] actualDistributions = graphical.StageOrLogFlowDistributions;
            for (int i = 0; i < frequenciesAtWhichToCheck.Length; i++)
            {
                double actual;
                double expected = expectedSD[i];
                double frequencyAtWhichToCheck = frequenciesAtWhichToCheck[i];
               
                int indexAtWhichToCheck = 0;
                for (int j = 0; j < graphical.ExceedanceProbabilities.Length; j++)
                {
                    if (graphical.ExceedanceProbabilities[j] == frequencyAtWhichToCheck)
                    {
                        indexAtWhichToCheck = j;
                        break;
                    }
                }

                if (usingStagesNotFlows)
                {
                    actual = ((Normal)actualDistributions[indexAtWhichToCheck]).StandardDeviation;
                    double levelToleranceInFeet = 0.20; //allowable error is 2 tenths of a foot 
                    double absoluteError = Math.Abs((actual - expected));
                    Assert.True(absoluteError < levelToleranceInFeet);
                }
                else
                {
                    actual = Math.Exp(((LogNormal)actualDistributions[indexAtWhichToCheck]).StandardDeviation);
                    double levelTolerance = 275; //allowable error is 275 CFS 
                    double levelError = Math.Abs((actual - expected));
                    Assert.True(levelError < levelTolerance);
                }

            }
        }

        [Theory]
        [InlineData(0.25, 1 / 0.1797, 50, 0.3408)]
        [InlineData(0.963, 1 / 0.0017, 50, 15.3195)]
        public void Equation6Should(double nonExceedanceProbability, double slope, int erl, double expected)
        {
            double actual = GraphicalDistribution.Equation6StandardError(nonExceedanceProbability, slope, erl);
            Assert.Equal(expected, actual, 0.4);
        }

        [Theory]
        [InlineData(0.999, 0.99, 900, 800, 0.995, 832.6595)]
        [InlineData(0.34, 0.31, 1002, 1000, 0.32, 1000.6752)]
        [InlineData(0.004, 0.002, 1200, 1100, 0.0025, 1131.4598)]
        public void InterpolateNormallyShould(double p, double p_minus, double q, double q_minus, double p_minusEpsilon, double expected)
        {
            double actual = GraphicalDistribution.InterpolateNormally(p, p_minus, q, q_minus, p_minusEpsilon);
            Assert.Equal(expected, actual, 0.01);
        }

        [Theory]
        [InlineData(new double[] { 0.5, 0.2, 0.1 }, new double[] { 102, 104, 104.2 }, 1, 5.0560)]
        [InlineData(new double[] { 0.99, 0.5, 0.2 }, new double[] { 101.5, 102, 104 }, 1, 3.2477)]
        public void ComputeSlopeShould(double[] exceedanceProbabilities, double[] stageOrLoggedFlowValues, int index, double expected)
        {
            double actual = GraphicalDistribution.ComputeSlope(exceedanceProbabilities, stageOrLoggedFlowValues, index);
            Assert.Equal(expected, actual, 0.5);
        }
    }
}
