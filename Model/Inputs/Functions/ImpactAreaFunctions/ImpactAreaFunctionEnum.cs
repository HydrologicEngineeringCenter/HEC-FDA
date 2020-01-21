namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    public enum ImpactAreaFunctionEnum
    {
        NotSet = 0,
        InflowFrequency = 1,            //AnnualExceedanceChance-> InflowPeakDischarge
        InflowOutflow = 2,              //InflowPeakDischarge   -> OutflowPeakDischarge
        OutflowFrequency = 3,           //AnnualExceedanceChance-> OutflowPeakDischarge
        Rating = 4,                     //OutflowPeakDischarge  -> PeakExteriorStage
        ExteriorStageFrequency = 5,     //AnnualExceedanceChance-> ExteriorPeakStage
        ExteriorInteriorStage = 6,      //ExteriorPeakStage     -> InteriorPeakStage
        InteriorStageFrequency = 7,     //AnnualExceedanceChance-> PeakInteriorStage
        InteriorStageDamage = 8,        //InteriorPeakStage     -> AggregatedDamage
        DamageFrequency = 9,            //AnnualExceedanceChance-> AggregatedDamage 
        LeveeFailure = 10,              //Stage                 -> ChanceOfFailure
        //UnUsed = 99,                  //Unknown or Ineligible Type (e.g. non-increasing)
    }
}
