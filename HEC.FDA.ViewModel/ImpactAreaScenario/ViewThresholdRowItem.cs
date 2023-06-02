using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario
{
    public class ViewThresholdRowItem
    {
        public string ImpactAreaName { get; }
        public string ThresholdType { get; }
        public double? ThresholdValue { get; }

        public ViewThresholdRowItem(string impactAreaName, string thresholdType, double? thresholdValue)
        {
            ImpactAreaName = impactAreaName;
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
        }
    }
}
