using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class AdditionalThresholdRowItem
    {


        public int ID { get; set; }
        public List<IMetricEnum> ThresholdTypes { get; set; }
        public IMetricEnum ThresholdType { get; set; }
        
        public double ThresholdValue { get; set; }

        public AdditionalThresholdRowItem(int id, IMetricEnum thresholdType, double value)
        {
            ID = id;
            ThresholdType = thresholdType;
            ThresholdValue = value;
            ThresholdTypes = new List<IMetricEnum>();
            ThresholdTypes.Add(IMetricEnum.ExteriorStage);
            ThresholdTypes.Add(IMetricEnum.InteriorStage);
            ThresholdTypes.Add(IMetricEnum.ExpectedAnnualDamage);
            ThresholdTypes.Add(IMetricEnum.Damages);

        }

    }
}
