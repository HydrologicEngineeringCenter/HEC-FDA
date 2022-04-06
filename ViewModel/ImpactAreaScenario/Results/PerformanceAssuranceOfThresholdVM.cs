using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAssuranceOfThresholdVM :  PerformanceVMBase
    {

        public PerformanceAssuranceOfThresholdVM(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {
            LoadData(iasResult, metrics);
        }

        private void LoadData(metrics.Results iasResult, List<ThresholdComboItem> metrics)
        {
            MetricsToRows = new Dictionary<Threshold, List<IPerformanceRowItem>>();

            for (int i = 0; i < metrics.Count; i++)
            {
                int thresholdKey = metrics[i].Metric.ThresholdID;
                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                List<double> xVals = new List<double>(){ .1, .04, .02, .01, .005, .002};
                foreach (double xVal in xVals)
                {
                    double exceedanceProb = 1.0 - xVal;
                    try
                    {
                        Threshold threshold = iasResult.PerformanceByThresholds.ThresholdsDictionary[thresholdKey];
                        ProjectPerformanceResults projectPerformanceResults = threshold.ProjectPerformanceResults;
                        double yVal = projectPerformanceResults.ConditionalNonExceedanceProbability(exceedanceProb);
                        rows.Add(new PerformanceFrequencyRowItem(xVal, yVal));
                    }
                    catch (Exception e)
                    {
                        //todo: Getting the y value can throw an exception if the exceedanceProb isn't in the dictionary
                    }
                }
                MetricsToRows.Add(metrics[i].Metric, rows);
            }         
            Rows = MetricsToRows[metrics[0].Metric];
        }
    }
}
