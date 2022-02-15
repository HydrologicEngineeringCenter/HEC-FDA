using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel, IAlternativeResult
    {
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("chart title");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }

        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }

        /// <summary>
        /// Ctor for EAD versions which don't have discount rate and period of analysis.
        /// </summary>
        public DamageWithUncertaintyVM(double mean, double other)
        {
            RateAndPeriodVisible = false;
            //load with dummy data
            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
            loadDummyData(other);
            Mean = mean;
        }

        /// <summary>
        /// Ctor for AAEQ versions which have discount rate and period of analysis.
        /// </summary>
        public DamageWithUncertaintyVM(double discountRate, int periodOfAnalysis, double mean, double other)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            RateAndPeriodVisible = true;
            //load with dummy data
            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
            loadDummyData(other);
            Mean = mean;
        }

        private void loadDummyData(double other)
        {
            List<double> xVals = loadXData();
            List<double> yVals = loadYData(other);

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }
        }

        private List<double> loadXData()
        {
            List<double> xValues = new List<double>();
            xValues.Add(.75);
            xValues.Add(.5);
            xValues.Add(.25);

            return xValues;
        }

        private List<double> loadYData(double firstVal)
        {
            List<double> yValues = new List<double>();
            yValues.Add(firstVal);
            yValues.Add(firstVal+1);
            yValues.Add(firstVal + 2);
            return yValues;
        }

        public void PlotHistogram()
        {
            double binWidth = 5;
            double binStart = 2.5;
            double[] values = new double[] { 2, 2.5, 2.7, 3.5, 3.8, 1, 1.5 };

            HistogramData2D _data = new HistogramData2D(binWidth, binStart, values, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }

    }
}
