using HEC.FDA.Model.metrics;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdRowItem
    {
        private double? _ThresholdValue = null;

        public int ID { get; set; }
        public List<ThresholdType> ThresholdTypes { get; } = new List<ThresholdType>();
        public ThresholdType ThresholdType { get; set; }
        public double? ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value;  }
        }

        public ThresholdRowItem(int id, ThresholdEnum thresholdType, double? value)
        {
            ID = id;
            LoadThresholdTypes();
            ThresholdType = ThresholdTypes.FirstOrDefault(tt => tt.Metric == thresholdType);
            ThresholdValue = value;
        }

        private void LoadThresholdTypes()
        {
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.DefaultExteriorStage, "Default Exterior Stage"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.TopOfLevee, "Top Elevation of Levee"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.LeveeSystemResponse, "Levee with System Response"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.AdditionalExteriorStage, "Additional Exterior Stage"));
            //TODO: THese are relics. Do we need to keep them for backward compatibilty?
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.ExteriorStage, "Exterior Stage"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.InteriorStage, "Interior Stage"));
            ThresholdTypes.Add(new ThresholdType(ThresholdEnum.Damage, "Damage"));

        }

        public Threshold GetThreshold()
        {
            Statistics.ConvergenceCriteria convergenceCriteria = new Statistics.ConvergenceCriteria();
            return new Threshold(ID, convergenceCriteria, ThresholdType.Metric, ThresholdValue.Value);
        }

    }
}
