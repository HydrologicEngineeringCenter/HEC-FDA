using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using metrics;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAEPVM : PerformanceVMBase
    {

        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Performance");
        public Dictionary<Threshold, HistogramData2D> HistogramData { get; } = new Dictionary<Threshold, HistogramData2D>();

        public PerformanceAEPVM(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, thresholdComboItems);
        }

        private void LoadData(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
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
                    LoadHistogramData(performanceResults, threshold);
                }
            }

            if(MetricsToRows.Count>0)
            {
                Rows = MetricsToRows.First().Value;
            }
        }

        private void LoadHistogramData(SystemPerformanceResults performanceResults, Threshold threshold)
        {
            ThreadsafeInlineHistogram histogramOfAEPs = performanceResults.GetAEPHistogram();
            int[] binCounts = histogramOfAEPs.BinCounts;
            double min = histogramOfAEPs.Min;
            double[] binsAsDoubles = binCounts.Select(x => (double)x).ToArray();

            HistogramData2D data = new HistogramData2D(histogramOfAEPs.BinWidth, histogramOfAEPs.Min, binsAsDoubles, "Chart", "Series", StringConstants.HISTOGRAM_EXCEEDANCE_PROBABILITY, StringConstants.HISTOGRAM_FREQUENCY);
            HistogramColor.SetHistogramColor(data);
            HistogramData.Add(threshold, data);
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
                HistogramColor.SetHistogramColor(histData);
                ChartViewModel.LineData.Set(new List<SciLineData>() { histData });
            }
        }
    }
}
