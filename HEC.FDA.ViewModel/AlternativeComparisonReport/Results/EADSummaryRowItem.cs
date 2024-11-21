using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using System.Security.Permissions;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class EADSummaryRowItem
    {
        [DisplayAsColumn("Impact Area")]
        public string ImpactArea { get; set; }
        [DisplayAsColumn("Damage Category")]
        public string DamCat { get; set; }
        [DisplayAsColumn("Asset Category")]
        public string AssetCat { get; set; }
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
        [DisplayAsColumn("75th Percentile EAD Reduced")]
        public double Point75 { get; set; }
        [DisplayAsColumn("50th Percentile EAD Reduced")]
        public double Point5 { get; set; }
        [DisplayAsColumn("25th Percentile EAD Reduced")]
        public double Point25 { get; set; }


        public EADSummaryRowItem(string impactArea, string damcat, string assetcat, string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ, double AAEQReduced, double point75, double point5, double point25 )
        {
            ImpactArea = impactArea;
            DamCat = damcat;
            AssetCat = assetcat;
            WithoutProjAlternative = withoutName;
            WithoutProjEAD = withoutAAEQDamage;
            WithProjAlternative = withProjName;
            WithProjEAD = withProjAAEQ;
            EADDamageReduced = AAEQReduced;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;
        }
    }
}
