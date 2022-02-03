using metrics;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdRowItem
    {
        public int ID { get; set; }
        public List<ThresholdType> ThresholdTypes { get; } = new List<ThresholdType>();
        public ThresholdType ThresholdType { get; set; }   
        public double ThresholdValue { get; set; }

        public ThresholdRowItem(int id, ThresholdEnum thresholdType, double value)
        {
            ID = id;
            LoadThresholdTypes();
            ThresholdType = ThresholdTypes.FirstOrDefault(tt => tt.Metric == thresholdType);
            ThresholdValue = value;
        }

        private void LoadThresholdTypes()
        {
            ThresholdTypes.Add(new ThresholdType( ThresholdEnum.ExteriorStage, "Exterior Stage"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.InteriorStage, "Interior Stage"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.Damage, "Damage"));
        }

        public Threshold GetMetric()
        {
            return new Threshold(-1, ThresholdType.Metric, ThresholdValue);
        }
    }
}
