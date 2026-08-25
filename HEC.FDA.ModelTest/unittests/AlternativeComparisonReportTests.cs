using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.scenarios;
using HEC.FDA.Model.alternativeComparisonReport;
using HEC.FDA.Model.alternatives;
using System.Threading;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class AlternativeComparisonReportTest
    {
        [Theory]
        [InlineData(37500, 37500, 300000, 300000, 50, .0275, 2023, 2072, 1, 7.5, "residential", "residential", 0)]
        [InlineData(150000, -112500, 300000, -225000, 50, .0275, 2023, 2050, 1, 7.5, "residential", "commercial", 1200000)]
        [InlineData(150000, -112500, 300000, 0, 50, .0275, 2023, 2050, 1, 7.5, "residential", "commercial", 0)]
        public void ComputeHandlesZeroDollarDamageAndDifferentSetsOfDamageCategories(double expectedEADReducedBaseYearDamCat1, double expectedEADReducedBaseYearDamCat2, double expectedEADReducedFutureYearDamCat1, double expectedEADReducedFutureYearDamCat2, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation, string damCat1, string damCat2, double futureYearDamageModified)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 15, 30 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string assetCategory = "structure";
            CurveMetaData metaData1 = new CurveMetaData(xLabel, yLabel, name, damCat1, assetCategory);
            CurveMetaData metaData2 = new CurveMetaData(xLabel, yLabel, name, damCat2, assetCategory);
            int impactAreaIdentifier = 1;
            int withoutAlternativeIdentifier = 1;
            int withAlternativeIdentifier = 2;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 30 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData1);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            IDistribution[] baseDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 600000),
                new Uniform(0, 600000)
            };
            IDistribution[] futureDamagesModified = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, futureYearDamageModified, 10),
                new Uniform(0, futureYearDamageModified, 10)
            };
            IDistribution[] futureDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 1200000, 10),
                new Uniform(0, 1200000, 10)
            };
            UncertainPairedData base_stage_damage_without = new UncertainPairedData(StageXs, baseDamages, metaData1);
            UncertainPairedData future_stage_damage_without = new UncertainPairedData(StageXs, futureDamages, metaData1);
            List<UncertainPairedData> updBaseWithoutProject = new List<UncertainPairedData>();
            updBaseWithoutProject.Add(base_stage_damage_without);
            List<UncertainPairedData> updFutureWithoutProject = new List<UncertainPairedData>();
            updFutureWithoutProject.Add(future_stage_damage_without);

            UncertainPairedData base_stage_damage_withProject = new UncertainPairedData(StageXs, baseDamages, metaData2);
            UncertainPairedData future_stage_damage_withProject = new UncertainPairedData(StageXs, futureDamagesModified, metaData2);
            List<UncertainPairedData> updBaseWithProject = new List<UncertainPairedData>();
            List<UncertainPairedData> updFutureWithProject = new List<UncertainPairedData>();
            updBaseWithProject.Add(base_stage_damage_withProject);
            updFutureWithProject.Add(future_stage_damage_withProject);

            //make a giant levee with a default system response curve
            double epsilon = 0.0001;
            double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
            IDistribution[] leveefailprobs = new IDistribution[3];
            for (int i = 0; i < 2; i++)
            {
                leveefailprobs[i] = new Deterministic(0); //probability at the top must be 1
            }
            leveefailprobs[2] = new Deterministic(1);
            UncertainPairedData levee = new UncertainPairedData(leveestages, leveefailprobs, metaData1);

            //Build without project alternative results 
            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updBaseWithoutProject)
                .Build();

            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updFutureWithoutProject)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            Scenario baseWithoutProjectScenario = new Scenario( impactAreaListBaseYear);
            ScenarioResults baseWithoutProjectScenarioResults = baseWithoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            Scenario futureWithoutProjectScenario = new Scenario( impactAreaListFutureYear);
            ScenarioResults futureWithoutProjectScenarioResults = futureWithoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withoutProjectAlternativeResults = Alternative.AnnualizationCompute(discountRate, poa, withoutAlternativeIdentifier, 
                baseWithoutProjectScenarioResults, futureWithoutProjectScenarioResults, baseYear,
                futureYear);

            //build with project alternative results 
            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updBaseWithProject)
                .Build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updFutureWithProject)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);


            Scenario baseWithProjectScenario = new Scenario( impactAreaListWithProjectBaseYear);
            ScenarioResults baseWithProjectScenarioResults = baseWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic:true);
            Scenario futureWithProjectScenario = new Scenario( impactAreaListWithProjectfutureYear);
            ScenarioResults futureWithProjectScenarioResults = futureWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withProjectAlternativeResults = Alternative.AnnualizationCompute(discountRate, poa, withAlternativeIdentifier, 
                baseWithProjectScenarioResults, futureWithProjectScenarioResults, baseYear,
                futureYear);

            List<AlternativeResults> withProjectAlternativeResultsList = new List<AlternativeResults>();
            withProjectAlternativeResultsList.Add(withProjectAlternativeResults);

            AlternativeComparisonReportResults alternativeComparisonReportResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);

            double actualBaseYearEADReducedDamCat1 = alternativeComparisonReportResults.SampleMeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat1);
            double differenceEADReducedBaseYearDamCat1 = Math.Abs(actualBaseYearEADReducedDamCat1 - expectedEADReducedBaseYearDamCat1);
            double eadErrorBaseDamCat1 = differenceEADReducedBaseYearDamCat1 / expectedEADReducedBaseYearDamCat1;

            double actualFutureYearEADReducedDamCat1 = alternativeComparisonReportResults.SampleMeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat1);
            double differenceEADReducedFutureYearDamCat1 = Math.Abs(actualFutureYearEADReducedDamCat1 - expectedEADReducedFutureYearDamCat1);
            double eadErrorFutureDamCat1 = differenceEADReducedFutureYearDamCat1 / expectedEADReducedFutureYearDamCat1;

            double actualBaseYearEADReducedDamCat2 = alternativeComparisonReportResults.SampleMeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat2);
            double differenceEADReducedBaseYearDamCat2 = Math.Abs(actualBaseYearEADReducedDamCat2 - expectedEADReducedBaseYearDamCat2);
            double eadErrorBaseDamCat2 = differenceEADReducedBaseYearDamCat2 / expectedEADReducedBaseYearDamCat2;

            double actualFutureYearEADReducedDamCat2 = alternativeComparisonReportResults.SampleMeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat2);
            double differenceFutureYearEADReducedDamCat2 = Math.Abs(actualFutureYearEADReducedDamCat2 - expectedEADReducedFutureYearDamCat2);
            double eadErrorFutureDamCat2;

            if (expectedEADReducedFutureYearDamCat2 == 0)
            {
                eadErrorFutureDamCat2 = Math.Abs(actualFutureYearEADReducedDamCat2 - expectedEADReducedFutureYearDamCat2);
            }
            else
            {
                eadErrorFutureDamCat2 = differenceFutureYearEADReducedDamCat2 / expectedEADReducedFutureYearDamCat2;
            }

            double tolerance = 0.11;
            Assert.True(eadErrorBaseDamCat1 < tolerance);
            Assert.True(eadErrorFutureDamCat1 < tolerance);
            Assert.True(eadErrorBaseDamCat2 < tolerance);
            Assert.True(eadErrorFutureDamCat2 < tolerance);
        }

        [Theory]
        [InlineData(50, .0275, 2023, 2050, 1, 7.5)]
        public void AlternativeComparisonReturnsCorrectDamCats(int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 150000, 300000 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string damCat = "residential";
            string assetCategory = "structure";
            CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCategory);
            int impactAreaIdentifier = 1;
            int withoutAlternativeIdentifier = 1;
            int withAlternativeIdentifier = 2;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            IDistribution[] baseDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 600000),
                new Uniform(0, 600000)
            };
            IDistribution[] futureDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 1200000, 10),
                new Uniform(0, 1200000, 10)
            };
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
            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            Scenario baseWithoutProjectScenario = new Scenario( impactAreaListBaseYear);
            ScenarioResults baseWithoutProjectScenarioResults = baseWithoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            Scenario futureWothoutProjectScenario = new Scenario( impactAreaListFutureYear);
            ScenarioResults futureWithoutProjectScenarioResults = futureWothoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withoutProjectAlternativeResults = Alternative.AnnualizationCompute(discountRate, poa, withoutAlternativeIdentifier, 
                baseWithoutProjectScenarioResults, futureWithoutProjectScenarioResults, baseYear, futureYear);

            //build with project alternative results 
            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);


            Scenario baseWithProjectScenario = new Scenario( impactAreaListWithProjectBaseYear);
            ScenarioResults baseWithProjectScenarioResults = baseWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic:true);
            Scenario futureWithProjectScenario = new Scenario( impactAreaListWithProjectfutureYear);
            ScenarioResults futureWithProjectScenarioResults = futureWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withProjectAlternativeResults = Alternative .AnnualizationCompute(discountRate, poa, withAlternativeIdentifier, 
                baseWithProjectScenarioResults, futureWithProjectScenarioResults, baseYear, futureYear);

            List<AlternativeResults> withProjectAlternativeResultsList = new List<AlternativeResults>();
            withProjectAlternativeResultsList.Add(withProjectAlternativeResults);

            AlternativeComparisonReportResults alternativeComparisonReportResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);
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

        [Theory]
        [InlineData(51442, 36500, 75000, 50, .0275, 2023, 2072, 1, 7.5)]
        [InlineData(59410, 36500, 75000, 50, .0275, 2023, 2050, 1, 7.5)]
        public void ComputeEqad(double expectedEqadReduced, double expectedEADReducedBaseYear, double expectedEADReducedFutureYear, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
        {
            double[] FlowXs = { 0, 100000 };
            double[] StageXs = { 0, 15, 30 };
            string xLabel = "x label";
            string yLabel = "y label";
            string name = "name";
            string residentialDamCat = "residential";
            string assetCategory = "structure";
            CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, residentialDamCat, assetCategory);
            int impactAreaIdentifier = 1;
            int withoutAlternativeIdentifier = 1;
            int withAlternativeIdentifier = 2;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 1, maxIterations: iterations);
            double exceedanceProbability = 0.5;

            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 30 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            IDistribution[] baseDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 600000),
                new Uniform(0, 600000)
            };
            IDistribution[] futureDamages = new IDistribution[3]
            {
                new Uniform(0,0,10),
                new Uniform(0, 1200000, 10),
                new Uniform(0, 1200000, 10)
            };
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
            ImpactAreaScenarioSimulation withoutProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation withoutProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(withoutProjectSimulationBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(withoutProjectSimulationFuture);

            Scenario baseWithoutProjectScenario = new Scenario( impactAreaListBaseYear);
            ScenarioResults baseWithoutProjectScenarioResults = baseWithoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            Scenario futureWothoutProjectScenario = new Scenario( impactAreaListFutureYear);
            ScenarioResults futureWithoutProjectScenarioResults = futureWothoutProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withoutProjectAlternativeResults = Alternative.AnnualizationCompute(discountRate, poa, 
                withoutAlternativeIdentifier, baseWithoutProjectScenarioResults, futureWithoutProjectScenarioResults, baseYear, futureYear);

            //build with project alternative results 
            ImpactAreaScenarioSimulation withProjectSimulationBase = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation withProjectSimulationFuture = ImpactAreaScenarioSimulation.Builder(impactAreaIdentifier)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithLevee(levee, topOfLeveeElevation)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectBaseYear.Add(withProjectSimulationBase);


            IList<ImpactAreaScenarioSimulation> impactAreaListWithProjectfutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListWithProjectfutureYear.Add(withProjectSimulationFuture);


            Scenario baseWithProjectScenario = new Scenario( impactAreaListWithProjectBaseYear);
            ScenarioResults baseWithProjectScenarioResults = baseWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            Scenario futureWithProjectScenario = new Scenario( impactAreaListWithProjectfutureYear);
            ScenarioResults futureWithProjectScenarioResults = futureWithProjectScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            AlternativeResults withProjectAlternativeResults = Alternative.AnnualizationCompute(discountRate, poa, withAlternativeIdentifier, 
                baseWithProjectScenarioResults, futureWithProjectScenarioResults, baseYear, futureYear);

            List<AlternativeResults> withProjectAlternativeResultsList = new List<AlternativeResults>();
            withProjectAlternativeResultsList.Add(withProjectAlternativeResults);

            AlternativeComparisonReportResults alternativeComparisonReportResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);
            double actualEqadReduced = alternativeComparisonReportResults.EqadReducedExceededWithProbabilityQ(exceedanceProbability, withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceEqad = actualEqadReduced - expectedEqadReduced;
            double EqadError = Math.Abs(differenceEqad / expectedEqadReduced);

            double actualBaseYearEADReduced = alternativeComparisonReportResults.SampleMeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceEADReducedBaseYear = Math.Abs(actualBaseYearEADReduced - expectedEADReducedBaseYear);
            double eadErrorBase = differenceEADReducedBaseYear / expectedEADReducedBaseYear;

            double actualFutureYearEADReduced = alternativeComparisonReportResults.SampleMeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceEADReducedFutureYear = Math.Abs(actualFutureYearEADReduced - expectedEADReducedFutureYear);
            double eadErrorFuture = differenceEADReducedFutureYear / expectedEADReducedFutureYear;

            double tolerance = 0.1;
            Assert.True(EqadError < tolerance);
            Assert.True(eadErrorBase < tolerance);
            Assert.True(eadErrorFuture < tolerance);

            double expectedBaseYearEADWithoutProject = withoutProjectAlternativeResults.SampleMeanBaseYearEAD(impactAreaIdentifier, residentialDamCat, assetCategory);
            double actualBaseYearEADWithoutProject = alternativeComparisonReportResults.SampleMeanWithoutProjectBaseYearEAD(impactAreaIdentifier, residentialDamCat, assetCategory);
            Assert.Equal(expectedBaseYearEADWithoutProject, actualBaseYearEADWithoutProject);

            double expectedEqADWithProject = withProjectAlternativeResults.SampleMeanEqad(impactAreaIdentifier, residentialDamCat, assetCategory);
            double actualEqADWithProject = alternativeComparisonReportResults.SampleMeanWithProjectEqad(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            Assert.Equal(expectedEqADWithProject, actualEqADWithProject);


        }



        private const string RiskTypeDamCat = "residential";
        private const string RiskTypeAssetCat = "content";

        private static DynamicHistogram RangeHistogram(int start, ConvergenceCriteria cc)
        {
            return new DynamicHistogram(Enumerable.Range(start, 100).Select(i => (double)i).ToList(), cc);
        }

        private static AlternativeResults BuildAlternativeCarryingBothRiskTypes(
            int alternativeID, int impactAreaID, int baseYear, int futureYear, int periodOfAnalysis, double discountRate,
            DynamicHistogram failBase, DynamicHistogram failFuture,
            DynamicHistogram nonFailBase, DynamicHistogram nonFailFuture)
        {
            ImpactAreaScenarioResults baseImpactArea = new(impactAreaID);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, failBase, impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, nonFailBase, impactAreaID, ConsequenceType.Damage, RiskType.Non_Fail));

            ImpactAreaScenarioResults futureImpactArea = new(impactAreaID);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, failFuture, impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, nonFailFuture, impactAreaID, ConsequenceType.Damage, RiskType.Non_Fail));

            ScenarioResults baseResults = new();
            baseResults.AddResults(baseImpactArea);
            ScenarioResults futureResults = new();
            futureResults.AddResults(futureImpactArea);

            return Alternative.AnnualizationCompute(
                discountRate, periodOfAnalysis, alternativeID, baseResults, futureResults, baseYear, futureYear);
        }

        /// <summary>
        /// EqAD consequences are stored per risk type - RiskType.Total is only ever a query wildcard in
        /// FilterByCategories, never a value assigned to a row - so a scenario with a system response function
        /// leaves both a Fail and a Non_Fail row in EqadResults. The counterpart lookups in
        /// ComputeDistributionOfEqadReduced must pass the risk type through. Omitting it falls back to the Total
        /// wildcard, so FirstOrDefault returns whichever row happens to be first and the with- and without-project
        /// conditions are subtracted across risk types, silently corrupting benefits.
        /// </summary>
        [Fact]
        public void EqadReducedPairsEachRiskTypeWithItsOwnKind()
        {
            const int impactAreaID = 1;
            const int withoutProjectAlternativeID = 1;
            const int withProjectAlternativeID = 2;
            const int baseYear = 2023;
            const int futureYear = 2072;
            const int periodOfAnalysis = 50;
            const double discountRate = 0.0275;
            const double tolerance = 1.0;

            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            //base and future year must differ, or AnnualizationCompute short circuits on identical scenarios
            //and never runs the discounting routine this test covers
            DynamicHistogram withoutFailBase = RangeHistogram(100, cc);
            DynamicHistogram withoutFailFuture = RangeHistogram(300, cc);
            DynamicHistogram withoutNonFailBase = RangeHistogram(700, cc);
            DynamicHistogram withoutNonFailFuture = RangeHistogram(900, cc);

            DynamicHistogram withFailBase = RangeHistogram(50, cc);
            DynamicHistogram withFailFuture = RangeHistogram(150, cc);
            DynamicHistogram withNonFailBase = RangeHistogram(200, cc);
            DynamicHistogram withNonFailFuture = RangeHistogram(400, cc);

            AlternativeResults withoutProject = BuildAlternativeCarryingBothRiskTypes(
                withoutProjectAlternativeID, impactAreaID, baseYear, futureYear, periodOfAnalysis, discountRate,
                withoutFailBase, withoutFailFuture, withoutNonFailBase, withoutNonFailFuture);

            AlternativeResults withProject = BuildAlternativeCarryingBothRiskTypes(
                withProjectAlternativeID, impactAreaID, baseYear, futureYear, periodOfAnalysis, discountRate,
                withFailBase, withFailFuture, withNonFailBase, withNonFailFuture);

            AlternativeComparisonReportResults report = AlternativeComparisonReport.ComputeAlternativeComparisonReport(
                withoutProject, new List<AlternativeResults> { withProject });

            Assert.NotNull(report);

            double expectedFailReduced =
                Alternative.ComputeEqad(withoutFailBase.SampleMean, baseYear, withoutFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate)
                - Alternative.ComputeEqad(withFailBase.SampleMean, baseYear, withFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate);

            double expectedNonFailReduced =
                Alternative.ComputeEqad(withoutNonFailBase.SampleMean, baseYear, withoutNonFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate)
                - Alternative.ComputeEqad(withNonFailBase.SampleMean, baseYear, withNonFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate);

            //pre-fix, the with-project Non_Fail row paired against the without-project Fail row, so this landed
            //on withoutNonFail - withFail instead of withoutNonFail - withNonFail
            double actualNonFailReduced = report.SampleMeanEqadReduced(withProjectAlternativeID, riskType: RiskType.Non_Fail);
            double actualFailReduced = report.SampleMeanEqadReduced(withProjectAlternativeID, riskType: RiskType.Fail);

            Assert.Equal(expectedFailReduced, actualFailReduced, tolerance);
            Assert.Equal(expectedNonFailReduced, actualNonFailReduced, tolerance);

            //Total is the wildcard that sums both rows, and is what the alternative comparison report surfaces
            Assert.Equal(expectedFailReduced + expectedNonFailReduced,
                report.SampleMeanEqadReduced(withProjectAlternativeID, riskType: RiskType.Total), tolerance);
        }
        private static AlternativeResults BuildAlternativeCarryingOnlyFailure(
            int alternativeID, int impactAreaID, int baseYear, int futureYear, int periodOfAnalysis, double discountRate,
            DynamicHistogram failBase, DynamicHistogram failFuture)
        {
            ImpactAreaScenarioResults baseImpactArea = new(impactAreaID);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, failBase, impactAreaID, ConsequenceType.Damage, RiskType.Fail));

            ImpactAreaScenarioResults futureImpactArea = new(impactAreaID);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned(RiskTypeDamCat, RiskTypeAssetCat, failFuture, impactAreaID, ConsequenceType.Damage, RiskType.Fail));

            ScenarioResults baseResults = new();
            baseResults.AddResults(baseImpactArea);
            ScenarioResults futureResults = new();
            futureResults.AddResults(futureImpactArea);

            return Alternative.AnnualizationCompute(
                discountRate, periodOfAnalysis, alternativeID, baseResults, futureResults, baseYear, futureYear);
        }

        /// <summary>
        /// The ordinary project configuration is asymmetric: only the with-project condition carries non-failure
        /// stage damage, so its Non_Fail row has no without-project counterpart. GetConsequenceResult signals that
        /// miss with an IsNull placeholder carrying RiskType.Fail, which unsubstituted stamps the reduced row Fail,
        /// collides with the genuine Fail row and is dropped - removing the whole Non_Fail component from benefits
        /// while it remains in with-project EqAD.
        /// </summary>
        [Fact]
        public void EqadReducedKeepsNonFailWhenOnlyTheWithProjectConditionHasNonFailureDamage()
        {
            const int impactAreaID = 1;
            const int withoutProjectAlternativeID = 1;
            const int withProjectAlternativeID = 2;
            const int baseYear = 2023;
            const int futureYear = 2072;
            const int periodOfAnalysis = 50;
            const double discountRate = 0.0275;
            const double tolerance = 1.0;

            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            //without project: no levee, so no non-failure stage damage functions and no Non_Fail rows
            DynamicHistogram withoutFailBase = RangeHistogram(100, cc);
            DynamicHistogram withoutFailFuture = RangeHistogram(300, cc);

            //with project: a levee, so the residual non-failure damages appear alongside the failure damages
            DynamicHistogram withFailBase = RangeHistogram(50, cc);
            DynamicHistogram withFailFuture = RangeHistogram(150, cc);
            DynamicHistogram withNonFailBase = RangeHistogram(200, cc);
            DynamicHistogram withNonFailFuture = RangeHistogram(400, cc);

            AlternativeResults withoutProject = BuildAlternativeCarryingOnlyFailure(
                withoutProjectAlternativeID, impactAreaID, baseYear, futureYear, periodOfAnalysis, discountRate,
                withoutFailBase, withoutFailFuture);

            AlternativeResults withProject = BuildAlternativeCarryingBothRiskTypes(
                withProjectAlternativeID, impactAreaID, baseYear, futureYear, periodOfAnalysis, discountRate,
                withFailBase, withFailFuture, withNonFailBase, withNonFailFuture);

            AlternativeComparisonReportResults report = AlternativeComparisonReport.ComputeAlternativeComparisonReport(
                withoutProject, new List<AlternativeResults> { withProject });

            Assert.NotNull(report);

            //the Non_Fail reduction must survive rather than colliding with the Fail row and being discarded
            List<RiskType> reducedRiskTypes = report
                .GetConsequencesReducedResultsForGivenAlternative(withProjectAlternativeID)
                .ConsequenceResultList.Select(c => c.RiskType).ToList();
            Assert.Contains(RiskType.Fail, reducedRiskTypes);
            Assert.Contains(RiskType.Non_Fail, reducedRiskTypes);

            double withoutFailEqad = Alternative.ComputeEqad(
                withoutFailBase.SampleMean, baseYear, withoutFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate);
            double withFailEqad = Alternative.ComputeEqad(
                withFailBase.SampleMean, baseYear, withFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate);
            double withNonFailEqad = Alternative.ComputeEqad(
                withNonFailBase.SampleMean, baseYear, withNonFailFuture.SampleMean, futureYear, periodOfAnalysis, discountRate);

            //the failure side pairs normally; the non-failure side discounts against zero because the
            //without-project condition genuinely has no such damages
            Assert.Equal(withoutFailEqad - withFailEqad,
                report.SampleMeanEqadReduced(withProjectAlternativeID, riskType: RiskType.Fail), tolerance);
            Assert.Equal(-withNonFailEqad,
                report.SampleMeanEqadReduced(withProjectAlternativeID, riskType: RiskType.Non_Fail), tolerance);

            //the alternative comparison report puts without-project EqAD, with-project EqAD and EqAD reduced on
            //a single row, so they have to subtract. Dropping the Non_Fail reduction breaks exactly this.
            double withoutProjectEqad = withoutProject.SampleMeanEqad();
            double withProjectEqad = withProject.SampleMeanEqad();
            double reducedTotal = report.SampleMeanEqadReduced(withProjectAlternativeID);

            Assert.Equal(withoutProjectEqad - withProjectEqad, reducedTotal, tolerance);
        }

    }
}
