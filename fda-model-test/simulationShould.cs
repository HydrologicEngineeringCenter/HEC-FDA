

using Xunit;
using ead;
using paireddata;
using Statistics;
using System.Collections.Generic;

namespace fda_model_test
{
    public class SimulationShould
    {
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        //static double[] ProbabilitiesOfFailure = { .001, .01, .1, .5, 1 };
        //static double[] ElevationsOfFailure = { 600, 610, 650, 700, 750 };
        [Theory]
        [InlineData(150000)]
        public void ComputeEAD(double expected)
        {
            
            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000*i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            metrics.Threshold threshold = new metrics.Threshold(1, metrics.ThresholdEnum.ExteriorStage, 150000);
            Simulation s = new Simulation(flow_frequency,flow_stage,upd);
            s.PerformanceThresholds.AddThreshold(threshold);
            ead.MeanRandomProvider mrp = new MeanRandomProvider();
            metrics.IContainResults r = s.Compute(mrp,1);
            double difference = expected - r.MeanEAD("residential");
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .01);
        }
        [Theory]
        [InlineData(1234, 100, 138098)]
        [InlineData(1234, 1, 336662)]
        [InlineData(4321, 1, 150834)]
        [InlineData(1111, 1, 78875)]
        public void ComputeEAD_Iterations(int seed, int iterations, double expected)
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
            RandomProvider rp = new RandomProvider(seed);
            metrics.IContainResults r = s.Compute(rp, iterations);
            double difference = expected - r.MeanEAD("residential");
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .01);
        }
        [Theory]
        [InlineData(0.0, 82500)]
        [InlineData(1.0, 115500)]
        public void ComputeEAD_withLevee(double failprobattop, double expected)
        {

            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);

            double[] leveestages = new double[] { 0.0d, 100000.0d };
            IDistribution[] leveefailprobs = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = IDistributionFactory.FactoryUniform(0, failprobattop*i, 10); //no damages at all, perfect levee
            }
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = new Simulation(flow_frequency, flow_stage, levee, upd);
            ead.MeanRandomProvider mrp = new MeanRandomProvider();
            metrics.IContainResults r = s.Compute(mrp, 1);
            double difference = expected - r.MeanEAD("residential");
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .01);
        }

    }
}
