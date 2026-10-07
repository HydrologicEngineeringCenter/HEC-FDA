using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;
using Utility.Progress;

namespace HEC.FDA.ViewModel.Reporting;

public record StageDamageResult(
    List<UncertainPairedData> StageDamageFunctions,
    List<UncertainPairedData> DamagedElementCounts,
    ScenarioStageDamage ScenarioStageDamage,
    ImpactAreaElement ImpactAreaElement);

public static class StageDamageRunner
{
    public static StageDamageResult RunStageDamage(string elementName)
        => RunStageDamage(ScenarioRunner.FindElement<AggregatedStageDamageElement>(elementName),
            ProgressReporter.ConsoleWrite(), Console.WriteLine, CancellationToken.None);

    public static StageDamageResult RunStageDamage(AggregatedStageDamageElement element,
        ProgressReporter reporter, Action<string> log, CancellationToken cancellationToken)
    {
        string elementName = element.Name;
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Stage damage element name cannot be empty.", nameof(elementName));
        }

        log($"    Running stage damage compute for '{elementName}'...");

        var impactAreaElements = BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        if (impactAreaElements.Count == 0)
        {
            throw new InvalidOperationException("No impact area element found in study.");
        }
        ImpactAreaElement impactAreaElement = impactAreaElements[0];

        if (element.IsManual)
        {
            log($"    Stage damage '{elementName}' is manual - returning existing curves.");
            return new StageDamageResult(ConvertCurvesToUPD(element.Curves), [], null, impactAreaElement);
        }

        HydraulicElement hydraulicElement = ScenarioRunner.FindElementById<HydraulicElement>(element.SelectedWSE);
        InventoryElement inventoryElement = ScenarioRunner.FindElementById<InventoryElement>(element.SelectedStructures);

        log($"    Using hydraulics: {hydraulicElement.Name}");
        log($"    Using inventory: {inventoryElement.Name}");
        log($"    Analysis year: {element.AnalysisYear}");

        StageDamageConfiguration config = new(
            impactAreaElement,
            hydraulicElement,
            inventoryElement,
            element.ImpactAreaFrequencyRows,
            element.AnalysisYear);

        FdaValidationResult validation = config.ValidateConfiguration();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Stage damage configuration is invalid: {validation.ErrorMessage}");
        }

        List<ImpactAreaStageDamage> impactAreaStageDamages = config.CreateStageDamages();
        if (impactAreaStageDamages.Count == 0)
        {
            throw new InvalidOperationException("No impact area stage damages could be created.");
        }

        ScenarioStageDamage scenarioStageDamage = new(impactAreaStageDamages);

        int totalStructureCount = impactAreaStageDamages.Sum(sd => sd.Inventory?.Structures?.Count ?? 0);
        if (totalStructureCount == 0)
        {
            throw new InvalidOperationException("No structures found in any impact area for stage damage compute.");
        }

        log($"    Computing stage damage with {totalStructureCount} structures...");

        (List<UncertainPairedData> stageDamageFunctions, List<UncertainPairedData> damagedElementCounts) = scenarioStageDamage.Compute(false, reporter, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (stageDamageFunctions.Count == 0)
            throw new InvalidOperationException($"Stage damage '{elementName}' produced no curves.");
        foreach (string error in scenarioStageDamage.GetErrorMessages())
            log($"Warning: {error}");

        log($"    Stage damage computation complete. Generated {stageDamageFunctions.Count} curves.");

        return new StageDamageResult(stageDamageFunctions, damagedElementCounts, scenarioStageDamage, impactAreaElement);
    }

    public static List<UncertainPairedData> ConvertCurvesToUPD(List<StageDamageCurve> curves)
    {
        List<UncertainPairedData> result = new();
        if (curves == null) return result;

        foreach (var curve in curves)
        {
            if (curve?.ComputeComponent == null) continue;
            UncertainPairedData upd = curve.ComputeComponent.SelectedItemToPairedData(curve.DamCat, curve.AssetCategory);
            // CurveComponentVM serializes the ordinates; the owning row supplies area/category identity.
            var metadata = new CurveMetaData(upd.XLabel, upd.YLabel, upd.Name, curve.DamCat,
                curve.ImpArea.ID, curve.AssetCategory);
            result.Add(new UncertainPairedData(upd.Xvals, upd.Yvals, metadata));
        }
        return result;
    }
}
