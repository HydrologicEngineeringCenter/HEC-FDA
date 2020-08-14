using System;
using Utilities;

namespace Model
{
    /// <summary>
    /// Utility and extension methods for objects implementing the <see cref="IParameter"/>, <see cref="IParameterRange"/> and <see cref="IParameterOrdinate"/> interfaces.
    /// </summary>
    public static class IParameterUtilities
    {
        /// <summary>
        /// The acceptable range ground elevations. 
        /// </summary>
        /// <remarks>
        /// The min is set to the lowest earth ground elevation: -1,355 feet, the Dead Sea Depression.
        /// The max is set to the highest earth ground elevation: 29,035 feet, the summit of Mount Everest.
        /// </remarks>
        internal static IRange<double> GroundElevationRange = IRangeFactory.Factory(-1365d, 29035);
        /// <summary>
        /// The acceptable range of elevations for lateral structures in feet.
        /// </summary>
        /// <remarks> 
        /// The min is set to the lowest earth ground elevation: -1,355 feet (the Dead Sea Depression). 
        /// The max is set to the highest earth ground elevation 29,035 feet (the summit of Mount Everest) plus the elevation of the highest dam on earth: 1001 feet (the Jinping-I Dam).  
        /// </remarks>
        internal static IRange<double> LateralStructureElevationRange = IRangeFactory.Factory(-1365d, GroundElevationRange.Max + 1001d);


        private static IRange<int> _ProbabilityRange => IRangeFactory.Factory(1, 9);
        private static IRange<int> _FlowRange => IRangeFactory.Factory(11, 19);
        private static IRange<int> _ElevationRange => IRangeFactory.Factory(21, 29);
        private static IRange<int> _DamageRange => IRangeFactory.Factory(31, 39);
        private static IRange<int> _FunctionRange => IRangeFactory.Factory(101, 110);

        /// <summary>
        /// Tests if the <see cref="IParameterEnum"/> is measured as a probability.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> being evaluated. </param>
        /// <returns> <see langword="true"/> if the <see cref="IParameterEnum"/> is measured as a probability, <see langword="false"/> otherwise. </returns>
        public static bool IsProbability(this IParameterEnum parameter) => 
            _ProbabilityRange.IsOnRange((int)parameter) ||  
            parameter == IParameterEnum.LateralStructureFailure ||
            parameter == IParameterEnum.InflowFrequency ||
            parameter == IParameterEnum.OutflowFrequency ||
            parameter == IParameterEnum.ExteriorStageFrequency ||
            parameter == IParameterEnum.InteriorStageFrequency ||
            parameter == IParameterEnum.DamageFrequency;
        /// <summary>
        /// Tests if the <see cref="IParameter"/> is measured as a probability.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameter"/> is measured as a probability, <see langword="false"/> otherwise. </returns>
        public static bool IsProbability(this IParameter parameter) => parameter.ParameterType.IsProbability();
        
        /// <summary>
        /// Tests if the <see cref="IParameterEnum"/> is measured as a flow.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameterEnum"/> is measured as a flow, <see langword="false"/> otherwise. </returns>
        public static bool IsFlow(this IParameterEnum parameter) =>
            _FlowRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.InflowFrequency ||
            parameter == IParameterEnum.InflowOutflow ||
            parameter == IParameterEnum.OutflowFrequency ||
            parameter == IParameterEnum.Rating;
        /// <summary>
        /// Tests if the <see cref="IParameter"/> is measured as a flow.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameter"/> is measured as a flow, <see langword="false"/> otherwise. </returns>
        public static bool IsFlow(this IParameter parameter) => parameter.ParameterType.IsFlow();

        /// <summary>
        /// Tests if the <see cref="IParameterEnum"/> is measured as an elevation.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameterEnum"/> is measured as an elevation, <see langword="false"/> otherwise. </returns>
        public static bool IsElevation(this IParameterEnum parameter) =>
            _ElevationRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.Rating ||
            parameter == IParameterEnum.ExteriorStageFrequency ||
            parameter == IParameterEnum.ExteriorInteriorStage  ||
            parameter == IParameterEnum.InteriorStageFrequency ||
            parameter == IParameterEnum.InteriorStageDamage;
        /// <summary>
        /// Tests if the <see cref="IParameter"/> is measured as an elevation.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameter"/> is measured as an elevation, <see langword="false"/> otherwise. </returns>
        public static bool IsElevation(this IParameter parameter) => parameter.ParameterType.IsElevation();
        
        /// <summary>
        /// Tests if the <see cref="IParameterEnum"/> is measured in units of dollars.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameterEnum"/> is measured in units of dollars, <see langword="false"/> otherwise. </returns>
        public static bool IsDamage(this IParameterEnum parameter) =>
            _DamageRange.IsOnRange((int)parameter) ||
            parameter == IParameterEnum.InteriorStageDamage ||
            parameter == IParameterEnum.DamageFrequency;
        /// <summary>
        /// Tests if the <see cref="IParameter"/> is measured in units of dollars.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> object to test. </param>
        /// <returns><see langword="true"/> if the <see cref="IParameter"/> is measured in units of dollars, <see langword="false"/> otherwise. </returns>
        public static bool IsDamage(this IParameter parameter) => parameter.ParameterType.IsDamage();

