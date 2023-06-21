using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Results
{
    public class AssuranceOfAEPRowItem
    {
        public string Name { get; set; }
        public string AnalysisYear { get; set; }
        public string ImpactArea { get; set; }
        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }

        public double Mean { get; set; }
        public double Median { get; set; }

        public double AEP1 { get; set; }
        public double AEP04 { get; set; }
        public double AEP02 { get; set; }
        public double AEP01 { get; set; }
        public double AEP004 { get; set; }
        public double AEP002 { get; set; }

        public AssuranceOfAEPRowItem(IASElement scenario, SpecificIAS ias, Threshold threshold)
        {
            ScenarioResults results = scenario.Results;

            Name = scenario.Name;
            AnalysisYear = scenario.AnalysisYear;
            ImpactArea = ias.GetSpecificImpactAreaName();

            ThresholdType = threshold.ThresholdType.ToString();
            ThresholdValue = threshold.ThresholdValue;
            int thresholdID = threshold.ThresholdID;
            int iasID = ias.ImpactAreaID;

            Mean = results.MeanAEP(iasID, thresholdID);
            Median = results.MedianAEP(iasID, thresholdID);

            AEP1 = results.AssuranceOfAEP(iasID, .1, thresholdID);
            AEP04 = results.AssuranceOfAEP(iasID, .04, thresholdID);
            AEP02 = results.AssuranceOfAEP(iasID, .02, thresholdID);
            AEP01 = results.AssuranceOfAEP(iasID, .01, thresholdID);
            AEP004 = results.AssuranceOfAEP(iasID, .004, thresholdID);
            AEP002 = results.AssuranceOfAEP(iasID, .002, thresholdID);

        }


    }
}
