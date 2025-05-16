using Statistics;
using System;
using System.Collections.Generic;
using HEC.FDA.Model.metrics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using Utility.Progress;
using Statistics.Histograms;
namespace HEC.FDA.Model.alternatives
{
    public static class Alternative
    {
        /// <summary>
        /// Computes annualized consequences for an alternative using distributions of EAD from base and future year scenarios.
        /// Returns an AlternativeResults object containing AAEQ damage for each damage category, asset category, and impact area combination.
        /// </summary>
        /// <param name="discountRate">Discount rate in decimal form.</param>
        /// <param name="periodOfAnalysis">Number of years in the analysis period.</param>
        /// <param name="alternativeResultsID">Identifier for the alternative results.</param>
        /// <param name="computedResultsBaseYear">Scenario results for the base year.</param>
        /// <param name="computedResultsFutureYear">Scenario results for the future year.</param>
        /// <param name="baseYear">Base year of analysis.</param>
        /// <param name="futureYear">Future year of analysis.</param>
        /// <param name="reporter">Optional progress reporter.</param>
        /// <returns>AlternativeResults containing AAEQ damages, or null if parameters are invalid.</returns>
        public static AlternativeResults AnnualizationCompute(
            double discountRate,
            int periodOfAnalysis,
            int alternativeResultsID,
            ScenarioResults computedResultsBaseYear,
            ScenarioResults computedResultsFutureYear,
            int baseYear,
            int futureYear,
            ProgressReporter reporter = null)
        {
            reporter ??= ProgressReporter.None();
            reporter.ReportMessage("Starting alternative compute.");

            var analysisYears = new List<int> { baseYear, futureYear };

            if (!CanCompute(baseYear, futureYear, periodOfAnalysis))
            {
                reporter.ReportMessage(new Utility.Logging.Message("The discounting parameters are not valid, discounting routine aborted."));
                return null;
            }
            return RunAnnualizationCompute(analysisYears, discountRate, periodOfAnalysis, alternativeResultsID, computedResultsBaseYear, computedResultsFutureYear, reporter);
        }

        private static AlternativeResults RunAnnualizationCompute(
            List<int> analysisYears,
            double discountRate,
            int periodOfAnalysis,
            int alternativeResultsID,
            ScenarioResults computedResultsBaseYear,
            ScenarioResults computedResultsFutureYear,
            ProgressReporter reporter)
        {
            var alternativeResults = new AlternativeResults(alternativeResultsID, analysisYears, periodOfAnalysis);
            reporter.ReportMessage(new Utility.Logging.Message("Initiating discounting routine."));

            alternativeResults.BaseYearScenarioResults = computedResultsBaseYear;
            alternativeResults.FutureYearScenarioResults = computedResultsFutureYear;

            if (computedResultsBaseYear.Equals(computedResultsFutureYear))
            {
                alternativeResults.ScenariosAreIdentical = true;
                alternativeResults.AAEQDamageResults = ScenarioResults.ConvertToStudyAreaConsequencesByQuantile(computedResultsBaseYear);
            }
            else
            {
                var futureYearResultsList = new List<ImpactAreaScenarioResults>(computedResultsFutureYear.ResultsList);

                ProcessBaseAndFutureYearScenarioResults(
                    analysisYears,
                    discountRate,
                    periodOfAnalysis,
                    computedResultsBaseYear,
                    computedResultsFutureYear,
                    alternativeResults,
                    futureYearResultsList,
                    reporter);

                if (futureYearResultsList.Count > 0)
                {
                    ProcessUnmatchedFutureResults(
                        analysisYears,
                        discountRate,
                        periodOfAnalysis,
                        computedResultsBaseYear,
                        alternativeResults,
                        futureYearResultsList);
                }
            }

            reporter.ReportMessage("Compute Complete");
            reporter.ReportProgressFraction(1);
            return alternativeResults;
        }

