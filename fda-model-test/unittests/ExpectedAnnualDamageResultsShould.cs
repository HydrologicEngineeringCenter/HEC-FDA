using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using paireddata;
using Statistics.Distributions;
using Statistics;
using compute;
using Statistics.Histograms;
using System.Xml.Linq;

namespace fda_model_test.unittests
{
    public class ExpectedAnnualDamageResultsShould
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "Residential";
        static string assetCat = "content";
        static int id = 0;
        static CurveTypesEnum curveTypesEnum = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
        static CurveMetaData curveMetaDataWithCategory = new CurveMetaData(xLabel, yLabel, name, damCat, curveTypesEnum,assetCat);
        static CurveMetaData curveMetaDataWithoutCategory = new CurveMetaData(xLabel, yLabel, name, curveTypesEnum);

        [Theory]
        [InlineData(1111, 100)]
        public void SerializationShouldReadTheSameObjectItWrites(int seed, int iterations)
        {

            Statistics.ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages, curveMetaDataWithoutCategory);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000 * i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, curveMetaDataWithCategory);
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stage_damage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(stageDamageList)
                .build();
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            metrics.ImpactAreaScenarioResults results = simulation.Compute(randomProvider, convergenceCriteria);
            XElement xElement = results.ConsequenceResults.WriteToXML();
            metrics.ConsequenceResults expectedAnnualDamageResultsFromXML = metrics.ConsequenceResults.ReadFromXML(xElement);
            bool success = results.ConsequenceResults.Equals(expectedAnnualDamageResultsFromXML);
            Assert.True(success);
        }
    }
}
