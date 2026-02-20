using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.metrics;

public class CategoriedPairedData
{
    public PairedData FrequencyCurve { get; }
    public ConsequenceType ConsequenceType { get; }
    public RiskType RiskType { get; }
    public string DamageCategory { get; }
    public string AssetCategory { get; }

    public CategoriedPairedData(PairedData frequencyCurve, string damageCategory, string assetCategory, ConsequenceType consequenceType, RiskType riskType)
    {
        FrequencyCurve = frequencyCurve;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
    }
}
