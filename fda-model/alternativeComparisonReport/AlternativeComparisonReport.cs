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

                List<ConsequenceDistributionResult> withoutProjectConsequenceDistList = new List<ConsequenceDistributionResult>();
                foreach (ConsequenceDistributionResult consequenceDistributionResult in withoutProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    withoutProjectConsequenceDistList.Add(consequenceDistributionResult);
                }

                foreach (ConsequenceDistributionResult withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID); //GetAAEQDamageHistogram;
                        withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);


                    ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectConsequenceDistList.Count > 0)
                {
                    foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectConsequenceDistList)
                    {
                        ConsequenceDistributionResult withProjectDamageResult = withProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                        damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
        
        private static ConsequenceDistributionResult IterateOnConsequenceDistributionResult(ConsequenceDistributionResult withProjectDamageResult, ConsequenceDistributionResult withoutProjectDamageResult, interfaces.IProvideRandomNumbers randomProvider, bool iterateOnWithProject = true)
        {//TODO rename local variables to reflect generalization of this extracted logic 
            IHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;
            IHistogram withProjectHistogram = withProjectDamageResult.ConsequenceHistogram;

            double withProjectDamageLowerBound = withProjectHistogram.Min;
            double withoutProjectDamageLowerBound = withoutProjectHistogram.Min;

            double withProjectDamageUpperBound = withProjectHistogram.Max;
            double withoutProjectDamageUpperBound = withoutProjectHistogram.Max;

            double damagesReducedUpperBound = withoutProjectDamageUpperBound - withProjectDamageLowerBound;
            double damagesReducedLowerBound = withoutProjectDamageLowerBound - withProjectDamageUpperBound;

            double range = damagesReducedUpperBound - damagesReducedLowerBound;
            double binQuantity = 1 + 3.322 * Math.Log(ITERATIONS);
            double binWidth = Math.Ceiling(range / binQuantity);
            Histogram damageReducedHistogram;
            ConsequenceDistributionResult damageReducedResult;
            if (iterateOnWithProject)
            {
                damageReducedHistogram = new Histogram(damagesReducedLowerBound, binWidth, withProjectDamageResult.ConvergenceCriteria);
                damageReducedResult = new ConsequenceDistributionResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, damageReducedHistogram, withProjectDamageResult.RegionID);
            }
            else
            {
                damageReducedHistogram = new Histogram(damagesReducedLowerBound, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
            }
            //TODO: run this loop until convergence        
            for (int i = 0; i < ITERATIONS; i++)
            {
                double withProjectDamage = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                double withoutProjectDamage = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                double damagesReduced = withoutProjectDamage - withProjectDamage;
                damageReducedResult.AddConsequenceRealization(damagesReduced, i);
            }
            return damageReducedResult;
        }

        private static List<ConsequenceDistributionResults> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damageReducedAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedAlternative = new ConsequenceDistributionResults(withProjectResults.AlternativeID); 

                foreach (ImpactAreaScenarioResults withProjectIAS in withProjectResults.BaseYearScenarioResults.ResultsList)
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectIAS = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(withProjectIAS.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectIAS.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectIAS.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new List<ConsequenceDistributionResult>();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList) 
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if(withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach(ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
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
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new List<ConsequenceDistributionResult>();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
