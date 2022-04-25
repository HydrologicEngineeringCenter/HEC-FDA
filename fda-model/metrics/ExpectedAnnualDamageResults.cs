using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using System.Xml.Linq;


namespace metrics
{
    public class ExpectedAnnualDamageResults
    {
        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private Dictionary<string, ThreadsafeInlineHistogram> _ead;

        public Dictionary<string, ThreadsafeInlineHistogram> HistogramsOfEADs
        {
            get
            {
                return _ead;
            }
        }
   
        public ExpectedAnnualDamageResults(){
            _ead = new Dictionary<string, ThreadsafeInlineHistogram>();
        }
        public ExpectedAnnualDamageResults(Dictionary<string, ThreadsafeInlineHistogram> eadDictionary)
        {
            _ead = eadDictionary;
        }
        public void AddEADKey(string category, ConvergenceCriteria convergenceCriteria)
        {
            if (!_ead.ContainsKey(category))
            {
                var histogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH, convergenceCriteria);
                histogram.SetIterationSize(convergenceCriteria.MaxIterations);
                _ead.Add(category, histogram);
            }
        }
        public void AddEADEstimate(double eadEstimate, string category, Int64 iteration)
        {
            _ead[category].AddObservationToHistogram(eadEstimate, iteration);
        }
        public double MeanEAD(string category)
        {
            return _ead[category].Mean;
        }

        public double EADExceededWithProbabilityQ(string category, double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _ead[category].InverseCDF(nonExceedanceProbability);
            return quartile;
        }
        
        public bool Equals(ExpectedAnnualDamageResults expectedAnnualDamageResults)
        {
           foreach (string category in HistogramsOfEADs.Keys)
            {
                bool histogramsMatch = HistogramsOfEADs[category].Equals(expectedAnnualDamageResults.HistogramsOfEADs[category]);
                if (!histogramsMatch)
                {
                    return false;
                }
            }
            return true;

        }
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (string key in HistogramsOfEADs.Keys)
            {
                XElement rowElement = new XElement($"{key}");
                rowElement = _ead[key].WriteToXML();
                rowElement.Name = $"{key}";
                masterElem.Add(rowElement);
            }
            return masterElem;
        }

        public static ExpectedAnnualDamageResults ReadFromXML(XElement xElement)
        {
            Dictionary<string, ThreadsafeInlineHistogram> eadHistogramDictionary = new Dictionary<string, ThreadsafeInlineHistogram>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                eadHistogramDictionary.Add(Convert.ToString(histogramElement.Name),ThreadsafeInlineHistogram.ReadFromXML(histogramElement));
            }
            return new ExpectedAnnualDamageResults(eadHistogramDictionary);
        }


    }
}