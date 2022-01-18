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
    class AlternativeComparisonReport
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
        /// <param name="rp"></param> random number provider
        /// <param name="iterations"></param> number of times to sample the aaeq damage histograms
        /// <param name="discountRate"></param> the discount rate at which to calculate the present value of damages, in decimal form
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers rp, Int64 iterations, double discountRate)
        {
            Dictionary<int, Dictionary<string, Histogram>> withoutProjectResults = _withoutProjectAlternative.AnnualizationCompute(rp, iterations, discountRate);

            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damagesReducedAllAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();
            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Dictionary<string, Histogram>> withProjectResults = alternative.AnnualizationCompute(rp, iterations, discountRate);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedOneAlternative = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withProjectResults.Keys)
                {
                    Dictionary<string, Histogram> damageReducedInImpactArea = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withProjectResults[impactAreaID].Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram damageReducedDamageCategory = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double withProjectDamageAAEQ = (withProjectResults[impactAreaID])[damageCategory].InverseCDF(rp.NextRandom());
                            double withoutProjectDamageAAEQ = (withoutProjectResults[impactAreaID])[damageCategory].InverseCDF(rp.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            damageReducedDamageCategory.AddObservationToHistogram(damagesReduced);
                        }
                        damageReducedInImpactArea.Add(damageCategory, damageReducedDamageCategory);
                    }
                    damageReducedOneAlternative.Add(impactAreaID, damageReducedInImpactArea);
                }
                damagesReducedAllAlternatives.Add(alternative.ID, damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
        /// <summary>
        /// This method computes the distribution of expected annual damage reduced between the without-project alternative and each of the with-project alternatives
        /// The function returns a nested dictionary of results. The first dictionary has a key of type int for the alternative ID, the second has a key of type 
        /// int for the impact area ID, and the third has a key of type string for the damage category. The value of the third dictionary is the histogram of 
        /// damage reduced for a given damage category in a given impact area and for a given alternative comparison. 
        /// </summary>
        /// <param name="rp"></param> random number provider
        /// <param name="iterations"></param> the number of iterations to sample the EAD distributions
        /// <param name="iWantBaseYearResults"></param> true if the results should be for the base year, false if for the most likely future year. 
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReduced(interfaces.IProvideRandomNumbers rp, Int64 iterations, bool iWantBaseYearResults)
        {
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> eadReducedAllAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();
            if (iWantBaseYearResults)
            {
                eadReducedAllAlternatives = ComputeDistributionEADReducedBaseYear(rp,iterations);
            }
            else
            {
                eadReducedAllAlternatives = ComputeDistributionEADReducedFutureYear(rp, iterations);
            }
            return eadReducedAllAlternatives;
        } 

        private Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers rp, Int64 iterations)
        {
            Dictionary<int, Results> withoutProjectEAD = _withoutProjectAlternative.CurrentYearScenario.Compute(rp, iterations);
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damageReducedAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Results> withProjectEAD = alternative.CurrentYearScenario.Compute(rp, iterations);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedImpactAreas = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withoutProjectEAD.Keys)
                {
                    Dictionary<string, Histogram> damageReducedDamageCategories = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withoutProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs.Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram damageReduced = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadSampledWithoutProject = withoutProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReduced.AddObservationToHistogram(eadDamageReduced);
                        }
                        damageReducedDamageCategories.Add(damageCategory, damageReduced);
                    }
                    damageReducedImpactAreas.Add(impactAreaID, damageReducedDamageCategories);
                }
                damageReducedAlternatives.Add(alternative.ID, damageReducedImpactAreas);
            }
            return damageReducedAlternatives;

        }


        private Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers rp, Int64 iterations)
        {
            Dictionary<int, Results> withoutProjectEAD = _withoutProjectAlternative.FutureYearScenario.Compute(rp, iterations);
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damagesReducedAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Results> withProjectEAD = alternative.FutureYearScenario.Compute(rp, iterations);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedImpactAreas = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withoutProjectEAD.Keys)
                {
                    Dictionary<string, Histogram> damageReducedDamageCategories = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withoutProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs.Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram damageReduced = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadSampledWithoutProject = withoutProjectEAD[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            damageReduced.AddObservationToHistogram(eadDamageReduced);
                        }
                        damageReducedDamageCategories.Add(damageCategory, damageReduced);
                    }
                    damageReducedImpactAreas.Add(impactAreaID, damageReducedDamageCategories);
                }
                damagesReducedAlternatives.Add(alternative.ID, damageReducedImpactAreas);
            }
            return damagesReducedAlternatives;

        }

    }
}
