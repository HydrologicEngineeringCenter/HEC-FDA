using Statistics;
using System;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.FDA.Model.metrics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using Utility.Progress;
namespace HEC.FDA.Model.alternatives
{
    public class Alternative
    {

        #region Methods 
        /// <summary>
        /// TODO TODO TODO TODO
        /// This method is so god awfully complicated because we do not compute at the structure level 
        /// If we computed everything at the structure level, we wouldn't have to keep track of so many dimensions at the same time 
        /// Those dimensions will not vary as we run through the heart of the calculations like those in this class. 
        /// TODO TODO TODO TODO
        /// 
        /// Annualization Compute takes the distributions of EAD in each of the Scenarios for a given Alternative and returns a 
        /// ConsequenceResults object with a ConsequenceResult that holds a ThreadsafeInlineHistogram of AAEQ damage for each damage category, asset category, impact area combination. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="discountRate"></param> Discount rate should be provided in decimal form.
        /// <param name="computedResultsBaseYear"<>/param> Previously computed Scenario results for the base year. Optionally, leave null and run scenario compute.  
        /// <param name="computedResultsFutureYear"<>/param> Previously computed Scenario results for the future year. Optionally, leave null and run scenario compute. 
        /// <returns></returns>
        /// 
        public AlternativeResults AnnualizationCompute(double discountRate, int periodOfAnalysis, int alternativeResultsID, ScenarioResults computedResultsBaseYear,
            ScenarioResults computedResultsFutureYear, int baseYear, int futureYear, ProgressReporter reporter = null)
        {
            if (reporter == null)
            {
                reporter = ProgressReporter.None();
            }
            reporter.ReportMessage("Starting alternative compute.");

            List<int> analysisYears = [baseYear, futureYear];

            if (CanCompute(baseYear, futureYear, periodOfAnalysis))
            {
                AlternativeResults alternativeResults = RunAnnualizationCompute(analysisYears, discountRate, periodOfAnalysis, alternativeResultsID, computedResultsBaseYear, computedResultsFutureYear, reporter);
                return alternativeResults;
            }
            else
            {
                Utility.Logging.Message message = new("The discounting parameters are not valid, discounting routine aborted.");
                reporter.ReportMessage(message);
                return null;
            }
        }

        private AlternativeResults RunAnnualizationCompute(List<int> analysisYears, double discountRate, int periodOfAnalysis, int alternativeResultsID, ScenarioResults computedResultsBaseYear, ScenarioResults computedResultsFutureYear, ProgressReporter reporter)
        {
            AlternativeResults alternativeResults = new(alternativeResultsID, analysisYears, periodOfAnalysis);
            MessageEventArgs messargs = new(new Message("Initiating discounting routine." + Environment.NewLine));
            alternativeResults.ReportMessage(this, messargs);

            alternativeResults.BaseYearScenarioResults = computedResultsBaseYear;
            alternativeResults.FutureYearScenarioResults = computedResultsFutureYear;

            //if scenarios are identical, no need to compute, just use one scenario 
            if (computedResultsBaseYear.Equals(computedResultsFutureYear))
            {
                alternativeResults.ScenariosAreIdentical = true;
                alternativeResults.AAEQDamageResults = ScenarioResults.ConvertToStudyAreaConsequencesByQuantile(computedResultsBaseYear);
            }
            else
            {
                //To keep track of which results have yet to be processed
                //I think this allows us to handle situations where we have uneven numbers of results 
                List<ImpactAreaScenarioResults> futureYearResultsList = [.. computedResultsFutureYear.ResultsList.Cast<ImpactAreaScenarioResults>()];

                //this quantity assumes uniformity of dimensionality 
                int integerQuantityImpactAreas = computedResultsBaseYear.ResultsList.Count;
                double quantityOfImpactAreas = Convert.ToDouble(integerQuantityImpactAreas);
                int integerQuantityDamCatAssetCatCombos = computedResultsBaseYear.ResultsList[0].ConsequenceResults.ConsequenceResultList.Count;
                double quantityOfDamageCatAssetCatCombinations = Convert.ToDouble(integerQuantityDamCatAssetCatCombos);
                double quantityOFDamCatAssetCatImpactAreaCombos = quantityOfImpactAreas * quantityOfDamageCatAssetCatCombinations;

                //Iterate through the base year and future year Scenario Results simultaneously  
                //There will be one base year results for each impact area in the impact area set
                ProcessBaseAndFutureYearScenarioResults(analysisYears, discountRate, periodOfAnalysis, computedResultsBaseYear, computedResultsFutureYear, alternativeResults, futureYearResultsList, quantityOFDamCatAssetCatImpactAreaCombos, reporter);

                //UNLIKELY TO HIT THIS CODE 
                //in case there future year impact area scenario results that did not match to any base year impact area scenario results
                //in other words, in case there is no damage in a particular impact area in the base year but there is damage in the future year 
                //or vice versa, such as with managed retreat 
                if (futureYearResultsList.Count > 0)
                {
                    ProcessUnmatchedFutureResults(analysisYears, discountRate, periodOfAnalysis, computedResultsBaseYear, alternativeResults, futureYearResultsList);
                }
                reporter.ReportProgressFraction(1);
            }
            return alternativeResults;
        }

