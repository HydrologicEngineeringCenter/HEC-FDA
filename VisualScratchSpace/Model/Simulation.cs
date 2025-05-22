namespace VisualScratchSpace.Model
{
    /// <summary>
    /// Simple data structure representing the fields which comprise a LifeSim simulation
    /// </summary>
    public class Simulation
    {
        public string Name { get; set; } 
        public string[] Alternatives { get; set; }
        public string[] HazardTimes { get; set; }
    }
}
