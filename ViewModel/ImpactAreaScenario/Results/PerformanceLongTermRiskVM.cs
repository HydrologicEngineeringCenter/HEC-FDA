using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceLongTermRiskVM : PerformanceVMBase
    {
        public PerformanceLongTermRiskVM(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {
            LoadData(iasResult, metrics);
        }

        private void LoadData(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {
            MetricsToRows = new Dictionary<Threshold, List<IPerformanceRowItem>>();

            for (int i = 0; i < metrics.Count; i++)
            {
                int thresholdKey = metrics[i].Metric.ThresholdID;

                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                List<int> xVals = new List<int>() { 10,20,30 };
                foreach (int xVal in xVals)
                {
                    double yVal = iasResult.PerformanceByThresholds.ThresholdsDictionary[thresholdKey].ProjectPerformanceResults.LongTermExceedanceProbability(xVal);
                    rows.Add(new PerformancePeriodRowItem(xVal, yVal));
                }
                MetricsToRows.Add(metrics[i].Metric, rows);
            }
            Rows = MetricsToRows[metrics[0].Metric];
        }

    }
}
