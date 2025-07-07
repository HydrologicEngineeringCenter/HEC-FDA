using HEC.FDA.Model.paireddata;

namespace VisualScratchSpace.Model
{
    public class LifeLossRelationship
    {
        public UncertainPairedData Data {  get; set; }
        public string SummaryZone { get; set; }
        public string HazardTime {  get; set; }
    }
}
