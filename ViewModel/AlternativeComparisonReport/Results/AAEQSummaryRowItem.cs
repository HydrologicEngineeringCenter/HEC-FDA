namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryRowItem
    {
        public double WithoutProjAlternative { get; set; }
        public double WithoutProjAAEQ { get; set; }
        public double WithProjAlternative { get; set; }
        public double WithProjAAEQ { get; set; }
        public double AAEQDamageReduced { get; set; }
        public double Point75 { get; set; }
        public double Point5 { get; set; }
        public double Point25 { get; set; }
        public AAEQSummaryRowItem(string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ, double AAEQReduced, double point75, double point5, double point25)
        {
            WithoutProjAlternative = 1;
            WithoutProjAAEQ = 2;
            WithProjAlternative = 3;
            WithProjAAEQ = 4;
            AAEQDamageReduced = 5;
            Point75 = 6;
            Point5 = 7;
            Point25 = 8;
        }
    }
}
