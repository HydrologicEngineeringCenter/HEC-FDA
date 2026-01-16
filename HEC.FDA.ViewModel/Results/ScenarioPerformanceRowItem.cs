using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioPerformanceRowItem
    {
        [DisplayAsColumn("Name")]
        public string Name { get; set; }
        [DisplayAsColumn("Analysis Year")]
        public string AnalysisYear { get; set; }
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }

        [DisplayAsColumn("Threshold Type")]
        public string ThresholdType { get; set; }
        [DisplayAsColumn("Threshold Value")]
        public double ThresholdValue { get; set; }

        [DisplayAsColumn("LT Risk 10yr")]
        public double LongTerm10 { get; set; }
        [DisplayAsColumn("LT Risk 30yr")]
        public double LongTerm30 { get; set; }
        [DisplayAsColumn("LT Risk 50yr")]
        public double LongTerm50 { get; set; }

        [DisplayAsColumn("Mean AEP")]
        public double Mean { get; set; }
        [DisplayAsColumn("Median AEP")]
        public double Median { get; set; }

        [DisplayAsColumn("Assurance 0.10")]
        public double Threshold1 { get; set; }
        [DisplayAsColumn("Assurance 0.04")]
        public double Threshold04 { get; set; }
        [DisplayAsColumn("Assurance 0.02")]
        public double Threshold02 { get; set; }
        [DisplayAsColumn("Assurance 0.01")]
        public double Threshold01 { get; set; }
        [DisplayAsColumn("Assurance 0.004")]
        public double Threshold004 { get; set; }
        [DisplayAsColumn("Assurance 0.002")]
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
