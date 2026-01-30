using System.Collections.Generic;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeSimImporterConfig
{
    public string LifeSimDatabaseFileName { get; set; }
    public int SelectedHydraulics { get; set; }
    public int SelectedIndexPoints { get; set; }
    public string SelectedSimulation { get; set; }
    public List<string> SelectedAlternatives { get; set; }
    public Dictionary<string, double> SelectedHazardTimes { get; set; }
    public Dictionary<string, double> ComputedHazardTimeWeights { get; set; }
}
