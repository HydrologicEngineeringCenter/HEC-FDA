using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AggregatedEqadSummaryRowItem
    {
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Without Project Alternative")]
        public string WithoutProjAlternative { get; set; }
        [DisplayAsColumn("Without Project EqAD")]
        public double WithoutProjEqad { get; set; }
        [DisplayAsColumn("With Project Alternative")]
        public string WithProjAlternative { get; set; }
        [DisplayAsColumn("With Project EqAD")]
        public double WithProjEqad { get; set; }
        [DisplayAsColumn("Mean EqAD Reduced")]
        public double EqadReduced { get; set; }
        [DisplayAsColumn("25th Percentile EqAD Reduced")] // This is intentionally swapped 1-x
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile EqAD Reduced")]
        public double Point5 { get; set; }
        [DisplayAsColumn("75th Percentile EqAD Reduced")] // This is intentionally swapped 1-x
        public double Point25 { get; set; }
        public AggregatedEqadSummaryRowItem(string impactArea, string withoutName, double withoutEqadDamage, string withProjName, double withProjEqad,
            double eqadReduced, double point75, double point5, double point25)
        {
            ImpactArea = impactArea;
            WithoutProjAlternative = withoutName;
            WithoutProjEqad = withoutEqadDamage;
            WithProjAlternative = withProjName;
            WithProjEqad = withProjEqad;
            EqadReduced = eqadReduced;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }
    }
}
