using System;
using System.Collections.Generic;
using alternatives;
using Statistics.Histograms;
using metrics;
using Statistics;

namespace alternativeComparisonReport
{
    public class AlternativeComparisonReport
{
        private const int _iterations = 50000;
        /// <summary>
        /// This method computes the distribution of average annual equivalent damage reduced between the without-project alternative and each of the with-project alternatives
        /// The function returns an AlternativeComparisonReportResults object which stores a list of AlternativeResults for each with-project condition. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="convergenceCriteria"></param> the study convergence criteria 
        /// <param name="discountRate"></param> the discount rate at which to calculate the present value of damages, in decimal form
        /// <returns></returns>
        public static AlternativeComparisonReportResults ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            AlternativeComparisonReportResults damagesReducedAllAlternatives = new AlternativeComparisonReportResults();

            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                AlternativeResults damageReducedOneAlternative = new AlternativeResults(withProjectAlternativeResults.AlternativeID);

                foreach (ConsequenceDistributionResult withProjectDamageResult in withProjectAlternativeResults.ConsequenceResults.ConsequenceResultList)
                {

                    {
                        IHistogram withProjectHistogram = withProjectDamageResult.ConsequenceHistogram;
                        IHistogram withoutProjectHistogram = withoutProjectAlternativeResults.GetConsequencesHistogram(withProjectDamageResult.RegionID,withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory);

                        double withProjectDamageAAEQLowerBound = withProjectHistogram.Min;
                        double withoutProjectDamageAAEQLowerBound = withoutProjectHistogram.Min; 

                        double withProjectDamageAAEQUpperBound = withProjectHistogram.Max; 
                        double withoutProjectDamageAAEQUpperBound = withoutProjectHistogram.Max; 

                        double damagesReducedUpperBound = withoutProjectDamageAAEQUpperBound - withProjectDamageAAEQLowerBound;
                        double damagesReducedLowerBound = withoutProjectDamageAAEQLowerBound - withProjectDamageAAEQUpperBound;

                        double range = damagesReducedUpperBound - damagesReducedLowerBound;
                        double binQuantity = 1 + 3.322 * Math.Log(_iterations);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(damagesReducedLowerBound, binWidth, withProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, damageReducedHistogram, withProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence        
                        for (int i = 0; i < _iterations; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddConsequenceRealization(damagesReduced,i);
                        }
                        damageReducedOneAlternative.AddConsequenceResults(damageReducedResult);

                    }
                }
                damagesReducedAllAlternatives.AddAlternativeResults(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
       
        /// <summary>
        /// This method computes the distribution of expected annual damage reduced between the without-project alternative and each of the with-project alternatives
        /// This method returns an AlternativeComparisonReportResults object, which contains a list of AlternativeResults for each with-project condition. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="iterations"></param> the number of iterations to sample the EAD distributions
        /// <param name="iWantBaseYearResults"></param> true if the results should be for the base year, false if for the most likely future year. 
        /// <returns></returns>
        public static AlternativeComparisonReportResults ComputeDistributionEADReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults, bool iWantBaseYearResults)
        {
            AlternativeComparisonReportResults damagesReducedAllAlternatives = new AlternativeComparisonReportResults();
            if (iWantBaseYearResults)
            {
                damagesReducedAllAlternatives = ComputeDistributionEADReducedBaseYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            }
            else
            {
                damagesReducedAllAlternatives = ComputeDistributionEADReducedFutureYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            }
            return damagesReducedAllAlternatives;
        } 

        private static AlternativeComparisonReportResults ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            AlternativeComparisonReportResults damageReducedAlternatives = new AlternativeComparisonReportResults();

            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {

                AlternativeResults damageReducedAlternative = new AlternativeResults(withProjectResults.AlternativeID);

                foreach (ImpactAreaScenarioResults impactAreaScenarioResults in withProjectResults.BaseYearScenarioResults.ResultsList)
                {
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(impactAreaScenarioResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = impactAreaScenarioResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = impactAreaScenarioResults.ConsequenceResults;


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
                        double binQuantity = 1 + 3.322 * Math.Log(_iterations);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(minDamageReduced, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < _iterations; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddConsequenceRealization(eadDamageReduced,i);
                        }
                        damageReducedAlternative.AddConsequenceResults(damageReducedResult);
                    }
                }
                damageReducedAlternatives.AddAlternativeResults(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }


        private static AlternativeComparisonReportResults ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            AlternativeComparisonReportResults damageReducedAlternatives = new AlternativeComparisonReportResults();

            foreach (AlternativeResults alternative in withProjectAlternativesResults)
            {
                AlternativeResults damageReducedAlternative = new AlternativeResults(alternative.AlternativeID);

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
                        double binQuantity = 1 + 3.22 * Math.Log(_iterations);
                        double binWidth = Math.Ceiling(binQuantity / range);
                        Histogram damageReducedHistogram = new Histogram(minDamageReduced, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                        ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < _iterations; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddConsequenceRealization(eadDamageReduced, i);
                        }
                        damageReducedAlternative.AddConsequenceResults(damageReducedResult);
                    }
                }
                damageReducedAlternatives.AddAlternativeResults(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
