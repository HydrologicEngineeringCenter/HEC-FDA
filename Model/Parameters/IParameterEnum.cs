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
    /// <item> 031 - 039: flood damages. </item>
    /// <item> 041 - 049: time. </item>
    /// <item> 101 - 109: compute point functions (i.e. index location) frequency functions (odd values) and transform functions (even values). </item>
    /// <item> 111 - 113: lateral structure, failure function, frequency of stage of failure. </item>
    /// <item> 121 - 129: metrics. </item>
    /// <itme> 131 - 139: metric year functions. </itme>
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
        /// Annual peak unregulated flow.
        /// </summary>
        UnregulatedAnnualPeakFlow = 11,
        /// <summary>
        /// Regulated peak flow.
        /// </summary>
        RegulatedAnnualPeakFlow = 12,

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
        /// Flood damages.
        /// </summary>
        FloodDamages = 31,

        /// <summary>
        /// Analysis year.
        /// </summary>
        Year = 41,

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

        /// <summary>
        /// A lateral structure.
        /// </summary>
        LateralStructure = 111,
        /// <summary>
        /// The frequency of the exterior stage associated with the failure of the lateral structure in a realization. 
        /// </summary>
        LatralStructureFailureElevationFrequency = 113,

        /// <summary>
        /// Annual chance that an exterior (in-channel) water surface elevation is exceeded.
        /// </summary>
        ExteriorStageAEP = 121,
        /// <summary>
        /// Annual chance that an interior (floodplain) water surface elevation is exceeded.
        /// </summary>
        InteriorStageAEP = 122,
        /// <summary>
        /// Annual chance that an amount of flood damages exceeded.
        /// </summary>
        DamageAEP = 123,
        /// <summary>
        /// Expected annual damages.
        /// </summary>
        EAD = 124,
        /// <summary>
        /// Equivalent annual damages (EAD discounted over project analysis period),
        /// </summary>
        EquivalentAnnualDamages,

        /// <summary>
        /// Condition year to exterior stage annual exceedance probability (AEP) function. 
        /// </summary>
        YearExteriorStageAEP = 131,
        /// <summary>
        /// Condition year to interior stage annual exceedance probability (AEP) function.
        /// </summary>
        YearInteriorStageAEP = 132,
        /// <summary>
        /// Condition year to flood damage annual exceedance probability (AEP) function.
        /// </summary>
        YearDamageAEP = 133,
        /// <summary>
        /// Condition year to expected annual damage (EAD) function.
        /// </summary>
        YearEAD = 134,
        /// <summary>
        /// Condition year to equivalent annual damage function.
        /// </summary>
        YearEquavalentAnnualDamages = 135
    }
}
