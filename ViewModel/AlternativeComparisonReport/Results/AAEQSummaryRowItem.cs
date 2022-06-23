namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryRowItem
    {
        public string WithoutProjAlternative { get; set; }
        public double WithoutProjAAEQ { get; set; }
        public string WithProjAlternative { get; set; }
        public double WithProjAAEQ { get; set; }
        public double AAEQDamageReduced { get; set; }
        public double Point75 { get; set; }
        public double Point5 { get; set; }
        public double Point25 { get; set; }
        public AAEQSummaryRowItem(string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ, 
            double AAEQReduced, double point75, double point5, double point25)
        {
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
