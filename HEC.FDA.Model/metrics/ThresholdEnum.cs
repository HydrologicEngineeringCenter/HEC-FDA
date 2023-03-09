namespace HEC.FDA.Model.metrics
{
    public enum ThresholdEnum
    {
        NotSupported = 0,
        DefaultExteriorStage = 1,
        TopOfLevee = 2,
        LeveeSystemResponse = 3,
        AdditionalExteriorStage = 4,
        //TODO: Do these need to remain for backward compatibility? 
        ExteriorStage = 5,
        InteriorStage = 6,
        Damage = 7
    }
}
