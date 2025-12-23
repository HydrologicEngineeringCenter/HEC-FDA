using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public static class StageDamageRunner
{
    public static List<UncertainPairedData> RunStageDamage(string elementName)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Stage damage element name cannot be empty.", nameof(elementName));
        }

        Console.WriteLine($"    Running stage damage compute for '{elementName}'...");

        AggregatedStageDamageElement element = ScenarioRunner.FindElement<AggregatedStageDamageElement>(elementName);

        if (element.IsManual)
        {
            Console.WriteLine($"    Stage damage '{elementName}' is manual - returning existing curves.");
            return ConvertCurvesToUPD(element.Curves);
        }

        var impactAreaElements = BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        if (impactAreaElements.Count == 0)
        {
            throw new InvalidOperationException("No impact area element found in study.");
        }
        ImpactAreaElement impactAreaElement = impactAreaElements[0];

        HydraulicElement hydraulicElement = ScenarioRunner.FindElementById<HydraulicElement>(element.SelectedWSE);
        InventoryElement inventoryElement = ScenarioRunner.FindElementById<InventoryElement>(element.SelectedStructures);

        Console.WriteLine($"    Using hydraulics: {hydraulicElement.Name}");
        Console.WriteLine($"    Using inventory: {inventoryElement.Name}");
        Console.WriteLine($"    Analysis year: {element.AnalysisYear}");

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

        Console.WriteLine($"    Computing stage damage with {totalStructureCount} structures...");

        (List<UncertainPairedData> stageDamageFunctions, _) = scenarioStageDamage.Compute();

        Console.WriteLine($"    Stage damage computation complete. Generated {stageDamageFunctions.Count} curves.");

        return stageDamageFunctions;
    }

    private static List<UncertainPairedData> ConvertCurvesToUPD(List<StageDamageCurve> curves)
    {
        List<UncertainPairedData> result = new();
        if (curves == null) return result;

        foreach (var curve in curves)
        {
            if (curve?.ComputeComponent == null) continue;
            UncertainPairedData upd = curve.ComputeComponent.SelectedItemToPairedData();
            result.Add(upd);
        }
        return result;
    }
}
