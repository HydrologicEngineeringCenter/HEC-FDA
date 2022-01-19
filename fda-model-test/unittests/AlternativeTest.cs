using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using compute;
using paireddata;
using Statistics;
using Statistics.Histograms;

namespace fda_model_test
{
    [Trait("Category", "Unit")]
    public class AlternativeTest
    {
        static double[] Flows = { 0, 100000 };
        static double[] BaseStages = { 0, 150000 };
        static double[] FutureStages = { 0, 300000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;

        [Theory]
        [InlineData(208213.8061,50,.0275,2023,2072,1000)]
        [InlineData(239260.1814, 50, .0275, 2023, 2050, 1000)]
        public void ComputeAAEQDamage(double expected, int poa, double discountRate, int baseYear, int futureYear, int iterations)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
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
            string damageCategory = "residential";
            UncertainPairedData base_stage_damage = new UncertainPairedData(BaseStages, damages, xLabel, yLabel, name, description, id, damageCategory);
            UncertainPairedData future_stage_damage = new UncertainPairedData(FutureStages, damages, xLabel, yLabel, name, description, id, damageCategory);
            List<UncertainPairedData> updBase = new List<UncertainPairedData>();
            updBase.Add(base_stage_damage);
            List<UncertainPairedData> updFuture = new List<UncertainPairedData>();
            updFuture.Add(future_stage_damage);

            Simulation sBase = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();
            //it feels weird that the EAD for this simulation is 150k because probability 
            //goes from 0 to 0.5
            Simulation sFuture = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();
            int impactAreaID = 17;
            impactarea.ImpactArea impactArea = new impactarea.ImpactArea("Quahog", impactAreaID);
            impactarea.ImpactAreaSimulation impactAreaBase = new impactarea.ImpactAreaSimulation("BaseYear", sBase, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListBaseYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListBaseYear.Add(impactAreaBase);
            impactarea.ImpactAreaSimulation impactAreaFuture = new impactarea.ImpactAreaSimulation("FutureYear", sFuture, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListFutureYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListFutureYear.Add(impactAreaFuture);

            scenarios.Scenario baseScenario = new scenarios.Scenario(baseYear, impactAreaListBaseYear);
            scenarios.Scenario futureScenario = new scenarios.Scenario(futureYear, impactAreaListFutureYear);
            int alternativeID = 23;
            alternatives.Alternative alternative = new alternatives.Alternative(baseScenario, futureScenario, poa, alternativeID);

            compute.MeanRandomProvider mrp = new MeanRandomProvider();
            Dictionary<int, Dictionary<string, Histogram>> alternativeResults = new Dictionary<int, Dictionary<string, Histogram>>();
            alternativeResults = alternative.AnnualizationCompute(mrp, iterations, discountRate);
            double actual = (alternativeResults[impactAreaID])[damageCategory].InverseCDF(mrp.NextRandom());
            double relativeDifference = (actual - expected) / expected;
            double tolerance = 0.01;
            Assert.True(relativeDifference < tolerance);

        }
    }
}
