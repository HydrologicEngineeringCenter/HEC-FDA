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
        static double[] FlowXs = { 0, 100000 };
        static double[] StageXs = { 0, 150000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string description = "description";
        static int id = 1;

        [Theory]
        [InlineData(51442, 50, .0275, 2023, 2072, 1, 75000)]
        [InlineData(59410, 50, .0275, 2023, 2050, 1, 75000)]
        public void ComputeAAEQDamage(double expected, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {

            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, xLabel, yLabel, name, description);
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
            string damageCategory = "residential";
            UncertainPairedData base_stage_damage = new UncertainPairedData(StageXs, baseDamages, xLabel, yLabel, name, description, damageCategory);
            UncertainPairedData future_stage_damage = new UncertainPairedData(StageXs, futureDamages, xLabel, yLabel, name, description, damageCategory);
            List<UncertainPairedData> updBase = new List<UncertainPairedData>();
            updBase.Add(base_stage_damage);
            List<UncertainPairedData> updFuture = new List<UncertainPairedData>();
            updFuture.Add(future_stage_damage);

            //make a giant levee with a default system response curve
            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, xLabel, yLabel, name, description);

            Simulation withoutProjectSimulationBase = Simulation.builder()
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();
 
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
            double err = Math.Abs((actual - expected) / expected);
            Assert.True(err<.01);

        }
    }
}
