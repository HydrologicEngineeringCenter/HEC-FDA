using HEC.FDA.Model.utilities;
using HEC.FDA.Model.utilities.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace HEC.FDA.Model.metrics
{
    public enum ThresholdEnum
    {
        [StoredProperty("NotSupported")]
        [DisplayName("Not Supported")]
        NotSupported = 0,
        [DisplayName("Default Exterior Stage")]
        [StoredProperty("DefaultExteriorStage")]
        DefaultExteriorStage = 1,
        [DisplayName("Top Of Levee")]
        [StoredProperty("TopOfLevee")]
        TopOfLevee = 2,
        [DisplayName("Levee System Response")]
        [StoredProperty("LeveeSystemResponse")]
        LeveeSystemResponse = 3,
        [DisplayName("Exterior Stage")]
        [StoredProperty("AdditionalExteriorStage", AlsoKnownAs = ["InteriorStage"])]
        AdditionalExteriorStage = 4,
    }

    public static class ThresholdEnumExtensions
    {
        public static string GetDisplayName(this ThresholdEnum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayNameAttribute>()
                            .DisplayName;
        }
    }
}
