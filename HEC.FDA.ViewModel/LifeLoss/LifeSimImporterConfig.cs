using System.Collections.Generic;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeSimImporterConfig
{
    public string LifeSimDatabasePath { get; set; }
    public int SelectedHydraulics { get; set; }
    public int SelectedIndexPoints { get; set; }
    public string SelectedSimulation { get; set; }
    public List<string> SelectedAlternatives { get; set; }
    public List<string> SelectedHazardTimes { get; set; }
}
