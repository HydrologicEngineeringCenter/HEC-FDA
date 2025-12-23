using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public class StageDamageRunner
{
    public List<UncertainPairedData> RunStageDamage(string elementName)
    {
        Console.WriteLine($"    Running stage damage compute for '{elementName}'...");

        // Find element by name
        AggregatedStageDamageElement element = FindElement<AggregatedStageDamageElement>(elementName);

        // Check if this is a manual stage damage (no compute needed)
        if (element.IsManual)
        {
            Console.WriteLine($"    Stage damage '{elementName}' is manual - returning existing curves.");
            return ConvertCurvesToUPD(element.Curves);
        }

        // Get the referenced elements needed for compute
        ImpactAreaElement impactAreaElement = GetImpactAreaElement();
        HydraulicElement hydraulicElement = FindElementById<HydraulicElement>(element.SelectedWSE);
        InventoryElement inventoryElement = FindElementById<InventoryElement>(element.SelectedStructures);

        Console.WriteLine($"    Using hydraulics: {hydraulicElement.Name}");
        Console.WriteLine($"    Using inventory: {inventoryElement.Name}");
        Console.WriteLine($"    Analysis year: {element.AnalysisYear}");

        // Create the configuration
        StageDamageConfiguration config = new(
            impactAreaElement,
            hydraulicElement,
            inventoryElement,
            element.ImpactAreaFrequencyRows,
            element.AnalysisYear);

        // Validate the configuration
        FdaValidationResult validation = config.ValidateConfiguration();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Stage damage configuration is invalid: {validation.ErrorMessage}");
        }

        // Create the stage damages and compute
        List<ImpactAreaStageDamage> impactAreaStageDamages = config.CreateStageDamages();
        ScenarioStageDamage scenarioStageDamage = new(impactAreaStageDamages);

        // Validate structure count
        int totalStructureCount = impactAreaStageDamages.Sum(sd => sd.Inventory.Structures.Count);
        if (totalStructureCount == 0)
        {
            throw new InvalidOperationException("No structures found in any impact area for stage damage compute.");
        }

        Console.WriteLine($"    Computing stage damage with {totalStructureCount} structures...");

        // Run the compute
        (List<UncertainPairedData> stageDamageFunctions, List<UncertainPairedData> _) = scenarioStageDamage.Compute();

        Console.WriteLine($"    Stage damage computation complete. Generated {stageDamageFunctions.Count} curves.");

        return stageDamageFunctions;
    }

    private static List<UncertainPairedData> ConvertCurvesToUPD(List<StageDamageCurve> curves)
    {
        List<UncertainPairedData> result = new();
        foreach (var curve in curves)
        {
            UncertainPairedData upd = curve.ComputeComponent.SelectedItemToPairedData();
            result.Add(upd);
        }
        return result;
    }

    private static ImpactAreaElement GetImpactAreaElement()
    {
        var impactAreaElements = BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        if (impactAreaElements.Count == 0)
        {
            throw new InvalidOperationException("No impact area element found in study.");
        }
        return impactAreaElements[0];
    }

    private static T FindElement<T>(string elementName) where T : ChildElement
    {
        var elements = BaseViewModel.StudyCache.GetChildElementsOfType<T>();

        var match = elements.FirstOrDefault(e =>
            e.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            string availableNames = string.Join(", ", elements.Select(e => e.Name));
            throw new InvalidOperationException(
                $"Element '{elementName}' of type {typeof(T).Name} not found. Available: {availableNames}");
        }

        return match;
    }

    private static T FindElementById<T>(int id) where T : ChildElement
    {
        var elements = BaseViewModel.StudyCache.GetChildElementsOfType<T>();

        var match = elements.FirstOrDefault(e => e.ID == id);

        if (match == null)
        {
            string availableIds = string.Join(", ", elements.Select(e => $"{e.Name}({e.ID})"));
            throw new InvalidOperationException(
                $"Element of type {typeof(T).Name} with ID {id} not found. Available: {availableIds}");
        }

        return match;
    }
}
