using System;
using System.Collections.Generic;
using Xunit;
using compute;
using paireddata;
using Statistics;
using metrics;
namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class AlternativeTest
    {
        static double[] FlowXs = { 0, 100000 };
        static double[] StageXs = { 0, 150000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "content";
        CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        static int id = 1;
        /// <summary>
        /// calculations for the below test can be found at https://docs.google.com/spreadsheets/d/1mPp8O2jm1wnsacQ7ZE3_sU_2xvghWOjC/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="poa"></param>
        /// <param name="discountRate"></param>
        /// <param name="baseYear"></param>
        /// <param name="futureYear"></param>
        /// <param name="iterations"></param>
        [Theory]
        [InlineData(208213.8061, 50,.0275,2023,2072,1)]
        [InlineData(239260.1814, 50, .0275, 2023, 2050, 1)]
        public void ComputeAAEQDamage(double expected, int poa, double discountRate, int baseYear, int futureYear, int iterations)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = new Statistics.Distributions.Uniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            IDistribution[] baseDamages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                baseDamages[i] = new Statistics.Distributions.Uniform(0, 600000 * i, 10);
            }
            IDistribution[] futureDamages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                futureDamages[i] = new Statistics.Distributions.Uniform(0, 1200000 * i, 10);
            }
            UncertainPairedData base_stage_damage = new UncertainPairedData(StageXs, baseDamages, metaData);
            UncertainPairedData future_stage_damage = new UncertainPairedData(StageXs, futureDamages, metaData);
            List<UncertainPairedData> updBase = new List<UncertainPairedData>();
            updBase.Add(base_stage_damage);
            List<UncertainPairedData> updFuture = new List<UncertainPairedData>();
            updFuture.Add(future_stage_damage);

            Simulation sBase = Simulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();
 
            Simulation sFuture = Simulation.builder(id)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();

            impactarea.ImpactArea impactArea = new impactarea.ImpactArea("Quahog", id);
            impactarea.ImpactAreaSimulation impactAreaBase = new impactarea.ImpactAreaSimulation("BaseYear", sBase, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListBaseYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListBaseYear.Add(impactAreaBase);
            impactarea.ImpactAreaSimulation impactAreaFuture = new impactarea.ImpactAreaSimulation("FutureYear", sFuture, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListFutureYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListFutureYear.Add(impactAreaFuture);

            scenarios.Scenario baseScenario = new scenarios.Scenario(baseYear, impactAreaListBaseYear);
            scenarios.Scenario futureScenario = new scenarios.Scenario(futureYear, impactAreaListFutureYear);
            alternatives.Alternative alternative = new alternatives.Alternative(baseScenario, futureScenario, poa, id);

            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            AlternativeResults alternativeResults = alternative.AnnualizationCompute(mrp, iterations, discountRate);
            double actual = alternativeResults.GetDamageResults(id).GetDamageResult(damCat,assetCat,id).DamageHistogram.InverseCDF(mrp.NextRandom());
            double err = Math.Abs((actual - expected) / actual);
            Assert.True(err<.01);

        }
    }
}
