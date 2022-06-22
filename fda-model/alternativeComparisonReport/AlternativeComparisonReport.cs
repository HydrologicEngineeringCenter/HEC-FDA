using System;
using System.Collections.Generic;
using alternatives;
using Statistics.Histograms;
using metrics;
using Statistics;
using System.Linq;

namespace alternativeComparisonReport
{
    public class AlternativeComparisonReport
{
        private const int ITERATIONS = 50000;
        
        public static AlternativeComparisonReportResults ComputeAlternativeComparisonReport(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> aaeqResults = ComputeDistributionOfAAEQDamageReduced(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            List<ConsequenceDistributionResults> baseYearEADResults = ComputeDistributionEADReducedBaseYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            List<ConsequenceDistributionResults> futureYearEADResults = ComputeDistributionEADReducedFutureYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, aaeqResults, baseYearEADResults, futureYearEADResults);
        }
        private static List<ConsequenceDistributionResults> ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damagesReducedAllAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedOneAlternative = new ConsequenceDistributionResults(withProjectAlternativeResults.AlternativeID);

                foreach (ConsequenceDistributionResult withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                        IHistogram withProjectHistogram = withProjectDamageResult.ConsequenceHistogram;
                        IHistogram withoutProjectHistogram = withoutProjectAlternativeResults.GetAAEQDamageHistogram(withProjectDamageResult.RegionID,withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory);

                        double withProjectDamageAAEQLowerBound = withProjectHistogram.Min;
                        double withoutProjectDamageAAEQLowerBound = withoutProjectHistogram.Min; 

                        double withProjectDamageAAEQUpperBound = withProjectHistogram.Max; 
                        double withoutProjectDamageAAEQUpperBound = withoutProjectHistogram.Max; 

                        double damagesReducedUpperBound = withoutProjectDamageAAEQUpperBound - withProjectDamageAAEQLowerBound;
                        double damagesReducedLowerBound = withoutProjectDamageAAEQLowerBound - withProjectDamageAAEQUpperBound;

                        double range = damagesReducedUpperBound - damagesReducedLowerBound;
                        double binQuantity = 1 + 3.322 * Math.Log(ITERATIONS);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(damagesReducedLowerBound, binWidth, withProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, damageReducedHistogram, withProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence        
                        for (int i = 0; i < ITERATIONS; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddConsequenceRealization(damagesReduced,i);
                        }
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }


        private static List<ConsequenceDistributionResults> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damageReducedAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedAlternative = new ConsequenceDistributionResults(withProjectResults.AlternativeID); 

                foreach (ImpactAreaScenarioResults impactAreaScenarioResults in withProjectResults.BaseYearScenarioResults.ResultsList)
                {
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(impactAreaScenarioResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = impactAreaScenarioResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        IHistogram withProjectHistogram = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID).ConsequenceHistogram;
                        IHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;

                        double withProjectMin = withProjectHistogram.Min;
                        double withProjectMax = withProjectHistogram.Max;
                        double withoutProjectMin = withoutProjectHistogram.Min;
                        double withoutProjectMax = withoutProjectHistogram.Max;

                        double maxDamageReduced = withoutProjectMax - withProjectMin;
                        double minDamageReduced = withoutProjectMin - withProjectMax;
                        double range = maxDamageReduced - minDamageReduced;
                        double binQuantity = 1 + 3.322 * Math.Log(ITERATIONS);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(minDamageReduced, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < ITERATIONS; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddConsequenceRealization(eadDamageReduced,i);
                        }
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }


        private static List<ConsequenceDistributionResults> ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damageReducedAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults alternative in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedAlternative = new ConsequenceDistributionResults(alternative.AlternativeID);

                foreach (ImpactAreaScenarioResults withProjectResults in alternative.FutureYearScenarioResults.ResultsList)
                {
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        IHistogram withProjectHistogram = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID).ConsequenceHistogram;
                        IHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;

                        double withMin = withProjectHistogram.Min;
                        double withMax = withProjectHistogram.Max;
                        double withoutMin = withoutProjectHistogram.Min;
                        double withoutMax = withoutProjectHistogram.Max;

                        double minDamageReduced = withoutMin - withMax;
                        double maxDamageReduced = withoutMax - withMin;
                        double range = maxDamageReduced - minDamageReduced;
                        double binQuantity = 1 + 3.22 * Math.Log(ITERATIONS);
                        double binWidth = Math.Ceiling(binQuantity / range);
                        Histogram damageReducedHistogram = new Histogram(minDamageReduced, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < ITERATIONS; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddConsequenceRealization(eadDamageReduced, i);
                        }
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
