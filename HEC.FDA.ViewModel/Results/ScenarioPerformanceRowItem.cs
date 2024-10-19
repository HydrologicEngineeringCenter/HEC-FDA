using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioPerformanceRowItem
    {
        public string Name { get; set; }
        public string AnalysisYear { get; set; }
        public string ImpactArea { get; set; }

        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }

        public double LongTerm10 { get; set; }
        public double LongTerm30 { get; set; }
        public double LongTerm50 { get; set; }

        public double Mean { get; set; }
        public double Median { get; set; }

        public double Threshold1 { get; set; }
        public double Threshold04 { get; set; }
        public double Threshold02 { get; set; }
        public double Threshold01 { get; set; }
        public double Threshold004 { get; set; }
        public double Threshold002 { get; set; }

        public ScenarioPerformanceRowItem(IASElement scenario, SpecificIAS ias, Threshold threshold)
        {
            ScenarioResults scenarioResults = scenario.Results;
            int iasID = ias.ImpactAreaID;
            ImpactAreaScenarioResults results = scenarioResults.GetResults(iasID);

            Name = scenario.Name;
            AnalysisYear = scenario.AnalysisYear;
            ImpactArea = ias.GetSpecificImpactAreaName();

            ThresholdType = threshold.ThresholdType.GetDisplayName();
            ThresholdValue = threshold.ThresholdValue;
            int thresholdID = threshold.ThresholdID;

            LongTerm10 = results.LongTermExceedanceProbability(thresholdID, 10);
            LongTerm30 = results.LongTermExceedanceProbability( thresholdID, 30);
            LongTerm50 = results.LongTermExceedanceProbability( thresholdID, 50);

            Mean = results.MeanAEP( thresholdID);
            Median = results.MedianAEP( thresholdID); 

            Threshold1 = results.AssuranceOfEvent(thresholdID, 1 - .1);
            Threshold04 = results.AssuranceOfEvent(thresholdID, 1 - .04);
            Threshold02 = results.AssuranceOfEvent(thresholdID, 1 - .02);
            Threshold01 = results.AssuranceOfEvent(thresholdID, 1 - .01);
            Threshold004 = results.AssuranceOfEvent(thresholdID, 1 - .004);
            Threshold002 = results.AssuranceOfEvent(thresholdID, 1 - .002);

        }

    }
}
