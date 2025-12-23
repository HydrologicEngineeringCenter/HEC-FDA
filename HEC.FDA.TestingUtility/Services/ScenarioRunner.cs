using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.TestingUtility.Services;

public static class ScenarioRunner
{
    public static ScenarioResults RunScenario(string elementName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Scenario element name cannot be empty.", nameof(elementName));
        }

        IASElement element = FindElement<IASElement>(elementName);

        Console.WriteLine($"    Running scenario '{elementName}'...");

        FdaValidationResult validation = element.CanCompute();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Scenario cannot compute: {validation.ErrorMessage}");
        }

        List<ImpactAreaScenarioSimulation> sims = ComputeScenarioVM.CreateSimulations(element.SpecificIASElements);
        if (sims.Count == 0)
        {
            throw new InvalidOperationException("No simulations could be created for this scenario.");
        }

        Scenario scenario = new(sims);

        var studyProps = BaseViewModel.StudyCache.GetStudyPropertiesElement();
        if (studyProps == null)
        {
            throw new InvalidOperationException("Study properties not found. Cannot retrieve convergence criteria.");
        }

        ConvergenceCriteria cc = studyProps.GetStudyConvergenceCriteria();
        Console.WriteLine($"    Computing with convergence criteria: min={cc.MinIterations}, max={cc.MaxIterations}");

        ScenarioResults results = scenario.Compute(cc, cancellationToken, computeIsDeterministic: false);
        Console.WriteLine($"    Scenario computation complete.");

        return results;
    }

    internal static T FindElement<T>(string elementName) where T : ChildElement
    {
        var elements = BaseViewModel.StudyCache.GetChildElementsOfType<T>();
        var match = elements.FirstOrDefault(e =>
            e.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            string availableNames = elements.Count > 0
                ? string.Join(", ", elements.Select(e => e.Name))
                : "(none)";
            throw new InvalidOperationException(
                $"Element '{elementName}' of type {typeof(T).Name} not found. Available: {availableNames}");
        }

        return match;
    }

    internal static T FindElementById<T>(int id) where T : ChildElement
    {
        var elements = BaseViewModel.StudyCache.GetChildElementsOfType<T>();
        var match = elements.FirstOrDefault(e => e.ID == id);

        if (match == null)
        {
            string availableIds = elements.Count > 0
                ? string.Join(", ", elements.Select(e => $"{e.Name}(ID={e.ID})"))
                : "(none)";
            throw new InvalidOperationException(
                $"Element of type {typeof(T).Name} with ID {id} not found. Available: {availableIds}");
        }

        return match;
    }
}