        private double ProcessBaseAndFutureYearScenarioResults(List<int> analysisYears, double discountRate, int periodOfAnalysis, ScenarioResults computedResultsBaseYear,
            ScenarioResults computedResultsFutureYear, AlternativeResults alternativeResults, List<ImpactAreaScenarioResults> futureYearResultsList, double quantityOFDamCatAssetCatImpactAreaCombos, ProgressReporter reporter)
        {
            double progressTicker = 0;
            foreach (ImpactAreaScenarioResults baseYearResults in computedResultsBaseYear.ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                //Try to get the most likely future result for the impact area to which baseYearResults corresponds 
                //I must be able to handle null results in case there is damage in the base year but not in the most likely future year 
                //such as with managed retreat 
                ImpactAreaScenarioResults mlfYearResults = computedResultsFutureYear.GetResults(baseYearResults.ImpactAreaID);

                //to keep track that this particular most likely future year result has been processed 
                futureYearResultsList.Remove(mlfYearResults);

                //This is again to be able to handle uneven results 
                //In the case that there is not a matching ConsequenceDistributionResult between analysis years
                //This is feasbile - you could have commercial damage in the base year but none in the future year 
                List<AggregatedConsequencesBinned> mlfYearDamageResultsList = new();
                foreach (AggregatedConsequencesBinned mlfResult in mlfYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    mlfYearDamageResultsList.Add(mlfResult);
                }

                //iterate through the base year consequence distribution results
                //there will be one for each damage category asset category combination
                foreach (AggregatedConsequencesBinned baseYearDamageResult in baseYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    //see if we can find a matching most likely future year result for the particular damage category asset category combination
                    //we are iterating by impact area one level higher so the impact area ID should be guaranteed to match 
                    AggregatedConsequencesBinned mlfYearDamageResult = mlfYearResults.ConsequenceResults.GetConsequenceResult(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, baseYearDamageResult.RegionID);


                    //Translate the base year EAD distribution and future year EAD distribution into an AAEQ distribution
                    //I must be able to handle a null ConsequenceDistributionResult in this method to handle uneven results
                    //such as there being base year results for a particular damage category asset category combination but none for the future year
                    //that is unlikely but reasonable 
                    AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(baseYearDamageResult, mlfYearDamageResult, analysisYears[0], analysisYears[1], periodOfAnalysis, discountRate, false);

                    //to keep track of having processed most likely future year results 
                    //because there could be more most likely future year results than base year results 
                    mlfYearDamageResultsList.Remove(mlfYearDamageResult);

                    //our aaeq result is complete 
                    alternativeResults.AddConsequenceResults(aaeqResult);

                    //at this level we are reporting progress at the impact area - damage category - asset category level 
                    //this math is an estimate and depends on the dimensions
                    progressTicker += 1.0;
                    double progressRatio = progressTicker / quantityOFDamCatAssetCatImpactAreaCombos;
                    double progressPercent = progressRatio * 100.00;

                    reporter.ReportProgressFraction((float)progressRatio);
                }
                //in the event that there are more most likely future year results than base year results 
                //or simply most likely future year results that were not matched to any base year results 
                //so in this case there is damage for a particular damage category asset category in the future year that does not occur in the base year
                //this situation is unlikey but reasonable 
                if (mlfYearDamageResultsList.Count > 0)
                {
                    foreach (AggregatedConsequencesBinned mlfYearDamageResult in mlfYearDamageResultsList)
                    {
                        //Try to get the base year result
                        AggregatedConsequencesBinned baseYearDamageResult = baseYearResults.ConsequenceResults.GetConsequenceResult(mlfYearDamageResult.DamageCategory, mlfYearDamageResult.AssetCategory, mlfYearDamageResult.RegionID);
                        //I must be able to handle a null ConsequenceDistributionResult here. We are unlikely to have a baseYearDamageResult that matches the mlfYearDamageResult if we got to this point. 
                        //The assumption must be zero damage in the base year 
                        AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(baseYearDamageResult, mlfYearDamageResult, analysisYears[0], analysisYears[1], periodOfAnalysis, discountRate);

                        //our aaeq result is complete 
                        alternativeResults.AddConsequenceResults(aaeqResult);
                    }
                }
            }
            return progressTicker;
        }


