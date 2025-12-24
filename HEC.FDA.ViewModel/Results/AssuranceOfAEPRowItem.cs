using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.Results
{
    public class AssuranceOfAEPRowItem
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

        [DisplayAsColumn("Mean AEP")]
        public double Mean { get; set; }
        [DisplayAsColumn("Median AEP")]
        public double Median { get; set; }
        [DisplayAsColumn("90% Assurance AEP")]
        public double NinetyPercentAssurance { get; set; }

        [DisplayAsColumn("Assurance of 0.10 AEP")]
        public double AEP1 { get; set; }
        [DisplayAsColumn("Assurance of 0.04 AEP")]
        public double AEP04 { get; set; }
        [DisplayAsColumn("Assurance of 0.02 AEP")]
        public double AEP02 { get; set; }
        [DisplayAsColumn("Assurance of 0.01 AEP")]
        public double AEP01 { get; set; }
        [DisplayAsColumn("Assurance of 0.004 AEP")]
        public double AEP004 { get; set; }
        [DisplayAsColumn("Assurance of 0.002 AEP")]
        public double AEP002 { get; set; }

        public AssuranceOfAEPRowItem(IASElement scenario, SpecificIAS ias, Threshold threshold)
        {
            ScenarioResults results = scenario.Results;

            Name = scenario.Name;
            AnalysisYear = scenario.AnalysisYear;
            ImpactArea = ias.GetSpecificImpactAreaName();

            ThresholdType = threshold.ThresholdType.GetDisplayName();
            ThresholdValue = threshold.ThresholdValue;
            int thresholdID = threshold.ThresholdID;
            int iasID = ias.ImpactAreaID;

            Mean = results.MeanAEP(iasID, thresholdID);
            Median = results.MedianAEP(iasID, thresholdID);

            NinetyPercentAssurance = results.AEPWithGivenAssurance(iasID, assurance:0.9, thresholdID);

            AEP1 = results.AssuranceOfAEP(iasID, .1, thresholdID);
            AEP04 = results.AssuranceOfAEP(iasID, .04, thresholdID);
            AEP02 = results.AssuranceOfAEP(iasID, .02, thresholdID);
            AEP01 = results.AssuranceOfAEP(iasID, .01, thresholdID);
            AEP004 = results.AssuranceOfAEP(iasID, .004, thresholdID);
            AEP002 = results.AssuranceOfAEP(iasID, .002, thresholdID);

        }


    }
}
