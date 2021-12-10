using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceLongTermRiskVM : PerformanceVMBase
    {

        public PerformanceLongTermRiskVM(List<ThresholdComboItem> metrics)
        {
            loadDummyData(metrics);
        }


        private void loadDummyData(List<ThresholdComboItem> metrics)
        {

            MetricsToRows = new Dictionary<IMetric, List<IPerformanceRowItem>>();

            for (int i = 0; i < metrics.Count; i++)
            {
                List<int> xVals = loadXData(i);
                List<double> yVals = loadYData(i);

                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                for (int j = 0; j < xVals.Count; j++)
                {
                    rows.Add(new PerformancePeriodRowItem(xVals[j], yVals[j]));
                }
                MetricsToRows.Add(metrics[i].Metric, rows);

            }
            Rows = MetricsToRows[metrics[0].Metric];
        }

        private List<int> loadXData(int i)
        {
            List<int> xValues = new List<int>();
            xValues.Add(i );
            xValues.Add(i );

            return xValues;
        }

        private List<double> loadYData(int i)
        {
            List<double> yValues = new List<double>();
            yValues.Add(i);
            yValues.Add(i + 1);

            return yValues;
        }

    }
}
