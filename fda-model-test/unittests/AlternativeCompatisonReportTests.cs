using System;
using System.Collections.Generic;
using Xunit;
using compute;
using paireddata;
using Statistics;
using metrics;
using alternativeComparisonReport;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class AlternativeComparisonReportTest
    {


        [Theory]
        [InlineData(51442, 50, .0275, 2023, 2072, 1, 75000)]
        [InlineData(59410, 50, .0275, 2023, 2050, 1, 75000)]
        public void ComputeAAEQDamage(double expected, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 150000 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string damCat = "residential";
            string assetCategory = "structure";
            CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCategory);
            int identifier = 1;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(maxIterations: iterations);
            Statistics.ContinuousDistribution flow_frequency = new Statistics.Distributions.Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
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

            //make a giant levee with a default system response curve
            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, metaData);

            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.builder(identifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();
 
            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(identifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            scenarios.Scenario baseWithoutProjectScenario = new scenarios.Scenario(baseYear, impactAreaListBaseYear);
            scenarios.Scenario futureWothoutProjectScenario = new scenarios.Scenario(futureYear, impactAreaListFutureYear);
            alternatives.Alternative withoutProjectAlternative = new alternatives.Alternative(baseWithoutProjectScenario, futureWothoutProjectScenario, poa, identifier);

            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.builder(identifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updBase)
                .build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(identifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);

            scenarios.Scenario baseWithProjectScenario = new scenarios.Scenario(baseYear, impactAreaListWithProjectBaseYear);
            scenarios.Scenario futureWithProjectScenario = new scenarios.Scenario(futureYear, impactAreaListWithProjectfutureYear);
            int withProjectAlternativeID = 1;
            alternatives.Alternative withProjectAlternative = new alternatives.Alternative(baseWithProjectScenario, futureWithProjectScenario, poa, withProjectAlternativeID);
            List<alternatives.Alternative> withProjectAlternativeList = new List<alternatives.Alternative>();
            withProjectAlternativeList.Add(withProjectAlternative);

            AlternativeComparisonReport alternativeComparisonReport = new AlternativeComparisonReport(withoutProjectAlternative, withProjectAlternativeList);
            compute.MeanRandomProvider mrp = new MeanRandomProvider();

            AlternativeComparisonReportResults alternativeComparisonReportResults = alternativeComparisonReport.ComputeDistributionOfAAEQDamageReduced(mrp, convergenceCriteria, discountRate);
            double actual = alternativeComparisonReportResults.ConsequencesReducedExceededWithProbabilityQ(mrp.NextRandom(), identifier, identifier, damCat, assetCategory);
            double difference = actual - expected;
            double err = Math.Abs(difference / expected);
            double tol = 0.01;
            Assert.True(err<tol);

        }
    }
}
