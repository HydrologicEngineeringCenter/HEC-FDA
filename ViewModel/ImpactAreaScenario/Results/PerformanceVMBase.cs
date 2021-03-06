using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.Plotting.SciChart2D.DataModel;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public abstract class PerformanceVMBase : BaseViewModel
    {
        private List<IPerformanceRowItem> _rows;
        private double _Mean;
        private double _Median;

        public double Mean
        {
            get { return _Mean; }
            set { _Mean = value; NotifyPropertyChanged(); }
        }

        public double Median
        {
            get { return _Median; }
            set { _Median = value; NotifyPropertyChanged(); }
        }

        public Dictionary<Threshold, List<IPerformanceRowItem>> MetricsToRows { get;} = new Dictionary<Threshold, List<IPerformanceRowItem>>();
        public List<IPerformanceRowItem> Rows
        {
            get { return _rows; }
            set { _rows = value; NotifyPropertyChanged(); }
        }

        public virtual void UpdateHistogram(ThresholdComboItem metric)
        {

        }

        public void updateSelectedMetric(ThresholdComboItem metric)
        {
            if (metric != null)
            {
                if (MetricsToRows.ContainsKey(metric.Metric))
                {
                    Rows = MetricsToRows[metric.Metric];
                }
                UpdateHistogram(metric);
            }
        }

    }
}
