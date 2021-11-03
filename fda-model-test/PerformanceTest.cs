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
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static double[] NonExceedanceProbs = { .5, .8, .9, .96, .98, .99, .996, .998 };
        static double[] StageForNonExceedanceProbs = { 5, 10, 15, 20, 25, 30, 35, 40 };

        [Theory]
        [InlineData(1234, 1, 0.5)]
        public void ComputePerformanceWithSimulation(int seed, int iterations, double expected)
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

            Simulation s = new Simulation(flow_frequency, flow_stage, upd);
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
        public void ComputeAEP(double thresholdValue, double expected)
        {
            ead.Simulation simulation = new Simulation(); 
            paireddata.IPairedData frequency_stage = new PairedData(NonExceedanceProbs, StageForNonExceedanceProbs);
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            simulation.ComputePerformance(frequency_stage);
            double actual = simulation.PerformanceThresholds.ListOfThresholds.First().Performance.MeanAEP();
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }

        [Theory]
        [InlineData(5, .5, .5)]
        [InlineData(10, .2, .8)]
        [InlineData(15, .1, .9)]
        [InlineData(20, .04, .96)]
        [InlineData(25, .02, .98)]
        [InlineData(30, .01, .99)]
        [InlineData(35, .004, .996)]
        [InlineData(40, .002, .998)]
        public void ComputeCNEP(double thresholdValue, double exceedanceProbability, double expected)
        {
            ead.Simulation simulation = new Simulation();
            paireddata.IPairedData frequency_stage = new PairedData(NonExceedanceProbs, StageForNonExceedanceProbs);
            Threshold threshold = new Threshold(1, ThresholdEnum.ExteriorStage, thresholdValue);
            simulation.PerformanceThresholds.AddThreshold(threshold);
            simulation.ComputePerformance(frequency_stage);
            double actual = simulation.PerformanceThresholds.ListOfThresholds.First().Performance.ConditionalNonExceedanceProbability(exceedanceProbability);
            double difference = expected - actual;
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .02);
        }
    }

}

