using compute;
using metrics;
using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace alternatives
{
    public class Alternative
    {
        /// <summary>
        /// Annualization Compute takes the distributions of EAD in each of the Scenarios for a given Alternative and returns a 
        /// ConsequenceResults object with a ConsequenceResult that holds a ThreadsafeInlineHistogram of AAEQ damage for each damage category, asset category, impact area combination. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="discountRate"></param> Discount rate should be provided in decimal form.
        /// <param name="computedResultsBaseYear"<>/param> Previously computed Scenario results for the base year. Optionally, leave null and run scenario compute.  
        /// <param name="computedResultsFutureYear"<>/param> Previously computed Scenario results for the future year. Optionally, leave null and run scenario compute. 
        /// <returns></returns>
        public static AlternativeResults AnnualizationCompute(interfaces.IProvideRandomNumbers randomProvider, double discountRate, int periodOfAnalysis, int alternativeResultsID, ScenarioResults computedResultsBaseYear, ScenarioResults computedResultsFutureYear)
        {
            int baseYear = computedResultsBaseYear.AnalysisYear;
            int futureYear = computedResultsFutureYear.AnalysisYear;
            List<int> analysisYears = new List<int>();
            analysisYears.Add(baseYear);
            analysisYears.Add(futureYear);
            AlternativeResults alternativeResults = new AlternativeResults(alternativeResultsID, analysisYears);
            alternativeResults.BaseYearScenarioResults = computedResultsBaseYear;
            alternativeResults.FutureYearScenarioResults = computedResultsFutureYear;

            List<IContainImpactAreaScenarioResults> futureYearResultsList = new List<IContainImpactAreaScenarioResults>();
            foreach (ImpactAreaScenarioResults futureYearImpactAreaScenarioResults in computedResultsFutureYear.ResultsList)
            {
                futureYearResultsList.Add(futureYearImpactAreaScenarioResults);
            }

            foreach (ImpactAreaScenarioResults baseYearResults in computedResultsBaseYear.ResultsList)
            {
                ImpactAreaScenarioResults mlfYearResults = computedResultsFutureYear.GetResults(baseYearResults.ImpactAreaID);
                futureYearResultsList.Remove(mlfYearResults);

                List<ConsequenceDistributionResult> mlfYearDamageResultsList = new List<ConsequenceDistributionResult>();
                foreach (ConsequenceDistributionResult mlfResult in mlfYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    mlfYearDamageResultsList.Add(mlfResult);
                }

                foreach (ConsequenceDistributionResult baseYearDamageResult in baseYearResults.ConsequenceResults.ConsequenceResultList)
                {
                    ConsequenceDistributionResult mlfYearDamageResult = mlfYearResults.ConsequenceResults.GetConsequenceResult(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, baseYearDamageResult.RegionID);
                    ConsequenceDistributionResult aaeqResult = IterateOnAAEQ(baseYearDamageResult, mlfYearDamageResult, baseYear, futureYear, periodOfAnalysis, discountRate, randomProvider, false);
                    mlfYearDamageResultsList.Remove(mlfYearDamageResult);
                    alternativeResults.AddConsequenceResults(aaeqResult);
                }
                if (mlfYearDamageResultsList.Count > 0)
                {
                    foreach (ConsequenceDistributionResult mlfYearDamageResult in mlfYearDamageResultsList)
                    {
                        ConsequenceDistributionResult baseYearDamageResult = baseYearResults.ConsequenceResults.GetConsequenceResult(mlfYearDamageResult.DamageCategory, mlfYearDamageResult.AssetCategory, mlfYearDamageResult.RegionID);
                        ConsequenceDistributionResult aaeqResult = IterateOnAAEQ(baseYearDamageResult, mlfYearDamageResult, baseYear, futureYear, periodOfAnalysis, discountRate, randomProvider);
                        alternativeResults.AddConsequenceResults(aaeqResult);
                    }
                }
            }
            if (futureYearResultsList.Count > 0)
            {

                foreach (ImpactAreaScenarioResults futureYearResults in futureYearResultsList)
                {
                    ImpactAreaScenarioResults baseYearResults = computedResultsBaseYear.GetResults(futureYearResults.ImpactAreaID);

                    List<ConsequenceDistributionResult> baseYearDamageResultsList = new List<ConsequenceDistributionResult>();
                    foreach (ConsequenceDistributionResult baseYearResult in baseYearResults.ConsequenceResults.ConsequenceResultList)
                    {
                        baseYearDamageResultsList.Add(baseYearResult);
                    }

                    foreach (ConsequenceDistributionResult futureYearDamageResult in futureYearResults.ConsequenceResults.ConsequenceResultList)
                    {
                        ConsequenceDistributionResult baseYearDamageResult = baseYearResults.ConsequenceResults.GetConsequenceResult(futureYearDamageResult.DamageCategory, futureYearDamageResult.AssetCategory, futureYearDamageResult.RegionID);
                        ConsequenceDistributionResult aaeqResult = IterateOnAAEQ(baseYearDamageResult, futureYearDamageResult, baseYear, futureYear, periodOfAnalysis, discountRate, randomProvider);
                        baseYearDamageResultsList.Remove(baseYearDamageResult);
                        alternativeResults.AddConsequenceResults(aaeqResult);
                    }
                    if (baseYearDamageResultsList.Count > 0)
                    {
                        foreach (ConsequenceDistributionResult baseYearDamageResult in baseYearDamageResultsList)
                        {
                            ConsequenceDistributionResult futureYearDamageResult = futureYearResults.ConsequenceResults.GetConsequenceResult(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, baseYearDamageResult.RegionID);
                            ConsequenceDistributionResult aaeqResult = IterateOnAAEQ(baseYearDamageResult, futureYearDamageResult, baseYear, futureYear, periodOfAnalysis, discountRate, randomProvider, false);
                            alternativeResults.AddConsequenceResults(aaeqResult);
                        }
                    }
                }
            }
            return alternativeResults;
        }

        private static ConsequenceDistributionResult IterateOnAAEQ(ConsequenceDistributionResult baseYearDamageResult, ConsequenceDistributionResult mlfYearDamageResult, int baseYear, int futureYear, int periodOfAnalysis, double discountRate, interfaces.IProvideRandomNumbers randomProvider, bool iterateOnFutureYear = true)
        {


            Histogram aaeqHistogram;//ThreadsafeInlineHistogram aaeqHistogram;
            ConsequenceDistributionResult aaeqResult;
            ConvergenceCriteria convergenceCriteria;
            if (iterateOnFutureYear)
            {
                convergenceCriteria = mlfYearDamageResult.ConvergenceCriteria;
                aaeqHistogram = new Histogram(20);// ThreadsafeInlineHistogram(convergenceCriteria);

                aaeqResult = new ConsequenceDistributionResult(mlfYearDamageResult.DamageCategory, mlfYearDamageResult.AssetCategory, aaeqHistogram, mlfYearDamageResult.RegionID);

            }
            else
            {
                convergenceCriteria = baseYearDamageResult.ConvergenceCriteria;
                aaeqHistogram = new Histogram(20);//ThreadsafeInlineHistogram(convergenceCriteria);
                aaeqResult = new ConsequenceDistributionResult(baseYearDamageResult.DamageCategory, baseYearDamageResult.AssetCategory, aaeqHistogram, baseYearDamageResult.RegionID);
            }
            int masterseed = 0;
            if (randomProvider is RandomProvider)
            {
                masterseed = randomProvider.Seed;
            }
            Int64 progressChunks = 1;
            Int64 _completedIterations = 0;
            Int64 _ExpectedIterations = convergenceCriteria.MaxIterations;
            if (_ExpectedIterations > 100)
            {
                progressChunks = _ExpectedIterations / 100;
            }
            Random masterSeedList = new Random(masterseed);//must be seeded.
            int[] seeds = new int[convergenceCriteria.MaxIterations];
            for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
            {
                seeds[i] = masterSeedList.Next();
            }
            Int64 iterations = convergenceCriteria.MinIterations;

            while (!aaeqResult.ConsequenceHistogram.IsConverged)
            {
                //Parallel.For(0, iterations, i =>
                for (int i = 0; i < iterations; i++)
                {
                    double eadSampledBaseYear = baseYearDamageResult.ConsequenceHistogram.InverseCDF(randomProvider.NextRandom());
                    double eadSampledFutureYear = mlfYearDamageResult.ConsequenceHistogram.InverseCDF(randomProvider.NextRandom());
                    double aaeqDamage = ComputeEEAD(eadSampledBaseYear, baseYear, eadSampledFutureYear, futureYear, periodOfAnalysis, discountRate);
                    aaeqResult.AddConsequenceRealization(aaeqDamage, i);
                    //Interlocked.Increment(ref _completedIterations);
                }
                //);
                if (!aaeqResult.ConsequenceHistogram.IsHistogramConverged(.95, .05))
                {
                    iterations = aaeqResult.ConsequenceHistogram.EstimateIterationsRemaining(.95, .05);
                    _ExpectedIterations = _completedIterations + iterations;
                    progressChunks = _ExpectedIterations / 100;
                }
                else
                {
                    iterations = 0;
                    break;
                }
            }
            aaeqResult.ConsequenceHistogram.ForceDeQueue();
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
            double presentValueInterestFactorOfAnnuity = (1 - (1 / Math.Pow(1 + discountRate, periodOfAnalysis))) / discountRate;
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
            double yearsBetweenBaseAndMLFInclusive = Convert.ToDouble(mostLikelyFutureYear - baseYear);
            double[] interpolatedEADs = new double[periodOfAnalysis];
            for (int i = 0; i < yearsBetweenBaseAndMLFInclusive; i++)
            {
                interpolatedEADs[i] = baseYearEAD + i * (1 / yearsBetweenBaseAndMLFInclusive) * (mostLikelyFutureEAD - baseYearEAD);
            }
            for (int i = Convert.ToInt32(yearsBetweenBaseAndMLFInclusive); i < periodOfAnalysis; i++)
            {
                interpolatedEADs[i] = mostLikelyFutureEAD;
            }
            return interpolatedEADs;
        }



    }
}