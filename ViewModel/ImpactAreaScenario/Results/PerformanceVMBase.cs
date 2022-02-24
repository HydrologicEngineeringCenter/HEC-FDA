using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public abstract class PerformanceVMBase : BaseViewModel
    {
        private List<IPerformanceRowItem> _rows;

        public Dictionary<Threshold, List<IPerformanceRowItem>> MetricsToRows { get; set; }

        public List<IPerformanceRowItem> Rows
        {
            get { return _rows; }
            set { _rows = value; NotifyPropertyChanged(); }
        }

        public void updateSelectedMetric(ThresholdComboItem metric)
        {
            if (MetricsToRows.ContainsKey(metric.Metric))
            {
                Rows = MetricsToRows[metric.Metric];
            }
        }

    }
}
