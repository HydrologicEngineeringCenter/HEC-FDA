using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;
using System.Collections.Generic;
using System.Windows.Markup.Localizer;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel, IAlternativeResult
    {
        public ViewResolvingPlotModel MyPlot { get; } = new ViewResolvingPlotModel();
        public bool HistogramVisible { get; set; } = true;
        public List<IQuartileRowItem> Rows { get; } = [];
        public double Mean { get; set; }
        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }
        public string QuartileLabel { get; }
        private readonly DamageMeasureYear _damageMeasureYear;
        private const string QUARTILE_EAD = "Quartile of EAD Distribution";
        private const string QUARTILE_EQAD = "Quartile of EqAD Distribution";
        private const string QUARTILE_REDUCED_EAD = "Quartile of EAD Reduced Distribution";
        private const string QUARTILE_REDUCED_EQAD = "Quartile of EqAD Reduced Distribution";

        public DamageWithUncertaintyVM(AlternativeResults results, DamageMeasureYear damageMeasureYear,
            double discountRate = double.NaN, int periodOfAnalysis = -1)
        {
            _damageMeasureYear = damageMeasureYear;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
            }
            else
            {
                RateAndPeriodVisible = true;
            }
            LoadHistogramData(results, damageMeasureYear);
            LoadData(results, damageMeasureYear);

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    Mean = results.SampleMeanBaseYearEAD();
                    QuartileLabel = QUARTILE_EAD;
                    break;
                case DamageMeasureYear.Future:
                    Mean = results.SampleMeanFutureYearEAD();
                    QuartileLabel = QUARTILE_EAD;
                    break;
                case DamageMeasureYear.Eqad:
                    Mean = results.SampleMeanEqad();
                    QuartileLabel = QUARTILE_EQAD;
                    break;
            }
        }

        public DamageWithUncertaintyVM(AlternativeComparisonReportResults altCompReport, int altID, DamageMeasureYear damageMeasureYear,
            double discountRate = double.NaN, int periodOfAnalysis = -1)
        {
            _damageMeasureYear = damageMeasureYear;
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;

            if (double.IsNaN(discountRate))
            {
                RateAndPeriodVisible = false;
            }
            else
            {
                RateAndPeriodVisible = true;
            }

            LoadHistogramData(altCompReport, altID, damageMeasureYear);

            LoadData(altCompReport, altID, damageMeasureYear);

            switch (damageMeasureYear)
            {
                case DamageMeasureYear.Base:
                    Mean = altCompReport.SampleMeanBaseYearEADReduced(altID);
                    QuartileLabel = QUARTILE_REDUCED_EAD;
                    break;
                case DamageMeasureYear.Future:
                    Mean = altCompReport.SampleMeanFutureYearEADReduced(altID);
                    QuartileLabel = QUARTILE_REDUCED_EAD;
                    break;
                case DamageMeasureYear.Eqad:
                    Mean = altCompReport.SampleMeanEqadReduced(altID);
                    QuartileLabel = QUARTILE_REDUCED_EQAD;
                    break;
            }

        }

        private void LoadData(AlternativeComparisonReportResults altResults, int altID, DamageMeasureYear damageMeasureYear)
        {
            List<double> xVals = [.75, .5, .25];
            List<string> xValNames = ["First", "Second", "Third"];
            List<double> yVals = LoadYData(xVals, altResults, altID, damageMeasureYear);

            if (DamageMeasureYear.Future.Equals(_damageMeasureYear) || DamageMeasureYear.Base.Equals(_damageMeasureYear))
            {
                for (int i = 0; i < xValNames.Count; i++)
                {
                    Rows.Add(new EadRowItem(xValNames[i], yVals[i]));
                }
            }
            else
            {
                for (int i = 0; i < xValNames.Count; i++)
                {
                    Rows.Add(new EqadRowItem(xValNames[i], yVals[i]));
                }
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
                case DamageMeasureYear.Eqad:
                    empirical = altResults.GetEqadDistribution();
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
                case DamageMeasureYear.Eqad:
                    empirical = altResults.GetEqadReducedResultsHistogram(altID);
                    break;
            }

            CreateHistogramData(empirical, false);
        }

        private void CreateHistogramData(Empirical empirical, bool isAlternative)
        {
            if (empirical != null)
            {
                if (empirical.CumulativeProbabilities.Length <= 1)
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
            if (isAlternative)
            {
                if (DamageMeasureYear.Future.Equals(_damageMeasureYear) || DamageMeasureYear.Base.Equals(_damageMeasureYear))
                {
                    MyPlot.Title = StringConstants.EAD_DISTRIBUTION; 
                }
                else
                {
                    MyPlot.Title = StringConstants.EqAD_DISTRIBUTION;
                }

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

            string yaxisLabel;
            if (DamageMeasureYear.Future.Equals(_damageMeasureYear) || DamageMeasureYear.Base.Equals(_damageMeasureYear))
            {
                yaxisLabel = StringConstants.EXPECTED_ANNUAL_DAMAGE;
            }
            else
            {
                yaxisLabel = StringConstants.EQUIVALENT_ANNUAL_DAMAGE;
            }
            LinearAxis y = new()
            {
                Position = AxisPosition.Left,
                Title = yaxisLabel,
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

        private void LoadData(AlternativeResults scenarioResults, DamageMeasureYear damageMeasureYear)
        {
            List<double> xVals = new() { .75, .5, .25 };
            List<string> xValNames = new() { "First", "Second", "Third" };
            List<double> yVals = LoadYData(xVals, scenarioResults, damageMeasureYear);

            if (DamageMeasureYear.Future.Equals(_damageMeasureYear) || DamageMeasureYear.Base.Equals(_damageMeasureYear))
            {
                for (int i = 0; i < xValNames.Count; i++)
                {
                    Rows.Add(new EadRowItem(xValNames[i], yVals[i]));
                }
            }
            else
            {
                for (int i = 0; i < xValNames.Count; i++)
                {
                    Rows.Add(new EqadRowItem(xValNames[i], yVals[i]));
                }
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
                    case DamageMeasureYear.Eqad:
                        yValues.Add(results.EqadExceededWithProbabilityQ(x));
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
                    case DamageMeasureYear.Eqad:
                        yValues.Add(scenarioResults.EqadReducedExceededWithProbabilityQ(x, altID));
                        break;
                }
            }
            return yValues;
        }

    }
}
