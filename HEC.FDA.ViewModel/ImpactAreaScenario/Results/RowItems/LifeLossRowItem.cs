using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
public class LifeLossRowItem : IQuartileRowItem
{
    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }
    [DisplayAsColumn("Quartile of EALL Distribution")]
    public string Frequency { get; }
    public double Value { get; }
    [DisplayAsColumn(StringConstants.QUARTILE_VALUE)]
    public string FormattedValue => Value.ToString("N4");

    public LifeLossRowItem(string frequency, double value, string riskType = null)
    {
        RiskType = riskType;
        Frequency = frequency;
        Value = value;
    }

}
