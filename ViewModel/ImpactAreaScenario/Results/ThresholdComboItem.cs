using metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    /// <summary>
    /// This class is used to create the custom item in the compute results viewer (IASResultsVM).
    /// This class combines the metric name (threshold name) and the threshold value.
    /// </summary>
    public class ThresholdComboItem
    {
        public string Description { get; set; }

        public Threshold Metric { get; set; }
        public ThresholdComboItem(Threshold metric)
        {
            Metric = metric;
            Description = metric.ThresholdType + " (" + metric.ThresholdValue + ")";
        }

    }
}
