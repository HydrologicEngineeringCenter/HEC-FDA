using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.Model.metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    /// <summary>
    /// I very much dislike the design of this class. Some of the inhereting vms use the mean, median, ninety, and some don't. I'm not stoked about the threshhold dictionaries either. I'd like to take the time to rethink and refactor this someday if remain in this UI for
    /// and extended period. TODO. 
    /// </summary>
    public abstract class PerformanceVMBase : BaseViewModel
    {
        private List<IPerformanceRowItem> _rows;
        private double _Mean;
        private double _Median;
        private double _NinetyPctAssurance;
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

        public double NinetyPercentAssurance
        {
            get { return _NinetyPctAssurance; }
            set { _NinetyPctAssurance = value; NotifyPropertyChanged(); }
        }
        public Dictionary<Threshold, List<IPerformanceRowItem>> MetricsToRows { get;} = new Dictionary<Threshold, List<IPerformanceRowItem>>();
        public Dictionary<Threshold, (double, double, double)> ThresholdToMetrics { get; } = [];
        public List<IPerformanceRowItem> Rows
        {
            get { return _rows; }
            set { _rows = value; NotifyPropertyChanged(); }
        }

        public virtual void UpdateHistogram(ThresholdComboItem metric)
        {
            //TODO
            //Is there a reason that this is empty? 
        }

        public void UpdateSelectedMetric(ThresholdComboItem metric)
        {
            if (metric != null)
            {
                if (MetricsToRows.ContainsKey(metric.Metric))
                {
                    Rows = MetricsToRows[metric.Metric];
                }
                if(ThresholdToMetrics.ContainsKey(metric.Metric))
                {
                    Mean = ThresholdToMetrics[metric.Metric].Item1;
                    Median = ThresholdToMetrics[metric.Metric].Item2;
                    NinetyPercentAssurance = ThresholdToMetrics[metric.Metric].Item3;
                }
                UpdateHistogram(metric);
            }
        }

    }
}
