using HEC.FDA.Model.SQLite;
using System;


namespace HEC.FDA.Model.LifeLoss.Saving;
public class LifeLossFunctionFilter : SQLiteFilter
{
    [SQLColumn(LifeLossStringConstants.ELEMENT_ID_HEADER)]
    public int[] Element_ID { get; init; } = Array.Empty<int>();
    [SQLColumn(LifeLossStringConstants.SIMULATION_HEADER)]
    public string[] Simulation { get; init; } = Array.Empty<string>();
    [SQLColumn(LifeLossStringConstants.SUMMARY_ZONE_HEADER)]
    public string[] Summary_Zone { get; init; } = Array.Empty<string>();
    [SQLColumn(LifeLossStringConstants.HAZARD_TIME_HEADER)]
    public string[] Hazard_Time { get; init; } = Array.Empty<string>();
    [SQLColumn(LifeLossStringConstants.ALTERNATIVE_HEADER)]
    public string[] Alternative { get; init; } = Array.Empty<string>();
}
