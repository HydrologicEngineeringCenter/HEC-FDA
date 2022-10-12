using System.Collections.Generic;
using Xunit;
using Statistics;
using System.Xml.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("Category", "Unit")]

    public class ImpactAreaScenarioResultsShould
    {//TODO: Access these results through ScenarioREsults 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000, 300000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "Residential";
        static string assetCat = "content";
        int id = 0;
        static CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        [Fact]
        public void ResultsShouldReadTheSameStuffItWrites()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
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
                IDistributionFactory.FactoryUniform(0, 600000, 10),
                IDistributionFactory.FactoryUniform(0, 600000, 10),
                IDistributionFactory.FactoryUniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stage_damage);

            Threshold threshold = new Threshold(1, convergenceCriteria, ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(stageDamageList)
                .withAdditionalThreshold(threshold)
                .build();
            MedianRandomProvider meanRandomProvider = new MedianRandomProvider();
            ImpactAreaScenarioResults results = simulation.Compute(meanRandomProvider, convergenceCriteria); //here we test compute, below we test preview compute 
            XElement resultsElement = results.WriteToXml();
            ImpactAreaScenarioResults resultsFromXML = ImpactAreaScenarioResults.ReadFromXML(resultsElement);
            bool success = results.Equals(resultsFromXML);
            Assert.True(success);
        }

        [Fact]
        public void ResultsShouldNotComputeWhenMaxIterationsAreGreaterThanMinIterations()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(maxIterations: 1);
            ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
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
                IDistributionFactory.FactoryUniform(0, 600000, 10),
                IDistributionFactory.FactoryUniform(0, 600000, 10),
                IDistributionFactory.FactoryUniform(0, 600000, 10)
            };
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, metaData);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stage_damage);

            Threshold threshold = new Threshold(1, convergenceCriteria, ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(stageDamageList)
                .withAdditionalThreshold(threshold)
                .build();
            RandomProvider randomProvider = new RandomProvider();
            ImpactAreaScenarioResults results = simulation.Compute(randomProvider, convergenceCriteria); //here we test compute, below we test preview compute 
            Assert.True(results.IsNull);
        }

    }
}
