using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel, IAlternativeResult
    {
        public ViewResolvingPlotModel MyPlot { get; } = new ViewResolvingPlotModel();
        public bool HistogramVisible { get; set; } = true;
        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }
        public string ProbabilityExceedsValueLabel { get; }

        public DamageWithUncertaintyVM(AlternativeResults results, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int periodOfAnalysis = -1)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
                ProbabilityExceedsValueLabel = StringConstants.ALTERNATIVE_EAD_LABEL;
            }
            else
            {
                RateAndPeriodVisible = true;
                ProbabilityExceedsValueLabel = StringConstants.ALTERNATIVE_AAEQ_LABEL;
            }
            LoadHistogramData(results, damageMeasureYear);
            LoadData(results, damageMeasureYear);

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    Mean = results.MeanBaseYearEAD();
                    break;
                case DamageMeasureYear.Future:
                    Mean = results.MeanFutureYearEAD();
                    break;
                case DamageMeasureYear.AAEQ:
                    Mean = results.MeanAAEQDamage();
                    break;
            }
        }

        public DamageWithUncertaintyVM( AlternativeComparisonReportResults altCompReport, int altID, DamageMeasureYear damageMeasureYear, double discountRate = double.NaN, int periodOfAnalysis = -1)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;

            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
                ProbabilityExceedsValueLabel = StringConstants.ALTERNATIVE_COMP_REPORT_EAD_LABEL;
            }
            else
            {
                RateAndPeriodVisible = true;
                ProbabilityExceedsValueLabel = StringConstants.ALTERNATIVE_COMP_REPORT_AAEQ_LABEL;
            }

            LoadHistogramData(altCompReport, altID, damageMeasureYear);

            LoadAAEQData(altCompReport, altID, damageMeasureYear);

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    Mean = altCompReport.MeanBaseYearEADReduced(altID);
                    break;
                case DamageMeasureYear.Future:
                    Mean = altCompReport.MeanFutureYearEADReduced(altID);
                    break;
                case DamageMeasureYear.AAEQ:
                    Mean = altCompReport.MeanAAEQDamageReduced(altID);
                    break;
            }

        }

        private void LoadAAEQData(AlternativeComparisonReportResults altResults, int altID, DamageMeasureYear damageMeasureYear)
        {
            List<double> xVals = new() { .75, .5, .25 };
            List<string> xValNames = new() { "First", "Second", "Third" };
            List<double> yVals = LoadYData(xVals, altResults, altID, damageMeasureYear);

            for (int i = 0; i < xValNames.Count; i++)
            {
                Rows.Add(new EadRowItem(xValNames[i], yVals[i]));
            }
        }

        private void LoadHistogramData(AlternativeResults altResults, DamageMeasureYear damageMeasureYear)
        {
            Empirical empirical = null;

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    empirical = altResults.GetBaseYearEADDistribution();
                    break;
                case DamageMeasureYear.Future:
                    empirical = altResults.GetFutureYearEADDistribution();
                    break;
                case DamageMeasureYear.AAEQ:
                    empirical = altResults.GetAAEQDamageDistribution();
                    break;
            }

            CreateHistogramData(empirical, true);
        }

        private void LoadHistogramData(AlternativeComparisonReportResults altResults, int altID, DamageMeasureYear damageMeasureYear)
        {
            Empirical empirical = null;

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    empirical = altResults.GetBaseYearEADReducedResultsHistogram(altID);
                    break;
                case DamageMeasureYear.Future:
                    empirical = altResults.GetFutureYearEADReducedResultsHistogram(altID);
                    break;
                case DamageMeasureYear.AAEQ:
                    empirical = altResults.GetAAEQReducedResultsHistogram(altID);
                    break;
            }

            CreateHistogramData(empirical, false);
        }

        private void CreateHistogramData(Empirical empirical, bool isAlternative)
        {
            if (empirical != null)
            {
                if (empirical.CumulativeProbabilities.Length <=1)
                {
                    HistogramVisible = false;
                }
                else
                {
                    InitializePlotModel(empirical, isAlternative);
                }
            }
            else
            {
                HistogramVisible = false;
            }
        }

        private void InitializePlotModel(Empirical empirical, bool isAlternative)
        {
            if(isAlternative)
            {
                MyPlot.Title = StringConstants.AAEQ_DAMAGE_DISTRIBUTION;
            }
            else
            {
                MyPlot.Title = StringConstants.DAMAGE_REDUCED;
            }
            AddAxes();
            AddSeries(empirical);
        }

        private void AddSeries(Empirical empirical)
        {
            var lineSeries = new LineSeries()
            {
                DataFieldX = nameof(NormalDataPoint.ZScore),
                DataFieldY = nameof(NormalDataPoint.Value),
                TrackerFormatString = "X: {Probability:0.####}, Y: {Value:F2}",
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
                Unit = "$",
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

        private void LoadData(AlternativeResults scenarioResults, DamageMeasureYear damageMeasureYear)
        {
            List<double> xVals = new() { .75, .5, .25 };
            List<string> xValNames = new() { "First", "Second", "Third" };
            List<double> yVals = LoadYData(xVals, scenarioResults, damageMeasureYear);

            for (int i = 0; i < xValNames.Count; i++)
            {
                Rows.Add(new EadRowItem(xValNames[i], yVals[i]));
            }
        }

        private static List<double> LoadYData(List<double> xVals, AlternativeResults results, DamageMeasureYear damageMeasureYear)
        {
            List<double> yValues = new();
            foreach (double x in xVals)
            {
                switch (damageMeasureYear)
                {
                    case DamageMeasureYear.Base:
                        yValues.Add(results.BaseYearEADDamageExceededWithProbabilityQ(x));
                        break;
                    case DamageMeasureYear.Future:
                        yValues.Add(results.FutureYearEADDamageExceededWithProbabilityQ(x));
                        break;
                    case DamageMeasureYear.AAEQ:
                        yValues.Add(results.AAEQDamageExceededWithProbabilityQ(x));
                        break;
                }
            }
            return yValues;
        }

        private static List<double> LoadYData(List<double> xVals, AlternativeComparisonReportResults scenarioResults, int altID, DamageMeasureYear damageMeasureYear)
        {
            List<double> yValues = new();
            foreach (double x in xVals)
            {
                switch (damageMeasureYear)
                {
                    case DamageMeasureYear.Base:
                        yValues.Add(scenarioResults.BaseYearEADReducedExceededWithProbabilityQ(x, altID));
                        break;
                    case DamageMeasureYear.Future:
                        yValues.Add(scenarioResults.FutureYearEADReducedExceededWithProbabilityQ(x, altID));
                        break;
                    case DamageMeasureYear.AAEQ:
                        yValues.Add(scenarioResults.AAEQDamageReducedExceededWithProbabilityQ(x, altID));
                        break;
                }
            }
            return yValues;
        }

    }
}
