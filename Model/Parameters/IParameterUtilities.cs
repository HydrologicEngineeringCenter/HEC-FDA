using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    internal static class IParameterUtilities
    {
        private static IRange<int> _ElevationRange => IRangeFactory.Factory(21, 29);
        internal static bool IsElevation(this IParameterEnum parameter) => _ElevationRange.IsOnRange((int)parameter);
        internal static string Print(this IParameterEnum parameter, bool abbreviate = false)
        {
            switch (parameter)
            {
                case IParameterEnum.ExceedanceProbability:
                    return abbreviate ? "Probability" : "Exceedance Probability";
                case IParameterEnum.NonExceedanceProbability:
                    return abbreviate ? "Probability" : "Non-Exceedance Probability";
                case IParameterEnum.FailureProbability:
                    return abbreviate ? "Probability" : "Probability of Failure";
                case IParameterEnum.GroundElevation:
                    return abbreviate ? "Elevation" : "Ground Elevation";
                case IParameterEnum.AssetHeight:
                    return abbreviate ? "Height" : "Asset Height";
                case IParameterEnum.AssetElevation:
                    return abbreviate ? "Elevation" : "Asset Elevation";
                case IParameterEnum.LateralStructureElevation:
                    return abbreviate ? "Elevation" : "Lateral Structure Elevation";
                case IParameterEnum.ExteriorElevation:
                    return abbreviate ? "Elevation" : "Exterior (In-channel) Elevation";
                case IParameterEnum.InteriorElevation:
                    return abbreviate ? "Elevation" : "Interior (Floodplain) Elevation";
                case IParameterEnum.InflowFrequency:
                    return abbreviate ? "Flow Frequency Function" : "Unregulated Flow Frequency Function";
                case IParameterEnum.InflowOutflow:
                    return abbreviate ? "Regulation Function" : "Unregulated to Regulated Flow Function";
                case IParameterEnum.OutflowFrequency:
                    return abbreviate ? "Flow Frequency Function" : "Regulated Flow Frequency Function";
                case IParameterEnum.Rating:
                    return abbreviate ? "Rating Function" : "Flow to Exterior (In-channel) Water Surface Elevation Transform Function";
                case IParameterEnum.ExteriorStageFrequency:
                    return abbreviate ? "Stage Frequency Function" : "Exterior (In-channel) Water Surface Elevation Frequency Function";
                case IParameterEnum.ExteriorInteriorStage:
                    return abbreviate ? "Stage Transform Function" : "Exterior (In-channel) to Interior (Floodplain) Water Elevation Transform Function";
                case IParameterEnum.InteriorStageFrequency:
                    return abbreviate ? "Stage Frequency Function" : "Interior (Floodplain) Water Surface Elevation Frequency Function";
                case IParameterEnum.InteriorStageDamage:
                    return abbreviate ? "Stage Damage Function" : "Interior (Floodplain) Water Surface Elevation Flood Damage Transform Function";
                case IParameterEnum.DamageFrequency:
                    return abbreviate ? "Damage Frequency Function" : "Flood Damage Frequency Function";
                case IParameterEnum.LateralStructureFailure:
                    return abbreviate ? "Failure Function" : "Lateral Structure Elevation to Failure Probability Transform Function";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
