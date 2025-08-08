using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.LifeLoss
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a life loss function
    /// </summary>
    public class LifeLossFunction
    {
        public UncertainPairedData Data {  get; }
        public string SimulationName { get; }
        public string SummaryZone { get; set; }
        public string HazardTime {  get; }
        public string[] AlternativeNames { get; }

        public LifeLossFunction(UncertainPairedData data, string[] alternativeNames, string simulationName, string summaryZone, string hazardTime)
        {
            Data = data;
            SimulationName = simulationName;
            SummaryZone = summaryZone;
            HazardTime = hazardTime;
            AlternativeNames = alternativeNames;
        }
    }
}
