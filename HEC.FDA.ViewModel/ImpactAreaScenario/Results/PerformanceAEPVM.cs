﻿using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAEPVM : PerformanceVMBase
    {

        public SciChart2DChartViewModel ChartViewModel { get; set; } = new SciChart2DChartViewModel("Performance");
        public Dictionary<Threshold, HistogramData2D> HistogramData { get; } = new Dictionary<Threshold, HistogramData2D>();
        public bool HistogramVisible { get; set; } = true;

        public PerformanceAEPVM(ScenarioResults iasResult, int impactAreaID, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, impactAreaID, thresholdComboItems);
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
                double ninetyPercentAssurance = iasResult.AEPWithGivenAssurance(impactAreaID, assurance:0.9, thresholdID);
                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                //get the table values
                List<double> xVals = new List<double>() { .1, .04, .02, .01, .004, .002 };
                foreach (double xVal in xVals)
                {
                    double yVal = iasResult.AssuranceOfAEP(impactAreaID, xVal,thresholdID);
                    rows.Add(new PerformanceFrequencyRowItem(xVal, yVal));
                }

                MetricsToRows.Add(threshold, rows);
                ThresholdToMetrics.Add(threshold, (mean, median, ninetyPercentAssurance));
                LoadHistogramData(iasResult, impactAreaID, threshold);
            }

            if(MetricsToRows.Count>0)
            {
                Rows = MetricsToRows.First().Value;
                Mean = ThresholdToMetrics.First().Value.Item1;
                Median = ThresholdToMetrics.First().Value.Item2;
                NinetyPercentAssurance = ThresholdToMetrics.First().Value.Item3;
            }
        }

        private void LoadHistogramData(ScenarioResults scenarioResults, int impactAreaID, Threshold threshold)
        {            
            IHistogram histogramOfAEPs = scenarioResults.GetAEPHistogramForPlotting(impactAreaID, threshold.ThresholdID);
            long[] binCounts = histogramOfAEPs.BinCounts;
            double[] binsAsDoubles = binCounts.Select(x => (double)x / histogramOfAEPs.SampleSize).ToArray();

            if (binsAsDoubles.Length == 0 || binsAsDoubles.Length == 1)
            {
                HistogramVisible = false;
            }
            else
            {
                HistogramData2D data = new HistogramData2D(histogramOfAEPs.BinWidth, histogramOfAEPs.Min, binsAsDoubles, "Chart", "Series", StringConstants.HISTOGRAM_EXCEEDANCE_PROBABILITY, StringConstants.HISTOGRAM_FREQUENCY);
                HistogramColor.SetHistogramColor(data);
                HistogramData.Add(threshold, data);
            }
        }

        public override void UpdateHistogram(ThresholdComboItem metric)
        {
            if (HistogramData.ContainsKey(metric.Metric))
            {
                HistogramData2D histData = HistogramData[metric.Metric];
                HistogramColor.SetHistogramColor(histData);
                ChartViewModel.LineData.Set(new List<SciLineData>() { histData });
            }
        }
    }
}
