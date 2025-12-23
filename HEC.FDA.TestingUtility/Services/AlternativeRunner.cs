using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public static class AlternativeRunner
{
    public static AlternativeResults RunAlternative(string elementName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Alternative element name cannot be empty.", nameof(elementName));
        }

        AlternativeElement element = ScenarioRunner.FindElement<AlternativeElement>(elementName);

        Console.WriteLine($"    Running alternative '{elementName}'...");

        FdaValidationResult validation = element.RunPreComputeValidation();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Alternative cannot compute: {validation.ErrorMessage}");
        }

        StudyPropertiesElement? props = BaseViewModel.StudyCache.GetStudyPropertiesElement();
        if (props == null)
        {
            throw new InvalidOperationException("Study properties not found.");
        }

        IASElement? baseScenarioElement = element.BaseScenario?.GetElement();
        IASElement? futureScenarioElement = element.FutureScenario?.GetElement();

        if (baseScenarioElement == null)
        {
            throw new InvalidOperationException("Base scenario element not found.");
        }

        if (futureScenarioElement == null)
        {
            throw new InvalidOperationException("Future scenario element not found.");
        }

        if (baseScenarioElement.Results == null)
        {
            throw new InvalidOperationException($"Base scenario '{baseScenarioElement.Name}' has no computed results. Run the scenario first.");
        }

        if (futureScenarioElement.Results == null)
        {
            throw new InvalidOperationException($"Future scenario '{futureScenarioElement.Name}' has no computed results. Run the scenario first.");
        }

        ScenarioResults baseResults = baseScenarioElement.Results;
        ScenarioResults futureResults = futureScenarioElement.Results;

        int baseYear = element.BaseScenario?.Year ?? 0;
        int futureYear = element.FutureScenario?.Year ?? 0;

        Console.WriteLine($"    Using base scenario: {baseScenarioElement.Name} (Year: {baseYear})");
        Console.WriteLine($"    Using future scenario: {futureScenarioElement.Name} (Year: {futureYear})");
        Console.WriteLine($"    Discount rate: {props.DiscountRate}, Period of analysis: {props.PeriodOfAnalysis}");

        AlternativeResults results = Alternative.AnnualizationCompute(
            props.DiscountRate,
            props.PeriodOfAnalysis,
            element.ID,
            baseResults,
            futureResults,
            baseYear,
            futureYear);

        Console.WriteLine($"    Alternative computation complete.");

        return results;
    }
}
