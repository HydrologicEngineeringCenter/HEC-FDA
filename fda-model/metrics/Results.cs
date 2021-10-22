using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class Results: IContainResults
    {
        //needs access to get AEP and EAD results.
        private const double AEP_HISTOGRAM_BINWIDTH = .01;
        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private double _aepThreshold;
        private Histogram _aep =  null;
        private Dictionary<string, Histogram> _ead; 

        public double AEPThreshold { 
            get {
                return _aepThreshold;
            } 
            set {
            _aepThreshold = value;
            }
        }
        public Results(){
            _aepThreshold = 0.0;
            _aep = null; //is this necessary?
            _ead = new Dictionary<string, Histogram>();
        }
        public void AddAEPEstimate(double aepEstimate)
        {
            double[] data = new double[1] { aepEstimate };
            IData aep = IDataFactory.Factory(data);
            if (aep!=null)
            {
                _aep.AddObservationToHistogram(aep);
            } else
            {
                var histo = new Histogram(aep, AEP_HISTOGRAM_BINWIDTH);
                _aep = histo;
            }
            

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

    }
}