using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.alternativeComparisonReport;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.TestingUtility.Services;

public static class AlternativeComparisonRunner
{
    public static AlternativeComparisonReportResults RunAlternativeComparison(string elementName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Alternative comparison report element name cannot be empty.", nameof(elementName));
        }

        Console.WriteLine($"    Running alternative comparison report '{elementName}'...");

        var element = ScenarioRunner.FindElement<AlternativeComparisonReportElement>(elementName);

        // Get study properties
        StudyPropertiesElement? props = BaseViewModel.StudyCache.GetStudyPropertiesElement();
        if (props == null)
        {
            throw new InvalidOperationException("Study properties not found.");
        }

        // Get the without-project alternative
        AlternativeElement? withoutProjAlt = GetAlternativeById(element.WithoutProjAltID);
        if (withoutProjAlt == null)
        {
            throw new InvalidOperationException($"Without-project alternative (ID={element.WithoutProjAltID}) not found.");
        }

        // Get all with-project alternatives
        List<AlternativeElement> withProjAlts = new();
        foreach (int altId in element.WithProjAltIDs)
        {
            var alt = GetAlternativeById(altId);
            if (alt == null)
            {
                throw new InvalidOperationException($"With-project alternative (ID={altId}) not found.");
            }
            withProjAlts.Add(alt);
        }

        if (withProjAlts.Count == 0)
        {
            throw new InvalidOperationException("No with-project alternatives found for comparison.");
        }

        Console.WriteLine($"    Without-project alternative: {withoutProjAlt.Name}");
        Console.WriteLine($"    With-project alternatives: {string.Join(", ", withProjAlts.Select(a => a.Name))}");

        // Compute without-project alternative results
        Console.WriteLine($"    Computing without-project alternative '{withoutProjAlt.Name}'...");
        AlternativeResults withoutProjResults = ComputeAlternativeResults(withoutProjAlt, props, cancellationToken);

        // Compute each with-project alternative results
        List<AlternativeResults> withProjResults = new();
        foreach (var withProjAlt in withProjAlts)
        {
            Console.WriteLine($"    Computing with-project alternative '{withProjAlt.Name}'...");
            var results = ComputeAlternativeResults(withProjAlt, props, cancellationToken);
            withProjResults.Add(results);
        }

        // Compute the comparison report
        Console.WriteLine($"    Computing alternative comparison report...");
        var comparisonResults = AlternativeComparisonReport.ComputeAlternativeComparisonReport(
            withoutProjResults,
            withProjResults);

        if (comparisonResults == null)
        {
            throw new InvalidOperationException("Alternative comparison report computation failed.");
        }

        Console.WriteLine($"    Alternative comparison report complete.");

        return comparisonResults;
    }

    private static AlternativeElement? GetAlternativeById(int id)
    {
        var alternatives = BaseViewModel.StudyCache.GetChildElementsOfType<AlternativeElement>();
        return alternatives.FirstOrDefault(a => a.ID == id);
    }

    private static AlternativeResults ComputeAlternativeResults(AlternativeElement element, StudyPropertiesElement props, CancellationToken cancellationToken)
    {
        FdaValidationResult validation = element.RunPreComputeValidation();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Alternative '{element.Name}' cannot compute: {validation.ErrorMessage}");
        }

        var baseScenarioElement = element.BaseScenario?.GetElement();
        var futureScenarioElement = element.FutureScenario?.GetElement();

        if (baseScenarioElement?.Results == null)
        {
            throw new InvalidOperationException($"Base scenario for alternative '{element.Name}' has no computed results.");
        }

        if (futureScenarioElement?.Results == null)
        {
            throw new InvalidOperationException($"Future scenario for alternative '{element.Name}' has no computed results.");
        }

        int baseYear = element.BaseScenario?.Year ?? 0;
        int futureYear = element.FutureScenario?.Year ?? 0;

        return Alternative.AnnualizationCompute(
            props.DiscountRate,
            props.PeriodOfAnalysis,
            element.ID,
            baseScenarioElement.Results,
            futureScenarioElement.Results,
            baseYear,
            futureYear);
    }
}
