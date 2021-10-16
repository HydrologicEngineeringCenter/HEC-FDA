using System;
using System.Collections.Generic;
namespace metrics
{
    public class Results: IContainResults
    {
        //needs access to get AEP and EAD results.
        private double _aepThreshold;
        private double _meanAEP;//replace with inlinehistogram
        private Int64 _aepCount;
        private Dictionary<string, double> _meanEADs; //replace double with inline histogram
        private Dictionary<string, Int64> _eadCounts;
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
            _meanAEP = 0.0;
            _aepCount = 0;
            _meanEADs = new Dictionary<string, double>();
            _eadCounts = new Dictionary<string, Int64>();
        }
        public void AddAEPEstimate(double aepEstimate)
        {
            _meanAEP = _meanAEP +((aepEstimate - _meanAEP)/(double)_aepCount);
            _aepCount +=1;
        }

        public void AddEADEstimate(double eadEstimate, string category)
        {
            if (_meanEADs.ContainsKey(category))
            {
                _meanEADs[category] = _meanEADs[category] +((eadEstimate - _meanEADs[category])/(double)_eadCounts[category]);
                _eadCounts[category] +=1;
            }
            else
            {
                _meanEADs.Add(category, eadEstimate);
                _eadCounts.Add(category, 1);
            }
        }
        public double MeanEAD(string category)
        {
            return _meanEADs[category];
        }
    }
}