        private static void ProcessBaseAndFutureYearScenarioResults(
        List<int> analysisYears,
        double discountRate,
        int periodOfAnalysis,
        ScenarioResults baseYearResults,
        ScenarioResults futureYearResults,
        AlternativeResults alternativeResults,
        List<ImpactAreaScenarioResults> unprocessedFutureResults,
        ProgressReporter reporter)
        {
            foreach (ImpactAreaScenarioResults baseYearImpactArea in baseYearResults.ResultsList)
            {
                // Get corresponding future year results for this impact area
                // (May be null in cases like managed retreat where damage exists in base year but not future)
                ImpactAreaScenarioResults futureYearImpactArea = futureYearResults.GetResults(baseYearImpactArea.ImpactAreaID);

                // Mark this future year result as processed
                unprocessedFutureResults.Remove(futureYearImpactArea);

                // Copy list to track which future consequence results have been processed
                var unprocessedFutureConsequences = new List<AggregatedConsequencesBinned>(
                    futureYearImpactArea.ConsequenceResults.ConsequenceResultList);

                // Process consequences that exist in base year
                foreach (AggregatedConsequencesBinned baseYearConsequence in baseYearImpactArea.ConsequenceResults.ConsequenceResultList)
                {
                    // Find matching future year result for this damage/asset category combination
                    AggregatedConsequencesBinned futureYearConsequence = futureYearImpactArea.ConsequenceResults.GetConsequenceResult(
                        baseYearConsequence.DamageCategory,
                        baseYearConsequence.AssetCategory,
                        baseYearConsequence.RegionID);

                    // Calculate AAEQ (Average Annual Equivalent) result
                    AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(
                        baseYearConsequence,
                        futureYearConsequence,
                        analysisYears[0],
                        analysisYears[1],
                        periodOfAnalysis,
                        discountRate,
                        false,
                        reporter);

                    // Mark this future year consequence as processed
                    unprocessedFutureConsequences.Remove(futureYearConsequence);

                    // Add the AAEQ result to our alternative
                    alternativeResults.AddConsequenceResults(aaeqResult);
                }

                // Process any future year consequences that didn't have matching base year results
                foreach (AggregatedConsequencesBinned futureYearConsequence in unprocessedFutureConsequences)
                {
                    // Try to get base year result (likely null if we reached this point)
                    AggregatedConsequencesBinned baseYearConsequence = baseYearImpactArea.ConsequenceResults.GetConsequenceResult(
                        futureYearConsequence.DamageCategory,
                        futureYearConsequence.AssetCategory,
                        futureYearConsequence.RegionID);

                    // Calculate AAEQ with assumed zero damage in base year if no base year result exists
                    AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(
                        baseYearConsequence,
                        futureYearConsequence,
                        analysisYears[0],
                        analysisYears[1],
                        periodOfAnalysis,
                        discountRate,
                        false,
                        reporter);

                    alternativeResults.AddConsequenceResults(aaeqResult);
                }
            }
        }


        private static void ProcessUnmatchedFutureResults(
            List<int> analysisYears,
            double discountRate,
            int periodOfAnalysis,
            ScenarioResults computedResultsBaseYear,
            AlternativeResults alternativeResults,
            List<ImpactAreaScenarioResults> futureYearResultsList)
        {
            foreach (var futureYearResults in futureYearResultsList)
            {
                var baseYearResults = computedResultsBaseYear.GetResults(futureYearResults.ImpactAreaID);
                var baseYearDamageResultsList = new List<AggregatedConsequencesBinned>(baseYearResults.ConsequenceResults.ConsequenceResultList);

                foreach (var futureYearDamageResult in futureYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    var baseYearDamageResult = baseYearResults.ConsequenceResults.GetConsequenceResult(
                        futureYearDamageResult.DamageCategory,
                        futureYearDamageResult.AssetCategory,
                        futureYearDamageResult.RegionID);

                    var aaeqResult = IterateOnAAEQ(
                        baseYearDamageResult,
                        futureYearDamageResult,
                        analysisYears[0],
                        analysisYears[1],
                        periodOfAnalysis,
                        discountRate);

                    baseYearDamageResultsList.Remove(baseYearDamageResult);
                    alternativeResults.AddConsequenceResults(aaeqResult);
                }

                if (baseYearDamageResultsList.Count > 0)
                {
                    foreach (var baseYearDamageResult in baseYearDamageResultsList)
                    {
                        var futureYearDamageResult = futureYearResults.ConsequenceResults.GetConsequenceResult(
                            baseYearDamageResult.DamageCategory,
                            baseYearDamageResult.AssetCategory,
                            baseYearDamageResult.RegionID);

                        var aaeqResult = IterateOnAAEQ(
                            baseYearDamageResult,
                            futureYearDamageResult,
                            analysisYears[0],
                            analysisYears[1],
                            periodOfAnalysis,
                            discountRate,
                            false);

                        alternativeResults.AddConsequenceResults(aaeqResult);

                        throw new Exception("The alternative compute reached an illogical stream of combinations. The alternative compute was aborted");
                    }
                }
            }
        }


        private static bool CanCompute(int baseYear, int futureYear, int periodOfAnalysis)
        {
            bool canCompute = true;
            if (baseYear > futureYear)
            {
                canCompute = false;
            }
            int differenceBetweenBaseAndFutureYearInclusive = futureYear - baseYear + 1;
            if (differenceBetweenBaseAndFutureYearInclusive < 2)
            {
                canCompute = false;
            }
            if (differenceBetweenBaseAndFutureYearInclusive > periodOfAnalysis)
            {
                canCompute = false;
            }
            return canCompute;
        }

