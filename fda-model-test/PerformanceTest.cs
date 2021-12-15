using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;
using ead;
using paireddata;
using Statistics;

namespace fda_model_test
{
   
    public class PerformanceTest
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static double[] NonExceedanceProbs = { .5, .8, .9, .96, .98, .99, .996, .998 };
        static double[] StageForNonExceedanceProbs = { 5, 10, 15, 20, 25, 30, 35, 40 };
        static double[] StandardDeviationOfStage = { 6, 11, 16, 21, 26, 31, 36, 41 };
        static double[] ProbLeveeFailure = { .01, .02, .05, .1, .2, .3, .4, 1 };
        

        [Theory]
        [InlineData(1234, 1, 0.5)]
        public void ComputePerformanceWithSimulation_Test(int seed, int iterations, double expected)
        {

            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);

            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .build();
            Threshold threshold = new Threshold(0, ThresholdEnum.ExteriorStage, 150000);
            s.PerformanceThresholds.AddThreshold(threshold);
            RandomProvider rp = new RandomProvider(seed);
            metrics.Results r = s.Compute(rp, iterations);

            double actual = r.Thresholds.ListOfThresholds.Last().Performance.MeanAEP(); 
            
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
            ead.Simulation simulation = Simulation.builder().build(); 
            paireddata.IPairedData frequency_stage = new PairedData(NonExceedanceProbs, StageForNonExceedanceProbs);
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            simulation.ComputePerformance(frequency_stage);
            //TODO: I think that we need a dictionary of thresholds, not a list 
            double actual = simulation.PerformanceThresholds.ListOfThresholds.First().Performance.MeanAEP();
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
            ead.Simulation simulation = Simulation.builder().build();
            paireddata.IPairedData frequency_stage = new PairedData(NonExceedanceProbs, StageForNonExceedanceProbs);
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            simulation.ComputePerformance(frequency_stage);
            double actual = simulation.PerformanceThresholds.ListOfThresholds.First().Performance.LongTermExceedanceProbability(years);
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }


        [Theory]
        [InlineData(45,.026)]
        public void ComputeLeveeAEP_Test(double thresholdValue, double expected)
        {
            ead.Simulation simulation = Simulation.builder().build();
            paireddata.IPairedData frequency_stage = new PairedData(NonExceedanceProbs, StageForNonExceedanceProbs);
            paireddata.IPairedData levee_curve = new PairedData(StageForNonExceedanceProbs, ProbLeveeFailure);
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            simulation.ComputeLeveePerformance(frequency_stage, levee_curve);
            double actual = simulation.PerformanceThresholds.ListOfThresholds.First().Performance.MeanAEP();
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
            UncertainPairedData frequency_stage = new UncertainPairedData(NonExceedanceProbs, stageDistributions);
            Simulation simulation = Simulation.builder()
                .withFrequencyStage(frequency_stage)
                .build();
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            RandomProvider randomProvider = new RandomProvider(seed);
            metrics.Results results = simulation.Compute(randomProvider, iterations);
            double actual = results.Thresholds.ListOfThresholds.First().Performance.ConditionalNonExceedanceProbability(nonExceedanceProbability);
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }
    }

}

