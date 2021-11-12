using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : AlternativeResultBase
    {

        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("chart title");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM():base()
        {
            //load with dummy data
            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
            loadDummyData();
            Mean = .123;
        }


        private void loadDummyData()
        {
            List<double> xVals = loadXData();
            List<double> yVals = loadYData();

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

        private List<double> loadYData()
        {
            List<double> yValues = new List<double>();
            yValues.Add(1);
            yValues.Add(2);
            yValues.Add(3);
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
