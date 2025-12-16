namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
public interface IQuartileRowItem
{
    public string Frequency { get; }
    public double Value { get; } // e.g. 100000
    public string FormattedValue { get; } // e.g. $100,000.00
}
