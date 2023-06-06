using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using OxyPlot.Axes;
using OxyPlot.Series;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel, IAlternativeResult
    {
        private HistogramData2D _data;
        public ViewResolvingPlotModel MyPlot { get; } = new ViewResolvingPlotModel();

        //public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("chart title");
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
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<string> xValNames = new List<string>() { "First", "Second", "Third" };
            List<double> yVals = loadYData(xVals, altResults, altID, damageMeasureYear);

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

            CreateHistogramData(empirical);

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

            CreateHistogramData(empirical);
        }

        private void CreateHistogramData(Empirical empirical)
        {
            if (empirical != null)
            {
                if (empirical.CumulativeProbabilities.Length <=1)
                {
                    HistogramVisible = false;
                }
                else
                {
                    InitializePlotModel(empirical);
                    //(double min, double valueStep, double[] cumulativeRelativeFrequencies) = empirical.ComputeCumulativeFrequenciesForPlotting();
                    //_data = new HistogramData2D(valueStep, min, cumulativeRelativeFrequencies, "Chart", "Cumulative Relative Frequency", StringConstants.HISTOGRAM_VALUE, StringConstants.HISTOGRAM_FREQUENCY);
                    //HistogramColor.SetHistogramColor(_data);
                    //MyPlot.LineData.Add(_data);
                }
            }
            else
            {
                HistogramVisible = false;
            }
        }

        private void InitializePlotModel(Empirical empirical)
        {
            MyPlot.Title = StringConstants.EAD_DISTRIBUTION;
            AddAxes(empirical);
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
                points[i] = new NormalDataPoint(exceedenceProbability, zScore, empirical.ObservationValues[i]);
            }
            lineSeries.ItemsSource = points;
            MyPlot.Series.Add(lineSeries);
            MyPlot.InvalidatePlot(true);
        }
        private void AddAxes(Empirical empirical)
        {
            LinearAxis x = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = StringConstants.EXCEEDANCE_PROBABILITY,
                LabelFormatter = _probabilityFormatter,
                Maximum = 3.719, //probability of .9999
                Minimum = -3.719, //probability of .0001
                StartPosition = 1,
                EndPosition = 0
            };
            LinearAxis y = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.EXPECTED_ANNUAL_DAMAGE,
                MinorTickSize = 0,
                Unit = "$",

            };
            MyPlot.Axes.Add(x);
            MyPlot.Axes.Add(y);
        }
        private static string _probabilityFormatter(double d)
        {
            Normal standardNormal = new Normal(0, 1);
            double value = standardNormal.CDF(d);
            string stringval = value.ToString("0.0000");
            return stringval;
        }

        private void LoadData(AlternativeResults scenarioResults, DamageMeasureYear damageMeasureYear)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<string> xValNames = new List<string>() { "First", "Second", "Third" };
            List<double> yVals = loadYData(xVals, scenarioResults, damageMeasureYear);

            for (int i = 0; i < xValNames.Count; i++)
            {
                Rows.Add(new EadRowItem(xValNames[i], yVals[i]));
            }
        }

        private List<double> loadYData(List<double> xVals, AlternativeResults results, DamageMeasureYear damageMeasureYear)
        {
            List<double> yValues = new List<double>();
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

        private List<double> loadYData(List<double> xVals, AlternativeComparisonReportResults scenarioResults, int altID, DamageMeasureYear damageMeasureYear)
        {
            List<double> yValues = new List<double>();
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

        //public void PlotHistogram()
        //{
        //    ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        //}

    }
}
