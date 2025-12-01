using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.Model.metrics;
using Statistics.Distributions;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        public ViewResolvingPlotModel MyPlot { get; } = new();
        public List<EadRowItem> Rows { get; } = new();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ScenarioResults scenarioResults, int impactAreaID)
        {
            ImpactAreaScenarioResults iasResult = scenarioResults.GetResults(impactAreaID);
            Mean = iasResult.MeanExpectedAnnualConsequences(impactAreaID: impactAreaID);
            Empirical empirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(impactAreaID: iasResult.ImpactAreaID);
            InitializePlotModel(empirical);

            List<double> qValues = new()
            {
                scenarioResults.ConsequencesExceededWithProbabilityQ(.75, impactAreaID),
                scenarioResults.ConsequencesExceededWithProbabilityQ(.5, impactAreaID),
                scenarioResults.ConsequencesExceededWithProbabilityQ(.25, impactAreaID)
            };

            LoadTableValues(qValues);
        }

        #region OxyPlot
        private void InitializePlotModel(Empirical empirical)
        {
            MyPlot.Title = StringConstants.EAD_DISTRIBUTION;
            AddAxes();
            AddSeries(empirical);
        }
        private void AddSeries(Empirical empirical)
        {
            var lineSeries = new LineSeries()
            {
                DataFieldX = nameof(NormalDataPoint.ZScore),
                DataFieldY = nameof(NormalDataPoint.Value),
                TrackerFormatString = "X: {Probability:0.####}, Y: {Value:C0}",
                Title = StringConstants.EAD_DISTRIBUTION,
            };
            var points = new NormalDataPoint[empirical.CumulativeProbabilities.Length];
            for (int i = 0; i < points.Length; i++)
            {
                double exceedenceProbability = 1 - empirical.CumulativeProbabilities[i];
                double zScore = Normal.StandardNormalInverseCDF(exceedenceProbability);
                points[i] = new NormalDataPoint(exceedenceProbability, zScore, empirical.Quantiles[i]);
            }
            lineSeries.ItemsSource = points;
            MyPlot.Series.Add(lineSeries);
            MyPlot.InvalidatePlot(true);
        }
        private void AddAxes()
        {
            LinearAxis x = new()
            {
                Position = AxisPosition.Bottom,
                Title = StringConstants.EXCEEDANCE_PROBABILITY,
                LabelFormatter = ProbabilityFormatter,
                Maximum = 3.719, //probability of .9999
                Minimum = -3.719, //probability of .0001
                StartPosition = 1,
                EndPosition = 0
            };
            LinearAxis y = new()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.EXPECTED_ANNUAL_DAMAGE,
                MinorTickSize = 0,
                StringFormat = "C0",

            };
            MyPlot.Axes.Add(x);
            MyPlot.Axes.Add(y);
        }
        private static string ProbabilityFormatter(double d)
        {
            Normal standardNormal = new(0, 1);
            double value = standardNormal.CDF(d);
            string stringval = value.ToString("0.0000");
            return stringval;
        }
        #endregion


        private void LoadTableValues(List<double> qValues)
        {
            List<EadRowItem> rows = new();
            if (qValues.Count == 3)
            {
                List<string> xValNames = new() { "First", "Second", "Third" };

                for (int i = 0; i < xValNames.Count; i++)
                {
                    rows.Add(new EadRowItem(xValNames[i], qValues[i]));
                }
            }
            Rows.AddRange(rows);
        }

    }
}
