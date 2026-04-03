using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using System.Security.Permissions;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AggregatedEADSummaryRowItem
    {
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Without Project Alternative")]
        public string WithoutProjAlternative { get; set; }
        [DisplayAsColumn("Without Project EAD")]
        public double WithoutProjEAD { get; set; }
        [DisplayAsColumn("With Project Alternative")]
        public string WithProjAlternative { get; set; }
        [DisplayAsColumn("With Project EAD")]
        public double WithProjEAD { get; set; }
        [DisplayAsColumn("Mean EAD Reduced")]
        public double EADDamageReduced { get; set; }
        [DisplayAsColumn("25th Percentile EAD Reduced")] // Point75 = ExceededWithProbabilityQ(.75) = InverseCDF(1-.75) = 25th percentile
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile EAD Reduced")]
        public double Point5 { get; set; }
        [DisplayAsColumn("75th Percentile EAD Reduced")] // Point25 = ExceededWithProbabilityQ(.25) = InverseCDF(1-.25) = 75th percentile
        public double Point25 { get; set; }


        public AggregatedEADSummaryRowItem(string impactArea, string withoutName, double withoutEqad, string withProjName, double withProjEqad, double eqadReduced, double point75, double point5, double point25)
        {
            ImpactArea = impactArea;
            WithoutProjAlternative = withoutName;
            WithoutProjEAD = withoutEqad;
            WithProjAlternative = withProjName;
            WithProjEAD = withProjEqad;
            EADDamageReduced = eqadReduced;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }
    }
}
