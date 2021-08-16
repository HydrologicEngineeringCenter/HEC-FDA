namespace Model
{
    /// <summary>
    /// Enumerates types of functions used in Fda:
    /// <list type="bullet">
    /// <item> Odd numbers 1 - 9 Frequency functions with X axis probability values. </item>
    /// <item> Even number 2 - 8 Transform functions used for functional composition. </item>
    /// <item> 10 failure functions. </item>
    /// </list>
    /// </summary>
    public enum IFdaFunctionEnum
    {
        /// <summary>
        /// Default error value.
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Annual peak inflow frequency function.
        /// </summary>
        InflowFrequency = 1,            //AnnualExceedanceChance-> InflowPeakDischarge
        /// <summary>
        /// Annual peak inflow to peak outflow transform function (for modeling dams and other control structures).
        /// </summary>
        InflowOutflow = 2,              //InflowPeakDischarge   -> OutflowPeakDischarge
        /// <summary>
        /// Peak outflow frequency function (annual peak inflow frequencies - only produced through composition).
        /// </summary>
        OutflowFrequency = 3,           //AnnualExceedanceChance-> OutflowPeakDischarge
        /// <summary>
        /// Peak flow to peak exterior (i.e. in-channel) stage transform function.
        /// </summary>
        Rating = 4,                     //OutflowPeakDischarge  -> PeakExteriorStage
        /// <summary>
        /// Peak exterior (i.e. in-channel) stage frequency function.
        /// </summary>
        ExteriorStageFrequency = 5,     //AnnualExceedanceChance-> ExteriorPeakStage
        /// <summary>
        /// Peak exterior (i.e. in-channel) stage to peak interior (i.e. floodplain) stage transform function. 
        /// </summary>
        ExteriorInteriorStage = 6,      //ExteriorPeakStage     -> InteriorPeakStage
        /// <summary>
        /// Peak interior (i.e. floodplain) stage frequency function (annual peak inflow or stage frequencies - only produced through composition).  
        /// </summary>
        InteriorStageFrequency = 7,     //AnnualExceedanceChance-> PeakInteriorStage
        /// <summary>
        /// Peak interior (i.e. floodplain) stage to aggregated damage transform function.
        /// </summary>
        InteriorStageDamage = 8,        //InteriorPeakStage     -> AggregatedDamage
        /// <summary>
        /// Peak annual inflow or stage event damage frequency function (only produced through composition - assumes existence of an undamaged inventory at the time of the annual peak inflow or exterior stage event).
        /// </summary>
        DamageFrequency = 9,            //AnnualExceedanceChance-> AggregatedDamage 
        /// <summary>
        /// Lateral structure exterior stage failure probability function.
        /// </summary>
        LateralStructureFailure = 10,   //ExteriorPeakStage     -> ChanceOfFailure
        //UnUsed = 99,                  //Unknown or Ineligible Type (e.g. non-increasing)
    }
}
