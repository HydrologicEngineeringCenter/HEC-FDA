using metrics;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAssuranceOfThresholdVM :  PerformanceVMBase
    {

        public PerformanceAssuranceOfThresholdVM(List<ThresholdComboItem> metrics)
        {
            loadDummyData(metrics);
        }


        private void loadDummyData(List<ThresholdComboItem> metrics)
        {
            MetricsToRows = new Dictionary<Threshold, List<IPerformanceRowItem>>();

            for (int i = 0; i < metrics.Count; i++)
            {
                List<double> xVals = loadXData(i);
                List<double> yVals = loadYData(i);

                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                for (int j = 0; j < xVals.Count; j++)
                {
                    rows.Add(new PerformanceFrequencyRowItem(xVals[j], yVals[j]));
                }
                MetricsToRows.Add(metrics[i].Metric, rows);
            }
            
            Rows = MetricsToRows[metrics[0].Metric];
        }

        private List<double> loadXData(int i)
        {
            List<double> xValues = new List<double>();
            xValues.Add(i / 10);
            xValues.Add(i / 2);

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
