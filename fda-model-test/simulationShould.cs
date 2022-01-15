

using Xunit;
using compute;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System;

namespace fda_model_test
{
    public class SimulationShould
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;
        //TODO: can the below lines be deleteD?
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
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel,yLabel,name,description,id);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, id, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            
            metrics.Threshold threshold = new metrics.Threshold(1, metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            metrics.Results r = s.Compute(mrp,1);
            double difference = expected - r.ExpectedAnnualDamageResults.MeanEAD("residential");
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
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description, id);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, id, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .build();
            RandomProvider rp = new RandomProvider(seed);
            metrics.Results r = s.Compute(rp, iterations);
            double difference = expected - r.ExpectedAnnualDamageResults.MeanEAD("residential");
            double relativeDifference = difference / expected;
            Assert.True(relativeDifference < .01);
        }

        [Theory]
        [InlineData(115500)]
        public void ComputeEAD_withLevee(double expected)
        {

            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description, id);

            double[] leveestages = new double[] { 0.0d, 100000.0d };
            IDistribution[] leveefailprobs = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = IDistributionFactory.FactoryUniform(i, i, 10); //probability at the top must be 1
            }
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, xLabel, yLabel, name, description, id);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, id,  "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee)
                .withStageDamages(upd)
                .build();
            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            metrics.Results r = s.Compute(mrp, 1);
            double difference = expected - r.ExpectedAnnualDamageResults.MeanEAD("residential");
            double relativeDifference = Math.Abs(difference / expected);
            double tolerance = 0.4;
            Assert.True(relativeDifference < tolerance);
        }

    }
}
