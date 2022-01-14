using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class PerformanceByThresholds
{        
        private Dictionary<int,Threshold> _thresholds; 
        public Dictionary<int,Threshold> ThresholdsDictionary
        {
            get
            {
                return _thresholds;
            }
            set
            {
                _thresholds = value;
            }
        }

        public PerformanceByThresholds()
        {
            _thresholds = new Dictionary<int,Threshold>();
                   
        }      

        public void AddThreshold(Threshold threshold)
        {
            _thresholds.Add(threshold.ThresholdID,threshold);
        }

        public void RemoveThreshold(Threshold threshold)
        {
            _thresholds.Remove(threshold.ThresholdID);
        } 


}
}
