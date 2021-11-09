namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem
    {

        public double Frequency { get;  }
        public double Value { get;  }

        public EadRowItem(double frequency, double value)
        {
            Frequency = frequency;
            Value = value;
        }

    }
}
