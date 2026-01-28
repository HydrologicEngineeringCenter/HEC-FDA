using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using System.Globalization;

namespace HEC.FDA.ViewModel.Alternatives.Results;

public interface IConsequenceByImpactAreaRowItem
{
    public string ImpactArea { get; }
    public double Value { get; }
    public string FormattedValue { get; }
    public string RiskType { get; }
}

public class EADByImpactAreaRowItem : IConsequenceByImpactAreaRowItem
{
    [DisplayAsColumn("Impact Area")]
    public string ImpactArea { get; }

    public double Value { get; }

    [DisplayAsColumn("Mean EAD")]
    public string FormattedValue
    {
        get
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.CurrencyNegativePattern = 1; // -$n
            return Value.ToString("C2", culture);
        }
    }

    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }

    public EADByImpactAreaRowItem(string impactArea, double value, string riskType)
    {
        ImpactArea = impactArea;
        Value = value;
        RiskType = riskType;
    }
}

public class EqADByImpactAreaRowItem : IConsequenceByImpactAreaRowItem
{
    [DisplayAsColumn("Impact Area")]
    public string ImpactArea { get; }

    public double Value { get; }

    [DisplayAsColumn("Mean EqAD")]
    public string FormattedValue
    {
        get
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.CurrencyNegativePattern = 1; // -$n
            return Value.ToString("C2", culture);
        }
    }

    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }

    public EqADByImpactAreaRowItem(string impactArea, double value, string riskType)
    {
        ImpactArea = impactArea;
        Value = value;
        RiskType = riskType;
    }
}

public class LifeLossByImpactAreaRowItem : IConsequenceByImpactAreaRowItem
{
    [DisplayAsColumn("Impact Area")]
    public string ImpactArea { get; }

    public double Value { get; }

    [DisplayAsColumn("Mean EALL")]
    public string FormattedValue => Value.ToString("N2");

    [DisplayAsColumn("Risk Type")]
    public string RiskType { get; }

    public LifeLossByImpactAreaRowItem(string impactArea, double value, string riskType)
    {
        ImpactArea = impactArea;
        Value = value;
        RiskType = riskType;
    }
}
