using Statistics.Distributions;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.metrics.Extensions;

public static class ConsequenceExtensions
{
    /// <summary>
    /// Filters a collection of AggregatedConsequencesByQuantile based on damage category, asset category, impact area, and risk type.
    /// Failure to match returns an empty IEnumerable.
    /// </summary>
    public static IEnumerable<AggregatedConsequencesByQuantile> FilterByCategories(
        this IEnumerable<AggregatedConsequencesByQuantile> consequences,
        string damageCategory = null,
        string assetCategory = null,
        int impactAreaID = -999,
        ConsequenceType type = ConsequenceType.Damage,
        RiskType riskType = RiskType.Total)
    {
        return consequences.Where(result =>
            (damageCategory == null || damageCategory.Equals(result.DamageCategory)) &&
            (assetCategory == null || assetCategory.Equals(result.AssetCategory)) &&
            (impactAreaID == -999 || impactAreaID == result.RegionID) &&
            (type == result.ConsequenceType) &&
            (riskType == RiskType.Total || riskType == result.RiskType));
    }

    /// <summary>
    /// Filters a collection of AggregatedConsequencesBinned based on damage category, asset category, impact area, and risk type.
    /// Failure to match returns an empty IEnumerable.
    /// </summary>
    public static IEnumerable<AggregatedConsequencesBinned> FilterByCategories(
        this IEnumerable<AggregatedConsequencesBinned> consequences,
        string damageCategory = null,
        string assetCategory = null,
        int impactAreaID = -999,
        ConsequenceType type = ConsequenceType.Damage,
        RiskType riskType = RiskType.Total)
    {
        return consequences.Where(result =>
            (damageCategory == null || damageCategory.Equals(result.DamageCategory)) &&
            (assetCategory == null || assetCategory.Equals(result.AssetCategory)) &&
            (impactAreaID == -999 || impactAreaID == result.RegionID) &&
            (type == result.ConsequenceType) &&
            (riskType == RiskType.Total || riskType == result.RiskType));
    }

    /// <summary>
    /// A zero damage stand in for a category one side carries and the other does not, which happens when two
    /// analysis years or two project conditions hold different categories or risk types. Carries the source
    /// row's identity so it pairs with its own kind.
    /// </summary>
    public static AggregatedConsequencesBinned ZeroDamageCounterpart(this AggregatedConsequencesBinned present)
    {
        return new AggregatedConsequencesBinned(
            present.DamageCategory,
            present.AssetCategory,
            present.RegionID,
            present.ConsequenceType,
            present.RiskType);
    }

    /// <summary>
    /// A zero damage stand in for a category one side carries and the other does not, which happens when two
    /// analysis years or two project conditions hold different categories or risk types. Carries the source
    /// row's identity so it pairs with its own kind.
    /// </summary>
    public static AggregatedConsequencesByQuantile ZeroDamageCounterpart(this AggregatedConsequencesByQuantile present)
    {
        return new AggregatedConsequencesByQuantile(
            present.DamageCategory,
            present.AssetCategory,
            new Empirical(),
            present.RegionID,
            present.ConsequenceType,
            present.RiskType);
    }
}