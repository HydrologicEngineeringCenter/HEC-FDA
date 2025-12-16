using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using static HEC.FDA.ViewModel.ImpactAreaScenario.Results.UncertaintyControlConfigs;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        private readonly IUncertaintyControlConfig _uncertaintyControlConfig;
        public ViewResolvingPlotModel MyPlot { get; } = new();
        public List<IQuartileRowItem> Rows { get; } = new();
        public double Mean { get; set; }
        public string FormattedMean => Mean.ToString(_uncertaintyControlConfig.MeanFormat);

        public DamageWithUncertaintyVM(
            ScenarioResults scenarioResults,
            int impactAreaID,
            Func<string, double, IQuartileRowItem> rowItemFactory = null,
            IUncertaintyControlConfig uncertaintyConfig = null)
        {
            rowItemFactory ??= (frequency, value) => new EadRowItem(frequency, value); // default to damage row item
            uncertaintyConfig ??= new DamageWithUncertaintyControlConfig(); // default to damage config
            _uncertaintyControlConfig = uncertaintyConfig;

            ImpactAreaScenarioResults iasResult = scenarioResults.GetResults(impactAreaID);
            Mean = iasResult.MeanExpectedAnnualConsequences(impactAreaID: impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType);
            Empirical empirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(impactAreaID: iasResult.ImpactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType);

            List<double> qValues = new()
            {
                scenarioResults.ConsequencesExceededWithProbabilityQ(.75, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType),
                scenarioResults.ConsequencesExceededWithProbabilityQ(.5, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType),
                scenarioResults.ConsequencesExceededWithProbabilityQ(.25, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType)
            };
            LoadTableValues(qValues, rowItemFactory);

            InitializePlotModel(empirical);
        }

        #region OxyPlot
        private void InitializePlotModel(Empirical empirical)
        {
            MyPlot.Title = _uncertaintyControlConfig.PlotTitle;
            AddAxes();
            AddSeries(empirical);
        }

        private void AddSeries(Empirical empirical)
        {
            string trackerFormat = _uncertaintyControlConfig.TrackerFormat;

            var lineSeries = new LineSeries()
            {
                DataFieldX = nameof(NormalDataPoint.ZScore),
                DataFieldY = nameof(NormalDataPoint.Value),
                TrackerFormatString = trackerFormat,
                Title = _uncertaintyControlConfig.PlotTitle,
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

            string yAxisTitle = _uncertaintyControlConfig.YAxisTitle;
            string yAxisFormat = _uncertaintyControlConfig.YAxisFormat;

            LinearAxis y = new()
            {
                Position = AxisPosition.Left,
                Title = yAxisTitle,
                MinorTickSize = 0,
                StringFormat = yAxisFormat,
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

        private void LoadTableValues(List<double> qValues, Func<string, double, IQuartileRowItem> rowItemFactory)
        {
            List<IQuartileRowItem> rows = new();
            if (qValues.Count == 3)
            {
                List<string> xValNames = new() { "First", "Second", "Third" };

                for (int i = 0; i < xValNames.Count; i++)
                {
                    rows.Add(rowItemFactory(xValNames[i], qValues[i]));
                }
            }
            Rows.AddRange(rows);
        }
    }
}