using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
public class LifeLossRowItem : IQuartileRowItem
{
    [DisplayAsColumn("Quartile of AALL Distribution")]
    public string Frequency { get; }
    public double Value { get; }
    [DisplayAsColumn(StringConstants.QUARTILE_VALUE)]
    public string FormattedValue => Value.ToString("N4");
    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }

    public LifeLossRowItem(string frequency, double value, string riskType)
    {
        Frequency = frequency;
        Value = value;
        RiskType = riskType;
    }

}
