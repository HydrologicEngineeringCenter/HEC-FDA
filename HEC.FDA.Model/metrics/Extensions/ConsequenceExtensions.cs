using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.metrics.Extensions;

public static class ConsequenceExtensions
{
    /// <summary>
    /// Filters a collection of AggregatedConsequencesByQuantile based on damage category, asset category, and impact area.
    /// Failure to match returns an empty IEnumerable. 
    /// </summary>
    public static IEnumerable<AggregatedConsequencesByQuantile> FilterByCategories(
        this IEnumerable<AggregatedConsequencesByQuantile> consequences,
        string damageCategory = null,
        string assetCategory = null,
        int impactAreaID = -999)
    {
        return consequences.Where(result =>
            (damageCategory == null || damageCategory.Equals(result.DamageCategory)) &&
            (assetCategory == null || assetCategory.Equals(result.AssetCategory)) &&
            (impactAreaID == -999 || impactAreaID == result.RegionID));
    }

    /// <summary>
    /// Filters a collection of AggregatedConsequencesBinned based on damage category, asset category, and impact area
    /// Failure to match returns an empty IEnumerable. 
    /// </summary>
    public static IEnumerable<AggregatedConsequencesBinned> FilterByCategories(
        this IEnumerable<AggregatedConsequencesBinned> consequences,
        string damageCategory = null,
        string assetCategory = null,
        int impactAreaID = -999)
    {
        return consequences.Where(result =>
            (damageCategory == null || damageCategory.Equals(result.DamageCategory)) &&
            (assetCategory == null || assetCategory.Equals(result.AssetCategory)) &&
            (impactAreaID == -999 || impactAreaID == result.RegionID));
    }

}