using metrics;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.ImpactAreaScenario.Results
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
