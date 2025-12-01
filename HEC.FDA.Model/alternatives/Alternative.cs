using Statistics;
using System;
using System.Collections.Generic;
using HEC.FDA.Model.metrics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using Utility.Progress;
using Statistics.Histograms;
using Statistics.Distributions;
namespace HEC.FDA.Model.alternatives
{
    public static class Alternative
    {
        /// <summary>
        /// Computes annualized consequences for an alternative using distributions of EAD from base and future year scenarios.
        /// Returns an AlternativeResults object containing EqAD damage for each damage category, asset category, and impact area combination.
        /// </summary>
        /// <param name="discountRate">Discount rate in decimal form.</param>
        /// <param name="periodOfAnalysis">Number of years in the analysis period.</param>
        /// <param name="alternativeResultsID">Identifier for the alternative results.</param>
        /// <param name="computedResultsBaseYear">Scenario results for the base year.</param>
        /// <param name="computedResultsFutureYear">Scenario results for the future year.</param>
        /// <param name="baseYear">Base year of analysis.</param>
        /// <param name="futureYear">Future year of analysis.</param>
        /// <param name="reporter">Optional progress reporter.</param>
        /// <returns>AlternativeResults containing EqAD damages, or null if parameters are invalid.</returns>
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

            //if scenarios are identical, no need to compute, just use one scenario
            if (computedResultsBaseYear.Equals(computedResultsFutureYear))
            {
                alternativeResults.ScenariosAreIdentical = true;
                alternativeResults.EqadResults = ScenarioResults.ConvertToStudyAreaConsequencesByQuantile(computedResultsBaseYear);
            }
            else
            {
                //To keep track of which results have yet to be processed
                //I think this allows us to handle situations where we have uneven numbers of results 
                var futureYearResultsList = new List<ImpactAreaScenarioResults>(computedResultsFutureYear.ResultsList);

                //Iterate through the base year and future year Scenario Results simultaneously  
                //There will be one base year results for each impact area in the impact area set
                ProcessBaseAndFutureYearScenarioResults(
                    analysisYears,
                    discountRate,
                    periodOfAnalysis,
                    computedResultsBaseYear,
                    computedResultsFutureYear,
                    alternativeResults,
                    futureYearResultsList,
                    reporter);

                //UNLIKELY TO HIT THIS CODE 
                //in case there future year impact area scenario results that did not match to any base year impact area scenario results
                //in other words, in case there is no damage in a particular impact area in the base year but there is damage in the future year 
                //or vice versa, such as with managed retreat 
                if (futureYearResultsList.Count > 0)
                {
                    throw new Exception("Unmatched future results not properly accounted for");
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

                    // Calculate EqAD (Equivalent Annual Damage) result
                    AggregatedConsequencesByQuantile eqadResult = IterateOnEqad(
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

                    // Add the EqAD result to our alternative
                    alternativeResults.AddConsequenceResults(eqadResult);
                }

                // Process any future year consequences that didn't have matching base year results
                foreach (AggregatedConsequencesBinned futureYearConsequence in unprocessedFutureConsequences)
                {
                    // Try to get base year result (likely null if we reached this point)
                    AggregatedConsequencesBinned baseYearConsequence = baseYearImpactArea.ConsequenceResults.GetConsequenceResult(
                        futureYearConsequence.DamageCategory,
                        futureYearConsequence.AssetCategory,
                        futureYearConsequence.RegionID);

                    // Calculate EqAD with assumed zero damage in base year if no base year result exists
                    AggregatedConsequencesByQuantile eqadResult = IterateOnEqad(
                        baseYearConsequence,
                        futureYearConsequence,
                        analysisYears[0],
                        analysisYears[1],
                        periodOfAnalysis,
                        discountRate,
                        true,
                        reporter);

                    alternativeResults.AddConsequenceResults(eqadResult);
                }
            }
        }

        private static bool CanCompute(int baseYear, int futureYear, int periodOfAnalysis)
        {
            int difference = futureYear - baseYear + 1;
            return baseYear <= futureYear
                && difference >= 2
                && difference <= periodOfAnalysis;
        }

        private static AggregatedConsequencesByQuantile IterateOnEqad(
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

            int probabilitySteps = 25000;
            var resultCollection = new ConcurrentBag<double>();
            //We compute the nth quantile of EqAD from the nth quantile of the base year EAD distribution and the nth quantile of the most likely future year EAD distribution, where n is determined by walking through 25,000 probability steps."
            // ex. we take the EAD from base at .01, the EAD from future at .01 and compute the EAD for .01. 
            //We then take that value and add it to the result collection. Using that collection as the sample, we fit an empirical distribution to it. 

            //The mean is done separately, because the SampleMeans of the Consequence histograms may not be the mean of those histograms. Sample mean is propograted from the original sample data. 
            Parallel.For(0, probabilitySteps, i =>
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;
                double eadSampledBaseYear = baseYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);
                double eadSampledFutureYear = mlfYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);
                double eqad = ComputeEqad(eadSampledBaseYear, baseYear, eadSampledFutureYear, futureYear, periodOfAnalysis, discountRate);
                resultCollection.Add(eqad);
            });

            //compute sample mean EqAD 
            double eadSampleMeanBase = baseYearDamageResult.ConsequenceHistogram.SampleMean;
            double eadSampleMeanFuture = mlfYearDamageResult.ConsequenceHistogram.SampleMean;
            double meanEqad = ComputeEqad(eadSampleMeanBase, baseYear, eadSampleMeanFuture, futureYear, periodOfAnalysis, discountRate);
            // end

            var damageCategory = iterateOnFutureYear ? mlfYearDamageResult.DamageCategory : baseYearDamageResult.DamageCategory;
            var assetCategory = iterateOnFutureYear ? mlfYearDamageResult.AssetCategory : baseYearDamageResult.AssetCategory;
            var regionID = iterateOnFutureYear ? mlfYearDamageResult.RegionID : baseYearDamageResult.RegionID;
            var resultList = resultCollection.ToList();

            Empirical empResult = Empirical.FitToSample(resultList);
            empResult.SampleMean = meanEqad;
            AggregatedConsequencesByQuantile result = new(damageCategory, assetCategory, empResult, regionID);
            return result;
        }

        //TODO: these functions should be private, but currently have unit tests 
        //so these will remain public until the unit tests are re-written on the above public method
        public static double ComputeEqad(double baseYearEAD, int baseYear, double mostLikelyFutureEAD, int mostLikelyFutureYear, int periodOfAnalysis, double discountRate)
        {
            //probably instantiate a rng to seed each impact area differently
            double[] interpolatedEADs = Interpolate(baseYearEAD, mostLikelyFutureEAD, baseYear, mostLikelyFutureYear, periodOfAnalysis);
            double sumPresentValueEAD = PresentValueCompute(interpolatedEADs, discountRate);
            double eqad = IntoEquivalentAnnualTerms(sumPresentValueEAD, periodOfAnalysis, discountRate);
            return eqad;
        }
        private static double IntoEquivalentAnnualTerms(double sumPresentValueEAD, int periodOfAnalysis, double discountRate)
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
