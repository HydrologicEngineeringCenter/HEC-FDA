using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alternatives;
using compute;
using interfaces;
using metrics;
using paireddata;
using scenarios;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using Xunit;
using System.Diagnostics;
using System.Threading;

namespace fda_model_test.integrationtests
{
    public class GraphicalUncertaintyShould
    {
        private static CurveMetaData curveMetaData = new CurveMetaData();
        [Theory]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 50, .9, true, new double[] { 481.66, 482.181, 483.82, 484.641 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 50, .95, true, new double[] { 481.66, 483.393, 487.254, 489.185 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 50, .98, true, new double[] { 482.734, 485.367, 490.633, 493.266 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 50, .998, true, new double[] { 488.361, 492.18, 499.82, 503.639 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 12, .9, true, new double[] { 480.969, 481.638, 484.675, 486.35 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 12, .95, true, new double[] {480.97, 481.638, 489.264, 493.205  })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 12, .98, true, new double[] {480.97, 482.625, 493.375, 498.750 })]
        [InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 480, 481, 482, 483, 486, 488, 490, 494, 496 }, 12, .998, true, new double[] {480.973, 488.203, 503.797, 511.593 })]
        //[InlineData(new double[] {.99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19}, 50, .9, true, new double[] {1.727, 2.333, 3.667, 4.335 })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 50, .95, true, new double[] {1.975, 3.262, 5.836, 7.123  })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 50, .98, true, new double[] { 1.975, 4.297, 13.703, 18.406 })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 50, .998, true, new double[] {8.672, 13.836, 24.164, 29.328  })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 12, .9, true, new double[] {1.485, 1.722, 4.362, 5.725 })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 12, .95, true, new double[] { 1.486, 1.921, 7.176, 9.804})]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 12, .98, true, new double[] {1.486, 1.922, 18.6, 28.2  })]
        //[InlineData(new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 }, new double[] { 1, 1.5, 2, 3, 5, 9, 12, 16, 19 }, 12, .998, true, new double[] {1.488, 8.459, 29.541, 40.081  })]

        public void GraphicalUncertaintyShouldMatchFDA143(double[] exceedanceProbabilities, double[] flowsOrStages, int erl, double nonExceedanceProbability, bool usingStages, double[] expectedFlowsOrStages)
        {
            GraphicalUncertainPairedData graphicalUncertainPairedData = new GraphicalUncertainPairedData(exceedanceProbabilities, flowsOrStages, erl, curveMetaData, usingStages);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            double smallProb = 0.01;
            double bigProb = 0.99;
            IPairedData minSampledCurve = graphicalUncertainPairedData.SamplePairedData(smallProb);
            IPairedData maxSampledCurve = graphicalUncertainPairedData.SamplePairedData(bigProb);
            double min = minSampledCurve.f(nonExceedanceProbability);
            double max = maxSampledCurve.f(nonExceedanceProbability);
            double range = max - min;
            double quantityOfBins = (int)(1 + 3.3 * Math.Log10(convergenceCriteria.MaxIterations));
            double binWidth = (int)(range / quantityOfBins);
            ThreadsafeInlineHistogram graphicalThreadsafeInlineHistogram = new ThreadsafeInlineHistogram(binWidth, convergenceCriteria);
            int masterseed = 1234;
            int progressChunks = 1;
            int _completedIterations = 0;
            int _ExpectedIterations = convergenceCriteria.MaxIterations;
            if (_ExpectedIterations > 100)
            {
                progressChunks = _ExpectedIterations / 100;
            }
            Random masterSeedList = new Random(masterseed);//must be seeded.
            int[] seeds = new int[convergenceCriteria.MaxIterations];
            for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            int iterations = convergenceCriteria.MinIterations;
            double lowerQuantile = 0.025;
            double upperQuantile = 0.975;
            
            while (!graphicalThreadsafeInlineHistogram.IsHistogramConverged(upperQuantile, lowerQuantile))
            {
                Parallel.For(0, iterations, i =>
                {

                    IProvideRandomNumbers threadlocalRandomProvider = new RandomProvider(seeds[i]);
                    IPairedData sampledFrequencyFunction = graphicalUncertainPairedData.SamplePairedData(threadlocalRandomProvider.NextRandom());
                    double stageOrFlowRealization = sampledFrequencyFunction.f(nonExceedanceProbability);
                    graphicalThreadsafeInlineHistogram.AddObservationToHistogram(stageOrFlowRealization, i);
                    Interlocked.Increment(ref _completedIterations);

                });
                if (!graphicalThreadsafeInlineHistogram.IsConverged)
                {
                    iterations = graphicalThreadsafeInlineHistogram.EstimateIterationsRemaining(.95, .05);
                    _ExpectedIterations = _completedIterations + iterations;
                    progressChunks = _ExpectedIterations / 100;
                }
            }
            graphicalThreadsafeInlineHistogram.ForceDeQueue();
            //double[] percentiles = new double[] { 0.05, 0.25, 0.75, 0.95 };
            double[] percentiles = new double[] { .0228, .1587, .8413, .9772 };
            for (int j = 0; j < expectedFlowsOrStages.Length; j++)
            {
                double actualStageOrFlow = graphicalThreadsafeInlineHistogram.InverseCDF(percentiles[j]);
                double expectedStageOrFlow = expectedFlowsOrStages[j];
                double difference = actualStageOrFlow - expectedStageOrFlow;
                double error = (actualStageOrFlow - expectedStageOrFlow)/expectedStageOrFlow;
                Debug.WriteLine("| {0,9} | {1,25} | {2,-19} | {3,-17} | {4, -18}| {5, -18}|", 1 - nonExceedanceProbability, percentiles[j], expectedStageOrFlow, actualStageOrFlow, difference, error);
            }
        }
    }
}
