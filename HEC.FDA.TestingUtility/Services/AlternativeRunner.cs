using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public class AlternativeRunner
{
    public AlternativeResults RunAlternative(string elementName, CancellationToken cancellationToken)
    {
        // Find element by name
        AlternativeElement element = FindElement<AlternativeElement>(elementName);

        Console.WriteLine($"    Running alternative '{elementName}'...");

        // Validate scenarios have results
        FdaValidationResult validation = element.RunPreComputeValidation();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Alternative cannot compute: {validation.ErrorMessage}");
        }

        // Get study properties
        StudyPropertiesElement props = BaseViewModel.StudyCache.GetStudyPropertiesElement();

        // Get scenario results from the referenced scenarios
        IASElement baseScenarioElement = element.BaseScenario.GetElement();
        IASElement futureScenarioElement = element.FutureScenario.GetElement();

        if (baseScenarioElement.Results == null)
        {
            throw new InvalidOperationException($"Base scenario '{baseScenarioElement.Name}' has no computed results.");
        }

        if (futureScenarioElement.Results == null)
        {
            throw new InvalidOperationException($"Future scenario '{futureScenarioElement.Name}' has no computed results.");
        }

        ScenarioResults baseResults = baseScenarioElement.Results;
        ScenarioResults futureResults = futureScenarioElement.Results;

        Console.WriteLine($"    Using base scenario: {baseScenarioElement.Name} (Year: {element.BaseScenario.Year})");
        Console.WriteLine($"    Using future scenario: {futureScenarioElement.Name} (Year: {element.FutureScenario.Year})");
        Console.WriteLine($"    Discount rate: {props.DiscountRate}, Period of analysis: {props.PeriodOfAnalysis}");

        // Compute
        AlternativeResults results = Alternative.AnnualizationCompute(
            props.DiscountRate,
            props.PeriodOfAnalysis,
            element.ID,
            baseResults,
            futureResults,
            element.BaseScenario.Year,
            element.FutureScenario.Year);

        Console.WriteLine($"    Alternative computation complete.");

        return results;
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
}
