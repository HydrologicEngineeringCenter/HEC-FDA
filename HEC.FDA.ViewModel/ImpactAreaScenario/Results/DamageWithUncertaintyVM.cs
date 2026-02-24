using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Statistics.Distributions;
using System.Collections.Generic;
using System.Linq;
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
            IUncertaintyControlConfig uncertaintyConfig = null)
        {
            uncertaintyConfig ??= new DamageWithUncertaintyControlConfig(); // default to damage config
            _uncertaintyControlConfig = uncertaintyConfig;

            ImpactAreaScenarioResults iasResult = scenarioResults.GetResults(impactAreaID);
            Mean = iasResult.MeanExpectedAnnualConsequences(impactAreaID: impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType);

            // Detect if NonFail results exist
            bool hasNonFailResults = iasResult.ConsequenceResults.ConsequenceResultList
                .Any(r => r.RiskType == RiskType.Non_Fail && r.ConsequenceType == _uncertaintyControlConfig.ConsequenceType);

            if (hasNonFailResults)
            {
                // Get empirical distributions for each risk type
                Empirical failEmpirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(
                    impactAreaID: iasResult.ImpactAreaID,
                    consequenceType: _uncertaintyControlConfig.ConsequenceType,
                    riskType: RiskType.Fail);
                Empirical nonFailEmpirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(
                    impactAreaID: iasResult.ImpactAreaID,
                    consequenceType: _uncertaintyControlConfig.ConsequenceType,
                    riskType: RiskType.Non_Fail);
                Empirical totalEmpirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(
                    impactAreaID: iasResult.ImpactAreaID,
                    consequenceType: _uncertaintyControlConfig.ConsequenceType,
                    riskType: RiskType.Total);

                // Initialize plot with 3 series
                InitializePlotModelMultiple(
                    (failEmpirical, "Fail", OxyColors.Red),
                    (nonFailEmpirical, "Non-Fail", OxyColors.Blue),
                    (totalEmpirical, "Total", OxyColors.Black));

                // Load 9 table rows (3 risk types x 3 quartiles)
                LoadTableValuesGrouped(scenarioResults, impactAreaID);
            }
            else
            {
                // Current behavior - single series labeled as distribution title
                Empirical empirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(
                    impactAreaID: iasResult.ImpactAreaID,
                    consequenceType: _uncertaintyControlConfig.ConsequenceType);

                List<double> qValues = new()
                {
                    scenarioResults.ConsequencesExceededWithProbabilityQ(.25, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType),
                    scenarioResults.ConsequencesExceededWithProbabilityQ(.5, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType),
                    scenarioResults.ConsequencesExceededWithProbabilityQ(.75, impactAreaID, consequenceType: _uncertaintyControlConfig.ConsequenceType)
                };
                LoadTableValues(qValues);

                InitializePlotModel(empirical);
            }
        }

        #region OxyPlot
        private void InitializePlotModel(Empirical empirical)
        {
            MyPlot.Title = _uncertaintyControlConfig.PlotTitle;
            AddAxes();
            AddSeries(empirical);
        }

        private void InitializePlotModelMultiple(params (Empirical empirical, string title, OxyColor color)[] seriesData)
        {
            MyPlot.Title = _uncertaintyControlConfig.PlotTitle;
            MyPlot.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopRight
            });
            AddAxes();

            foreach (var (empirical, title, color) in seriesData)
            {
                AddSeries(empirical, title, color);
            }
        }

        private void AddSeries(Empirical empirical, string title = null, OxyColor? color = null)
        {
            string trackerFormat = _uncertaintyControlConfig.TrackerFormat;

            var lineSeries = new LineSeries()
            {
                DataFieldX = nameof(NormalDataPoint.ZScore),
                DataFieldY = nameof(NormalDataPoint.Value),
                TrackerFormatString = trackerFormat,
                Title = title ?? _uncertaintyControlConfig.PlotTitle,
            };

            if (color.HasValue)
            {
                lineSeries.Color = color.Value;
            }

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

        private void LoadTableValues(List<double> qValues)
        {
            List<IQuartileRowItem> rows = new();
            if (qValues.Count == 3)
            {
                List<string> xValNames = new() { "25%", "50%", "75%" };

                for (int i = 0; i < xValNames.Count; i++)
                {
                    rows.Add(_uncertaintyControlConfig.CreateRowItem(xValNames[i], qValues[i], RiskType.Total.ToString()));
                }
            }
            Rows.AddRange(rows);
        }

        private void LoadTableValuesGrouped(ScenarioResults scenarioResults, int impactAreaID)
        {
            var riskTypes = new[] { ("Fail", RiskType.Fail), ("Non-Fail", RiskType.Non_Fail), ("Total", RiskType.Total) };
            var quartileNames = new[] { "25%", "50%", "75%" };
            var probabilities = new[] { 0.25, 0.5, 0.75 };

            foreach (var (riskTypeName, riskType) in riskTypes)
            {
                for (int i = 0; i < quartileNames.Length; i++)
                {
                    double value = scenarioResults.ConsequencesExceededWithProbabilityQ(
                        probabilities[i], impactAreaID,
                        consequenceType: _uncertaintyControlConfig.ConsequenceType,
                        riskType: riskType);
                    Rows.Add(_uncertaintyControlConfig.CreateRowItem(quartileNames[i], value, riskTypeName));
                }
            }
        }
    }
}
