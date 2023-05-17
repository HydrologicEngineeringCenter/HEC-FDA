using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Results
{
    public class AssuranceOfAEPRowItem
    {
        public string Name { get; set; }
        public int AnalysisYear { get; set; }
        public string ImpactArea { get; set; }
        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }

        public double LongTerm10 { get; set; }
        public double LongTerm20 { get; set; }
        public double LongTerm30 { get; set; }

        public double Mean { get; set; }
        public double Median { get; set; }

        public double Threshold1 { get; set; }
        public double Threshold04 { get; set; }
        public double Threshold02 { get; set; }
        public double Threshold01 { get; set; }
        public double Threshold004 { get; set; }
        public double Threshold002 { get; set; }

        public AssuranceOfAEPRowItem(IASElement scenario, SpecificIAS ias, Threshold threshold)
        {
            ScenarioResults results = scenario.Results;

            Name = scenario.Name;
            AnalysisYear = results.AnalysisYear;
            ImpactArea = ias.GetSpecificImpactAreaName();

            ThresholdType = threshold.ThresholdType.ToString();
            ThresholdValue = threshold.ThresholdValue;
            int thresholdID = threshold.ThresholdID;
            int iasID = ias.ImpactAreaID;

            Mean = results.MeanAEP(iasID, thresholdID);
            Median = results.MedianAEP(iasID, thresholdID);

            Threshold1 = results.AssuranceOfAEP(ias.ImpactAreaID, .1, threshold.ThresholdID);
            Threshold04 = results.AssuranceOfAEP(ias.ImpactAreaID, .04, threshold.ThresholdID);
            Threshold02 = results.AssuranceOfAEP(ias.ImpactAreaID, .02, threshold.ThresholdID);
            Threshold01 = results.AssuranceOfAEP(ias.ImpactAreaID, .01, threshold.ThresholdID);
            Threshold004 = results.AssuranceOfAEP(ias.ImpactAreaID, .004, threshold.ThresholdID);
            Threshold002 = results.AssuranceOfAEP(ias.ImpactAreaID, .002, threshold.ThresholdID);

        }


    }
}
