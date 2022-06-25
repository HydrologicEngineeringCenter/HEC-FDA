using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;
using metrics;
using Statistics.Histograms;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        private readonly HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Damage Uncertainty");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ImpactAreaScenarioResults iasResult)
        {
            int impactAreaID = iasResult.ImpactAreaID;
            Mean = iasResult.ConsequenceResults.MeanDamage(impactAreaID: impactAreaID);
            IHistogram totalHistogram = iasResult.ConsequenceResults.GetConsequenceResultsHistogram(impactAreaID: impactAreaID);
            double[] binsAsDoubles = totalHistogram.BinCounts.Select(x => (double)x).ToArray();

            _data = new HistogramData2D(totalHistogram.BinWidth, totalHistogram.Min, binsAsDoubles, "Chart", "Series", StringConstants.HISTOGRAM_VALUE, StringConstants.HISTOGRAM_FREQUENCY);
            HistogramColor.SetHistogramColor(_data);
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });

            ConsequenceDistributionResults eadResults = iasResult.ConsequenceResults;
            loadTableValues(eadResults);

        }

        private void loadTableValues(ConsequenceDistributionResults eadResults)
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

        private List<double> loadYData(List<double> xVals, ConsequenceDistributionResults eadResults)
        {
            List<double> yValues = new List<double>();
            foreach(double x in xVals)
            {
                yValues.Add( eadResults.ConsequenceExceededWithProbabilityQ(x));
            }
            return yValues;
        }

    }
}
