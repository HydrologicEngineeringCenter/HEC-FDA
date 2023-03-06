namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class DamageCategoryRowItem
    {
        public string DamageCategory { get; set; }
        public double EAD { get; set; }

        public DamageCategoryRowItem(string damCat, double ead)
        {
            EAD = ead;
            DamageCategory = damCat;
        }

    }
}
