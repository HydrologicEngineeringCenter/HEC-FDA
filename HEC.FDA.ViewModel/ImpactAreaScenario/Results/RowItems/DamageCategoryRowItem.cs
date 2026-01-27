namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class DamageCategoryRowItem
    {
        public string DamageCategory { get; set; }
        public double EAD { get; set; }
        public string RiskType { get; set; }


        public DamageCategoryRowItem(string damCat, double ead, string riskType = null)
        {
            RiskType = riskType;
            EAD = ead;
            DamageCategory = damCat;
        }

    }
}
