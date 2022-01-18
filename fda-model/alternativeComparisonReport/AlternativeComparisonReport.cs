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

        public Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers rp, Int64 iterations, double discountRate)
        {
            Dictionary<int, Dictionary<string, Histogram>> withoutProjectResults = _withoutProjectAlternative.AnnualizationCompute(rp, iterations, discountRate);

            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damagesReducedDictionaryAllAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();
            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Dictionary<string, Histogram>> withProjectResults = alternative.AnnualizationCompute(rp, iterations, discountRate);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedDictionaryOneAlternative = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withProjectResults.Keys)
                {
                    Dictionary<string, Histogram> AaeqDamageReducedHistogramDictionary = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withProjectResults[impactAreaID].Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram histogram = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double withProjectDamageAAEQ = (withProjectResults[impactAreaID])[damageCategory].InverseCDF(rp.NextRandom());
                            double withoutProjectDamageAAEQ = (withoutProjectResults[impactAreaID])[damageCategory].InverseCDF(rp.NextRandom());
                            double damagesReduced = withoutProjectDamageAAEQ - withProjectDamageAAEQ;
                            histogram.AddObservationToHistogram(damagesReduced);
                        }
                        AaeqDamageReducedHistogramDictionary.Add(damageCategory, histogram);
                    }
                    damageReducedDictionaryOneAlternative.Add(impactAreaID, AaeqDamageReducedHistogramDictionary);
                }
                damagesReducedDictionaryAllAlternatives.Add(alternative.ID, damageReducedDictionaryOneAlternative);
            }
            return damagesReducedDictionaryAllAlternatives;
        }

        public Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReduced(interfaces.IProvideRandomNumbers rp, Int64 iterations, bool iWantBaseYearResults)
        {
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> results = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();
            if (iWantBaseYearResults)
            {
                results = ComputeDistributionEADReducedBaseYear(rp,iterations);
            }
            else
            {
                results = ComputeDistributionEADReducedFutureYear(rp, iterations);
            }
            return results;
        } 

        private Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers rp, Int64 iterations)
        {
            Dictionary<int, Results> withoutProjectEADDictionary = _withoutProjectAlternative.CurrentYearScenario.Compute(rp, iterations);
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damagesReducedDictionaryAllAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Results> withProjectEADDictionary = alternative.CurrentYearScenario.Compute(rp, iterations);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedOneAlternative = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withoutProjectEADDictionary.Keys)
                {
                    Dictionary<string, Histogram> EADDamageHistogramDictionary = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withoutProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs.Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram histogram = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadSampledWithoutProject = withoutProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            histogram.AddObservationToHistogram(eadDamageReduced);
                        }
                        EADDamageHistogramDictionary.Add(damageCategory, histogram);
                    }
                    damageReducedOneAlternative.Add(impactAreaID, EADDamageHistogramDictionary);
                }
                damagesReducedDictionaryAllAlternatives.Add(alternative.ID, damageReducedOneAlternative);
            }
            return damagesReducedDictionaryAllAlternatives;

        }


        private Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers rp, Int64 iterations)
        {
            Dictionary<int, Results> withoutProjectEADDictionary = _withoutProjectAlternative.FutureYearScenario.Compute(rp, iterations);
            Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>> damagesReducedDictionaryAllAlternatives = new Dictionary<int, Dictionary<int, Dictionary<string, Histogram>>>();

            foreach (Alternative alternative in _withProjectAlternatives)
            {
                Dictionary<int, Results> withProjectEADDictionary = alternative.FutureYearScenario.Compute(rp, iterations);

                Dictionary<int, Dictionary<string, Histogram>> damageReducedOneAlternative = new Dictionary<int, Dictionary<string, Histogram>>();
                foreach (int impactAreaID in withoutProjectEADDictionary.Keys)
                {
                    Dictionary<string, Histogram> EADDamageHistogramDictionary = new Dictionary<string, Histogram>();
                    foreach (string damageCategory in withoutProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs.Keys)
                    {
                        double min = 0;
                        double binWidth = 1;
                        Histogram histogram = new Histogram(min, binWidth);
                        for (int i = 0; i < iterations; i++)
                        {
                            double eadSampledWithProject = withProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadSampledWithoutProject = withoutProjectEADDictionary[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                            double eadDamageReduced = eadSampledWithoutProject - eadSampledWithProject;
                            histogram.AddObservationToHistogram(eadDamageReduced);
                        }
                        EADDamageHistogramDictionary.Add(damageCategory, histogram);
                    }
                    damageReducedOneAlternative.Add(impactAreaID, EADDamageHistogramDictionary);
                }
                damagesReducedDictionaryAllAlternatives.Add(alternative.ID, damageReducedOneAlternative);
            }
            return damagesReducedDictionaryAllAlternatives;

        }

    }
}
