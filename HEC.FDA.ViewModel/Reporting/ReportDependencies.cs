using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;

namespace HEC.FDA.ViewModel.Reporting;

public class ReportDependencies
{
    private readonly IStudyCache _study;
    public ReportDependencies(IStudyCache study) => _study = study;

    public T Find<T>(int id) where T : ChildElement =>
        _study.GetChildElementsOfType<T>().FirstOrDefault(e => e.ID == id)
        ?? throw new InvalidOperationException($"Required {typeof(T).Name} (ID {id}) no longer exists.");

    public static bool IsDirty(ChildElement element) =>
        element.CustomTreeViewHeader?.Name == element.Name + "*" ||
        element.CustomTreeViewHeader?.Decoration == "*";

    public IEnumerable<ChildElement> StageDamageInputs(AggregatedStageDamageElement element)
    {
        if (element.IsManual) yield break;
        yield return _study.GetStudyPropertiesElement()
            ?? throw new InvalidOperationException("Study properties are required to compute stage damage.");
        var inventory = Find<InventoryElement>(element.SelectedStructures);
        yield return inventory;
        yield return Find<HydraulicElement>(element.SelectedWSE);
        foreach (var impactArea in _study.GetChildElementsOfType<ImpactAreaElement>()) yield return impactArea;
        foreach (int groupId in inventory.OcctypeMapping.Values.Select(o => o.GroupID).Distinct())
            yield return Find<OccupancyTypesElement>(groupId);
        if (inventory.SelectionMappings.IsUsingTerrainFile)
            foreach (var terrain in _study.GetChildElementsOfType<TerrainElement>()) yield return terrain;
        foreach (var row in element.ImpactAreaFrequencyRows)
        {
            if (row.FrequencyFunction?.Element != null)
                yield return Find<FrequencyElement>(row.FrequencyFunction.Element.ID);
            if (row.StageDischargeFunction?.Element != null)
                yield return Find<StageDischargeElement>(row.StageDischargeFunction.Element.ID);
            if (row.RegulatedUnregulatedFunction?.Element != null)
                yield return Find<InflowOutflowElement>(row.RegulatedUnregulatedFunction.Element.ID);
        }
    }

    public IEnumerable<ChildElement> ScenarioInputs(IASElement element)
    {
        yield return _study.GetStudyPropertiesElement()
            ?? throw new InvalidOperationException("Study properties are required to compute scenarios.");
        foreach (var impactArea in _study.GetChildElementsOfType<ImpactAreaElement>()) yield return impactArea;
        foreach (var ias in element.SpecificIASElements)
        {
            if (ias.FlowFreqID >= 0) yield return Find<FrequencyElement>(ias.FlowFreqID);
            if (ias.InflowOutflowID >= 0) yield return Find<InflowOutflowElement>(ias.InflowOutflowID);
            if (ias.RatingID >= 0) yield return Find<StageDischargeElement>(ias.RatingID);
            if (ias.LeveeFailureID >= 0) yield return Find<LateralStructureElement>(ias.LeveeFailureID);
            if (ias.ExtIntStageID >= 0) yield return Find<ExteriorInteriorElement>(ias.ExtIntStageID);
        }
        foreach (int id in StageDamageIds(element)) yield return Find<AggregatedStageDamageElement>(id);
        if (element.FailureStageLifeLossID >= 0) yield return Find<StageLifeLossElement>(element.FailureStageLifeLossID);
        if (element.NonFailureStageLifeLossID >= 0) yield return Find<StageLifeLossElement>(element.NonFailureStageLifeLossID);
    }

    public static IEnumerable<int> StageDamageIds(IASElement element)
    {
        if (element.FailureStageDamageID >= 0) yield return element.FailureStageDamageID;
        if (element.HasNonFailureStageDamage && element.NonFailureStageDamageID >= 0)
            yield return element.NonFailureStageDamageID;
    }
}
