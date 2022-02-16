namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class PerformanceFrequencyRowItem : IPerformanceRowItem
    {

        public double Frequency { get; set; }
        public double AEP { get; set; }

        public PerformanceFrequencyRowItem(double freq, double aep)
        {
            Frequency = freq;
            AEP = aep;
        }

    }
}
