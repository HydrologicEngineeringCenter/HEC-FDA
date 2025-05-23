﻿using System;
using System.Collections.Generic;
using Xunit;
using Statistics;
using Statistics.Distributions;
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

            AlternativeComparisonReportResults alternativeComparisonReportResults = new AlternativeComparisonReport().ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);
            //double actualAAEQReduced = alternativeComparisonReportResults.AAEQDamageReducedExceededWithProbabilityQ(exceedanceProbability, withAlternativeIdentifier, impactAreaIdentifier, damCat, assetCategory);
            //double differenceAAEQ = actualAAEQReduced - expectedAAEQReduced;
            //double aaeqError = Math.Abs(differenceAAEQ / expectedAAEQReduced);

            double actualBaseYearEADReducedDamCat1 = alternativeComparisonReportResults.MeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat1);
            double differenceEADReducedBaseYearDamCat1 = Math.Abs(actualBaseYearEADReducedDamCat1 - expectedEADReducedBaseYearDamCat1);
            double eadErrorBaseDamCat1 = differenceEADReducedBaseYearDamCat1 / expectedEADReducedBaseYearDamCat1;

            double actualFutureYearEADReducedDamCat1 = alternativeComparisonReportResults.MeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat1);
            double differenceEADReducedFutureYearDamCat1 = Math.Abs(actualFutureYearEADReducedDamCat1 - expectedEADReducedFutureYearDamCat1);
            double eadErrorFutureDamCat1 = differenceEADReducedFutureYearDamCat1 / expectedEADReducedFutureYearDamCat1;

            double actualBaseYearEADReducedDamCat2 = alternativeComparisonReportResults.MeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat2);
            double differenceEADReducedBaseYearDamCat2 = Math.Abs(actualBaseYearEADReducedDamCat2 - expectedEADReducedBaseYearDamCat2);
            double eadErrorBaseDamCat2 = differenceEADReducedBaseYearDamCat2 / expectedEADReducedBaseYearDamCat2;

            double actualFutureYearEADReducedDamCat2 = alternativeComparisonReportResults.MeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, damCat2);
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

            AlternativeComparisonReportResults alternativeComparisonReportResults = new AlternativeComparisonReport().ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);
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
        public void ComputeAAEQDamage(double expectedAAEQReduced, double expectedEADReducedBaseYear, double expectedEADReducedFutureYear, int poa, double discountRate, int baseYear, int futureYear, int iterations, double topOfLeveeElevation)
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

            AlternativeComparisonReportResults alternativeComparisonReportResults = new AlternativeComparisonReport().ComputeAlternativeComparisonReport(withoutProjectAlternativeResults, withProjectAlternativeResultsList);
            double actualAAEQReduced = alternativeComparisonReportResults.AAEQDamageReducedExceededWithProbabilityQ(exceedanceProbability, withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceAAEQ = actualAAEQReduced - expectedAAEQReduced;
            double aaeqError = Math.Abs(differenceAAEQ / expectedAAEQReduced);

            double actualBaseYearEADReduced = alternativeComparisonReportResults.MeanBaseYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceEADReducedBaseYear = Math.Abs(actualBaseYearEADReduced - expectedEADReducedBaseYear);
            double eadErrorBase = differenceEADReducedBaseYear / expectedEADReducedBaseYear;

            double actualFutureYearEADReduced = alternativeComparisonReportResults.MeanFutureYearEADReduced(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            double differenceEADReducedFutureYear = Math.Abs(actualFutureYearEADReduced - expectedEADReducedFutureYear);
            double eadErrorFuture = differenceEADReducedFutureYear / expectedEADReducedFutureYear;

            double tolerance = 0.11;
            Assert.True(aaeqError < tolerance);
            Assert.True(eadErrorBase < tolerance);
            Assert.True(eadErrorFuture < tolerance);

            double expectedBaseYearEADWithoutProject = withoutProjectAlternativeResults.MeanBaseYearEAD(impactAreaIdentifier, residentialDamCat, assetCategory);
            double actualBaseYearEADWithoutProject = alternativeComparisonReportResults.MeanWithoutProjectBaseYearEAD(impactAreaIdentifier, residentialDamCat, assetCategory);
            Assert.Equal(expectedBaseYearEADWithoutProject, actualBaseYearEADWithoutProject);

            double expectedAAEQWithProject = withProjectAlternativeResults.MeanAAEQDamage(impactAreaIdentifier, residentialDamCat, assetCategory);
            double actualAAEQWithProject = alternativeComparisonReportResults.MeanWithProjectAAEQDamage(withAlternativeIdentifier, impactAreaIdentifier, residentialDamCat, assetCategory);
            Assert.Equal(expectedAAEQWithProject, actualAAEQWithProject);


        }


    }
}
