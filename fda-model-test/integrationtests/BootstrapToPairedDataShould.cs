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

namespace fda_model_test.integrationtests
{
    public class BootstrapToPairedDataShould
    {

        //I am including this method here because I want to be able to compare flow dists between 1.4.3 and 2.0 
        //without all of the simulation stuff getting in the way 
        public IPairedData BootstrapToPairedData(IProvideRandomNumbers randomProvider, ContinuousDistribution continuousDistribution, int ordinates)
        {
            double[] samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
            IDistribution bootstrap = continuousDistribution.Sample(samples);

            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            for (int i = 0; i < ordinates; i++)
            {
                double val = (double)i + .5;
                //equally spaced non-exceedance (cumulative) probabilities in increasing order
                double prob = (val) / ((double)ordinates);
                x[i] = prob;

                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
            }

            return new PairedData(x, y);

        }
        [Theory]
        //The expected values for these cases were obtained from HEC-FDA Version 1.4.3
        [InlineData(3.3, .254, -.1021, 48, .9, new double[] { 3540, 3896, 4547, 5192 })]
        [InlineData(3.3, .254, -.1021, 48, .95, new double[] { 4248, 4725, 5626, 6557 })]
        [InlineData(3.3, .254, -.1021, 48, .998, new double[] { 7651, 8876, 11426, 14357 })]
        [InlineData(2.898, .324, -.539, 16, .9, new double[] { 1376, 1669, 2343, 3306})]
        [InlineData(2.898, .324, -.539, 16, .95, new double[] { 1635, 2009, 2921, 4318 })]
        [InlineData(2.898, .324, -.539, 16, .998, new double[] { 2618, 3376, 5502, 9383 })]
        [InlineData(5.943, .103, .045, 62, .9, new double[] { 1118936, 1158689, 1224539, 1282778 })]
        [InlineData(5.943, .103, .045, 62, .95, new double[] {1212505, 1260829, 1342924, 1417390  })]
        [InlineData(5.943, .103, .045, 62, .998, new double[] {1588715, 1681514, 1848145, 2008312  })]
        //The below cases were obtained from SSP using adjusted statistics 
        //[InlineData(4.5, 1, -1, 84, .9, new double[] { 594859.5,  307028.4 })]
        //[InlineData(4.5, 1, -1, 84, .95, new double[] { 985106.7,    475130.3 })]
        //[InlineData(4.5, 1, -1, 84, .998, new double[] { 4987799.5,  927866.8 })]
        //[InlineData(4.5, 2, -1, 84, .9, new double[] { 11562557.0,   2933456.8 })]
        //[InlineData(4.5, 2, -1, 84, .95, new double[] { 29736308.0,  7106754.0 })]
        //[InlineData(4.5, 2, -1, 84, .998, new double[] { 559861568.0,    34109292.0 })]
        //[InlineData(4.5, 2, 2, 84, .9, new double[] { 102177704.0,    3033279.0 })]
        //[InlineData(4.5, 2, 2, 84, .95, new double[] { 5414240256.0, 43098772.0 })]
        //[InlineData(4.5, 2, 2, 84, .998, new double[] { 22641183021581990000.0,  1094899073024.0 })]
        //These are straight from EMA
        //[InlineData(3.697, .217, -.339, 84, .9, new double[] { 10364.4,  8407.9 })]
        //[InlineData(3.697, .217, -.339, 84, .95, new double[] { 12463.0, 9697.2 })]
        //[InlineData(3.697, .217, -.339, 84, .998, new double[] { 24639.9, 13908.1 })]
        //[InlineData(3.762, .188, -.123, 22, .9, new double[] { 12988.7,   8399.8 })]
        //[InlineData(3.762, .188, -.123, 22, .95, new double[] { 16359.9, 9604.3 })]
        //[InlineData(3.762, .188, -.123, 22, .998, new double[] { 41022.8,    13424.9 })]
        //[InlineData(3.695, .214, -.405, 17, .9, new double[] { 12346.6,   7385.4 })]
        //[InlineData(3.695, .214, -.405, 17, .95, new double[] { 15595.6,  8474.7 })]
        //[InlineData(3.695, .214, -.405, 17, .998, new double[] { 35517.1,    10647.3 })]
        //[InlineData(2.898, .324, -.539, 16, .9, new double[] { 3081.0,    1434.0 })]
        //[InlineData(2.898, .324, -.539, 16, .95, new double[] { 4250.5,  1743.8 })]
        //[InlineData(2.898, .324, -.539, 16, .98, new double[] { 13140.4,  2199.1 })]
        //[InlineData(5.168, .128, -1.41, 3, .9, new double[] { 253338.5,   174622.3 })]
        //[InlineData(5.168, .128, -1.41, 3, .95, new double[] { 338290.3,  190408.0 })]
        //[InlineData(5.168, .128, -1.41, 3, .998, new double[] { 2058570.4,    132506.9 })]
        //[InlineData(5.943, .103, .045, 62, .9, new double[] { 1290324.2,  1117555.4 })]
        //[InlineData(5.943, .103, .045, 62, .95, new double[] { 1449160.2, 1209378.0 })]
        //[InlineData(5.943, .103, .045, 62, .998, new double[] { 2339637.5,   1529118.5 })]
        //[InlineData(2.625, .212, -.377, 60, .9, new double[] { 874.0, 692.8 })]
        //[InlineData(2.625, .212, -.377, 60, .95, new double[] { 1047.0,   794.2 })]
        //[InlineData(2.625, .212, -.377, 60, .998, new double[] { 2111.0,  1115.1 })]
        //[InlineData(3.13, .522, -.280, 14, .9, new double[] { 22800.5,    5196.6 })]
        //[InlineData(3.13, .522, -.280, 14, .95, new double[] { 43927.9,   7435.5 })]
        //[InlineData(3.13, .522, -.280, 14, .998, new double[] { 487939.9, 15130.8 })]
        public void BootstrapToPairedDataProducesSameDistributionsAsFDA143(double mean, double standardDeviation, double skew, int equivalentRecordLength, double nonExceedanceProbability, double[] expectedFlows)
        {
            int seed = 1234;
            int iterations = 10000;

            LogPearson3 logPearson3 = new LogPearson3(mean, standardDeviation, skew, equivalentRecordLength);
            RandomProvider randomProvider = new RandomProvider(seed);
            double histoMin = logPearson3.InverseCDF(nonExceedanceProbability);
            double smallProb = 0.01;
            double bigProb = 0.99;
            double range = logPearson3.InverseCDF(bigProb) - logPearson3.InverseCDF(smallProb);
            int quantityOfBins = (int)(1 + 3.3 * Math.Log10(iterations));
            int binWidth = (int)(range / quantityOfBins);
            Histogram flowHistogram = new Histogram(histoMin, binWidth); //guess of a decent bin width 
            flowHistogram.note = $"{nonExceedanceProbability}";
 
            for (int i = 0; i < iterations; i++)
            {
                IPairedData frequencyFlow = BootstrapToPairedData(randomProvider, logPearson3, 200);
                flowHistogram.AddObservationToHistogram(frequencyFlow.f(nonExceedanceProbability));
            }



            double[] percentiles = new double[] {.05, .25, .75, .95 };
            //double[] percentiles = new double[] { .95, .05 };
            for (int j = 0; j < expectedFlows.Length; j++)
            {
                double actualFlow = flowHistogram.InverseCDF(percentiles[j]);
                double expectedFlow = expectedFlows[j];
                double error = (actualFlow - expectedFlow) / expectedFlow;
                Debug.WriteLine("|{0,6} | {1,18} | {2,6} | {3,3} | {4,9} | {5,25} | {6,-19} | {7,-17} | {8, -18}|", mean, standardDeviation, skew, equivalentRecordLength, 1-nonExceedanceProbability, percentiles[j], expectedFlow, actualFlow, error);
            }

        }
    }
}
