

using Xunit;
using compute;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System;

namespace fda_model_test
{
    [Trait("Category", "Unit")]
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
            
            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000*i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel,yLabel,name,description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            
            metrics.Threshold threshold = new metrics.Threshold(1, new ConvergenceCriteria(), metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            metrics.Results r = s.Compute(mrp,cc); //here we test compute, below we test preview compute 
            double difference = expected - r.ExpectedAnnualDamageResults.MeanEAD("residential");
            double relativeDifference = Math.Abs(difference / expected);
            Assert.True(relativeDifference < .01);
        }

        [Theory]
        [InlineData(150000)]
        public void PreviewCompute_Test(double expectedEAD)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);

            metrics.Threshold threshold = new metrics.Threshold(1, new ConvergenceCriteria(), metrics.ThresholdEnum.ExteriorStage, 150000);
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            metrics.Results results = s.PreviewCompute(); //here we test preview compute 
            double difference = expectedEAD - results.ExpectedAnnualDamageResults.MeanEAD("residential");
            double relativeDifference = Math.Abs(difference / expectedEAD);
            Assert.True(relativeDifference < .01);
        }


        [Theory]
        [InlineData(1234, 100, 124987.126536313)]
        [InlineData(2345, 100, 120189.843743947)]
        [InlineData(4321, 100, 116493.377846062)]
        [InlineData(1111, 100, 143316.627604432)]
        public void ComputeEAD_Iterations(int seed, int iterations, double expected)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .build();
            RandomProvider rp = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            metrics.Results r = s.Compute(rp, cc);
            double actual = r.ExpectedAnnualDamageResults.MeanEAD("residential");

            Assert.Equal(expected, actual, 2);
        }

        [Theory]
        [InlineData(83333.33, 100000.0d)]
        [InlineData(0.0, 400000.0d)] //top of levee elevation above all stages
        public void ComputeEAD_withLevee(double expected, double topOfLeveeElevation)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, xLabel, yLabel, name, description);
            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, xLabel, yLabel, name, description);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, xLabel, yLabel, name, description,  "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, 100000.0d)
                .withStageDamages(upd)
                .build();
            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            metrics.Results r = s.Compute(mrp, cc);
            double actual = r.ExpectedAnnualDamageResults.MeanEAD("residential");
            if (actual == 0) //handle assertion differently if EAD is zero
            {
                Assert.Equal(expected, actual, 0);
            } 
            else
            {
                double difference = expected - actual;
                double relativeDifference = Math.Abs(difference / expected);
                double tolerance = 0.01;
                Assert.True(relativeDifference < tolerance);
            }

            
        }

    }
}
