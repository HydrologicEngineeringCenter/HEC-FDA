using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class PerformanceAssuranceOfThresholdVM :  PerformanceVMBase
    {
        public PerformanceAssuranceOfThresholdVM(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            LoadData(iasResult, thresholdComboItems);
        }
      
        private void LoadData(ImpactAreaScenarioResults iasResult, List<ThresholdComboItem> thresholdComboItems)
        {
            for (int i = 0; i < thresholdComboItems.Count; i++)
            {
                int thresholdKey = thresholdComboItems[i].Metric.ThresholdID;
                List<IPerformanceRowItem> rows = new List<IPerformanceRowItem>();
                //The xVals are exceedance probabilities
                List<double> xVals = new List<double>(){ .1, .04, .02, .01, .004, .002};
                foreach (double xVal in xVals)
                {
                    double nonExceedanceProb = 1.0 - xVal;
                    try
                    {
                        double yVal = iasResult.AssuranceOfEvent(thresholdKey, nonExceedanceProb);
                        //TODO: should this be non-exceedance probabilities, too?
                        rows.Add(new PerformanceFrequencyRowItem(xVal, yVal));
                    }
                    catch
                    {
                        throw new Exception("exceedence probability not found)"); //TODO: handle this better. 
                    }
                }
                MetricsToRows.Add(thresholdComboItems[i].Metric, rows);
            }         
            Rows = MetricsToRows[thresholdComboItems[0].Metric];
        }
    }
}
