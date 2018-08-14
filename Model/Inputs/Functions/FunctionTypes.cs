
namespace FdaModel
{
    public enum FunctionTypes
    {
        #region Notes
        /*  1. Frequency Functions are ALWAY EVEN numbered enums. 
            2. The first 3 even numbered functions are user defined types.
            3. They MUST BE ENTERED in LOGICAL COMPUTATIONAL ORDER.
        */
        #endregion

        #region Enums
        InflowFrequency = 0,
        InflowOutflow = 1,              //InflowPeakDischarge   -> OutflowPeakDischarge
        OutflowFrequency = 2,           //AnnualExceedanceChance-> OutflowPeakDischarge
        Rating = 3,                     //OutflowPeakDischarge  -> PeakExteriorStage
        ExteriorStageFrequency = 4,     //AnnualExceedanceChance-> ExteriorPeakStage
        ExteriorInteriorStage = 5,      //ExteriorPeakStage     -> InteriorPeakStage
        InteriorStageFrequency = 6,     //AnnualExceedanceChance-> PeakExteriorStage
        InteriorStageDamage = 7,        //InteriorPeakStage     -> AggregatedDamage
        NoLeveeDamageFrequency = 8,     //AnnualExceedanceChance > AggregatedDamage (withoutlevee)
        StageTruncatedDamage = 9,       //ExteriorPeakStage     -> AggregatedDamage (truncation for levee projects)
        DamageFrequency = 10,           //AnnualExceedanceChance-> AggregatedDamage 
        LeveeFailure = 11,              //Stage                 -> ChanceOfFailure
        UnUsed = 99,                    //Unknown or Ineligible Type (e.g. non-increasing)
        #endregion
    }
}
