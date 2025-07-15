namespace VisualScratchSpace.Model.Saving;
public class PlotFilter : SQLiteFilter
{
    public string[] Simulation { get; init; } = Array.Empty<string>();
    public string[] Summary_Zone { get; init; } = Array.Empty<string>();
    public string[] Hazard_Time { get; init; } = Array.Empty<string>();
    public string[] Alternative { get; init; } = Array.Empty<string>();
} // Property Names MUST match column headers in SQLite exactly, including case
