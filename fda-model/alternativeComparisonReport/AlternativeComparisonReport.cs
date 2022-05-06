using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alternatives;
using Statistics.Histograms;
using metrics;

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
        /// The function returns a nested dictionary of results. The first dictionary has a key of type int for the alternative ID, the second has a key of type 
        /// int for the impact area ID, and the third has a key of type string for the damage category. The value of the third dictionary is the histogram of 
        /// damage reduced for a given damage category in a given impact area and for a given alternative comparison. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="iterations"></param> number of times to sample the aaeq damage histograms
        /// <param name="discountRate"></param> the discount rate at which to calculate the present value of damages, in decimal form
        /// <returns></returns>
        public List<AlternativeResults> ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, Int64 iterations, double discountRate)
        {
            AlternativeResults withoutProjectAlternativeResults = _withoutProjectAlternative.AnnualizationCompute(randomProvider, iterations, discountRate);
            List<AlternativeResults> damagesReducedAllAlternatives = new List<AlternativeResults>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                AlternativeResults withProjectAlternativeResults = alternative.AnnualizationCompute(randomProvider, iterations, discountRate);
                AlternativeResults damageReducedOneAlternative = new AlternativeResults(withoutProjectAlternativeResults.AlternativeID);

                foreach (DamageResults withProjectDamageResults in withProjectAlternativeResults.DamageResultsList)
                {
                    DamageResults withoutProjectDamageResults = withoutProjectAlternativeResults.GetDamageResults(withProjectDamageResults.ImpactAreaID);
                    DamageResults damageReducedInImpactArea = new DamageResults(withProjectDamageResults.ImpactAreaID);

                    foreach (DamageResult damageResult in withProjectDamageResults.DamageResultList)
                    {
                        ThreadsafeInlineHistogram withProjectHistogram = withProjectDamageResults.GetDamageResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ImpactAreaID).DamageHistogram;
                        ThreadsafeInlineHistogram withoutProjectHistoram = withoutProjectDamageResults.GetDamageResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ImpactAreaID).DamageHistogram;

                        double withProjectDamageAAEQLowerBound = withProjectHistogram.Min;
                        double withoutProjectDamageAAEQLowerBound = withoutProjectHistoram.Min;  //InverseCDF(lowerBoundProbability);
                        double damagesReducedLowerBound = withoutProjectDamageAAEQLowerBound - withProjectDamageAAEQLowerBound;

                        double withProjectDamageAAEQUpperBound = withProjectHistogram.Max; //InverseCDF(upperBoundProbability);
                        double withoutProjectDamageAAEQUpperBound = withoutProjectHistoram.Max; //InverseCDF(upperBoundProbability);
                        double damagesReducedUpperBound = withoutProjectDamageAAEQUpperBound - withProjectDamageAAEQUpperBound;

                        double range = damagesReducedUpperBound - damagesReducedLowerBound;
                        double binQuantity = 1 + 3.322 * Math.Log(iterations);
                        double binWidth = Math.Ceiling(range / binQuantity);

                        DamageResult damageReducedResult = new DamageResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ConvergenceCriteria, damageResult.ImpactAreaID, binWidth);

                        for (int i = 0; i < iterations; i++)
                        {
                            double withProjectDamageAAEQ = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double withoutProjectDamageAAEQ = withoutProjectDamageResults.GetDamageResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ImpactAreaID).DamageHistogram.InverseCDF(randomProvider.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedResult.AddDamageRealization(damagesReduced,i);
                        }
                        damageReducedInImpactArea.AddDamageResultObject(damageReducedResult);
                    }
                    damageReducedOneAlternative.AddDamageResults(damageReducedInImpactArea); 
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
        /// <summary>
        /// This method computes the distribution of expected annual damage reduced between the without-project alternative and each of the with-project alternatives
        /// The function returns a nested dictionary of results. The first dictionary has a key of type int for the alternative ID, the second has a key of type 
        /// int for the impact area ID, and the third has a key of type string for the damage category. The value of the third dictionary is the histogram of 
        /// damage reduced for a given damage category in a given impact area and for a given alternative comparison. 
        /// </summary>
        /// <param name="randomProvider"></param> random number provider
        /// <param name="iterations"></param> the number of iterations to sample the EAD distributions
        /// <param name="iWantBaseYearResults"></param> true if the results should be for the base year, false if for the most likely future year. 
        /// <returns></returns>
        public List<AlternativeResults> ComputeDistributionEADReduced(interfaces.IProvideRandomNumbers randomProvider, Int64 iterations, bool iWantBaseYearResults)
        {
            List<AlternativeResults> eadReducedAllAlternatives;
            if (iWantBaseYearResults)
            {
                eadReducedAllAlternatives = ComputeDistributionEADReducedBaseYear(randomProvider,iterations);
            }
            else
            {
                eadReducedAllAlternatives = ComputeDistributionEADReducedFutureYear(randomProvider, iterations);
            }
            return eadReducedAllAlternatives;
        } 

        private List<AlternativeResults> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, Int64 iterations)
        {
            ScenarioResults withoutProjectScenario = _withoutProjectAlternative.CurrentYearScenario.Compute(randomProvider, iterations);

            List<AlternativeResults> damageReducedAlternatives = new List<AlternativeResults>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                ScenarioResults withProjectScenario = alternative.CurrentYearScenario.Compute(randomProvider, iterations);

                AlternativeResults damageReducedAlternative = new AlternativeResults(alternative.ID);

                foreach (Results withProjectResults in withProjectScenario.ResultsList)
                {
                    Results withoutProjectResults = withoutProjectScenario.GetResults(withProjectResults.ImpactAreaID);
                    DamageResults withprojectDamageResults = withProjectResults.DamageResults;
                    DamageResults withoutProjectDamageResults = withoutProjectResults.DamageResults;

                    DamageResults damageReducedResults = new DamageResults(withProjectResults.ImpactAreaID);

                    foreach (DamageResult withoutProjectDamageResult in withoutProjectDamageResults.DamageResultList)
                    {
                        ThreadsafeInlineHistogram withProjectHistogram = withprojectDamageResults.GetDamageResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ImpactAreaID).DamageHistogram;
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResult.DamageHistogram;

                        DamageResult damageReducedResult = new DamageResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ConvergenceCriteria, withoutProjectDamageResult.ImpactAreaID);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddDamageRealization(eadDamageReduced,i);
                        }
                        damageReducedResults.AddDamageResultObject(damageReducedResult);
                    }
                    damageReducedAlternative.AddDamageResults(damageReducedResults);
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }


        private List<AlternativeResults> ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers randomProvider, Int64 iterations)
        {
            ScenarioResults withoutProjectScenario = _withoutProjectAlternative.FutureYearScenario.Compute(randomProvider, iterations);

            List<AlternativeResults> damageReducedAlternatives = new List<AlternativeResults>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                ScenarioResults withProjectScenario = alternative.FutureYearScenario.Compute(randomProvider, iterations);

                AlternativeResults damageReducedAlternative = new AlternativeResults(alternative.ID);

                foreach (Results withProjectResults in withProjectScenario.ResultsList)
                {
                    Results withoutProjectResults = withoutProjectScenario.GetResults(withProjectResults.ImpactAreaID);
                    DamageResults withprojectDamageResults = withProjectResults.DamageResults;
                    DamageResults withoutProjectDamageResults = withoutProjectResults.DamageResults;

                    DamageResults damageReducedResults = new DamageResults(withProjectResults.ImpactAreaID);

                    foreach (DamageResult withoutProjectDamageResult in withoutProjectDamageResults.DamageResultList)
                    {
                        ThreadsafeInlineHistogram withProjectHistogram = withprojectDamageResults.GetDamageResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ImpactAreaID).DamageHistogram;
                        ThreadsafeInlineHistogram withoutProjectHistogram = withoutProjectDamageResult.DamageHistogram;

                        DamageResult damageReducedResult = new DamageResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.ConvergenceCriteria, withoutProjectDamageResult.ImpactAreaID);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadSampledWithoutProject = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReducedResult.AddDamageRealization(eadDamageReduced, i);
                        }
                        damageReducedResults.AddDamageResultObject(damageReducedResult);
                    }
                    damageReducedAlternative.AddDamageResults(damageReducedResults);
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
