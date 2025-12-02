using RasMapperLib;
using System.Collections.Generic;
namespace HEC.FDA.Model.LifeLoss
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a LifeSim simulation
    /// </summary>
    public class LifeSimSimulation
    {
        public string Name { get; }
        public string HydraulicsFolder { get; }
        public List<string> Alternatives { get; set; }
        public List<string> HazardTimes { get; }
        public Dictionary<string, PointM> SummarySet { get; set; }

        public LifeSimSimulation(string name, string hydraulicsFolder = "")
        {
            Name = name;
            HydraulicsFolder = hydraulicsFolder;
            Alternatives = new List<string>();
            HazardTimes = new List<string>();
            SummarySet = new Dictionary<string, PointM>();
        }
    }
}
