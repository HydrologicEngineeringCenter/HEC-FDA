

using Xunit;
using compute;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using Statistics.Distributions;
using metrics;
using Statistics.Histograms;
using interfaces;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class SimulationShould
    {//TODO: Access the requisite logic through ScenarioREsults 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000, 300000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "Structure";
        CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        static int id = 1;
        private int seed =  3456;

        [Theory]
        [InlineData(150000)]
        public void ComputeEAD(double expected)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000*i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[3]
            {
                    new Uniform(0, 0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            
            Threshold threshold = new Threshold(1, convergenceCriteria, metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            MeanRandomProvider mrp = new MeanRandomProvider();
            ImpactAreaScenarioResults impactAreaScenarioResult = simulation.Compute(mrp,convergenceCriteria); //here we test compute, below we test preview compute 
            double actual = impactAreaScenarioResult.MeanExpectedAnnualConsequences(id, damCat, assetCat);
            double difference = expected - actual;
            double relativeDifference = Math.Abs(difference / expected);
            Assert.True(relativeDifference < .01);
        }

        [Theory]
        [InlineData(150000)]
        public void PreviewCompute_Test(double expectedEAD)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[3]
            {
                    new Uniform(0, 0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Threshold threshold = new Threshold(1, convergenceCriteria, metrics.ThresholdEnum.ExteriorStage, 150000);
            ImpactAreaScenarioSimulation s = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();

            metrics.ImpactAreaScenarioResults results = s.PreviewCompute(); //here we test preview compute 
            double actual = results.MeanExpectedAnnualConsequences(id, damCat, assetCat);

            double difference = expectedEAD - actual;
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

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[3]
            {
                    new Uniform(0, 0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .build();
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            ImpactAreaScenarioResults results = simulation.Compute(randomProvider, convergenceCriteria);
            double actual = results.MeanExpectedAnnualConsequences(id,damCat,assetCat);
            Assert.Equal(expected, actual, 2);
        }

        [Theory]
        [InlineData(83333.33, 100000.0d)]
        [InlineData(0.0, 400000.0d)] //top of levee elevation above all stages
        public void ComputeEAD_withLevee(double expected, double topOfLeveeElevation)
        {

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages,metaData);
            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[3]
            {
                    new Uniform(0, 0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stage_damage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, 100000.0d)
                .withStageDamages(stageDamageList)
                .build();
            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria convergencriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ImpactAreaScenarioResults results = simulation.Compute(meanRandomProvider, convergencriteria);
            double actual = results.MeanExpectedAnnualConsequences(id, damCat, assetCat);
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

        [Fact]
        public void SimulationReadsTheSameThingItWrites()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new LogPearson3(1,1,1,200);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, metaData);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[3]
            {
                    new Uniform(0, 0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);

            Threshold threshold = new Threshold(1, convergenceCriteria, ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            MeanRandomProvider mrp = new MeanRandomProvider();
            ImpactAreaScenarioResults impactAreaScenarioResult = simulation.Compute(mrp, convergenceCriteria); //here we test compute, below we test preview compute 
            XElement simulationElement = simulation.WriteToXML();
            ImpactAreaScenarioSimulation simulationFromXML = ImpactAreaScenarioSimulation.ReadFromXML(simulationElement);
            bool simulationMatches = simulation.Equals(simulationFromXML);
            Assert.True(simulationMatches);
        }

        [Theory]
        [InlineData(0,true)]
        public void ComputeShouldReturnBlankResultsIfNoDamages(double expectedEAD, bool expectedZeroValued)
        {
            int impactAreaID = 1;
            int erl = 50;
            double[] exceedanceProabilities = new double[] { .5, .2, .1, .04, .02, .01, .005, .002 };
            double[] stagesForFrequency = new double[] { .001, .002, .003, .004, .005, .006, .007, .553 };
            CurveMetaData metaDataDefault = new CurveMetaData("x", "y", "name", damCat, assetCat);
            GraphicalUncertainPairedData graphicalUncertain = new GraphicalUncertainPairedData(exceedanceProabilities, stagesForFrequency, erl, metaDataDefault);
            double[] stagesForDamage = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1 };

            IDistribution[] zeroDamageDistributions = new IDistribution[]
            {
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0)
            };
            UncertainPairedData zeroStageDamage = new UncertainPairedData(stagesForDamage, zeroDamageDistributions, metaDataDefault);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(zeroStageDamage);

            int seed = 1234;
            RandomProvider randomProvider = new RandomProvider(seed);

            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFrequencyStage(graphicalUncertain)
                .withStageDamages(stageDamageList)
                .build();
            ImpactAreaScenarioResults impactAreaScenarioResults = simulation.Compute(randomProvider, new ConvergenceCriteria());
            double actualMeanEAD = impactAreaScenarioResults.MeanExpectedAnnualConsequences();
            Assert.Equal(expectedEAD, actualMeanEAD);
            bool actualHistogramZeroValued = impactAreaScenarioResults.GetConsequencesHistogram(impactAreaID, damCat, assetCat).HistogramIsZeroValued;
            Assert.Equal(expectedZeroValued, actualHistogramZeroValued);
        }
    }
}
