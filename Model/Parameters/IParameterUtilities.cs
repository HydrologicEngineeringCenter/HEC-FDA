using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    internal static class IParameterUtilities
    {
        private static IRange<int> _ProbabilityRange => IRangeFactory.Factory(1, 9);
        private static IRange<int> _FlowRange => IRangeFactory.Factory(11, 19);
        private static IRange<int> _ElevationRange => IRangeFactory.Factory(21, 29);
        private static IRange<int> _DamageRange => IRangeFactory.Factory(31, 39);

        internal static bool IsProbability(this IParameterEnum parameter, bool xUnits = false) => 
            _ProbabilityRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.LateralStructureFailure ||
            !xUnits &&
            parameter == IParameterEnum.InflowFrequency ||
            parameter == IParameterEnum.OutflowFrequency ||
            parameter == IParameterEnum.ExteriorStageFrequency ||
            parameter == IParameterEnum.InteriorStageFrequency ||
            parameter == IParameterEnum.DamageFrequency;
        internal static bool IsFlow(this IParameterEnum parameter, bool xUnits = false) =>
            _FlowRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.InflowOutflow ||
            (xUnits && parameter == IParameterEnum.Rating) ||
            (!xUnits && (parameter == IParameterEnum.InflowFrequency || 
                        parameter == IParameterEnum.OutflowFrequency || 
                        parameter == IParameterEnum.Rating));
        internal static bool IsElevation(this IParameterEnum parameter, bool xUnits = false) =>
            _ElevationRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.ExteriorInteriorStage ||
            (xUnits &&  parameter == IParameterEnum.InteriorStageDamage) ||
            (!xUnits && (parameter == IParameterEnum.ExteriorStageFrequency ||
                        parameter == IParameterEnum.InteriorStageFrequency));
        internal static bool IsDamage(this IParameterEnum parameter, bool xUnits = false) => 
            _DamageRange.IsOnRange((int)parameter) ||
            (!xUnits && (parameter == IParameterEnum.InteriorStageDamage ||
                        parameter == IParameterEnum.DamageFrequency));

        internal static UnitsEnum DefaultUnits(this IParameterEnum parameter)
        {
            if (parameter.IsProbability()) return UnitsEnum.Probability;
            else if (parameter.IsFlow()) return UnitsEnum.CubicFootPerSecond;
            else if (parameter.IsElevation()) return UnitsEnum.Foot;
            else if (parameter.IsDamage()) return UnitsEnum.Dollars;
            else return UnitsEnum.NotSet;
        }

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
                
                case IParameterEnum.UnregulatedAnnualPeakFlow:
                    return abbreviate ? "Unregulated Flow" : "Annual Unregulated Peak Flow";
                case IParameterEnum.RegulatedAnnualPeakFlow:
                    return abbreviate ? "Regulated Flow" : "Regulated Peak Flow";
                
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

                case IParameterEnum.FloodDamages:
                    return abbreviate ? "Damages" : "Annual Peak Event Flood Damages";
                
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
