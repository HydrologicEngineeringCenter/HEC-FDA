using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{

    public class PerformanceLongTermRiskVM : PerformanceVMBase
    {
        public PerformanceLongTermRiskVM(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, thresholdComboItems);
        }

        private void LoadData(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            for (int i = 0; i < thresholdComboItems.Count; i++)
            {
                int thresholdKey = thresholdComboItems[i].Metric.ThresholdID;

                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                List<int> xVals = new List<int>() { 10,20,30 };
                foreach (int xVal in xVals)
                {
                    double yVal = iasResult.LongTermExceedanceProbability(thresholdKey, xVal);
                    rows.Add(new PerformancePeriodRowItem(xVal, yVal));
                }
                MetricsToRows.Add(thresholdComboItems[i].Metric, rows);
            }
            Rows = MetricsToRows[thresholdComboItems[0].Metric];
        }

    }
}
