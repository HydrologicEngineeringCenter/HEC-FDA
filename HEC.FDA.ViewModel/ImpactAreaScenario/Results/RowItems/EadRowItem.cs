namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem
    {
        public string Frequency { get;  }
        public double Value { get;  }

        public EadRowItem(string frequency, double value)
        {
            Frequency = frequency;
            Value = value;
        }

    }
}
