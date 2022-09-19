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
        private HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Damage Uncertainty");
        public bool HistogramVisible { get; set; } = true;

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ImpactAreaScenarioResults iasResult, ScenarioResults scenarioResults)
        {
            int impactAreaID = iasResult.ImpactAreaID;
            Mean = iasResult.MeanExpectedAnnualConsequences(impactAreaID: impactAreaID);

            LoadHistogram(iasResult);

            List<double> qValues = new List<double>();
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.75, impactAreaID));
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.5, impactAreaID));
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.25, impactAreaID));

            loadTableValues(qValues);
        }

        private void LoadHistogram(ImpactAreaScenarioResults iasResult)
        {
            IHistogram totalHistogram = iasResult.ConsequenceResults.GetConsequenceResultsHistogram(impactAreaID: iasResult.ImpactAreaID);
            double[] binsAsDoubles = totalHistogram.BinCounts.Select(x => (double)x / totalHistogram.SampleSize).ToArray();
            if (binsAsDoubles.Length == 0 || binsAsDoubles.Length == 1)
            {
                HistogramVisible = false;
            }
            else
            {
                _data = new HistogramData2D(totalHistogram.BinWidth, totalHistogram.Min, binsAsDoubles, "Chart", "Series", StringConstants.HISTOGRAM_VALUE, StringConstants.HISTOGRAM_FREQUENCY);
                HistogramColor.SetHistogramColor(_data);
                ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
            }
        }

        private void loadTableValues(List<double> qValues)
        {
            List<EadRowItem> rows = new List<EadRowItem>();
            if (qValues.Count == 3)
            {
                List<double> xVals = new List<double>() { .75, .5, .25 };
                List<string> xValNames = new List<string>() { "First", "Second", "Third" };

                for (int i = 0; i < xValNames.Count; i++)
                {
                    rows.Add(new EadRowItem(xValNames[i], qValues[i]));
                }
            }
            Rows.AddRange( rows);
        }

    }
}
