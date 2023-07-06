using HEC.FDA.Model.utilities;

namespace HEC.FDA.Model.metrics
{
    public enum ThresholdEnum
    {
        [StoredProperty("NotSupported")]
        NotSupported = 0,
        [StoredProperty("DefaultExteriorStage")]
        DefaultExteriorStage = 1,
        [StoredProperty("TopOfLevee")]
        TopOfLevee = 2,
        [StoredProperty("LeveeSystemResponse")]
        LeveeSystemResponse = 3,
        [StoredProperty("AdditionalExteriorStage", AlsoKnownAs = new[] {"InteriorStage"})]
        AdditionalExteriorStage = 4,
    }
}
