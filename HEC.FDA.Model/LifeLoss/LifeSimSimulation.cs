using HEC.FDA.Model.hydraulics.enums;
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
        public HydraulicDataSource HydraulicsDataSource { get; }
        public List<string> Alternatives { get; set; }
        public Dictionary<string, double> HazardTimes { get; }
        public Dictionary<string, PointM> SummarySet { get; set; }

        public LifeSimSimulation(string name, string hydraulicsFolder = "", HydraulicDataSource source = HydraulicDataSource.UnsteadyHDF)
        {
            Name = name;
            HydraulicsFolder = hydraulicsFolder;
            HydraulicsDataSource = source;
            Alternatives = new List<string>();
            HazardTimes = new Dictionary<string, double>();
            SummarySet = new Dictionary<string, PointM>();
        }
    }
}
