using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
internal class EqadRowItem : IQuartileRowItem
{
    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }
    [DisplayAsColumn(StringConstants.ALTERNATIVE_EqAD_LABEL)]
    public string Frequency { get; }
    public double Value { get; }
    [DisplayAsColumn(StringConstants.QUARTILE_VALUE)]
    public string FormattedValue => Value.ToString("C2");

    public EqadRowItem(string frequency, double value, string riskType = null)
    {
        RiskType = riskType;
        Frequency = frequency;
        Value = value;
    }
}
