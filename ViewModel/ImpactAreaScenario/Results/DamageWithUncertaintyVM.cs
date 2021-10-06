using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario.Results.RowItems;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("chart title");

        public List<EadRowItem> Rows { get; set; }

        public DamageWithUncertaintyVM()
        {
            loadDummyData();
        }


        private void loadDummyData()
        {
            List<string> xVals = loadXData();
            List<double> yVals = loadYData();

            List<EadRowItem> rows = new List<EadRowItem>();
            for(int i = 0;i<xVals.Count;i++)
            {
                rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }

            Rows = rows;
        }

        private List<string> loadXData()
        {
            List<string>  xValues = new List<string>();
            xValues.Add("Mean");
            xValues.Add("First Quartile");
            xValues.Add("Median");
            xValues.Add("Third Quartile");

            return xValues;
        }

        private List<double> loadYData()
        {
            List<double> yValues = new List<double>();
            yValues.Add(1);
            yValues.Add(2);
            yValues.Add(3);
            yValues.Add(4);
            return yValues;
        }


        public void PlotLineData()
        {
            List<double> xValues = new List<double>();
            xValues.Add(1);
            xValues.Add(2);

            List<double> yValues = new List<double>();
            yValues.Add(1);
            yValues.Add(2);

            SciLineData lineData = new NumericLineData(xValues.ToArray(), yValues.ToArray(), "asdf", "asdf", "adsf", "asdf", PlotType.Line);
            ChartViewModel.LineData.Set(new List<SciLineData>() { lineData });
        }

    }
}
