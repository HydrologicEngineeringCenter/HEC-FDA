using RasMapperLib;
namespace VisualScratchSpace.Model
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a LifeSim simulation
    /// </summary>
    public class Simulation
    {
        public string Name { get; set; } 
        public string[] Alternatives { get; set; } // array because we use .Split() on the alternatives csv which returns an array
        public List<string> HazardTimes { get; set; } = new(); // list because we do not know how many hazard times there will be
        public Dictionary<string, PointM> SummarySet { get; set; } = new();
    }
}
