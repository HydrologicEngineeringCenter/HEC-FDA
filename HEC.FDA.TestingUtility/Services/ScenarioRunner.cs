using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.TestingUtility.Services;

public class ScenarioRunner
{
    public ScenarioResults RunScenario(string elementName, CancellationToken cancellationToken)
    {
        // Find element by name
        IASElement element = FindElement<IASElement>(elementName);

        Console.WriteLine($"    Running scenario '{elementName}'...");

        // Validate
        FdaValidationResult validation = element.CanCompute();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Scenario cannot compute: {validation.ErrorMessage}");
        }

        // Create simulations (reuse existing static method)
        List<ImpactAreaScenarioSimulation> sims = ComputeScenarioVM.CreateSimulations(element.SpecificIASElements);

        if (sims.Count == 0)
        {
            throw new InvalidOperationException("No simulations could be created for this scenario.");
        }

        // Build scenario
        Scenario scenario = new(sims);

        // Get convergence criteria
        ConvergenceCriteria cc = BaseViewModel.StudyCache.GetStudyPropertiesElement().GetStudyConvergenceCriteria();

        Console.WriteLine($"    Computing with convergence criteria: min={cc.MinIterations}, max={cc.MaxIterations}");

        // Compute
        ScenarioResults results = scenario.Compute(cc, cancellationToken, computeIsDeterministic: false);

        Console.WriteLine($"    Scenario computation complete.");

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
