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
        public List<ThresholdType> ThresholdTypes { get; } = new List<ThresholdType>();
        public ThresholdType ThresholdType { get; set; }   
        public double ThresholdValue { get; set; }

        public ThresholdRowItem(int id, IMetricEnum thresholdType, double value)
        {
            ID = id;
            LoadThresholdTypes();
            ThresholdType = ThresholdTypes.FirstOrDefault(tt => tt.Metric == thresholdType);
            ThresholdValue = value;
        }

        private void LoadThresholdTypes()
        {
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.ExteriorStage, "Exterior Stage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.InteriorStage, "Interior Stage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.ExpectedAnnualDamage, "Expected Annual Damage"));
            ThresholdTypes.Add(new ThresholdType( IMetricEnum.Damages, "Damages"));
        }

        public IMetric GetMetric()
        {
            return IMetricFactory.Factory(ThresholdType.Metric, ThresholdValue);
        }
    }
}
