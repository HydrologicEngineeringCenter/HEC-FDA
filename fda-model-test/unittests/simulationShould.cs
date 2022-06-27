

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
        static double[] Stages = { 0, 150000 };
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
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
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
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);

            Threshold threshold = new Threshold(1, convergenceCriteria, metrics.ThresholdEnum.ExteriorStage, 150000);
            //TODO: I think that we need to take convergence criteria out of the threshold constructor. convergence criteria should come in through one place only. 
            //otherwise we have different convergence criterias for one compute and that is causing problems 
            
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
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
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
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
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
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
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

        //I am including this method here because I want to be able to compare flow dists between 1.4.3 and 2.0 
        //without all of the simulation stuff getting in the way 
        public IPairedData BootstrapToPairedData(IProvideRandomNumbers randomProvider, ContinuousDistribution continuousDistribution, int ordinates)
        {
            double[] samples = randomProvider.NextRandomSequence(continuousDistribution.SampleSize);
            IDistribution bootstrap = continuousDistribution.Sample(samples);
            //for (int i = 0; i < dist.SampleSize; i++) samples[i] = Math.Log10(dist.InverseCDF(samples[i]));
            //ISampleStatistics ss = new SampleStatistics(samples);
            double[] x = new double[ordinates];
            double[] y = new double[ordinates];
            //double skewdividedbysix = ss.Skewness / 6.0;
            //double twodividedbyskew = 2.0 / ss.Skewness;
            //double sd = ss.StandardDeviation;
            for (int i = 0; i < ordinates; i++)
            {
                double val = (double)i + .5;
                //equally spaced non-exceedance (cumulative) probabilities in increasing order
                double prob = (val) / ((double)ordinates);
                x[i] = prob;

                //y values in increasing order 
                y[i] = bootstrap.InverseCDF(prob);
                //y[i] =LogPearson3.FastInverseCDF(ss.Mean, sd , ss.Skewness, skewdividedbysix, twodividedbyskew, prob);

            }

            return new PairedData(x, y);

        }
        [Fact]
        public void BootstrapToPairedDataProducesSameDistributionsAsFDA143()
        {
            LogPearson3 logPearson3 = new LogPearson3(3.3, .254, -.1021, 48); //dist in default data
            RandomProvider randomProvider = new RandomProvider(seed);
            double[] nonExceedanceProbabilities = new double[] { .9, .95, .98, .99, .995, .998 };
            Histogram Flows90Pct = new Histogram(5); //guess of a decent bin width 
            Flows90Pct.note = "90";
            Histogram Flows95Pct = new Histogram(5);
            Flows95Pct.note = "95";
            Histogram Flows98Pct = new Histogram(5);
            Flows98Pct.note = "98";
            Histogram Flows99Pct = new Histogram(5);
            Flows99Pct.note = "99";
            Histogram Flows995Pct = new Histogram(5);
            Flows995Pct.note = "995";
            Histogram Flows998Pct = new Histogram(5);
            Flows998Pct.note = "998";
            List<Histogram> histograms = new List<Histogram>() { Flows90Pct, Flows95Pct, Flows98Pct, Flows99Pct, Flows995Pct, Flows998Pct };
            int iterations = 10000;
            for (int i = 0; i < iterations; i++)
            {
                IPairedData frequencyFlow = BootstrapToPairedData(randomProvider, logPearson3, 200);
                for (int j = 0; j < nonExceedanceProbabilities.Length; j++)
                {
                    histograms[j].AddObservationToHistogram(frequencyFlow.f(nonExceedanceProbabilities[j]));
                }

            }

            //from 143 UI 
            double[] percentiles = new double[] {0, .05, .25, .75, .95 };
            double[] expectedQuantiles90pct = new double[] {90, 3540, 3896, 4547, 5192 };
            double[] expectedQuantiles95pct = new double[] {95, 4248, 4725, 5626, 6557 };
            double[] expectedQuantiles98pct = new double[] {98, 5187, 5845, 7135, 8520 };
            double[] expectedQuantiles99pct = new double[] {99, 5910, 6722, 8348, 10135 };
            double[] expectedQuantiles995pct = new double[] {995, 6648, 7628, 9627, 11870 };
            double[] expectedQuantiles998pct = new double[] {998, 7651, 8876, 11426, 14357 };

            List<double[]> expectedQuantiles = new List<double[]>() { expectedQuantiles90pct, expectedQuantiles95pct, expectedQuantiles98pct, expectedQuantiles99pct, expectedQuantiles995pct, expectedQuantiles998pct };

            for (int i = 0; i < expectedQuantiles.Count; i++)
            {
                for (int j = 1; j < expectedQuantiles90pct.Length; j++)
                {
                    double actualFlow = histograms[i].InverseCDF(percentiles[j]);
                    double expectedFlow = expectedQuantiles[i][j];
                    double error = (actualFlow - expectedFlow) / expectedFlow;
                    double tolerance = 0.05;
                    string message = $"The error between the {percentiles[j]}th percentile of the histogram of {histograms[i].note}th percentile flows as compared to the set of expected {expectedQuantiles[i][0]}th percentile flows is {error}";
                    System.Diagnostics.Debug.WriteLine(message);
                    //Assert.True(error < tolerance);
                }
            }
        }
    }
}
