using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAEPVM : PerformanceVMBase
    {
        private static readonly Normal _standardNormal = new(0, 1);

        public ViewResolvingPlotModel MyPlot { get; } = new();
        public Dictionary<Threshold, Empirical> EmpiricalData { get; } = new();
        public bool ChartVisible { get; set; } = true;

        public PerformanceAEPVM(ScenarioResults iasResult, int impactAreaID, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, impactAreaID, thresholdComboItems);
            InitializePlotModel();
        }

        private void LoadData(ScenarioResults iasResult, int impactAreaID, List<ThresholdComboItem> thresholdComboItems)
        {
            for (int i = 0; i < thresholdComboItems.Count; i++)
            {
                Threshold threshold = thresholdComboItems[i].Metric;
                ThresholdEnum thresholdType = threshold.ThresholdType;
                int thresholdID = threshold.ThresholdID;
                double mean = iasResult.MeanAEP(impactAreaID, thresholdID);
                double median = iasResult.MedianAEP(impactAreaID, thresholdID);
                double ninetyPercentAssurance = iasResult.AEPWithGivenAssurance(impactAreaID, assurance: 0.9, thresholdID);
                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                //get the table values
                List<double> xVals = new List<double>() { .1, .04, .02, .01, .004, .002 };
                foreach (double xVal in xVals)
                {
                    double yVal = iasResult.AssuranceOfAEP(impactAreaID, xVal, thresholdID);
                    rows.Add(new PerformanceFrequencyRowItem(xVal, yVal));
                }

                MetricsToRows.Add(threshold, rows);
                ThresholdToMetrics.Add(threshold, (mean, median, ninetyPercentAssurance));
                LoadEmpiricalData(iasResult, impactAreaID, threshold);
            }

            if (MetricsToRows.Count > 0)
            {
                Rows = MetricsToRows.First().Value;
                Mean = ThresholdToMetrics.First().Value.Item1;
                Median = ThresholdToMetrics.First().Value.Item2;
                NinetyPercentAssurance = ThresholdToMetrics.First().Value.Item3;
            }
        }

        private void LoadEmpiricalData(ScenarioResults scenarioResults, int impactAreaID, Threshold threshold)
        {
            IHistogram histogramOfAEPs = scenarioResults.GetAEPHistogramForPlotting(impactAreaID, threshold.ThresholdID);

            if (histogramOfAEPs.BinCounts.Length <= 1)
            {
                ChartVisible = false;
            }
            else
            {
                Empirical empirical = DynamicHistogram.ConvertToEmpiricalDistribution(histogramOfAEPs);
                EmpiricalData.Add(threshold, empirical);
            }
        }

        #region OxyPlot
        private void InitializePlotModel()
        {
            MyPlot.Title = "AEP Assurance";
            AddAxes();

            // Add series for first threshold if available
            if (EmpiricalData.Count > 0 && MetricsToRows.Count > 0)
            {
                var firstThreshold = EmpiricalData.Keys.First();
                AddSeries(EmpiricalData[firstThreshold], MetricsToRows[firstThreshold]);
            }
        }

        private void AddSeries(Empirical empirical, List<IPerformanceRowItem> tableRows)
        {
            MyPlot.Series.Clear();

            // Add the CDF line from the empirical distribution
            var lineSeries = new LineSeries()
            {
                Title = "AEP CDF",
                // {0} = series title
                // {1} = X axis title
                // {2} = X value (z-score, displayed as AEP via axis formatter)
                // {3} = Y axis title
                // {4} = Y value (assurance)
                TrackerFormatString = "Series: {0}\nAEP: {2:0.0000}\nAssurance: {4:P2}",
                Color = OxyColors.Blue,
                StrokeThickness = 2,
            };

            // Plot AEP (quantiles) on X-axis as z-scores, Assurance (cumulative probabilities) on Y-axis
            for (int i = 0; i < empirical.CumulativeProbabilities.Length; i++)
            {
                double aep = empirical.Quantiles[i];
                double assurance = empirical.CumulativeProbabilities[i];
                double zScore = AepToZScore(aep);
                if (!double.IsInfinity(zScore) && !double.IsNaN(zScore))
                {
                    lineSeries.Points.Add(new DataPoint(zScore, assurance));
                }
            }

            MyPlot.Series.Add(lineSeries);

            // Add table data points as scatter markers
            var scatterSeries = new ScatterSeries()
            {
                Title = "Table Values",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.Red,
                // {2} = X value (z-score, displayed as AEP via axis formatter)
                // {4} = Y value (assurance)
                TrackerFormatString = "AEP: {Probability:N4}\nAssurance: {4:P2}",
            };

            foreach (var row in tableRows)
            {
                if (row is PerformanceFrequencyRowItem freqRow)
                {
                    // X = AEP (Frequency) as z-score, Y = Assurance
                    double zScore = AepToZScore(freqRow.Frequency);
                    if (!double.IsInfinity(zScore) && !double.IsNaN(zScore))
                    {
                        scatterSeries.Points.Add(new ScatterPoint(zScore, freqRow.AEP));
                    }
                }
            }

            MyPlot.Series.Add(scatterSeries);
            MyPlot.InvalidatePlot(true);
        }

        private static double AepToZScore(double aep)
        {
            // Clamp AEP to avoid infinity at extremes
            double clampedAep = Math.Max(0.0001, Math.Min(0.9999, aep));
            // Convert exceedance probability to non-exceedance, then to z-score
            return Normal.StandardNormalInverseCDF(1 - clampedAep);
        }

        private static string ProbabilityFormatter(double zScore)
        {
            // Convert z-score back to exceedance probability (AEP) for display
            double nonExceedance = _standardNormal.CDF(zScore);
            double aep = 1 - nonExceedance;
            return aep.ToString("N4");
        }

        private void AddAxes()
        {
            // X-axis: AEP (Annual Exceedance Probability) using normal transform
            // Z-scores are used internally, with labels showing probabilities
            // Large AEPs (50%) on right, small AEPs (0.1%) on left
            LinearAxis x = new()
            {
                Position = AxisPosition.Bottom,
                Title = StringConstants.EXCEEDANCE_PROBABILITY,
                LabelFormatter = ProbabilityFormatter,
            };

            // Y-axis: Assurance
            LinearAxis y = new()
            {
                Position = AxisPosition.Left,
                Title = "Assurance",
                MinorTickSize = 0,
                StringFormat = "P0",
                Minimum = 0,
                Maximum = 1,
            };

            MyPlot.Axes.Add(x);
            MyPlot.Axes.Add(y);
        }
        #endregion

        public override void UpdateHistogram(ThresholdComboItem metric)
        {
            if (EmpiricalData.ContainsKey(metric.Metric) && MetricsToRows.ContainsKey(metric.Metric))
            {
                AddSeries(EmpiricalData[metric.Metric], MetricsToRows[metric.Metric]);
            }
        }
    }
}
