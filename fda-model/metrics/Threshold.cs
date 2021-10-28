using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Threshold
    {
        public ThresholdEnum ThresholdType { get; }
        public double ThresholdValue { get; }
        private ProjectPerformance performance;

        public Threshold(ThresholdEnum thresholdType, double thresholdValue)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            performance = new ProjectPerformance(thresholdType,thresholdValue);
        }
    }
}