        private void ProcessUnmatchedFutureResults(List<int> analysisYears, double discountRate, int periodOfAnalysis, ScenarioResults computedResultsBaseYear, AlternativeResults alternativeResults,
            List<ImpactAreaScenarioResults> futureYearResultsList)
        {
            foreach (ImpactAreaScenarioResults futureYearResults in futureYearResultsList.Cast<ImpactAreaScenarioResults>())
            {
                //get the baseYearResults for the same impact area as futureYearResults
                //this should be zero if we got to this point 
                //if that is guaranteed to be the case, then I don't think all of this computation is necessary 
                ImpactAreaScenarioResults baseYearResults = computedResultsBaseYear.GetResults(futureYearResults.ImpactAreaID);

                //keep track of baseYearResults in case we have any baseYearResults that are not matched to futureYearResults
                //seems unlikely if we expect baseYearResults to be zero 
                List<AggregatedConsequencesBinned> baseYearDamageResultsList = new(baseYearResults.ConsequenceResults.ConsequenceResultList);

                foreach (AggregatedConsequencesBinned futureYearDamageResult in futureYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    //we expect baseYearResults to be zero, so baseYearDamageResult should be zero, too
                    AggregatedConsequencesBinned baseYearDamageResult = baseYearResults.ConsequenceResults.GetConsequenceResult(futureYearDamageResult.DamageCategory, futureYearDamageResult.AssetCategory, futureYearDamageResult.RegionID);
                    //I must be able to handle a consequence distribution result with zero damage in this method 
                    //baseYearDamageResult is probably zero damage 
                    AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(baseYearDamageResult, futureYearDamageResult, analysisYears[0], analysisYears[1], periodOfAnalysis, discountRate);

                    //to keep track of base year damage results 
                    //in case there is a base year damage result that does not match with a future year damage result
                    //unlikely because we expect base year damage results to be zero 
                    baseYearDamageResultsList.Remove(baseYearDamageResult);

                    //the aaeq damage result is complete 
                    alternativeResults.AddConsequenceResults(aaeqResult);
                }

                //in case there were base year damage results that did not match any future year damage results 
                //this is unlikely because we expect baseYearResults to be zero 
                if (baseYearDamageResultsList.Count > 0)
                {
                    foreach (AggregatedConsequencesBinned baseYearDamageResult in baseYearDamageResultsList)
                    {
                        //try to get the future year result
                        //I think we will actually get the future year result here 
                        AggregatedConsequencesBinned futureYearDamageResult = futureYearResults.ConsequenceResults.GetConsequenceResult(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, baseYearDamageResult.RegionID);

                        //so what happens here - we have null base year result but we have a future year result? 
                        AggregatedConsequencesByQuantile aaeqResult = IterateOnAAEQ(baseYearDamageResult, futureYearDamageResult, analysisYears[0], analysisYears[1], periodOfAnalysis, discountRate, false);
                        alternativeResults.AddConsequenceResults(aaeqResult);

                        //I am concerned about our possibility of getting here. We need to wave a really big red flag if it happens. 
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
        /// <summary>
        /// Progress reporter added as optional to not change method signature. Should always be passed in.
        /// </summary>
        private AggregatedConsequencesByQuantile IterateOnAAEQ(AggregatedConsequencesBinned baseYearDamageResult, AggregatedConsequencesBinned mlfYearDamageResult, int baseYear, int futureYear, int periodOfAnalysis, double discountRate, bool iterateOnFutureYear = true, ProgressReporter reporter = null)
        {
            if (reporter == null)
            {
                reporter = ProgressReporter.None();
            }
            AggregatedConsequencesByQuantile aaeqResult = new();
            ConvergenceCriteria convergenceCriteria;
            if (iterateOnFutureYear)
            {
                convergenceCriteria = mlfYearDamageResult.ConvergenceCriteria;
                Utility.Logging.Message message = new($"Average annual equivalent damage compute for damage category {mlfYearDamageResult.DamageCategory}, asset category {mlfYearDamageResult.AssetCategory}, and impact area ID {mlfYearDamageResult.RegionID} has been initiated.");
                reporter.ReportMessage(message);
            }
            else
            {
                convergenceCriteria = baseYearDamageResult.ConvergenceCriteria;
                Utility.Logging.Message message = new($"Average annual equivalent damage compute for damage category {baseYearDamageResult.DamageCategory}, asset category {baseYearDamageResult.AssetCategory}, and impact area ID {baseYearDamageResult.RegionID} has been initiated.");
                reporter.ReportMessage(message);
            }
            var resultCollection = new ConcurrentBag<double>();

            int probabilitySteps = 2500;

            Parallel.For(0, probabilitySteps, i =>
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;

                //get base year EAD and frequency at probability step
                double eadSampledBaseYear = baseYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);

                //get future year EAD and frequency at probability step
                double eadSampledFutureYear = mlfYearDamageResult.ConsequenceHistogram.InverseCDF(probabilityStep);

                //calculate AAEQ at probability step
                double aaeqDamage = ComputeEEAD(eadSampledBaseYear, baseYear, eadSampledFutureYear, futureYear, periodOfAnalysis, discountRate);

                resultCollection.Add(aaeqDamage);

            }
            );
            if (iterateOnFutureYear)
            {
                aaeqResult = new AggregatedConsequencesByQuantile(mlfYearDamageResult.DamageCategory, mlfYearDamageResult.AssetCategory, resultCollection.ToList(), mlfYearDamageResult.RegionID);

            }
            else
            {
                aaeqResult = new AggregatedConsequencesByQuantile(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, resultCollection.ToList(), baseYearDamageResult.RegionID);
            }
            Utility.Logging.Message msg = new($"Average annual equivalent damage compute for damage category {aaeqResult.DamageCategory}, asset category {aaeqResult.AssetCategory}, and impact area ID {aaeqResult.RegionID} has completed.");
            reporter.ReportMessage(msg);
            return aaeqResult;
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
            double yearsBetweenBaseAndMLFInclusive = Convert.ToDouble(mostLikelyFutureYear - baseYear + 1);
            double[] interpolatedEADs = new double[periodOfAnalysis];
            interpolatedEADs[0] = baseYearEAD;
            for (int i = 0; i < yearsBetweenBaseAndMLFInclusive; i++)
            {
                interpolatedEADs[i] = baseYearEAD + (mostLikelyFutureEAD - baseYearEAD) * (i / (yearsBetweenBaseAndMLFInclusive - 1));
            }
            for (int i = Convert.ToInt32(yearsBetweenBaseAndMLFInclusive); i < periodOfAnalysis; i++)
            {
                interpolatedEADs[i] = mostLikelyFutureEAD;
            }
            return interpolatedEADs;
        }
        #endregion 
    }
}
