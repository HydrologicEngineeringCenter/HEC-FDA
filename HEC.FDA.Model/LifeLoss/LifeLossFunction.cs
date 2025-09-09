using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.LifeLoss
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a life loss function
    /// </summary>
    public class LifeLossFunction
    {
        /// <summary>
        /// The ID of the Stage Life Loss element which this function belongs to.
        /// </summary>
        public int ElementID { get; set; }
        public UncertainPairedData Data { get; }
        public string SimulationName { get; }
        public string SummaryZone { get; set; }
        public string HazardTime { get; }
        public string[] AlternativeNames { get; }

        public LifeLossFunction(int elementID, UncertainPairedData data, string[] alternativeNames, string simulationName, string summaryZone, string hazardTime)
        {
            ElementID = elementID;
            Data = data;
            SimulationName = simulationName;
            SummaryZone = summaryZone;
            HazardTime = hazardTime;
            AlternativeNames = alternativeNames;
        }
    }
}
