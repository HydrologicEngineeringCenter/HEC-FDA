using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Threshold
    {
        public ThresholdEnum ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public ProjectPerformance Performance { get; set; }
        public int ThresholdID { get; }

        public Threshold(int thresholdID, ThresholdEnum thresholdType=0, double thresholdValue=0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            Performance = new ProjectPerformance(thresholdType, thresholdValue); 
            ThresholdID = thresholdID;
        }

      
    }
}