namespace VisualScratchSpace.Model.Saving;
public class PlotFilter : SQLiteFilter
{
    public string[] Simulation { get; init; } = Array.Empty<string>();
}
