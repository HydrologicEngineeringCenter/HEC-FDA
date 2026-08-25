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
using Utility.Logging;

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

        /// <summary>
        /// Scenarios with a system response function carry Non_Fail damage consequences alongside Fail ones.
        /// The counterpart lookup defaults riskType to Fail, so omitting it made a Non_Fail category present in
        /// BOTH years pair with the other year's Fail row rather than with its own kind. AddConsequenceResults
        /// then discarded that mispaired result as a duplicate against the Total wildcard, leaving Non_Fail EqAD
        /// absent from the results entirely. Both risk types must pair with their own kind and both must survive.
        /// </summary>
        [Fact]
        public void NonFailureConsequencesPairWithTheirOwnRiskType()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            var failBaseHist = new DynamicHistogram(Enumerable.Range(100, 100).Select(i => (double)i).ToList(), cc);
            var failFutureHist = new DynamicHistogram(Enumerable.Range(200, 100).Select(i => (double)i).ToList(), cc);
            var nonFailBaseHist = new DynamicHistogram(Enumerable.Range(300, 100).Select(i => (double)i).ToList(), cc);
            var nonFailFutureHist = new DynamicHistogram(Enumerable.Range(400, 100).Select(i => (double)i).ToList(), cc);

            var baseImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", failBaseHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", nonFailBaseHist, impactAreaID, ConsequenceType.Damage, RiskType.Non_Fail));

            var futureImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", failFutureHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", nonFailFutureHist, impactAreaID, ConsequenceType.Damage, RiskType.Non_Fail));

            var baseResults = new ScenarioResults();
            baseResults.AddResults(baseImpactArea);
            var futureResults = new ScenarioResults();
            futureResults.AddResults(futureImpactArea);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseResults, futureResults, baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);

            //both risk types must survive; the Non_Fail row was previously dropped as a duplicate
            AggregatedConsequencesByQuantile fail = results.EqadResults.ConsequenceResultList
                .Single(c => c.RiskType == RiskType.Fail);
            AggregatedConsequencesByQuantile nonFail = results.EqadResults.ConsequenceResultList
                .Single(c => c.RiskType == RiskType.Non_Fail);

            //each pairs with its own risk type in the other year, rather than discounting against zero
            double expectedFail = Alternative.ComputeEqad(
                failBaseHist.SampleMean, 2023, failFutureHist.SampleMean, 2072, 50, 0.0275);
            double expectedNonFail = Alternative.ComputeEqad(
                nonFailBaseHist.SampleMean, 2023, nonFailFutureHist.SampleMean, 2072, 50, 0.0275);

            Assert.Equal(expectedFail, fail.ConsequenceDistribution.SampleMean, 6);
            Assert.Equal(expectedNonFail, nonFail.ConsequenceDistribution.SampleMean, 6);

            //guard the value specifically: pairing against the Fail row, or discounting against zero, would
            //both land this below its base year mean
            Assert.True(nonFail.ConsequenceDistribution.SampleMean > nonFailBaseHist.SampleMean,
                "Non_Fail EqAD fell below its base year mean, so it did not pair with its own risk type");
        }

        /// <summary>
        /// A damage category can exist in one analysis year and not the other, for example under managed retreat
        /// where an impact area is bought out. The missing year must be treated as zero damage rather than
        /// dereferenced - GetConsequenceResult returns null in that case.
        /// </summary>
        [Theory]
        [InlineData(true)]  //category present in the base year, absent in the future year
        [InlineData(false)] //category present in the future year, absent in the base year
        public void CategoryMissingFromOneAnalysisYear_IsTreatedAsZeroDamage(bool missingFromFutureYear)
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            //"residential" exists in both years, "commercial" only in one
            var sharedBaseHist = new DynamicHistogram(Enumerable.Range(100, 100).Select(i => (double)i).ToList(), cc);
            var sharedFutureHist = new DynamicHistogram(Enumerable.Range(200, 100).Select(i => (double)i).ToList(), cc);
            var loneHist = new DynamicHistogram(Enumerable.Range(300, 100).Select(i => (double)i).ToList(), cc);

            var baseImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", sharedBaseHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail));

            var futureImpactArea = new ImpactAreaScenarioResults(impactAreaID);
            futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content", sharedFutureHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail));

            var lone = new AggregatedConsequencesBinned("commercial", "content", loneHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);
            if (missingFromFutureYear)
            {
                baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(lone);
            }
            else
            {
                futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(lone);
            }

            var baseResults = new ScenarioResults();
            baseResults.AddResults(baseImpactArea);
            var futureResults = new ScenarioResults();
            futureResults.AddResults(futureImpactArea);

            //this threw a NullReferenceException out of the Parallel.For in IterateOnEqad before the fix
            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseResults, futureResults, baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);

            //both categories survive into EqAD
            List<string> damageCategories = results.GetDamageCategories();
            Assert.Contains("residential", damageCategories);
            Assert.Contains("commercial", damageCategories);

            //the one sided category still carries damage, discounted from zero in the year it was absent,
            //so it lands strictly between zero and the value it holds in the year it is present
            AggregatedConsequencesByQuantile commercial = results.EqadResults.ConsequenceResultList
                .Single(c => c.DamageCategory == "commercial");
            double mean = commercial.ConsequenceDistribution.SampleMean;
            Assert.True(mean > 0, $"expected positive EqAD for the one sided category, got {mean}");
            Assert.True(mean < loneHist.SampleMean, $"expected EqAD {mean} below the single year mean {loneHist.SampleMean}");
        }

        [Theory]
        //base year, future year, period of analysis
        [InlineData(2023, 2072, 50)] //span exactly fills the period of analysis
        [InlineData(2023, 2047, 50)] //span well inside the period of analysis
        [InlineData(2023, 2024, 50)] //minimum valid span
        public void TryValidateAnalysisYears_Succeeds_WhenYearsFitThePeriodOfAnalysis(int baseYear, int futureYear, int periodOfAnalysis)
        {
            OperationResult result = Alternative.TryValidateAnalysisYears(baseYear, futureYear, periodOfAnalysis);
            Assert.True(result.Result);
        }

        [Theory]
        [InlineData(2023, 2073, 50)] //one year past the inclusive span a 50 year period allows
        [InlineData(2030, 2100, 50)] //71 year span exceeds a 50 year period of analysis
        [InlineData(2023, 2023, 50)] //base and future year are the same
        [InlineData(2072, 2023, 50)] //future year is before the base year
        [InlineData(2023, 2024, 1)]  //period of analysis is too short for any alternative
        public void TryValidateAnalysisYears_ExplainsTheProblem_WhenYearsDoNotFit(int baseYear, int futureYear, int periodOfAnalysis)
        {
            OperationResult result = Alternative.TryValidateAnalysisYears(baseYear, futureYear, periodOfAnalysis);
            Assert.False(result.Result);
        }

        [Fact]
        public void AnnualizationCompute_ThrowsDescriptiveException_WhenYearsExceedPeriodOfAnalysis()
        {
            int baseYear = 2030;
            int futureYear = 2100;
            int periodOfAnalysis = 50;

            InvalidAnalysisYearsException ex = Assert.Throws<InvalidAnalysisYearsException>(() => Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: periodOfAnalysis, alternativeResultsID: 1,
                computedResultsBaseYear: new ScenarioResults(), computedResultsFutureYear: new ScenarioResults(),
                baseYear: baseYear, futureYear: futureYear));

            //the message has to name the values so the user knows what to change
            Assert.Contains(baseYear.ToString(), ex.Message);
            Assert.Contains(futureYear.ToString(), ex.Message);
            Assert.Contains(periodOfAnalysis.ToString(), ex.Message);
        }

        [Fact]
        public void AnnualizationCompute_ReturnsNull_WhenNeitherScenarioHasResults()
        {
            //the doc contract promises null here; before the guard was made reachable this threw a
            //NullReferenceException out of the identical-scenario branch instead
            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: 1,
                computedResultsBaseYear: null, computedResultsFutureYear: null,
                baseYear: 2030, futureYear: 2050);

            Assert.Null(results);
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

        /// <summary>
        /// Regression test for the exceedance/non-exceedance quartile swap bug.
        /// ExceededWithProbabilityQ(0.75) should return the 25th percentile (small value),
        /// and ExceededWithProbabilityQ(0.25) should return the 75th percentile (large value).
        /// Previously, ScenarioResults.ConsequencesExceededWithProbabilityQ was missing the
        /// 1-p conversion, causing quartiles to be swapped in certain code paths.
        /// </summary>
        [Fact]
        public void ExceededWithProbabilityQ_HigherExceedance_ReturnsLowerValue()
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 100, maxIterations: 100);

            // Create a histogram with values [1000, 1001, ..., 1099] — has clear spread for distinct quartiles
            var damageHist = new DynamicHistogram(Enumerable.Range(1000, 100).Select(i => (double)i).ToList(), cc);
            var damage = new AggregatedConsequencesBinned(damCat, assetCat, damageHist, impactAreaID, ConsequenceType.Damage, RiskType.Fail);

            var impactArea = new ImpactAreaScenarioResults(impactAreaID);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(damage);
            var scenarioResults = new ScenarioResults();
            scenarioResults.AddResults(impactArea);

            // ScenarioResults path (the fixed method)
            double exceededWith25Pct = scenarioResults.ConsequencesExceededWithProbabilityQ(0.25, impactAreaID, damCat, assetCat);
            double exceededWith75Pct = scenarioResults.ConsequencesExceededWithProbabilityQ(0.75, impactAreaID, damCat, assetCat);

            // Value exceeded with 25% probability = 75th percentile = LARGE
            // Value exceeded with 75% probability = 25th percentile = SMALL
            Assert.True(exceededWith25Pct > exceededWith75Pct,
                $"The value exceeded with 25% probability ({exceededWith25Pct}) should be greater than " +
                $"the value exceeded with 75% probability ({exceededWith75Pct}). " +
                $"If these are swapped, the 1-p conversion in ScenarioResults may be missing.");

            // Also verify via AlternativeResults (ScenariosAreIdentical path) for consistency
            AlternativeResults altResults = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                computedResultsBaseYear: scenarioResults, computedResultsFutureYear: null,
                baseYear: 2023, futureYear: 2072);

            double altExceededWith25Pct = altResults.EqadExceededWithProbabilityQ(0.25, impactAreaID, damCat, assetCat);
            double altExceededWith75Pct = altResults.EqadExceededWithProbabilityQ(0.75, impactAreaID, damCat, assetCat);

            Assert.True(altExceededWith25Pct > altExceededWith75Pct,
                $"AlternativeResults (ScenariosAreIdentical path): value exceeded with 25% probability ({altExceededWith25Pct}) " +
                $"should be greater than value exceeded with 75% probability ({altExceededWith75Pct}).");
        }

        /// <summary>
        /// Two configurations of the same curves produce the same EAD in both analysis years, and discounting a
        /// flat stream of damages back into equivalent annual terms is an identity operation. ComputeEqad is
        /// linear in (base, future) with weights that sum to one, so EqAD must reproduce that EAD exactly -
        /// mean, aggregate, and quantile for quantile.
        ///
        /// The scenarios below carry identical damages but differing threshold values, so ScenarioResults.Equals
        /// reports them as distinct and AnnualizationCompute runs the real discounting routine. Without that the
        /// identical-scenarios shortcut copies base year EAD across and the assertions prove nothing, which is
        /// why each test guards on ScenariosAreIdentical first.
        /// </summary>
        [Fact]
        public void MeanEqadEqualsMeanEad_ForEveryDamageCategoryAndInTotal()
        {
            ScenarioResults baseYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 10);
            ScenarioResults futureYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 20);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseYearResults, futureYearResults, baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);
            Assert.False(results.ScenariosAreIdentical,
                "This test must exercise the discounting routine, not the identical-scenarios shortcut.");

            foreach (AggregatedConsequencesBinned consequence in baseYearResults.ResultsList
                         .SelectMany(r => r.ConsequenceResults.ConsequenceResultList))
            {
                double ead = consequence.ConsequenceHistogram.SampleMean;
                double eqad = results.SampleMeanEqad(impactAreaID, consequence.DamageCategory, consequence.AssetCategory);

                AssertRelativelyEqual(ead, eqad, 1e-9,
                    $"Mean EqAD for {consequence.DamageCategory}/{consequence.AssetCategory}");
            }

            //aggregated over everything, which is the figure the alternative results summary surfaces
            double totalEad = baseYearResults.SampleMeanExpectedAnnualConsequences(riskType: RiskType.Total);
            AssertRelativelyEqual(totalEad, results.SampleMeanEqad(), 1e-9, "Total mean EqAD");
        }

        [Fact]
        public void EqadDistributionEqualsEadDistribution_AcrossExceedanceProbabilities()
        {
            ScenarioResults baseYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 10);
            ScenarioResults futureYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 20);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseYearResults, futureYearResults, baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);
            Assert.False(results.ScenariosAreIdentical,
                "This test must exercise the discounting routine, not the identical-scenarios shortcut.");

            double[] exceedanceProbabilities = [0.99, 0.9, 0.75, 0.5, 0.25, 0.1, 0.04, 0.01];

            foreach (double exceedanceProbability in exceedanceProbabilities)
            {
                double ead = baseYearResults.ConsequencesExceededWithProbabilityQ(
                    exceedanceProbability, impactAreaID, riskType: RiskType.Total);
                double eqad = results.EqadExceededWithProbabilityQ(exceedanceProbability, impactAreaID);

                AssertRelativelyEqual(ead, eqad, 1e-3,
                    $"EqAD exceeded with probability {exceedanceProbability}");
            }
        }

        [Fact]
        public void EqadEmpiricalMatchesBaseYearEadEmpirical_QuantileForQuantile()
        {
            ScenarioResults baseYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 10);
            ScenarioResults futureYearResults = BuildScenarioResultsWithFlatDamages(thresholdValue: 20);

            AlternativeResults results = Alternative.AnnualizationCompute(
                discountRate: 0.0275, periodOfAnalysis: 50, alternativeResultsID: alternativeID,
                baseYearResults, futureYearResults, baseYear: 2023, futureYear: 2072);

            Assert.NotNull(results);
            Assert.False(results.ScenariosAreIdentical,
                "This test must exercise the discounting routine, not the identical-scenarios shortcut.");

            Empirical eadDistribution = results.GetBaseYearEADDistribution(impactAreaID);
            Empirical eqadDistribution = results.GetEqadDistribution(impactAreaID);

            AssertRelativelyEqual(eadDistribution.SampleMean, eqadDistribution.SampleMean, 1e-9,
                "Sample mean of the aggregated distribution");

            for (int i = 1; i < 100; i++)
            {
                double nonExceedanceProbability = i / 100.0;
                AssertRelativelyEqual(
                    eadDistribution.InverseCDF(nonExceedanceProbability),
                    eqadDistribution.InverseCDF(nonExceedanceProbability),
                    1e-3,
                    $"Quantile at non-exceedance probability {nonExceedanceProbability}");
            }
        }

        /// <summary>
        /// The damages are the same in every call; only the threshold value varies, which is what keeps
        /// ScenarioResults.Equals from short circuiting the discounting routine.
        /// </summary>
        private static ScenarioResults BuildScenarioResultsWithFlatDamages(double thresholdValue)
        {
            ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1000, maxIterations: 1000);

            var impactArea = new ImpactAreaScenarioResults(impactAreaID);
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "structure",
                    new DynamicHistogram(SkewedConsequenceSample(600000), cc),
                    impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("residential", "content",
                    new DynamicHistogram(SkewedConsequenceSample(200000), cc),
                    impactAreaID, ConsequenceType.Damage, RiskType.Fail));
            impactArea.ConsequenceResults.AddExistingConsequenceResultObject(
                new AggregatedConsequencesBinned("commercial", "structure",
                    new DynamicHistogram(SkewedConsequenceSample(122493), cc),
                    impactAreaID, ConsequenceType.Damage, RiskType.Fail));

            impactArea.PerformanceByThresholds.AddThreshold(
                new Threshold(1, cc, ThresholdEnum.AdditionalExteriorStage, thresholdValue));

            var scenarioResults = new ScenarioResults();
            scenarioResults.AddResults(impactArea);
            return scenarioResults;
        }

        /// <summary>
        /// Skewed sample with a mean of <paramref name="mean"/>. Exponential quantiles give the long right tail
        /// real EAD distributions have, and being quantile based rather than sampled it is deterministic, so
        /// both analysis years receive identical inputs.
        /// </summary>
        private static List<double> SkewedConsequenceSample(double mean, int count = 10000)
        {
            var sample = new List<double>(count);
            for (int i = 0; i < count; i++)
            {
                double nonExceedanceProbability = (i + 0.5) / count;
                sample.Add(-mean * Math.Log(1 - nonExceedanceProbability));
            }
            return sample;
        }

        private static void AssertRelativelyEqual(double expected, double actual, double relativeTolerance, string what)
        {
            double error = expected == 0 ? Math.Abs(actual) : Math.Abs((actual - expected) / expected);
            Assert.True(error <= relativeTolerance,
                $"{what}: expected {expected:N4}, actual {actual:N4}, relative error {error:P4} exceeds {relativeTolerance:P4}.");
        }
    }
}
