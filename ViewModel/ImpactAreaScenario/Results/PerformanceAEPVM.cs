using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAEPVM : PerformanceVMBase
    {
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Performance");
        public Dictionary<Threshold, HistogramData2D> HistogramData { get; } = new Dictionary<Threshold, HistogramData2D>();

        public PerformanceAEPVM(metrics.ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, thresholdComboItems);
        }

        private void LoadData(metrics.ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            for (int i = 0; i < thresholdComboItems.Count; i++)
            {
                Threshold threshold = thresholdComboItems[i].Metric;
                ThresholdEnum thresholdType = threshold.ThresholdType;
                SystemPerformanceResults performanceResults = GetResultsOfType(iasResult, thresholdType);

                if (performanceResults != null)
                {
                    List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                    //get the table values
                    List<double> xVals = new List<double>() { .1, .04, .02, .01, .005, .002 };
                    foreach (double xVal in xVals)
                    {
                        double yVal = performanceResults.AssuranceOfAEP(xVal);
                        rows.Add(new PerformanceFrequencyRowItem(xVal, yVal));
                    }

                    MetricsToRows.Add(threshold, rows);

                    //get the histogram data
                    //Statistics.Histograms.ThreadsafeInlineHistogram histogramOfAEPs = performanceResults.GetAssurance("AEP").AssuranceHistogram;
                    //int[] binCounts = histogramOfAEPs.BinCounts;
                    //double binWidth = histogramOfAEPs.BinWidth;
                    //double min = histogramOfAEPs.Min;
                    //double[] binsAsDoubles = binCounts.Select(x => (double)x).ToArray();
                    //HistogramData2D _data = new HistogramData2D(binWidth, min, binsAsDoubles, "Chart", "Series", "X Data", "YData");
                    //HistogramData.Add(threshold, _data);
                }
            }

            if(MetricsToRows.Count>0)
            {
                Rows = MetricsToRows.First().Value;
            }
        }

        private SystemPerformanceResults GetResultsOfType(ImpactAreaScenarioResults iasResult, ThresholdEnum thresholdType)
        {
            SystemPerformanceResults retval = null;
            foreach (Threshold threshold in iasResult.PerformanceByThresholds.ListOfThresholds)
            {
                if(threshold.ThresholdType == thresholdType)
                {
                    retval = threshold.SystemPerformanceResults;
                    break;
                }
            }
            return retval;
        }

        public override void UpdateHistogram(ThresholdComboItem metric)
        {
            if (HistogramData.ContainsKey(metric.Metric))
            {
                HistogramData2D histData = HistogramData[metric.Metric];
                ChartViewModel.LineData.Set(new List<SciLineData>() { histData });
            }
        }
    }
}
