using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metrics
{
    public class Threshold
    {
        public string ThresholdType { get; }
        public double ThresholdValue { get; }

        public Threshold(string thresholdType, double thresholdValue)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
        }
    }
}