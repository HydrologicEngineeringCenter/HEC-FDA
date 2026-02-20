using System;
using System.Collections.Generic;
using Xunit;
using Statistics;
using Statistics.Distributions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.scenarios;
using HEC.FDA.Model.alternatives;
using System.Linq;
using System.Threading;
using Statistics.Histograms;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class AlternativeTest
    {
        static double[] FlowXs = { 0, 100000 };
        static double[] StageXs = { 0, 150000, 300000 };
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string damCat = "residential";
        static string assetCat = "content";
        CurveMetaData metaData = new CurveMetaData(xLabel, yLabel, name, damCat, assetCat);
        static int impactAreaID = 1;
        static int alternativeID = 1;
        static double exceedanceProbability = 0.5;
        /// <summary>
        /// calculations for the below test can be found at https://docs.google.com/spreadsheets/d/1mPp8O2jm1wnsacQ7ZE3_sU_2xvghWOjC/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(208213.8061, 208213.8061, 150000, 300000, 150000, 300000, 50, .0275, 2023, 2072, 1, 2.0)]
        [InlineData(239260.1814, 239260.1814, 150000, 300000, 150000, 300000, 50, .0275, 2023, 2050, 1, 2.0)]
        [InlineData(150000, 150000, 150000, 150000, 150000, 150000, 50, .0275, 2023, 2072, 1, 1.0)]//if base year EAD = future year EAD then EAD = EqAD
        [InlineData(150000, 150000, 150000, 150000, 150000, 150000, 50, .0275, 2023, 2050, 1, 1.0)]//if base year EAD = future year EAD then EAD = EqAD
        public void AlternativeResults_Test(double expectedEqadExceededWithAnyProbability, double expectedMeanEqad, double expectedBaseYearEAD, double expectedFutureYearEAD, double expectedBaseYearDamageExceededWithAnyProb, double expectedFutureYearDamageExceededWithAnyProb, int poa, double discountRate, int baseYear, int futureYear, int iterations, double futureDamageFractionOfExistingDamage)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = new Uniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            double baseyearDamage = 600000;
            IDistribution[] baseDamages = new IDistribution[3]
            {
                    new Uniform(0,0, 10),
                    new Uniform(0, baseyearDamage, 10),
                    new Uniform(0,baseyearDamage, 10)
            };
            IDistribution[] futureDamages = new IDistribution[3]
            {
                    new Uniform(0,0,10),
                    new Uniform(0,baseyearDamage*futureDamageFractionOfExistingDamage,10),
                    new Uniform(0,baseyearDamage*futureDamageFractionOfExistingDamage, 10)
            };
            UncertainPairedData base_stage_damage = new UncertainPairedData(StageXs, baseDamages, metaData);
            UncertainPairedData future_stage_damage = new UncertainPairedData(StageXs, futureDamages, metaData);
            List<UncertainPairedData> updBase = new List<UncertainPairedData>();
            updBase.Add(base_stage_damage);
            List<UncertainPairedData> updFuture = new List<UncertainPairedData>();
            updFuture.Add(future_stage_damage);

            ImpactAreaScenarioSimulation sBase = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation sFuture = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(sBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(sFuture);

            Scenario baseScenario = new Scenario( impactAreaListBaseYear);
            ScenarioResults baseScenarioResults = baseScenario.Compute(convergenceCriteria, computeIsDeterministic:true);
            Scenario futureScenario = new Scenario(impactAreaListFutureYear);
            ScenarioResults futureScenarioResults = futureScenario.Compute(convergenceCriteria, computeIsDeterministic: true);

            AlternativeResults alternativeResults = Alternative.AnnualizationCompute(discountRate, poa, alternativeID, 
                baseScenarioResults, futureScenarioResults,baseYear, futureYear);
            double tolerance = 0.01;

            double actualEqadExceededWithProb = alternativeResults.EqadExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damCat, assetCat);
            double differenceEqadExceededWithProb = actualEqadExceededWithProb - expectedEqadExceededWithAnyProbability;
            double errorEqadExceededWithProb = Math.Abs(differenceEqadExceededWithProb / actualEqadExceededWithProb);
            Assert.True(errorEqadExceededWithProb < tolerance);

            double actualMeanEqad = alternativeResults.SampleMeanEqad(impactAreaID, damCat, assetCat);
            double differenceEqadMean = actualMeanEqad - expectedMeanEqad;
            double errorMeanEqad = Math.Abs(differenceEqadMean / actualMeanEqad);
            Assert.True(errorMeanEqad < tolerance);

            double actualBaseYearEAD = alternativeResults.SampleMeanBaseYearEAD(impactAreaID, damCat, assetCat);
            double differenceActualBaseYearEAD = actualBaseYearEAD - expectedBaseYearEAD;
            double errorBaseYearEAD = Math.Abs(differenceActualBaseYearEAD / actualBaseYearEAD);
            Assert.True(errorBaseYearEAD < tolerance);

            double actualFutureYearEAD = alternativeResults.SampleMeanFutureYearEAD(impactAreaID, damCat, assetCat);
            double differenceActualFutureYearEAD = actualFutureYearEAD - expectedFutureYearEAD;
            double errorFutureYearEAD = Math.Abs(differenceActualFutureYearEAD / actualFutureYearEAD);
            Assert.True(errorFutureYearEAD < tolerance);

            double actualBaseYearEADExceeded = alternativeResults.BaseYearEADDamageExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damCat, assetCat);
            double differenceActualBaseYearEADExceeded = actualBaseYearEADExceeded - expectedBaseYearDamageExceededWithAnyProb;
            double errorBaseYearEADExceeded = Math.Abs(differenceActualBaseYearEADExceeded / actualBaseYearEADExceeded);
            Assert.True(errorBaseYearEADExceeded < tolerance);

            double actualFutureYearEADExceeded = alternativeResults.FutureYearEADDamageExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damCat, assetCat);
            double differenceActualFutureYearEADExceeded = actualFutureYearEADExceeded - expectedFutureYearDamageExceededWithAnyProb;
            double errorFutureYearEADExceeded = Math.Abs(differenceActualFutureYearEADExceeded / actualFutureYearEADExceeded);
            Assert.True(errorFutureYearEADExceeded < tolerance);

        }

        [Theory]
        [InlineData(50, .0275, 2023, 2072, 1)]
        [InlineData(50, .0275, 2023, 2050, 1)]
        public void AlternativeReturnsCorrectDamCats(int poa, double discountRate, int baseYear, int futureYear, int iterations)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: iterations, maxIterations: iterations);
            ContinuousDistribution flow_frequency = new Uniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = new Uniform(0, 300000 * i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(FlowXs, stages, metaData);
            //create a damage distribution for base and future year (future year assumption is massive economic development) 
            IDistribution[] baseDamages = new IDistribution[3]
            {
                    new Uniform(0,0, 10),
                    new Uniform(0, 600000, 10),
                    new Uniform(0,600000, 10)
            };
            IDistribution[] futureDamages = new IDistribution[3]
            {
                    new Uniform(0,0,10),
                    new Uniform(0,1200000,10),
                    new Uniform(0,1200000, 10)
            };
            CurveMetaData commercialCurveMetaData = new CurveMetaData(xLabel, yLabel, name, "commercial", "structure");

            UncertainPairedData base_stage_damage_residential = new UncertainPairedData(StageXs, baseDamages, metaData);
            UncertainPairedData base_stage_damage_commercial = new UncertainPairedData(StageXs, baseDamages, commercialCurveMetaData);

            UncertainPairedData future_stage_damage_commercial = new UncertainPairedData(StageXs, futureDamages, commercialCurveMetaData);
            UncertainPairedData future_stage_damage_residential = new UncertainPairedData(StageXs, futureDamages, metaData);

            List<UncertainPairedData> updBase = new List<UncertainPairedData>();
            updBase.Add(base_stage_damage_residential);
            updBase.Add(base_stage_damage_commercial);

            List<UncertainPairedData> updFuture = new List<UncertainPairedData>();
            updFuture.Add(future_stage_damage_commercial);
            updFuture.Add(future_stage_damage_residential);

            ImpactAreaScenarioSimulation sBase = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updBase)
                .Build();

            ImpactAreaScenarioSimulation sFuture = ImpactAreaScenarioSimulation.Builder(impactAreaID)
                .WithFlowFrequency(flow_frequency)
                .WithFlowStage(flow_stage)
                .WithStageDamages(updFuture)
                .Build();

            IList<ImpactAreaScenarioSimulation> impactAreaListBaseYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListBaseYear.Add(sBase);
            IList<ImpactAreaScenarioSimulation> impactAreaListFutureYear = new List<ImpactAreaScenarioSimulation>();
            impactAreaListFutureYear.Add(sFuture);

            Scenario baseScenario = new Scenario(impactAreaListBaseYear);
            ScenarioResults baseScenarioResults = baseScenario.Compute(convergenceCriteria, computeIsDeterministic: true);
            Scenario futureScenario = new Scenario(impactAreaListFutureYear);
            ScenarioResults futureScenarioResults = futureScenario.Compute(convergenceCriteria, computeIsDeterministic: true);


            AlternativeResults alternativeResults = Alternative.AnnualizationCompute(discountRate, poa, alternativeID, 
                baseScenarioResults, futureScenarioResults, baseYear, futureYear);
            List<string> damCats = alternativeResults.GetDamageCategories();
            List<string> expectedList = new List<string>() { "residential", "commercial" };
            bool testPasses = true;
            foreach (string damCat in damCats)
            {
                if (!expectedList.Contains(damCat))
                {
                    testPasses = false;
                }

            }
            if (expectedList.Count != damCats.Count)
            {
                testPasses = false;
            }
            Assert.True(testPasses);
        }

        /// <summary>
        ///  The calculations for the below test can be found at https://docs.google.com/spreadsheets/d/1uY1tJBap-y7evLE5oK8-lx3pQUjSJ3go/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(35000, 2023, 50000, 2072, 50, .07, 38835.3)]
        [InlineData(0, 2023, 1000, 2072, 50, .07, 255.68)]
        [InlineData(35000, 2023, 35000, 2072, 50, .07, 35000)]
        [InlineData(35000, 2023, 50000, 2047, 50, .07, 41893.12)]
        [InlineData(35000, 2023, 50000, 2072, 50, .03, 40680.87)]
        [InlineData(0, 2023, 1000, 2072, 50, .03, 378.72)]
        [InlineData(35000, 2023, 35000, 2072, 50, .03, 35000)]
        [InlineData(35000, 2023, 50000, 2047, 50, .03, 44279.92)]
        public void ComputeEEAD_Test(double baseYearEAD, int baseYear, double mostLikelyFutureEAD, int mostLikelyFutureYear, int periodOfAnalysis, double discountRate, double expected)
        {
            double actual = Alternative.ComputeEqad(baseYearEAD, baseYear, mostLikelyFutureEAD, mostLikelyFutureYear, periodOfAnalysis, discountRate);
            Assert.Equal(expected, actual, .01);
        }

        [Fact]
        public void LifeLossResultsExcludedFromEqad()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            // Build base year consequences: one damage + one life loss
            var baseDamageHist = new DynamicHistogram(Enumerable.Range(100, 100).Select(i => (double)i).ToList(), cc);
            var baseLifeLossHist = new DynamicHistogram(Enumerable.Range(10, 100).Select(i => (double)i).ToList(), cc);
            var baseDamage = new AggregatedConsequencesBinned("residential", "content", baseDamageHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);
            var baseLifeLoss = new AggregatedConsequencesBinned("LifeLoss", "LifeLoss", baseLifeLossHist, impactAreaID, ConsequenceType.LifeLoss, RiskType.Fail);

            // Build future year consequences: same categories, different values
            var futureDamageHist = new DynamicHistogram(Enumerable.Range(200, 100).Select(i => (double)i).ToList(), cc);
            var futureLifeLossHist = new DynamicHistogram(Enumerable.Range(20, 100).Select(i => (double)i).ToList(), cc);
            var futureDamage = new AggregatedConsequencesBinned("residential", "content", futureDamageHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);
            var futureLifeLoss = new AggregatedConsequencesBinned("LifeLoss", "LifeLoss", futureLifeLossHist, impactAreaID, ConsequenceType.LifeLoss, RiskType.Fail);

            // Assemble base year ScenarioResults
            var baseImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(baseDamage);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(baseLifeLoss);
            var baseResults = new ScenarioResults();
            baseResults.AddResults(baseImpactArea);

            // Assemble future year ScenarioResults
            var futureImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(futureDamage);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(futureLifeLoss);
            var futureResults = new ScenarioResults();
            futureResults.AddResults(futureImpactArea);

            // Act
            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseResults, futureResults, baseYear: 2023, futureYear: 2072);

            // Assert
            Assert.NotNull(results);

            // EqAD should contain only damage, no life loss
            bool hasLifeLoss = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.LifeLoss);
            Assert.False(hasLifeLoss, "EqAD results should not contain life loss consequences");

            bool hasDamage = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.Damage);
            Assert.True(hasDamage, "EqAD results should contain damage consequences");

            // Component scenario results should still contain life loss
            bool baseHasLifeLoss = results.BaseYearScenarioResults.ResultsList
                .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
                .Any(c => c.ConsequenceType == ConsequenceType.LifeLoss);
            Assert.True(baseHasLifeLoss, "Base year scenario results should still contain life loss");

            bool futureHasLifeLoss = results.FutureYearScenarioResults.ResultsList
                .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
                .Any(c => c.ConsequenceType == ConsequenceType.LifeLoss);
            Assert.True(futureHasLifeLoss, "Future year scenario results should still contain life loss");
        }

        [Fact]
        public void SingleBaseScenario_EqadMatchesInputAndExcludesLifeLoss()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            var damageHist = new DynamicHistogram(Enumerable.Range(100, 100).Select(i => (double)i).ToList(), cc);
            var lifeLossHist = new DynamicHistogram(Enumerable.Range(10, 100).Select(i => (double)i).ToList(), cc);
            var damage = new AggregatedConsequencesBinned("residential", "content", damageHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);
            var lifeLoss = new AggregatedConsequencesBinned("LifeLoss", "LifeLoss", lifeLossHist, impactAreaID, ConsequenceType.LifeLoss, RiskType.Fail);

            var impactArea = new ImpactAreaScenarioResults(impactAreaID);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(damage);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(lifeLoss);
            var baseResults = new ScenarioResults();
            baseResults.AddResults(impactArea);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                computedResultsBaseYear: baseResults, computedResultsFutureYear: null,
                baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);

            bool hasLifeLoss = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.LifeLoss);
            Assert.False(hasLifeLoss, "EqAD results should not contain life loss consequences");

            bool hasDamage = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.Damage);
            Assert.True(hasDamage, "EqAD results should contain damage consequences");

            // EqAD should match the input damage result exactly (no discounting for single scenario)
            double eqadMean = results.EqadResults.ConsequenceResultList
                .First(c => c.ConsequenceType == ConsequenceType.Damage)
                .ConsequenceSampleMean();
            Assert.Equal(damageHist.SampleMean, eqadMean);
        }

        [Fact]
        public void SingleFutureScenario_EqadMatchesInputAndExcludesLifeLoss()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            var damageHist = new DynamicHistogram(Enumerable.Range(200, 100).Select(i => (double)i).ToList(), cc);
            var lifeLossHist = new DynamicHistogram(Enumerable.Range(20, 100).Select(i => (double)i).ToList(), cc);
            var damage = new AggregatedConsequencesBinned("residential", "content", damageHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);
            var lifeLoss = new AggregatedConsequencesBinned("LifeLoss", "LifeLoss", lifeLossHist, impactAreaID, ConsequenceType.LifeLoss, RiskType.Fail);

            var impactArea = new ImpactAreaScenarioResults(impactAreaID);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(damage);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(lifeLoss);
            var futureResults = new ScenarioResults();
            futureResults.AddResults(impactArea);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                computedResultsBaseYear: null, computedResultsFutureYear: futureResults,
                baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);

            bool hasLifeLoss = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.LifeLoss);
            Assert.False(hasLifeLoss, "EqAD results should not contain life loss consequences");

            bool hasDamage = results.EqadResults.ConsequenceResultList
                .Any(c => c.ConsequenceType == ConsequenceType.Damage);
            Assert.True(hasDamage, "EqAD results should contain damage consequences");

            // EqAD should match the input damage result exactly (no discounting for single scenario)
            double eqadMean = results.EqadResults.ConsequenceResultList
                .First(c => c.ConsequenceType == ConsequenceType.Damage)
                .ConsequenceSampleMean();
            Assert.Equal(damageHist.SampleMean, eqadMean);
        }

        [Fact]
        public void LifeLossOnly_EqadResultsIsNull()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            var lifeLossHist = new DynamicHistogram(Enumerable.Range(10, 100).Select(i => (double)i).ToList(), cc);
            var lifeLoss = new AggregatedConsequencesBinned("LifeLoss", "LifeLoss", lifeLossHist, impactAreaID, ConsequenceType.LifeLoss, RiskType.Fail);

            var impactArea = new ImpactAreaScenarioResults(impactAreaID);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(lifeLoss);
            var scenarioResults = new ScenarioResults();
            scenarioResults.AddResults(impactArea);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                computedResultsBaseYear: scenarioResults, computedResultsFutureYear: null,
                baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);
            Assert.Empty(results.EqadResults.ConsequenceResultList);
        }
    }
}