        /// <summary>
        /// Provides the default unit of measurement (<seealso cref="UnitsEnum"/> for the <see cref="IParameterEnum"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> to describe. </param>
        /// <returns> The <see cref="UnitsEnum"/> for the default unit of measurement. </returns>
        public static UnitsEnum DefaultUnits(this IParameterEnum parameter)
        {
            if (parameter.IsProbability()) return UnitsEnum.Probability;
            else if (parameter.IsFlow()) return UnitsEnum.CubicFootPerSecond;
            else if (parameter.IsElevation()) return UnitsEnum.Foot;
            else if (parameter.IsDamage()) return UnitsEnum.Dollars;
            else return UnitsEnum.NotSet;
        }
        /// <summary>
        /// Prints a text description of the <see cref="IParameterEnum"/> value.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> to be printed as text. </param>
        /// <param name="abbreviate"> <see langword="true"/> if an abbreviated description should be printed, <see langword="false"/> otherwise. </param>
        /// <returns></returns>
        public static string Print(this IParameterEnum parameter, bool abbreviate = false)
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
                    return abbreviate ? "Regulation Function" : "Unregulated to Regulated Flow Transform Function";
                case IParameterEnum.OutflowFrequency:
                    return abbreviate ? "Flow Frequency Function" : "Regulated Flow Frequency Function";
                case IParameterEnum.Rating:
                    return abbreviate ? "Rating Function" : "Flow to Exterior (In-channel) Water Surface Elevation Transform Function";
                case IParameterEnum.ExteriorStageFrequency:
                    return abbreviate ? "Stage Frequency Function" : "Exterior (In-channel) Water Surface Elevation Frequency Function";
                case IParameterEnum.ExteriorInteriorStage:
                    return abbreviate ? "Exterior to Interior Stage Transform Function" : "Exterior (In-channel) to Interior (Floodplain) Water Elevation Transform Function";
                case IParameterEnum.InteriorStageFrequency:
                    return abbreviate ? "Stage Frequency Function" : "Interior (Floodplain) Water Surface Elevation Frequency Function";
                case IParameterEnum.InteriorStageDamage:
                    return abbreviate ? "Stage Damage Function" : "Interior (Floodplain) Water Surface Elevation to Flood Damage Transform Function";
                case IParameterEnum.DamageFrequency:
                    return abbreviate ? "Damage Frequency Function" : "Flood Damage Frequency Function";


