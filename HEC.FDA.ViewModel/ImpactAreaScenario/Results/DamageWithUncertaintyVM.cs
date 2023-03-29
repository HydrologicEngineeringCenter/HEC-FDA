using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System.Linq;
using Statistics.Histograms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.Model.metrics;
using Statistics.Distributions;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        private HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Damage Uncertainty");
        public bool HistogramVisible { get; set; } = true;

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ScenarioResults scenarioResults, int impactAreaID)
        {
            ImpactAreaScenarioResults iasResult = scenarioResults.GetResults(impactAreaID);
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
            Empirical empirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(impactAreaID: iasResult.ImpactAreaID);
            if (empirical.CumulativeProbabilities.Length <= 1)
            {
                HistogramVisible = false;
            }
            else
            {
                (double min, double valueStep, double[] cumulativeRelativeFrequencies) = empirical.ComputeCumulativeFrequenciesForPlotting();
                _data = new HistogramData2D(valueStep, min, cumulativeRelativeFrequencies, "Chart", "Cumulative Relative Frequency", StringConstants.HISTOGRAM_VALUE, StringConstants.HISTOGRAM_FREQUENCY);
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
