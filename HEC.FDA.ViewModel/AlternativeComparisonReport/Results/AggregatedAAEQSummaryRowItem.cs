using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AggregatedAAEQSummaryRowItem
    {
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Without Project Alternative")]
        public string WithoutProjAlternative { get; set; }
        [DisplayAsColumn("Without Project EqAD")]
        public double WithoutProjAAEQ { get; set; }
        [DisplayAsColumn("With Project Alternative")]
        public string WithProjAlternative { get; set; }
        [DisplayAsColumn("With Project EqAD")]
        public double WithProjAAEQ { get; set; }
        [DisplayAsColumn("Mean EqAD Reduced")]
        public double AAEQDamageReduced { get; set; }
        [DisplayAsColumn("25th Percentile EqAD Reduced")] // This is intentionally swapped 1-x
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile EqAD Reduced")]
        public double Point5 { get; set; }
        [DisplayAsColumn("75th Percentile EqAD Reduced")] // This is intentionally swapped 1-x
        public double Point25 { get; set; }
        public AggregatedAAEQSummaryRowItem(string impactArea, string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ,
            double AAEQReduced, double point75, double point5, double point25)
        {
            ImpactArea = impactArea;
            WithoutProjAlternative = withoutName;
            WithoutProjAAEQ = withoutAAEQDamage;
            WithProjAlternative = withProjName;
            WithProjAAEQ = withProjAAEQ;
            AAEQDamageReduced = AAEQReduced;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }
    }
}
