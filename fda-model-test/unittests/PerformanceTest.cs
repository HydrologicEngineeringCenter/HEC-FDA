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

namespace fda_model_test
{
    [Trait("Category", "Unit")]
    public class PerformanceTest
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static double[] NonExceedanceProbs = { .5, .8, .9, .96, .98, .99, .996, .998 };
        static double[] StageForNonExceedanceProbs = { 5, 10, 15, 20, 25, 30, 35, 40 };
        static double[] StandardDeviationOfStage = { 6, 11, 16, 21, 26, 31, 36, 41 };
        static double[] ProbLeveeFailure = { .01, .02, .05, .1, .2, .3, .4, 1 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;

        [Theory]
        [InlineData(1234, 1, 0.0005)]
        public void ComputePerformanceWithSimulation_Test(int seed, int iterations, double expected)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description, id);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }

            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, id,  "residential");
            List<UncertainPairedData> uncertainPairedDataList = new List<UncertainPairedData>();
            uncertainPairedDataList.Add(stage_damage);
            int thresholdID = 1;
            Threshold threshold = new Threshold(thresholdID, ThresholdEnum.ExteriorStage, 150000);

            Simulation simulation = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(uncertainPairedDataList)
                .withAdditionalThreshold(threshold)
                .build();
 
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            metrics.Results results = simulation.Compute(randomProvider, cc,false);

            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.MeanAEP();
            
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }

        [Theory]
        [InlineData(5,.5)]
        [InlineData(10,.2)]
        [InlineData(15,.1)]
        [InlineData(20,.04)]
        [InlineData(25, .02)]
        [InlineData(30,.01)]
        [InlineData(35, .004)]
        [InlineData(40, .002)]
        public void ComputeAEP_Test(double thresholdValue, double expected)
        {
            //TODO: this needs to be re-written to set up the compute better
            IDistribution[] stageDistributions = new IDistribution[StageForNonExceedanceProbs.Length];
            for (int i = 0; i<StageForNonExceedanceProbs.Length; i++)
            {
                stageDistributions[i] = new Statistics.Distributions.Deterministic(StageForNonExceedanceProbs[i]);
            }
            paireddata.UncertainPairedData frequency_stage = new UncertainPairedData(NonExceedanceProbs, stageDistributions, xLabel, yLabel, name, description, id);
            int thresholdID = 1;
            Threshold threshold = new Threshold(thresholdID, ThresholdEnum.ExteriorStage, thresholdValue);
            compute.Simulation simulation = Simulation.builder()
                .withFrequencyStage(frequency_stage)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            int iterations = 1;
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            metrics.Results results = simulation.Compute(meanRandomProvider,cc,false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.MeanAEP();
            //TODO: why do both of these work? Richard did something wrong, it needs to be fixed.
            //double actual = simulation.PerformanceThresholds.ThresholdsDictionary[thresholdID].Performance.MeanAEP();
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }


        [Theory]
        [InlineData(30, 10, .095618)]
        [InlineData(35, 30, .113293)]
        [InlineData(40, 50, .095253)]
        public void ComputeLTEP_Test(double thresholdValue, int years, double expected)
        {

            IDistribution[] stageDistributions = new IDistribution[StageForNonExceedanceProbs.Length];
            for (int i = 0; i < StageForNonExceedanceProbs.Length; i++)
            {
                stageDistributions[i] = new Statistics.Distributions.Deterministic(StageForNonExceedanceProbs[i]);
            }
            paireddata.UncertainPairedData frequency_stage = new UncertainPairedData(NonExceedanceProbs, stageDistributions, xLabel, yLabel, name, description, id);
            int thresholdID = 1;
            Threshold threshold = new Threshold(thresholdID, ThresholdEnum.ExteriorStage, thresholdValue);
            compute.Simulation simulation = Simulation.builder()
                .withFrequencyStage(frequency_stage)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            metrics.Results results = simulation.Compute(meanRandomProvider, cc,false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.LongTermExceedanceProbability(years);
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }


        [Theory]
        [InlineData(45,.026)]
        public void ComputeLeveeAEP_Test(double thresholdValue, double expected)
        {
            IDistribution[] stageDistributions = new IDistribution[StageForNonExceedanceProbs.Length];
            IDistribution[] failureDistributions = new IDistribution[ProbLeveeFailure.Length];
            for (int i = 0; i < StageForNonExceedanceProbs.Length; i++)
            {
                stageDistributions[i] = new Statistics.Distributions.Deterministic(StageForNonExceedanceProbs[i]);
                failureDistributions[i] = new Statistics.Distributions.Deterministic(ProbLeveeFailure[i]);
            }
            paireddata.UncertainPairedData frequency_stage = new UncertainPairedData(NonExceedanceProbs, stageDistributions, xLabel, yLabel, name, description, id);
            paireddata.UncertainPairedData levee_curve = new UncertainPairedData(StageForNonExceedanceProbs, failureDistributions, xLabel, yLabel, name, description, id);
            int thresholdID = 1;
            Threshold threshold = new Threshold(thresholdID, ThresholdEnum.ExteriorStage, thresholdValue);
            Simulation simulation = Simulation.builder()
                .withFrequencyStage(frequency_stage)
                .withLevee(levee_curve, 45)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            Results results = simulation.Compute(meanRandomProvider, cc,false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.MeanAEP();
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }

        [Theory]
        [InlineData(3456,10000,28.75,.1,.8054)]
        [InlineData(5678, 10000, 100, .02, .998)]
        [InlineData(6789, 10000, 60.6, .01, .8382)]
        [InlineData(9876, 10000, 45.65, .004, .6136)]
        [InlineData(8765, 10000, 92.59, .002, .9002)]
        public void ComputeConditionalNonExceedanceProbability_Test(int seed, int iterations, double thresholdValue, double recurrenceInterval, double expected)
        {
            double nonExceedanceProbability = 1 - recurrenceInterval;
            IDistribution[] stageDistributions = new IDistribution[NonExceedanceProbs.Length];
           for (int i = 0; i<NonExceedanceProbs.Length; i++)
            {
                stageDistributions[i] = IDistributionFactory.FactoryNormal(StageForNonExceedanceProbs[i],StandardDeviationOfStage[i]);
            }
            UncertainPairedData frequency_stage = new UncertainPairedData(NonExceedanceProbs, stageDistributions, xLabel, yLabel, name, description, id);
            int thresholdID = 1;
            Threshold threshold = new Threshold(thresholdID, ThresholdEnum.ExteriorStage, thresholdValue);
            Simulation simulation = Simulation.builder()
                .withFrequencyStage(frequency_stage)
                .withAdditionalThreshold(threshold)
                .build();
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            metrics.Results results = simulation.Compute(randomProvider, cc,false);
            double actual = results.PerformanceByThresholds.ThresholdsDictionary[thresholdID].ProjectPerformanceResults.ConditionalNonExceedanceProbability(nonExceedanceProbability);
            Assert.Equal(expected, actual, 1);
        }
    }

}

