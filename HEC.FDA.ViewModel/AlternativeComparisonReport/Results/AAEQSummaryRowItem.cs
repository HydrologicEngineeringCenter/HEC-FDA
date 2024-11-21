using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryRowItem
    {
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Damage Category")]
        public string DamCat { get; set; }
        [DisplayAsColumn("Asset Category")]
        public string AssetCat { get; set; }
        [DisplayAsColumn("Without Project Alternative")]
        public string WithoutProjAlternative { get; set; }
        [DisplayAsColumn("Without Project AAEQ")]
        public double WithoutProjAAEQ { get; set; }
        [DisplayAsColumn("With Project Alternative")]
        public string WithProjAlternative { get; set; }
        [DisplayAsColumn("With Project AAEQ")]
        public double WithProjAAEQ { get; set; }
        [DisplayAsColumn("Mean AAEQ Reduced")]
        public double AAEQDamageReduced { get; set; }
        [DisplayAsColumn("75th Percentile AAEQ Reduced")]
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile AAEQ Reduced")]
        public double Point5 { get; set; }
        [DisplayAsColumn("25th Percentile AAEQ Reduced")]
        public double Point25 { get; set; }
        public AAEQSummaryRowItem(string impactArea, string damcat, string assetcat,string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ, 
            double AAEQReduced, double point75, double point5, double point25)
        {
            ImpactArea = impactArea;
            DamCat = damcat;
            AssetCat = assetcat;
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
