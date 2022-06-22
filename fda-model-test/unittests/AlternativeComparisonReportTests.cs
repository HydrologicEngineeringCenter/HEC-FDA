using System;
using System.Collections.Generic;
using Xunit;
using compute;
using paireddata;
using Statistics;
using metrics;
using alternativeComparisonReport;
using alternatives;
using Statistics.Distributions;
using scenarios;
using System.Xml.Linq;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class AlternativeComparisonReportTest
    {


        [Theory]
        [InlineData(51442, 36500, 75000, 50, .0275, 2023, 2072, 1, 75000)]
        [InlineData(59410, 36500, 75000, 50, .0275, 2023, 2050, 1, 75000)]
        public void ComputeAAEQDamage(double expectedAAEQReduced, double expectedEADReducedBaseYear, double expectedEADReducedFutureYear, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 150000 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string damCat = "residential";
            string assetCategory = "structure";
            CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCategory);
            int impactAreaIdentifier = 1;
            int withoutAlternativeIdentifier = 1;
            int withAlternativeIdentifier = 2;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(maxIterations: iterations);
            MeanRandomProvider mrp = new MeanRandomProvider();
            double exceedanceProbability = 0.5;

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
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
                baseDamages[i] = new Uniform(0, 600000 * i, 10);
            }
            IDistribution[] futureDamages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                futureDamages[i] = new Uniform(0, 1200000 * i, 10);
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
                leveefailprobs[i] = new Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, metaData);

            //Build without project alternative results 
            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();

            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            Scenario baseWithoutProjectScenario = new Scenario(baseYear, impactAreaListBaseYear);
            ScenarioResults baseWithoutProjectScenarioResults = baseWithoutProjectScenario.Compute(mrp, convergenceCriteria);
            Scenario futureWothoutProjectScenario = new Scenario(futureYear, impactAreaListFutureYear);
            ScenarioResults futureWithoutProjectScenarioResults = futureWothoutProjectScenario.Compute(mrp, convergenceCriteria);
            AlternativeResults withoutProjectAlternativeResults = Alternative.AnnualizationCompute(mrp, discountRate, poa, withoutAlternativeIdentifier, baseWithoutProjectScenarioResults, futureWithoutProjectScenarioResults);

            //build with project alternative results 
            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updBase)
                .build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);


            Scenario baseWithProjectScenario = new Scenario(baseYear, impactAreaListWithProjectBaseYear);
            ScenarioResults baseWithProjectScenarioResults = baseWithProjectScenario.Compute(mrp, convergenceCriteria);
            Scenario futureWithProjectScenario = new Scenario(futureYear, impactAreaListWithProjectfutureYear);
            ScenarioResults futureWithProjectScenarioResults = futureWithProjectScenario.Compute(mrp, convergenceCriteria);
            AlternativeResults withProjectAlternativeResults = Alternative.AnnualizationCompute(mrp, discountRate, poa, withAlternativeIdentifier, baseWithProjectScenarioResults, futureWithProjectScenarioResults);

            List<AlternativeResults> withProjectAlternativeResultsList = new List<AlternativeResults>();
            withProjectAlternativeResultsList.Add(withProjectAlternativeResults);

            AlternativeComparisonReportResults alternativeComparisonReportResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(mrp, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativeResultsList);
            double actualAAEQReduced = alternativeComparisonReportResults.AAEQDamageReducedExceededWithProbabilityQ(exceedanceProbability, withAlternativeIdentifier, impactAreaIdentifier, damCat, assetCategory);
            double differenceAAEQ = actualAAEQReduced - expectedAAEQReduced;
            double aaeqError = Math.Abs(differenceAAEQ / expectedAAEQReduced);

            double actualBaseYearEADReduced = alternativeComparisonReportResults.MeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat, assetCategory);
            double differenceEADReducedBaseYear = Math.Abs(actualBaseYearEADReduced - expectedEADReducedBaseYear);
            double eadErrorBase = differenceEADReducedBaseYear / expectedEADReducedBaseYear;

            double actualFutureYearEADReduced = alternativeComparisonReportResults.MeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat, assetCategory);
            double differenceEADReducedFutureYear = Math.Abs(actualFutureYearEADReduced - expectedEADReducedFutureYear);
            double eadErrorFuture = differenceEADReducedFutureYear / expectedEADReducedFutureYear;

            double tolerance = 0.05;
            Assert.True(aaeqError < tolerance);
            Assert.True(eadErrorBase < tolerance);
            Assert.True(eadErrorFuture < tolerance);

            double expectedBaseYearEADWithoutProject = withoutProjectAlternativeResults.MeanBaseYearEAD(impactAreaIdentifier, damCat, assetCategory);
            double actualBaseYearEADWithoutProject = alternativeComparisonReportResults.MeanWithoutProjectBaseYearEAD(impactAreaIdentifier, damCat, assetCategory);
            Assert.Equal(expectedBaseYearEADWithoutProject, actualBaseYearEADWithoutProject);

            double expectedAAEQWithProject = withProjectAlternativeResults.MeanAAEQDamage(impactAreaIdentifier, damCat, assetCategory);
            double actualAAEQWithProject = alternativeComparisonReportResults.MeanWithProjectAAEQDamage(withAlternativeIdentifier, impactAreaIdentifier, damCat, assetCategory);
            Assert.Equal(expectedAAEQWithProject, actualAAEQWithProject);
        }

        [Theory]
        [InlineData(50, .0275, 2023, 2050, 1, 75000)]
        public void AlternativeComparisonReturnsCorrectDamCats(int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 150000 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string damCat = "residential";
            string assetCategory = "structure";
            CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCategory);
            int impactAreaIdentifier = 1;
            int withoutAlternativeIdentifier = 1;
            int withAlternativeIdentifier = 2;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(maxIterations: iterations);
            MeanRandomProvider mrp = new MeanRandomProvider();

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
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
                baseDamages[i] = new Uniform(0, 600000 * i, 10);
            }
            IDistribution[] futureDamages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                futureDamages[i] = new Uniform(0, 1200000 * i, 10);
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
                leveefailprobs[i] = new Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, metaData);

            //Build without project alternative results 
            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updBase)
                .build();

            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            Scenario baseWithoutProjectScenario = new Scenario(baseYear, impactAreaListBaseYear);
            ScenarioResults baseWithoutProjectScenarioResults = baseWithoutProjectScenario.Compute(mrp, convergenceCriteria);
            Scenario futureWothoutProjectScenario = new Scenario(futureYear, impactAreaListFutureYear);
            ScenarioResults futureWithoutProjectScenarioResults = futureWothoutProjectScenario.Compute(mrp, convergenceCriteria);
            AlternativeResults withoutProjectAlternativeResults = Alternative.AnnualizationCompute(mrp, discountRate, poa, withoutAlternativeIdentifier, baseWithoutProjectScenarioResults, futureWithoutProjectScenarioResults);

            //build with project alternative results 
            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updBase)
                .build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.builder(impactAreaIdentifier)
                .withFlowFrequency(flow_frequency)
                .withFlowStage(flow_stage)
                .withLevee(levee, topOfLeveeElevation)
                .withStageDamages(updFuture)
                .build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);


            Scenario baseWithProjectScenario = new Scenario(baseYear, impactAreaListWithProjectBaseYear);
            ScenarioResults baseWithProjectScenarioResults = baseWithProjectScenario.Compute(mrp, convergenceCriteria);
            Scenario futureWithProjectScenario = new Scenario(futureYear, impactAreaListWithProjectfutureYear);
            ScenarioResults futureWithProjectScenarioResults = futureWithProjectScenario.Compute(mrp, convergenceCriteria);
            AlternativeResults withProjectAlternativeResults = Alternative.AnnualizationCompute(mrp, discountRate, poa, withAlternativeIdentifier, baseWithProjectScenarioResults, futureWithProjectScenarioResults);

            List<AlternativeResults> withProjectAlternativeResultsList = new List<AlternativeResults>();
            withProjectAlternativeResultsList.Add(withProjectAlternativeResults);

            AlternativeComparisonReportResults alternativeComparisonReportResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(mrp, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativeResultsList);
            List<string> reportedDamCats = alternativeComparisonReportResults.GetDamageCategories();

            List<string> expectedList = new List<string>() { damCat };
            bool testPasses = true;
            foreach (string damageCat in reportedDamCats)
            {
                if (!expectedList.Contains(damCat))
                {
                    testPasses = false;
                }

            }
            if (expectedList.Count != reportedDamCats.Count)
            {
                testPasses = false;
            }
            Assert.True(testPasses);
        }
    }
}
