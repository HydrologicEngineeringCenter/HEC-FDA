namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class EADSummaryRowItem
    {
        public double WithoutProjAlternative { get; set; }
        public double WithoutProjEAD { get; set; }
        public double WithProjAlternative { get; set; }
        public double WithProjEAD { get; set; }
        public double EADDamageReduced { get; set; }
        public double Point75 { get; set; }
        public double Point5 { get; set; }
        public double Point25 { get; set; }
        public EADSummaryRowItem()
        {
            WithoutProjAlternative = 1;
            WithoutProjEAD = 2;
            WithProjAlternative = 3;
            WithProjEAD = 4;
            EADDamageReduced = 5;
            Point75 = 6;
            Point5 = 7;
            Point25 = 8;
        }
    }
}
