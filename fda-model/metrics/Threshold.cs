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

        public Threshold(ThresholdEnum thresholdType, double thresholdValue)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
        }
    }
}