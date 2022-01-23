using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class ExpectedAnnualDamageResults
    {
        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private Dictionary<string, ThreadsafeInlineHistogram> _ead;
        private object _eadLock = new object();

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
        public void AddEADKey(string category)
        {
            if (!_ead.ContainsKey(category))
            {
                //double[] nullData = null;
                var histo = new ThreadsafeInlineHistogram(null, EAD_HISTOGRAM_BINWIDTH);
                _ead.Add(category, histo);
            }
        }
        public void AddEADEstimate(double eadEstimate, string category)
        {
            lock (_eadLock)
            {
                _ead[category].AddObservationToHistogram(eadEstimate);
            }

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
        


    }
}