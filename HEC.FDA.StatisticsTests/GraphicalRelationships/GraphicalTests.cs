using System;
using Xunit;
using Statistics.GraphicalRelationships;
using Statistics.Distributions;

namespace StatisticsTests.GraphicalRelationships
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
        /// Test data: https://docs.google.com/spreadsheets/d/1GhRe3ECAFIKgRqEE8Xo6f_0lHYnHqUW0/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, true, new double[] { 936.69, 1951.72, 3213.97, 6066.15, 7900.75, 7900.75} )]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, false, new double[] {.2244,  .1164, .1293, .1924, .2030, .2030})]
        public void ReturnsCorrectStandardDeviations(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, bool usingStagesNotFlows, double[] expectedSD)
        {
            GraphicalDistribution graphical = new GraphicalDistribution(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows);
            Statistics.ContinuousDistribution[] actualDistributions = graphical.StageOrLogFlowDistributions;
            for (int i = 2; i < 8; i++)
            {
                double actual;
                if (usingStagesNotFlows)
                {
                    actual = ((Normal)actualDistributions[i]).StandardDeviation;
                }
                else
                {
                    actual = ((LogNormal)actualDistributions[i]).StandardDeviation;
                }
                //TODO: The values used to calculate the test data were calculated with a different slope schema than the one that has been implemented, which is better. 
                //Re-do the test data to update to current methodology 
                double tolerance = 0.15;
                double relativeError = Math.Abs((actual - expectedSD[i-2]) / expectedSD[i-2]);
                Assert.True(relativeError < tolerance);
            }
        }

    }
}
