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
        private const int _iterations = 10000;
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

                foreach (ConsequenceResult withProjectDamageResult in withProjectAlternativeResults.ConsequenceResults.ConsequenceResultList)
                {
                    //ConsequenceResults withoutProjectDamageResults = withoutProjectAlternativeResults.GetConsequenceResults(withProjectDamageResults.RegionID);
                    //ConsequenceResults damageReducedInImpactArea = new ConsequenceResults(withProjectDamageResults.RegionID);

                    //foreach (ConsequenceResult damageResult in withProjectDamageResult.ConsequenceResultList)
                    {
                        IHistogram withProjectHistogram = withProjectDamageResult.ConsequenceHistogram;//(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID).ConsequenceHistogram;
                        IHistogram withoutProjectHistogram = withoutProjectAlternativeResults.GetConsequencesHistogram(withProjectDamageResult.RegionID,withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory);

                        double withProjectDamageAAEQLowerBound = withProjectHistogram.Min;
                        double withoutProjectDamageAAEQLowerBound = withoutProjectHistogram.Min;  //InverseCDF(lowerBoundProbability);

                        double withProjectDamageAAEQUpperBound = withProjectHistogram.Max; //InverseCDF(upperBoundProbability);
                        double withoutProjectDamageAAEQUpperBound = withoutProjectHistogram.Max; //InverseCDF(upperBoundProbability);

                        double damagesReducedUpperBound = withoutProjectDamageAAEQUpperBound - withProjectDamageAAEQLowerBound;
                        double damagesReducedLowerBound = withoutProjectDamageAAEQLowerBound - withProjectDamageAAEQUpperBound;

                        double range = damagesReducedUpperBound - damagesReducedLowerBound;
                        //TODO: how does this work if based on convergence criteria?
                        double binQuantity = 1 + 3.322 * Math.Log(_iterations);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(damagesReducedLowerBound, binWidth, withProjectDamageResult.ConvergenceCriteria);
                        ConsequenceResult damageReducedResult = new ConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, damageReducedHistogram, withProjectDamageResult.RegionID);
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
                    ConsequenceResults withprojectDamageResults = impactAreaScenarioResults.ConsequenceResults;
                    ConsequenceResults withoutProjectDamageResults = impactAreaScenarioResults.ConsequenceResults;


                    foreach (ConsequenceResult withoutProjectDamageResult in withoutProjectDamageResults.ConsequenceResultList)
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
                        //TODO: how does this work if based on convergence criteria?
                        double binQuantity = 1 + 3.322 * Math.Log(_iterations);
                        double binWidth = Math.Ceiling(range / binQuantity);
                        Histogram damageReducedHistogram = new Histogram(minDamageReduced, binWidth, withoutProjectDamageResult.ConvergenceCriteria);
                        ConsequenceResult damageReducedResult = new ConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
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
                    ConsequenceResults withprojectDamageResults = withProjectResults.ConsequenceResults;
                    ConsequenceResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    //ConsequenceResults damageReducedResults = new ConsequenceResults(withProjectResults.ImpactAreaID);

                    foreach (ConsequenceResult withoutProjectDamageResult in withoutProjectDamageResults.ConsequenceResultList)
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
                        ConsequenceResult damageReducedResult = new ConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, damageReducedHistogram, withoutProjectDamageResult.RegionID);
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
