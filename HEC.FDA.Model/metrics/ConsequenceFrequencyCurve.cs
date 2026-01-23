using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.metrics;

public class ConsequenceFrequencyCurve
{
    public PairedData FrequencyCurve { get; }
    public ConsequenceType ConsequenceType { get; }
    public RiskType RiskType { get; }
    public string DamageCategory { get; }
    public string AssetCategory { get; }

    public ConsequenceFrequencyCurve(PairedData frequencyCurve, ConsequenceType consequenceType, RiskType riskType, string damageCategory, string assetCategory)
    {
        FrequencyCurve = frequencyCurve;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
    }
}
