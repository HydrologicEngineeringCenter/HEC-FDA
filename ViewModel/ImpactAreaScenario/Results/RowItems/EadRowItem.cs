namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem
    {

        public double Frequency { get; set; }
        public double Value { get; set; }

        public EadRowItem(double frequency, double value)
        {
            Frequency = frequency;
            Value = value;
        }

    }
}
