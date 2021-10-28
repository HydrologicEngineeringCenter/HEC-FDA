using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdRowItem
    {


        public int ID { get; set; }
        public List<ThresholdType> ThresholdTypes { get; set; }
        public ThresholdType ThresholdType { get; set; }
     
        
        public double ThresholdValue { get; set; }

        public ThresholdRowItem(int id, IMetricEnum thresholdType, double value)
        {
            ID = id;
            LoadThresholdTypes();
            ThresholdType = SelectThresholdType(thresholdType);
            ThresholdValue = value;
        }

        private ThresholdType SelectThresholdType(IMetricEnum metricType)
        {
            foreach(ThresholdType tt in ThresholdTypes)
            {
                if(tt.Metric == metricType)
                {
                    return tt;
                }
            }
            return null; //this shouldn't ever happen.
        }
        private void LoadThresholdTypes()
        {
            ThresholdTypes = new List<ThresholdType>();

            ThresholdTypes.Add(new ThresholdType( IMetricEnum.ExteriorStage, "Exterior Stage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.InteriorStage, "Interior Stage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.ExpectedAnnualDamage, "Expected Annual Damage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.Damages, "Damages"));
        }

    }
}
