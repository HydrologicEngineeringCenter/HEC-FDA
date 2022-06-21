using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paireddata;
using Xunit;
using Statistics;
using Statistics.Distributions;
using compute;
using metrics;
using scenarios;
using System.Xml.Linq;

namespace fda_model_test.unittests
{
    public class ScenarioShould
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "Structure";
        CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        static int id1 = 1;
        static int id2 = 2;
        static int year = 2300;

        [Fact]
        public void ScenarioShouldReadWhatItWrites()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new LogPearson3(1,2,3,100);
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

            Threshold threshold = new Threshold(1, convergenceCriteria, metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            
            // build IAS for Impact Area 1
            ImpactAreaScenarioSimulation simulation1 = ImpactAreaScenarioSimulation.builder(id1)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();

            // build IAS for Impact Area 2
            ImpactAreaScenarioSimulation simulation2 = ImpactAreaScenarioSimulation.builder(id2)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            IList<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation1);
            impactAreaScenarioSimulations.Add(simulation2);
            Scenario scenario = new Scenario(year, impactAreaScenarioSimulations);
            XElement element = scenario.WriteToXML();
            Scenario scenarioFromXML = Scenario.ReadFromXML(element);
            bool scenariosAreEqual = scenario.Equals(scenarioFromXML);
            Assert.True(scenariosAreEqual);
        }

        [Fact]
        public void ScenarioResultsShouldReadWhatItWrites()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            ContinuousDistribution flow_frequency = new LogPearson3(1, 2, 3, 100);
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

            Threshold threshold = new Threshold(1, convergenceCriteria, metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?

            // build IAS for Impact Area 1
            ImpactAreaScenarioSimulation simulation1 = ImpactAreaScenarioSimulation.builder(id1)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();

            // build IAS for Impact Area 2
            ImpactAreaScenarioSimulation simulation2 = ImpactAreaScenarioSimulation.builder(id2)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(upd)
                .withAdditionalThreshold(threshold)
                .build();
            IList<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            impactAreaScenarioSimulations.Add(simulation1);
            impactAreaScenarioSimulations.Add(simulation2);
            Scenario scenario = new Scenario(year, impactAreaScenarioSimulations);

            MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ScenarioResults scenarioResults = scenario.Compute(meanRandomProvider, convergenceCriteria);
            XElement element = scenarioResults.WriteToXML();
            ScenarioResults scenarioResultsFromXML = ScenarioResults.ReadFromXML(element);
            bool scenariosAreEqual = scenarioResults.Equals(scenarioResultsFromXML);
            Assert.True(scenariosAreEqual);
        }
    }
}