        private static AggregatedConsequencesByQuantile IterateOnAAEQ(
        AggregatedConsequencesBinned baseYearDamageResult,
        AggregatedConsequencesBinned mlfYearDamageResult,
        int baseYear,
        int futureYear,
        int periodOfAnalysis,
        double discountRate,
        bool iterateOnFutureYear = true,
        ProgressReporter reporter = null)
        {
            reporter ??= ProgressReporter.None();

            var convergenceCriteria = iterateOnFutureYear
                ? mlfYearDamageResult.ConvergenceCriteria
                : baseYearDamageResult.ConvergenceCriteria;

            int probabilitySteps = 2500;
            var resultCollection = new ConcurrentBag<double>();

            Parallel.For(0, probabilitySteps, i =>
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;
                double eadSampledBaseYear = baseYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);
                double eadSampledFutureYear = mlfYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);
                double aaeqDamage = ComputeEEAD(eadSampledBaseYear, baseYear, eadSampledFutureYear, futureYear, periodOfAnalysis, discountRate);
                resultCollection.Add(aaeqDamage);
            });

            var damageCategory = iterateOnFutureYear ? mlfYearDamageResult.DamageCategory : baseYearDamageResult.DamageCategory;
            var assetCategory = iterateOnFutureYear ? mlfYearDamageResult.AssetCategory : baseYearDamageResult.AssetCategory;
            var regionID = iterateOnFutureYear ? mlfYearDamageResult.RegionID : baseYearDamageResult.RegionID;

            DynamicHistogram dynamicHistogram = new(resultCollection.ToList(), new());
            AggregatedConsequencesByQuantile ret = new AggregatedConsequencesByQuantile(damageCategory, assetCategory, [.. resultCollection], regionID);
            return ret;
        }

        //TODO: these functions should be private, but currently have unit tests 
        //so these will remain public until the unit tests are re-written on the above public method
        public static double ComputeEEAD(double baseYearEAD, int baseYear, double mostLikelyFutureEAD, int mostLikelyFutureYear, int periodOfAnalysis, double discountRate)
        {
            //probably instantiate a rng to seed each impact area differently
            double[] interpolatedEADs = Interpolate(baseYearEAD, mostLikelyFutureEAD, baseYear, mostLikelyFutureYear, periodOfAnalysis);
            double sumPresentValueEAD = PresentValueCompute(interpolatedEADs, discountRate);
            double averageAnnualEquivalentDamage = IntoAverageAnnualEquivalentTerms(sumPresentValueEAD, periodOfAnalysis, discountRate);
            return averageAnnualEquivalentDamage;
        }
        private static double IntoAverageAnnualEquivalentTerms(double sumPresentValueEAD, int periodOfAnalysis, double discountRate)
        {
            double presentValueInterestFactorOfAnnuity = (1 - 1 / Math.Pow(1 + discountRate, periodOfAnalysis)) / discountRate;
            double averageAnnualEquivalentDamage = sumPresentValueEAD / presentValueInterestFactorOfAnnuity;
            return averageAnnualEquivalentDamage;
        }
        private static double PresentValueCompute(double[] interpolatedEADs, double discountRate)
        {
            int periodOfAnalysis = interpolatedEADs.Length;
            double[] presentValueInterestFactor = new double[periodOfAnalysis];
            double sumPresentValueEAD = 0;
            for (int i = 0; i < periodOfAnalysis; i++)
            {
                presentValueInterestFactor[i] = 1 / Math.Pow(1 + discountRate, i + 1);
                sumPresentValueEAD += interpolatedEADs[i] * presentValueInterestFactor[i];
            }
            return sumPresentValueEAD;
        }
        private static double[] Interpolate(double baseYearEAD, double mostLikelyFutureEAD, int baseYear, int mostLikelyFutureYear, int periodOfAnalysis)
        {
            int yearsBetweenBaseAndMLFInclusive = mostLikelyFutureYear - baseYear + 1;
            double[] interpolatedEADs = new double[periodOfAnalysis];
            interpolatedEADs[0] = baseYearEAD;
            for (int i = 0; i < yearsBetweenBaseAndMLFInclusive; i++)
            {
                interpolatedEADs[i] = baseYearEAD + (mostLikelyFutureEAD - baseYearEAD) * ((double)i / (yearsBetweenBaseAndMLFInclusive - 1));
            }
            for (int i = yearsBetweenBaseAndMLFInclusive; i < periodOfAnalysis; i++)
            {
                interpolatedEADs[i] = mostLikelyFutureEAD;
            }
            return interpolatedEADs;
        }
    }
}
