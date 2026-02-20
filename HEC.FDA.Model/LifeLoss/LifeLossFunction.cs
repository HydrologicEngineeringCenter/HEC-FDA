using HEC.FDA.Model.paireddata;
using Statistics.Distributions;

namespace HEC.FDA.Model.LifeLoss
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a life loss function
    /// </summary>
    public class LifeLossFunction
    {
        /// <summary>
        /// The ID of the Stage Life Loss element which this function belongs to
        /// It is always set to -1 in the constructor (in the model layer) and whomever calls the save function (in the VM) is responsible for setting it before saving.
        /// </summary>
        public int ElementID { get; set; }
        /// <summary>
        /// The ID of the function/relationship itself (unique for each function within an element)
        /// </summary>
        public int FunctionID { get; set; }
        public UncertainPairedData Data { get; }
        public string SimulationName { get; }
        public string SummaryZone { get; set; }
        public string HazardTime { get; }
        public string[] AlternativeNames { get; }
        public bool IsCombined => Data?.Yvals.Length > 0 && Data?.Yvals[0] is Empirical;

        public LifeLossFunction(int elementID, int functionID, UncertainPairedData data, string[] alternativeNames, string simulationName, string summaryZone, string hazardTime)
        {
            ElementID = elementID;
            FunctionID = functionID;
            Data = data;
            SimulationName = simulationName;
            SummaryZone = summaryZone;
            HazardTime = hazardTime;
            AlternativeNames = alternativeNames;
        }
    }
}
