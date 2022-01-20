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
using alternativeComparisonReport;

namespace fda_model_test
{
    [Trait("Category", "Unit")]
    public class AlternativeComparisonReportTest
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
        [InlineData(208213.8061, 50, .0275, 2023, 2072, 1000, 300000)]
        [InlineData(239260.1814, 50, .0275, 2023, 2050, 1000, 300000)]
        public void ComputeAAEQDamage(double expected, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
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

            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, xLabel, yLabel, name, description, id);

            Simulation withoutProjectSimulationBase = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();
            //it feels weird that the EAD for this simulation is 150k because probability 
            //goes from 0 to 0.5
            Simulation withoutProjectSimulationFuture = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();
            int impactAreaID = 17;

            impactarea.ImpactArea impactArea = new impactarea.ImpactArea("Quahog", impactAreaID);
            impactarea.ImpactAreaSimulation impactAreaWithoutBase = new impactarea.ImpactAreaSimulation("BaseYear Without", withoutProjectSimulationBase, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListBaseYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListBaseYear.Add(impactAreaWithoutBase);
            impactarea.ImpactAreaSimulation impactAreaWithoutFuture = new impactarea.ImpactAreaSimulation("FutureYear without", withoutProjectSimulationFuture, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListFutureYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListFutureYear.Add(impactAreaWithoutFuture);

            scenarios.Scenario baseWithoutProjectScenario = new scenarios.Scenario(baseYear, impactAreaListBaseYear);
            scenarios.Scenario futureWothoutProjectScenario = new scenarios.Scenario(futureYear, impactAreaListFutureYear);
            int withoutProjectalternativeID = 23;
            alternatives.Alternative withoutProjectAlternative = new alternatives.Alternative(baseWithoutProjectScenario, futureWothoutProjectScenario, poa, withoutProjectalternativeID);

            Simulation withProjectSimulationBase = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updBase)
                .build();

            Simulation withProjectSimulationFuture = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updFuture)
                .build();

            impactarea.ImpactAreaSimulation impactAreaWithBase = new impactarea.ImpactAreaSimulation("BaseYear With", withProjectSimulationBase, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListWithProjectBaseYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListWithProjectBaseYear.Add(impactAreaWithBase);


            impactarea.ImpactAreaSimulation impactAreaWithFUture = new impactarea.ImpactAreaSimulation("Future Year With", withProjectSimulationFuture, id, impactArea);
            IList<impactarea.ImpactAreaSimulation> impactAreaListWithProjectfutureYear = new List<impactarea.ImpactAreaSimulation>();
            impactAreaListWithProjectfutureYear.Add(impactAreaWithFUture);

            scenarios.Scenario baseWithProjectScenario = new scenarios.Scenario(baseYear, impactAreaListWithProjectBaseYear);
            scenarios.Scenario futureWithProjectScenario = new scenarios.Scenario(futureYear, impactAreaListWithProjectfutureYear);
            int withProjectAlternativeID = 34;
            alternatives.Alternative withProjectAlternative = new alternatives.Alternative(baseWithProjectScenario, futureWithProjectScenario, poa, withProjectAlternativeID);
            List<alternatives.Alternative> withProjectAlternativeList = new List<alternatives.Alternative>();
            withProjectAlternativeList.Add(withProjectAlternative);

            AlternativeComparisonReport alternativeComparisonReport = new AlternativeComparisonReport(withoutProjectAlternative, withProjectAlternativeList);
            compute.MeanRandomProvider mrp = new MeanRandomProvider();


            Dictionary<int, Dictionary<string, Histogram>> alternativeResults = new Dictionary<int, Dictionary<string, Histogram>>();

            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> alternativeComparisonReportResults = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();

            alternativeComparisonReportResults = alternativeComparisonReport.ComputeDistributionOfAAEQDamageReduced(mrp, iterations, discountRate);
            double actual = ((alternativeComparisonReportResults[withProjectAlternativeID])[impactAreaID])[damageCategory].InverseCDF(mrp.NextRandom());
            double relativeDifference = Math.Abs((actual - expected) / expected);
            double tolerance = 0.01;
            Assert.True(relativeDifference < tolerance);

        }
    }
}