                case IParameterEnum.LateralStructure:
                    return "Lateral Structure";
                case IParameterEnum.LateralStructureFailure:
                    return abbreviate ? "Lateral Structure Failure Function" : "Lateral Structure Elevation to Failure Probability Transform Function";
                case IParameterEnum.LatralStructureFailureElevationFrequency:
                    return abbreviate ? "Frequency of Failure Stage" : "Frequency of Exterior Stage Associated with Lateral Structure Failure";
                default:
                    throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Prints a text label for the <see cref="IParameterEnum"/> containing a description (<seealso cref="Print(IParameterEnum, bool)"/> of the <see cref="IParameterEnum"/> and the <see cref="UnitsEnum"/> (<seealso cref="UnitsUtilities.Print(UnitsEnum, bool)"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> to be evaluated. </param>
        /// <param name="units"> The <see cref="UnitsEnum"/> for the <see cref="IParameterEnum"/>. The default units for the <see cref="IParameterEnum"/> are used if none are provided. </param>
        /// <param name="abbreviate"><see langword="true"/> if the label should be abbreviated in form, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>        
        public static string PrintLabel(this IParameterEnum parameter, UnitsEnum units = UnitsEnum.NotSet, bool abbreviate = true)
        {
            string unitsLabel = units == UnitsEnum.NotSet ? parameter.DefaultUnits().Print(abbreviate) : units.Print(abbreviate);
            return $"{parameter.Print(abbreviate)} {unitsLabel}";
        }
        /// <summary>
        /// Prints a text label for the <see cref="IParameter"/> containing a description of its <see cref="IParameter.ParameterType"/> property and <see cref="UnitsEnum"/> (<seealso cref="UnitsUtilities.Print(UnitsEnum, bool)"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> to be evaluated. </param>
        /// <param name="units"> The <see cref="UnitsEnum"/> for the <see cref="IParameter"/>. The <see cref="IParameter.ParameterType"/> default units are used if none are provided. </param>
        /// <param name="abbreviate"><see langword="true"/> if the label should be abbreviated in form, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintLabel(this IParameter parameter, UnitsEnum units = UnitsEnum.NotSet, bool abbreviate = true)
        {
            return PrintLabel(parameter.ParameterType, units, abbreviate);
        }
        /// <summary>
        /// Prints a text label for the <see cref="IParameterRange"/> containing a description of its <see cref="IParameter.ParameterType"/> and <see cref="IParameterRange.Units"/> properties (<seealso cref="UnitsUtilities.Print(UnitsEnum, bool)"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterRange"/> to be evaluated. </param>      
        /// <param name="abbreviate"><see langword="true"/> if the label should be abbreviated in form, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintLabel(this IParameterRange parameter, bool abbreviate = true)
        {
            return PrintLabel(parameter.ParameterType, parameter.Units, abbreviate);
        }
        /// <summary>
        /// Prints a text label for the <see cref="IParameterOrdinate"/> containing a description of its <see cref="IParameter.ParameterType"/> and <see cref="IParameterOrdinate.Units"/> properties (<seealso cref="UnitsUtilities.Print(UnitsEnum, bool)"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterOrdinate"/> to be evaluated. </param>      
        /// <param name="abbreviate"><see langword="true"/> if the label should be abbreviated in form, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintLabel(this IParameterOrdinate parameter, bool abbreviate = true)
        {
            return PrintLabel(parameter.ParameterType, parameter.Units, abbreviate);
        }
        /// <summary>
        /// Prints a label for the <see cref="IParameterEnum"/> x axis, if one exists.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> to be evaluated. </param>
        /// <param name="units"> The unit of measure (<seealso cref="UnitsEnum"/>) for the <see cref="IParameterEnum"/>. The <see cref="IParameterEnum"/> default units are used if none are provided (<seealso cref="IParameterUtilities.DefaultUnits(IParameterEnum)"/>). </param>
        /// <param name="abbreviate"> <see langword="true"/> if an abbreviated label should be printed, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintXLabel (IParameterEnum parameter, UnitsEnum units = UnitsEnum.NotSet, bool abbreviate = true)
        {
            switch (parameter) 
            {
                case IParameterEnum.InflowFrequency:
                case IParameterEnum.OutflowFrequency:
                case IParameterEnum.ExteriorStageFrequency:
                case IParameterEnum.InteriorStageFrequency:
                case IParameterEnum.DamageFrequency:
                    return PrintLabel(IParameterEnum.ExceedanceProbability, units, abbreviate);
                case IParameterEnum.InflowOutflow:
                    return PrintLabel(IParameterEnum.UnregulatedAnnualPeakFlow, units, abbreviate);
                case IParameterEnum.Rating:
                    return PrintLabel(IParameterEnum.RegulatedAnnualPeakFlow, units, abbreviate);
                case IParameterEnum.ExteriorInteriorStage:
                    return PrintLabel(IParameterEnum.ExteriorElevation, units, abbreviate);
                case IParameterEnum.InteriorStageDamage:
                    return PrintLabel(IParameterEnum.InteriorElevation, units, abbreviate);
                case IParameterEnum.LateralStructureFailure:
                    return PrintLabel(IParameterEnum.ExteriorElevation, units, abbreviate);
                default:
                    return "No x axis.";
            }

        }
        /// <summary>
        /// Prints a label for the <see cref="IParameterEnum"/> y axis, if one exists.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameterEnum"/> to be evaluated. </param>
        /// <param name="units"> The unit of measure (<seealso cref="UnitsEnum"/>) for the <see cref="IParameterEnum"/>. The <see cref="IParameterEnum"/> default units are used if none are provided (<seealso cref="IParameterUtilities.DefaultUnits(IParameterEnum)"/>). </param>
        /// <param name="abbreviate"> <see langword="true"/> if an abbreviated label should be printed, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintYLabel(IParameterEnum parameter, UnitsEnum units = UnitsEnum.NotSet, bool abbreviate = true)
        {
            switch (parameter)
            {
                case IParameterEnum.InflowFrequency:
                    return PrintLabel(IParameterEnum.UnregulatedAnnualPeakFlow, units, abbreviate);
                case IParameterEnum.InflowOutflow:
                case IParameterEnum.OutflowFrequency:
                    return PrintLabel(IParameterEnum.RegulatedAnnualPeakFlow, units, abbreviate);
                case IParameterEnum.Rating:
                case IParameterEnum.ExteriorStageFrequency:
                    return PrintLabel(IParameterEnum.ExteriorElevation, units, abbreviate);
                case IParameterEnum.ExteriorInteriorStage:
                case IParameterEnum.InteriorStageFrequency:
                    return PrintLabel(IParameterEnum.InteriorElevation, units, abbreviate);
                case IParameterEnum.InteriorStageDamage:
                case IParameterEnum.DamageFrequency:
                    return PrintLabel(IParameterEnum.FloodDamages, units, abbreviate);
                case IParameterEnum.LateralStructureFailure:
                    return PrintLabel(IParameterEnum.FailureProbability, units, abbreviate);
                default:
                    return "No x axis.";
            }
        }
    }
}
