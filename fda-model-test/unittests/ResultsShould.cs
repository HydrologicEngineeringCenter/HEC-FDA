using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Statistics;
using paireddata;
using compute;
using System.Xml.Linq;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]

    public class ResultsShould
    {
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
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
            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
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
            List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
            stageDamageList.Add(stage_damage);

            metrics.Threshold threshold = new metrics.Threshold(1, new ConvergenceCriteria(), metrics.ThresholdEnum.ExteriorStage, 150000);//do we want to access this through _results?
            Simulation simulation = Simulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(stageDamageList)
                .withAdditionalThreshold(threshold)
                .build();
            compute.MeanRandomProvider meanRandomProvider = new MeanRandomProvider();
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            metrics.Results results = simulation.Compute(meanRandomProvider, convergenceCriteria); //here we test compute, below we test preview compute 
            XElement resultsElement = results.WriteToXml();
            metrics.IContainResults resultsFromXML = metrics.Results.ReadFromXML(resultsElement);
            bool success = results.Equals(resultsFromXML);
        }
    }
}
