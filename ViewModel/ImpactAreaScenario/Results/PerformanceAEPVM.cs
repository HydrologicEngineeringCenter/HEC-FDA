using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAEPVM : PerformanceVMBase
    {
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Performance");

        public PerformanceAEPVM(List<ThresholdComboItem> metrics)
        {
            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
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
            xValues.Add(i/10);
            xValues.Add(i/2);
          
            return xValues;
        }

        private List<double> loadYData(int i)
        {
            List<double> yValues = new List<double>();
            yValues.Add(i);
            yValues.Add(i+1);
      
            return yValues;
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
