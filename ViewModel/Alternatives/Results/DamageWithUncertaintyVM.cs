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
        private readonly HistogramData2D _data;
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

            ThreadsafeInlineHistogram histogram = scenarioResults.GetConsequencesHistogram();

            RateAndPeriodVisible = false;
            //load with dummy data
            double[] binValues = histogram.BinCounts.Select(i => (double)i).ToArray();
            _data = new HistogramData2D(histogram.BinWidth, 0,  binValues, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
            LoadData(scenarioResults);
            Mean = mean;
        }

        /// <summary>
        /// Ctor for AAEQ versions which have discount rate and period of analysis.
        /// </summary>
        public DamageWithUncertaintyVM(double discountRate, int periodOfAnalysis, double mean, double other)
        {
            DiscountRate = discountRate;
            PeriodOfAnalysis = periodOfAnalysis;
            RateAndPeriodVisible = true;
            //load with dummy data
            _data = new HistogramData2D(5, 0, new double[] { }, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Add(_data);
            //LoadData(other);
            Mean = mean;
        }

        private void LoadData(ScenarioResults scenarioResults)
        {
            //double mean = scenarioResults.MeanExpectedAnnualConsequences();
            //double damage25 = scenarioResults.ConsequencesExceededWithProbabilityQ(.25);
            //double damage5 = scenarioResults.ConsequencesExceededWithProbabilityQ(.5);
            //double damage75 = scenarioResults.ConsequencesExceededWithProbabilityQ(.75);
            List<double> xVals = new List<double>() { .75, .5, .25 };

            List<double> yVals = loadYData(xVals, scenarioResults);

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

        public void PlotHistogram()
        {
            //double binWidth = 5;
            //double binStart = 2.5;
            //double[] values = new double[] { 2, 2.5, 2.7, 3.5, 3.8, 1, 1.5 };

            //HistogramData2D _data = new HistogramData2D(binWidth, binStart, values, "Chart", "Series", "X Data", "YData");
            ChartViewModel.LineData.Set(new List<SciLineData>() { _data });
        }

    }
}
