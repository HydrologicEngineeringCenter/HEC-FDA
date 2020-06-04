using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Enumerates parameter types in Fda.
    /// <list type="bullet">
    /// <listheader> Enumerated values structure: </listheader>
    /// <item> 000: default value. </item>
    /// <item> 001 - 009: probabilities. </item> 
    /// <item> 011 - 019: flows. </item>
    /// <item> 021 - 029: elevation/lengths (odd values include ground elevation, even values do not). </item>
    /// <item> 101 - 109: compute point functions (i.e. index location) frequency functions (odd values) and transform functions (even values). </item>
    /// <item> 110: lateral structure failure function. </item>
    /// </list>
    /// </summary>
    public enum IParameterEnum
    {
        /// <summary>
        /// Default value.
        /// </summary>
        NotSet = 0,


        /// <summary>
        /// The probability that an equal or larger value is selected or occurs.
        /// </summary>
        ExceedanceProbability = 1,
        /// <summary>
        /// The probability that a smaller value is selected or occurs.
        /// </summary>
        NonExceedanceProbability = 2,
        /// <summary>
        /// A chance of failure.
        /// </summary>
        FailureProbability = 3,


        /// <summary>
        /// Ground elevation.
        /// </summary>
        GroundElevation = 21,
        /// <summary>
        /// Height of asset above ground elevation, exclusive of the ground elevation.
        /// </summary>
        AssetHeight = 22,
        /// <summary>
        /// Asset elevation, including the ground elevation.
        /// </summary>
        AssetElevation = 23,
        /// <summary>
        /// Levee or flood wall elevation, including the ground elevation.
        /// </summary>
        LateralStructureElevation = 25,
        /// <summary>
        /// In-channel elevation.
        /// </summary>
        ExteriorElevation = 27,
        /// <summary>
        /// Floodplain elevation.
        /// </summary>
        InteriorElevation = 29,


        /// <summary>
        /// Annual peak inflow frequency function.
        /// </summary>
        InflowFrequency = 101,            //AnnualExceedanceChance-> InflowPeakDischarge
        /// <summary>
        /// Annual peak inflow to peak outflow transform function (for modeling dams and other control structures).
        /// </summary>
        InflowOutflow = 102,              //InflowPeakDischarge   -> OutflowPeakDischarge
        /// <summary>
        /// Peak outflow frequency function (annual peak inflow frequencies - only produced through composition).
        /// </summary>
        OutflowFrequency = 103,           //AnnualExceedanceChance-> OutflowPeakDischarge
        /// <summary>
        /// Peak flow to peak exterior (i.e. in-channel) stage transform function.
        /// </summary>
        Rating = 104,                     //OutflowPeakDischarge  -> PeakExteriorStage
        /// <summary>
        /// Peak exterior (i.e. in-channel) stage frequency function.
        /// </summary>
        ExteriorStageFrequency = 105,     //AnnualExceedanceChance-> ExteriorPeakStage
        /// <summary>
        /// Peak exterior (i.e. in-channel) stage to peak interior (i.e. floodplain) stage transform function. 
        /// </summary>
        ExteriorInteriorStage = 106,      //ExteriorPeakStage     -> InteriorPeakStage
        /// <summary>
        /// Peak interior (i.e. floodplain) stage frequency function (annual peak inflow or stage frequencies - only produced through composition).  
        /// </summary>
        InteriorStageFrequency = 107,     //AnnualExceedanceChance-> PeakInteriorStage
        /// <summary>
        /// Peak interior (i.e. floodplain) stage to aggregated damage transform function.
        /// </summary>
        InteriorStageDamage = 108,        //InteriorPeakStage     -> AggregatedDamage
        /// <summary>
        /// Peak annual inflow or stage event damage frequency function (only produced through composition - assumes existence of an undamaged inventory at the time of the annual peak inflow or exterior stage event).
        /// </summary>
        DamageFrequency = 109,            //AnnualExceedanceChance-> AggregatedDamage 

        /// <summary>
        /// Lateral structure exterior stage failure probability function.
        /// </summary>
        LateralStructureFailure = 110,   //ExteriorPeakStage     -> ChanceOfFailure
    }
}
