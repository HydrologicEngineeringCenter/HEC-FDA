namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class PerformancePeriodRowItem : IPerformanceRowItem
    {
        public int Years { get; set; }
        public double LongTermRisk { get; set; }

        public PerformancePeriodRowItem(int years, double longTermRisk)
        {
            Years = years;
            LongTermRisk = longTermRisk;
        }
    }
}
