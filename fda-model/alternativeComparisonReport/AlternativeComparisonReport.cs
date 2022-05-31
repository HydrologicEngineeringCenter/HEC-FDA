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
        private List<Alternative> _withProjectAlternatives;
        private Alternative _withoutProjectAlternative;

        public List<Alternative> WithProjectAlternatives
        {
            get
            {
                return _withProjectAlternatives;
            } set
            {
                _withProjectAlternatives = value;
            }
        }
        public Alternative WithoutProjectAlternative
        {
            get
            {
                return _withoutProjectAlternative;
            }
            set
            {
                _withoutProjectAlternative = value;
            }
        }

        public AlternativeComparisonReport(Alternative withoutProject, List<Alternative> withProjecs)
        {
            _withoutProjectAlternative = withoutProject;
            _withProjectAlternatives = withProjecs;
        }
        /// <summary>
        /// This method computes the distribution of average annual equivalent damage reduced between the without-project alternative and each of the with-project alternatives
        /// The function returns an AlternativeComparisonReportResults object which stores a list of AlternativeResults for each with-project condition. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="convergenceCriteria"></param> the study convergence criteria 
        /// <param name="discountRate"></param> the discount rate at which to calculate the present value of damages, in decimal form
        /// <returns></returns>
        public AlternativeComparisonReportResults ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            AlternativeComparisonReportResults damagesReducedAllAlternatives = new AlternativeComparisonReportResults();

            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                AlternativeResults damageReducedOneAlternative = new AlternativeResults(withoutProjectAlternativeResults.AlternativeID);

                foreach (ConsequenceResults withProjectDamageResults in withProjectAlternativeResults.ConsequenceResultsList)
                {
                    ConsequenceResults withoutProjectDamageResults = withoutProjectAlternativeResults.GetConsequenceResults(withProjectDamageResults.RegionID);
                    ConsequenceResults damageReducedInImpactArea = new ConsequenceResults(withProjectDamageResults.RegionID);

                    foreach (ConsequenceResult damageResult in withProjectDamageResults.ConsequenceResultList)
                    {
                        ThreadsafeInlineHistogram withProjectHistogram = withProjectDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID).ConsequenceHistogram;
                        withProjectHistogram.ForceDeQueue();
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID).ConsequenceHistogram;
                        withoutProjectHistogram.ForceDeQueue();

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

                        ConsequenceResult damageReducedResult = new ConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ConvergenceCriteria, damageResult.RegionID, binWidth);
                        //TODO: run this loop until convergence
                        for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddConsequenceRealization(damagesReduced,i);
                        }
                        damageReducedResult.ConsequenceHistogram.ForceDeQueue();
                        damageReducedInImpactArea.AddConsequenceResult(damageReducedResult);
                    }
                    damageReducedOneAlternative.AddConsequenceResults(damageReducedInImpactArea); 
                }
                damagesReducedAllAlternatives.AddAlternativeResults(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
        [Obsolete("This method is deprecated. The intent of the alternative comparison report is to use existing alternative results for comparison")]
        /// <summary>
        /// This method computes the distribution of average annual equivalent damage reduced between the without-project alternative and each of the with-project alternatives
        /// The function returns an AlternativeComparisonReportResults object which stores a list of AlternativeResults for each with-project condition. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="convergenceCriteria"></param> the study convergence criteria 
        /// <param name="discountRate"></param> the discount rate at which to calculate the present value of damages, in decimal form
        /// <returns></returns>
        public AlternativeComparisonReportResults ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, double discountRate)
        {
            AlternativeResults withoutProjectAlternativeResults = _withoutProjectAlternative.AnnualizationCompute(randomProvider, convergenceCriteria, discountRate);
            AlternativeComparisonReportResults damagesReducedAllAlternatives = new AlternativeComparisonReportResults();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                AlternativeResults withProjectAlternativeResults = alternative.AnnualizationCompute(randomProvider, convergenceCriteria, discountRate);
                AlternativeResults damageReducedOneAlternative = new AlternativeResults(withoutProjectAlternativeResults.AlternativeID);

                foreach (ConsequenceResults withProjectDamageResults in withProjectAlternativeResults.ConsequenceResultsList)
                {
                    ConsequenceResults withoutProjectDamageResults = withoutProjectAlternativeResults.GetConsequenceResults(withProjectDamageResults.RegionID);
                    ConsequenceResults damageReducedInImpactArea = new ConsequenceResults(withProjectDamageResults.RegionID);

                    foreach (ConsequenceResult damageResult in withProjectDamageResults.ConsequenceResultList)
                    {
                        ThreadsafeInlineHistogram withProjectHistogram = withProjectDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID).ConsequenceHistogram;
                        withProjectHistogram.ForceDeQueue();
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID).ConsequenceHistogram;
                        withoutProjectHistogram.ForceDeQueue();

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

                        ConsequenceResult damageReducedResult = new ConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ConvergenceCriteria, damageResult.RegionID, binWidth);
                        //TODO: run this loop until convergence
                        for (int i = 0; i < convergenceCriteria.MaxIterations; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddConsequenceRealization(damagesReduced, i);
                        }
                        damageReducedResult.ConsequenceHistogram.ForceDeQueue();
                        damageReducedInImpactArea.AddConsequenceResult(damageReducedResult);
                    }
                    damageReducedOneAlternative.AddConsequenceResults(damageReducedInImpactArea);
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
        public AlternativeComparisonReportResults ComputeDistributionEADReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults, bool iWantBaseYearResults)
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

        private AlternativeComparisonReportResults ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            AlternativeComparisonReportResults damageReducedAlternatives = new AlternativeComparisonReportResults();

            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                //ScenarioResults withProjectScenario = alternative.CurrentYearScenario.Compute(randomProvider, convergenceCriteria);

                AlternativeResults damageReducedAlternative = new AlternativeResults(withProjectResults.AlternativeID);

                foreach (ImpactAreaScenarioResults impactAreaScenarioResults in withProjectResults.BaseYearScenarioResults.ResultsList)
                {
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(impactAreaScenarioResults.ImpactAreaID);
                    ConsequenceResults withprojectDamageResults = impactAreaScenarioResults.ConsequenceResults;
                    ConsequenceResults withoutProjectDamageResults = impactAreaScenarioResults.ConsequenceResults;

                    ConsequenceResults damageReducedResults = new ConsequenceResults(impactAreaScenarioResults.ImpactAreaID);

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
                        damageReducedResults.AddConsequenceResult(damageReducedResult);
                    }
                    damageReducedAlternative.AddConsequenceResults(damageReducedResults);
                }
                damageReducedAlternatives.AddAlternativeResults(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }


        private AlternativeComparisonReportResults ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
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

                    ConsequenceResults damageReducedResults = new ConsequenceResults(withProjectResults.ImpactAreaID);

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
                        damageReducedResults.AddConsequenceResult(damageReducedResult);
                    }
                    damageReducedAlternative.AddConsequenceResults(damageReducedResults);
                }
                damageReducedAlternatives.AddAlternativeResults(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
