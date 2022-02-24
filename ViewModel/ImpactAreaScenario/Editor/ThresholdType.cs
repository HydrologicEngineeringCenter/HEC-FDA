using metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdType
    {
        public string DisplayName { get; set; }

        public ThresholdEnum Metric {get;set;}

        public ThresholdType(ThresholdEnum metric, string displayName)
        {
            Metric = metric;
            DisplayName = displayName;
        }
    }
}
