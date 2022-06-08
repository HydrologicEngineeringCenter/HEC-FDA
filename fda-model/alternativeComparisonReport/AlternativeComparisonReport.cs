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
                        //withProjectHistogram.ForceDeQueue();
                        IHistogram withoutProjectHistogram = withoutProjectAlternativeResults.GetConsequencesHistogram(withProjectDamageResult.RegionID,withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory);
                        //withoutProjectHistogram.ForceDeQueue();

                        double withProjectDamageAAEQLowerBound = withProjectHistogram.Min;
                        double withoutProjectDamageAAEQLowerBound = withoutProjectHistogram.Min;  //InverseCDF(lowerBoundProbability);
                        double damagesReducedLowerBound = withoutProjectDamageAAEQLowerBound - withProjectDamageAAEQLowerBound;

                        double withProjectDamageAAEQUpperBound = withProjectHistogram.Max; //InverseCDF(upperBoundProbability);
                        double withoutProjectDamageAAEQUpperBound = withoutProjectHistogram.Max; //InverseCDF(upperBoundProbability);
                        double damagesReducedUpperBound = withoutProjectDamageAAEQUpperBound - withProjectDamageAAEQUpperBound;

                        double range = damagesReducedUpperBound - damagesReducedLowerBound;
                        //TODO: how does this work if based on convergence criteria?
                        double binQuantity = 1 + 3.322 * Math.Log(convergenceCriteria.MaxIterations);
                        double binWidth = Math.Ceiling(range / binQuantity);

                        ConsequenceResult damageReducedResult = new ConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.ConvergenceCriteria, withProjectDamageResult.RegionID, binWidth);
                        //TODO: run this loop until convergence
                        for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddConsequenceRealization(damagesReduced,i);
                        }
                        damageReducedResult.ConsequenceHistogram.ForceDeQueue();
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
                        ThreadsafeInlineHistogram withProjectHistogram = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID).ConsequenceHistogram;
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;

                        ConsequenceResult damageReducedResult = new ConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ConvergenceCriteria, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
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
                        ThreadsafeInlineHistogram withProjectHistogram = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID).ConsequenceHistogram;
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;

                        ConsequenceResult damageReducedResult = new ConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ConvergenceCriteria, withoutProjectDamageResult.RegionID);
                        //TODO: run this loop until convergence 
                        for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
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
