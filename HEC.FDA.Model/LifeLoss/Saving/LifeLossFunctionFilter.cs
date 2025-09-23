using System;

namespace HEC.FDA.Model.LifeLoss.Saving;
public class LifeLossFunctionFilter : SQLiteFilter
{
    [SQLColumn(LifeLossStringConstants.ELEMENT_ID_HEADER)]
    public int[] ElementId { get; init; } = Array.Empty<int>();
    public string[] Simulation { get; init; } = Array.Empty<string>();
    public string[] Summary_Zone { get; init; } = Array.Empty<string>();
    public string[] Hazard_Time { get; init; } = Array.Empty<string>();
    public string[] Alternative { get; init; } = Array.Empty<string>();
}
