using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioPerformanceRowItem
    {

        public string Name { get; set; }
        public int AnalysisYear { get; set; }
        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public double Mean { get; set; }
        public double Median { get; set; }
        public double Prob1 { get; set; }
        public double Prob04 { get; set; }
        public double Prob02 { get; set; }
        public double Prob01 { get; set; }
        public double Prob004 { get; set; }
        public double Prob002 { get; set; }

        public double Threshold1 { get; set; }
        public double Threshold04 { get; set; }
        public double Threshold02 { get; set; }
        public double Threshold01 { get; set; }
        public double Threshold004 { get; set; }
        public double Threshold002 { get; set; }

        public ScenarioPerformanceRowItem(IASElement scenario, Threshold threshold)
        {
            ScenarioResults results = scenario.Results;

            Name = scenario.Name;
            AnalysisYear = results.AnalysisYear;

            ThresholdType = threshold.ThresholdType.ToString();
            ThresholdValue = threshold.ThresholdValue;

            Mean = results.MeanExpectedAnnualConsequences();
            //todo: How do i get this one?
            Median = 111;// results.MedianAEP() 
            //look at PerformanceAssuranceOfThresholdVM class for this data.
            Prob1 = results.AssuranceOfEvent(threshold.ThresholdID, .1);
            Prob04 = results.AssuranceOfEvent(threshold.ThresholdID, .04);
            Prob02 = results.AssuranceOfEvent(threshold.ThresholdID, .02);
            Prob01 = results.AssuranceOfEvent(threshold.ThresholdID, .01);
            Prob004 = results.AssuranceOfEvent(threshold.ThresholdID, .004);
            Prob002 = results.AssuranceOfEvent(threshold.ThresholdID, .002);

            Threshold1 = results.AssuranceOfEvent(threshold.ThresholdID, 1-.1);
            Threshold04 = results.AssuranceOfEvent(threshold.ThresholdID, 1 - .04);
            Threshold02 = results.AssuranceOfEvent(threshold.ThresholdID, 1 - .02);
            Threshold01 = results.AssuranceOfEvent(threshold.ThresholdID, 1 - .01);
            Threshold004 = results.AssuranceOfEvent(threshold.ThresholdID, 1 - .004);
            Threshold002 = results.AssuranceOfEvent(threshold.ThresholdID, 1 - .002);

        }



    }
}
