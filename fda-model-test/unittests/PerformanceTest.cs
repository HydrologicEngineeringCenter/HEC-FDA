using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;
using compute;
using paireddata;
using Statistics;
using Statistics.Distributions;

namespace fda_model_test
{
    [Trait("Category", "Unit")]
    public class PerformanceTest
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static double[] StageForNonLeveeFailureProbs = { 5000, 8000, 9000, 9600, 9800, 9900, 9960, 9980 };
        static double[] ProbLeveeFailure = { .01, .02, .05, .1, .2, .3, .4, 1 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;

        [Theory]
        [InlineData(9200, 80, 1, .08, 0.998732271693343)]
        [InlineData(9400, 60, 1, .06, 0.975584185541488)]
        [InlineData(9600, 40, 1, .04, 0.80463384844468)]
        [InlineData(9800, 20, 1, .02, 0.332392028244906)]
        public void ComputePerformanceWithSimulation_Test(double thresholdValue, int years, int iterations, double expectedAEP, double expectedLTEP)
        {
            
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 20000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description,  "residential");
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.ExteriorStage, thresholdValue);

            Simulation simulation = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();
 
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            metrics.Results results = simulation.Compute(meanRandomProvider, cc,false);

            double actualAEP = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.MeanAEP();
            double actualLTEP = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.LongTermExceedanceProbability(years);

            double aepDifference = Math.Abs(expectedAEP - actualAEP);
            double aepRelativeDifference = aepDifference / expectedAEP;
            Assert.True(aepRelativeDifference < .025);

            double ltepDifference = Math.Abs(expectedLTEP - actualLTEP);
            double ltepRelativeDifference = ltepDifference / expectedLTEP;
            Assert.True(ltepRelativeDifference < .025);
        }


        [Theory]
        [InlineData(9980, 1, .028)] //TODO: on paper, I calculate 0.026. I think discretization makes this .028. Investigate. 
        public void ComputeLeveeAEP_Test(double thresholdValue, int iterations, double expected)
        {
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 20000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, "residential");
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            IDistribution[] failureProbs = new IDistribution[StageForNonLeveeFailureProbs.Length];
            for (int i = 0; i < StageForNonLeveeFailureProbs.Length; i++)
            {
                failureProbs[i] = new Deterministic(ProbLeveeFailure[i]);
            }
            UncertainPairedData leveeCurve = new UncertainPairedData(StageForNonLeveeFailureProbs, failureProbs, xLabel, yLabel, name, description);

            int thresholdID = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.ExteriorStage, thresholdValue);

            Simulation simulation = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .withLevee(leveeCurve,thresholdValue)
                .build();

            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            metrics.Results results = simulation.Compute(meanRandomProvider, cc, false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.MeanAEP();
            double difference = Math.Abs(actual - expected);
            double relativeDifference = difference / expected;
            double tolerance = 0.025;
            Assert.True(relativeDifference < tolerance);
        }

        [Theory]
        [InlineData(3456,10000,12000,.9,.33333)]
        [InlineData(5678, 10000, 13000,.98, .336735)]
        [InlineData(6789, 10000, 14000, .99, .292929)]
        [InlineData(8910, 10000, 15000 , .996, .246988)]
        [InlineData(9102, 10000, 16000, .998, .198397)]
        public void ComputeConditionalNonExceedanceProbability_Test(int seed, int iterations, double thresholdValue, double recurrenceInterval, double expected)
        {
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 20000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, "residential");
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            Threshold threshold = new Threshold(thresholdID, cc, ThresholdEnum.ExteriorStage, thresholdValue);

            Simulation simulation = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();

            RandomProvider randomProvider = new RandomProvider(seed);
            metrics.Results results = simulation.Compute(randomProvider, cc, false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.ConditionalNonExceedanceProbability(recurrenceInterval);
            double difference = Math.Abs(actual - expected);
            double relativeDifference = difference / expected;
            double tolerance = 0.01;
            Assert.True(relativeDifference < tolerance);
        }
    }

}

