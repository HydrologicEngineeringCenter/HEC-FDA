namespace VisualScratchSpace.Model.Saving;
public class PlotFilter : SQLiteFilter
{
    public string[] Simulations { get; init; } = Array.Empty<string>();
}
