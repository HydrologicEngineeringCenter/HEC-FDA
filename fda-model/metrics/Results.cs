using System;
namespace metrics
{
    public class Results: IContainResults
    {
        //needs access to get AEP and EAD results.
        private double _aepThreshold;
        private double _meanAEP;//replace with inlinehistogram
        private Int64 _aepCount;
        private double _meanEAD;//replace with inlinehistogram
        private Int64 _eadCount;
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
            _meanEAD = 0.0;
            _eadCount = 0;
        }
        public void AddAEPEstimate(double aepEstimate)
        {
            _meanAEP = _meanAEP +((aepEstimate - _meanAEP)/(double)_aepCount);
            _aepCount +=1;
        }

        public void AddEADEstimate(double eadEstimate)
        {
            _meanEAD = _meanEAD +((eadEstimate - _meanEAD)/(double)_eadCount);
            _eadCount +=1;
        }

    }
}