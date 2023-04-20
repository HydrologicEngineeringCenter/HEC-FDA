using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
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

        public ScenarioPerformanceRowItem(IASElement scenario)
        {
            ScenarioResults results = scenario.Results;

            Name = scenario.Name;
            AnalysisYear = results.AnalysisYear;

            ThresholdType = "test";
            ThresholdValue = 123;

            Mean = 1;
            Median = 2;

            Prob1 = 3;
            Prob04 = 4;
            Prob02 = 5;
            Prob01 = 6;
            Prob004 = 7;
            Prob002 = 8;

            Threshold1 = 3;
            Threshold04 = 4;
            Threshold02 = 5;
            Threshold01 = 6;
            Threshold004 = 7;
            Threshold002 = 8;

        }



    }
}
