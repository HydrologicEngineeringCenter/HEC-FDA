using HEC.FDA.Model.paireddata;

namespace VisualScratchSpace.Model
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a life loss function
    /// </summary>
    public class LifeLossFunction
    {
        public UncertainPairedData Data {  get; }
        public string SummaryZone { get; set; }
        public string HazardTime {  get; }

        public LifeLossFunction(UncertainPairedData data, string summaryZone, string hazardTime)
        {
            Data = data;
            SummaryZone = summaryZone;
            HazardTime = hazardTime;
        }
    }
}
