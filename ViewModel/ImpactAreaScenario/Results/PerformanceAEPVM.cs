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
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Performance");
        public Dictionary<Threshold, HistogramData2D> HistogramData { get; set; } = new Dictionary<Threshold, HistogramData2D>();


        public PerformanceAEPVM(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {

            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);

            LoadData(iasResult, metrics);
        }

        private void LoadData(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {

            for (int i = 0; i < metrics.Count; i++)
            {
                Threshold threshold = metrics[i].Metric;
                ThresholdEnum thresholdType = threshold.ThresholdType;
                ProjectPerformanceResults performanceResults = GetResultsOfType(iasResult, thresholdType);

                if (performanceResults != null)
                {
                    //get the table values
                    List<double> xVals = new List<double>() { .1, .04, .02, .01, .005, .002 };
                    List<double> yVals = new List<double>();
                    foreach (double xVal in xVals)
                    {
                        yVals.Add(performanceResults.AssuranceOfAEP(xVal));
                    }

                    List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                    for (int j = 0; j < xVals.Count; j++)
                    {
                        rows.Add(new PerformanceFrequencyRowItem(xVals[j], yVals[j]));
                    }
                    MetricsToRows.Add(threshold, rows);

                    //get the histogram data
                    Statistics.Histograms.ThreadsafeInlineHistogram histogramOfAEPs = performanceResults.HistogramOfAEPs;
                    int[] binCounts = histogramOfAEPs.BinCounts;
                    double binWidth = histogramOfAEPs.BinWidth;
                    double min = histogramOfAEPs.Min;
                    double[] binsAsDoubles = binCounts.Select(x => (double)x).ToArray();
                    ////load with dummy data
                    HistogramData2D _data = new HistogramData2D(binWidth, min, binsAsDoubles, "Chart", "Series", "X Data", "YData");
                    HistogramData.Add(threshold, _data);
                }

            }

            Rows = MetricsToRows[metrics[0].Metric];
        }

        private ProjectPerformanceResults GetResultsOfType(metrics.Results iasResult, ThresholdEnum thresholdType)
        {
            ProjectPerformanceResults retval = null;
            foreach (KeyValuePair<int, Threshold> result in iasResult.PerformanceByThresholds.ThresholdsDictionary)
            {
                if(result.Value.ThresholdType == thresholdType)
                {
                    retval = result.Value.ProjectPerformanceResults;
                    break;
                }
            }
            return retval;

        }

        //private List<double> loadXData(int i)
        //{
        //    List<double> xValues = new List<double>();
        //    xValues.Add(i/10);
        //    xValues.Add(i/2);

        //    return xValues;
        //}

        //private List<double> loadYData(int i)
        //{
        //    List<double> yValues = new List<double>();
        //    yValues.Add(i);
        //    yValues.Add(i+1);

        //    return yValues;
        //}

        public override void UpdateHistogram(ThresholdComboItem metric)
        {
            if (HistogramData.ContainsKey(metric.Metric))
            {
                HistogramData2D histData = HistogramData[metric.Metric];
                ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
            }
        }
        public void PlotHistogram()
        {
            double binWidth = 5;
            double binStart = 2.5;
            double[] values = new double[] { 2, 2.5, 2.7, 3.5, 3.8, 1, 1.5 };

            //todo: update this logic with the updateHistogram call once Ryan fixes it.
            HistogramData2D _data = new HistogramData2D(binWidth, binStart, values, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }

    }
}
