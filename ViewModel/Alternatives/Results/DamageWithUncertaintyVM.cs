using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using metrics;
using Statistics.Histograms;
using System.Linq;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel, IAlternativeResult
    {
        private HistogramData2D _data;
        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("chart title");

        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }

        public double DiscountRate { get; set; }
        public int PeriodOfAnalysis { get; set; }
        public bool RateAndPeriodVisible { get; }

        /// <summary>
        /// Ctor for EAD versions which don't have discount rate and period of analysis.
        /// </summary>
        public DamageWithUncertaintyVM(ScenarioResults scenarioResults)
        {
            double mean = scenarioResults.MeanExpectedAnnualConsequences();
            RateAndPeriodVisible = false;
            LoadHistogramData(scenarioResults);            
            LoadData(scenarioResults);
            Mean = mean;
        }

        /// <summary>
        /// Ctor for AAEQ versions which have discount rate and period of analysis.
        /// </summary>
        public DamageWithUncertaintyVM(double discountRate, int periodOfAnalysis, AlternativeResults altResults)
        {    
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            RateAndPeriodVisible = true;
            LoadHistogramData(altResults);

            LoadAAEQData(altResults);
            Mean = altResults.MeanConsequence();
        }

        public DamageWithUncertaintyVM(double discountRate, int periodOfAnalysis, AlternativeComparisonReportResults altResults, int altID)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            RateAndPeriodVisible = true;
            LoadHistogramData(altResults, altID);

            LoadAAEQData(altResults, altID);
            Mean = altResults.MeanConsequencesReduced(altID);
        }

        private void LoadAAEQData(AlternativeResults altResults)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<double> yVals = loadYData(xVals, altResults);

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }
        }

        private void LoadAAEQData(AlternativeComparisonReportResults altResults, int altID)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<double> yVals = loadYData(xVals, altResults, altID);

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }
        }

        private void LoadHistogramData(ScenarioResults scenarioResults)
        {
            ThreadsafeInlineHistogram histogram = scenarioResults.GetConsequencesHistogram();
            double[] binValues = histogram.BinCounts.Select(i => (double)i).ToArray();
            _data = new HistogramData2D(histogram.BinWidth, 0, binValues, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
        }

        private void LoadHistogramData(AlternativeResults altResults)
        {
            ThreadsafeInlineHistogram histogram = altResults.GetConsequencesHistogram();
            double[] binValues = histogram.BinCounts.Select(i => (double)i).ToArray();
            _data = new HistogramData2D(histogram.BinWidth, 0, binValues, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
        }

        private void LoadHistogramData(AlternativeComparisonReportResults altResults, int altID)
        {
            ThreadsafeInlineHistogram histogram = altResults.GetAlternativeResultsHistogram(altID);
            double[] binValues = histogram.BinCounts.Select(i => (double)i).ToArray();
            _data = new HistogramData2D(histogram.BinWidth, 0, binValues, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
        }

        private void LoadData(ScenarioResults scenarioResults)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<double> yVals = loadYData(xVals, scenarioResults);

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }
        }

        private void LoadData(AlternativeComparisonReportResults scenarioResults, int altID)
        {
            List<double> xVals = new List<double>() { .75, .5, .25 };
            List<double> yVals = loadYData(xVals, scenarioResults, altID);

            for (int i = 0; i < xVals.Count; i++)
            {
                Rows.Add(new EadRowItem(xVals[i], yVals[i]));
            }
        }

        private List<double> loadYData(List<double> xVals, ScenarioResults scenarioResults)
        {
            List<double> yValues = new List<double>();
            foreach (double x in xVals)
            {
                yValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(x));
            }
            return yValues;
        }

        private List<double> loadYData(List<double> xVals, AlternativeResults scenarioResults)
        {
            List<double> yValues = new List<double>();
            foreach (double x in xVals)
            {
                yValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(x));
            }
            return yValues;
        }

        private List<double> loadYData(List<double> xVals, AlternativeComparisonReportResults scenarioResults, int altID)
        {
            List<double> yValues = new List<double>();
            foreach (double x in xVals)
            {
                yValues.Add(scenarioResults.ConsequencesReducedExceededWithProbabilityQ(x, altID));
            }
            return yValues;
        }

        public void PlotHistogram()
        {
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }

    }
}
