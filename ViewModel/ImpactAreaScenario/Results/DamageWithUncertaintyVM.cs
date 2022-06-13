using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;
using metrics;
using Statistics.Histograms;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Damage Uncertainty");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ImpactAreaScenarioResults iasResult, int impactAreaID)
        { 
            Mean = iasResult.ConsequenceResults.MeanDamage("Total", "Total", impactAreaID);
            IHistogram totalHistogram = iasResult.ConsequenceResults.GetConsequenceResult("Total", "Total", impactAreaID).ConsequenceHistogram;
            int[] binCounts = totalHistogram.BinCounts;
            double binWidth = totalHistogram.BinWidth;
            double min = totalHistogram.Min;
            double[] binsAsDoubles = binCounts.Select(x => (double)x).ToArray();
            _data = new HistogramData2D(binWidth, min, binsAsDoubles, "Chart", "Series", "X Data", "YData");
            ConsequenceResults eadResults = iasResult.ConsequenceResults;
            loadTableValues(eadResults);
        }

        private void loadTableValues(ConsequenceResults eadResults)
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

        private List<double> loadYData(List<double> xVals, ConsequenceResults eadResults)
        {
            List<double> yValues = new List<double>();
            foreach(double x in xVals)
            {
                //TODO: this is a WIP:
               // yValues.Add( eadResults.ConsequenceExceededWithProbabilityQ("Total",x, "Total", eadResults.RegionID));
            }
            return yValues;
        }

        public void PlotHistogram()
        {
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }
    }
}
