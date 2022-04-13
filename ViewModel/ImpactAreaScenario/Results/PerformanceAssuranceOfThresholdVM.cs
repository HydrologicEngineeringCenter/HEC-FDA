using metrics;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAssuranceOfThresholdVM :  PerformanceVMBase
    {

        public PerformanceAssuranceOfThresholdVM(metrics.Results iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, thresholdComboItems);
        }

        private void LoadData(metrics.Results iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            for (int i = 0; i < thresholdComboItems.Count; i++)
            {
                int thresholdKey = thresholdComboItems[i].Metric.ThresholdID;
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
                MetricsToRows.Add(thresholdComboItems[i].Metric, rows);
            }         
            Rows = MetricsToRows[thresholdComboItems[0].Metric];
        }
    }
}
