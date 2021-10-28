using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class ExpectedAnnualDamageResults: IContainResults
    {
        //needs access to get AEP and EAD results.
        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private Dictionary<string, Histogram> _ead; 
   
        public ExpectedAnnualDamageResults(){
            _ead = new Dictionary<string, Histogram>();
        }

        public void AddEADEstimate(double eadEstimate, string category)
        {
            double[] data = new double[1] { eadEstimate };
            IData ead = IDataFactory.Factory(data);
            if (_ead.ContainsKey(category))
            {
                _ead[category].AddObservationToHistogram(ead); 
            }
            else
            {
                var histo = new Histogram(ead, EAD_HISTOGRAM_BINWIDTH);
                _ead.Add(category, histo);
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