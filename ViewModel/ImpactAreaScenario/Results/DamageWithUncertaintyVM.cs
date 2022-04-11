using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;
using metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Damage Uncertainty");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(metrics.Results iasResult)
        { 
            Mean = iasResult.ExpectedAnnualDamageResults.MeanEAD("Total");
            Statistics.Histograms.ThreadsafeInlineHistogram totalHistogram = iasResult.ExpectedAnnualDamageResults.HistogramsOfEADs["Total"];
            int[] binCounts = totalHistogram.BinCounts;
            double binWidth = totalHistogram.BinWidth;
            double min = totalHistogram.Min;
            double[] binsAsDoubles = binCounts.Select(x => (double)x).ToArray();
            _data = new HistogramData2D(binWidth, min, binsAsDoubles, "Chart", "Series", "X Data", "YData");
            ExpectedAnnualDamageResults eadResults = iasResult.ExpectedAnnualDamageResults;
            loadTableValues(eadResults);
        }

        private void loadTableValues(ExpectedAnnualDamageResults eadResults)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<double> yVals = loadYData(xVals, eadResults);

            List<EadRowItem> rows = new List<EadRowItem>();
            for(int i = 0;i<xVals.Count;i++)
            {
                rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }

            Rows.AddRange( rows);
        }

        private List<double> loadYData(List<double> xVals, ExpectedAnnualDamageResults eadResults)
        {
            List<double> yValues = new List<double>();
            foreach(double x in xVals)
            {
                yValues.Add( eadResults.EADExceededWithProbabilityQ("Total", x));
            }
            return yValues;
        }

        public void PlotHistogram()
        {
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }
    }
}
