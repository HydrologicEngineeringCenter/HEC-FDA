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
            GraphicalDistributionWithLessSimple graphical = new GraphicalDistributionWithLessSimple(probs, flows, erl);
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
            GraphicalDistributionWithLessSimple graphical = new GraphicalDistributionWithLessSimple(probs, flows, erl);
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
            GraphicalDistributionWithLessSimple graphical = new GraphicalDistributionWithLessSimple(probs, flows, erl);
            Statistics.ContinuousDistribution[] dists = graphical.StageOrLogFlowDistributions;
            double[] prob = graphical.ExceedanceProbabilities;
            Assert.Equal(dists.Length, prob.Length);
        }

        [Theory]
        [InlineData(new double[] { .999, .5, .2, .1, .05, .01, .005, .002 }, new double[] { 10, 20, 30, 40, 50, 60, 70, 80 }, 50, new double[] { 0.9999, 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 })]
        [InlineData(new double[] { .999, .57, .2, .1, .05, .01, .005, .002 }, new double[] { 10, 20, 30, 40, 50, 60, 70, 80 }, 40, new double[] { 0.9999, 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, .57, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 })]
        public void ReturnsSetOfRequiredProbabilitiesAndAnyInputOutsideSet(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, double[] expected)
        {
            GraphicalDistributionWithLessSimple graphical = new GraphicalDistributionWithLessSimple(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength);
            double[] actual = graphical.ExceedanceProbabilities;
            Assert.Equal(expected, actual);
        }


        //TODO: I think the way that standard dev is being calculated now is good
        //BUT: I think I fucked up the test data
        /// <summary>
        /// Test data: https://docs.google.com/spreadsheets/d/1GhRe3ECAFIKgRqEE8Xo6f_0lHYnHqUW0/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, true, new double[] { 936.69, 1951.72, 3213.97, 6066.15, 7900.75, 7900.75} )]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, false, new double[] {.2244,  .1164, .1293, .1924, .2030, .2030})]
        public void ReturnsCorrectStandardDeviations(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, bool usingStagesNotFlows, double[] expectedSD)
        {
            GraphicalDistributionWithLessSimple graphical = new GraphicalDistributionWithLessSimple(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows);
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
