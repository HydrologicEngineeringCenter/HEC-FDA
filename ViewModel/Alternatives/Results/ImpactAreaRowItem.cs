namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class ImpactAreaRowItem
    {
        public string ImpactArea { get; set; }
        public double EAD { get; set; }

        public ImpactAreaRowItem(string impactArea, double ead)
        {
            EAD = ead;
            ImpactArea = impactArea;
        }
    }
}
