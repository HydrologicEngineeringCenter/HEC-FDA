using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using Utility.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.ViewModel.Reporting;

public static class ScenarioRunner
{
    public static ScenarioResults RunScenario(string elementName, CancellationToken cancellationToken)
        => RunScenario(FindElement<IASElement>(elementName), cancellationToken,
            ProgressReporter.ConsoleWrite(), Console.WriteLine);

    public static ScenarioResults RunScenario(IASElement element, CancellationToken cancellationToken,
        ProgressReporter reporter, Action<string> log)
    {
        string elementName = element.Name;
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Scenario element name cannot be empty.", nameof(elementName));
        }

        log($"    Running scenario '{elementName}'...");

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
        log($"    Computing with convergence criteria: min={cc.MinIterations}, max={cc.MaxIterations}");

        int completed = 0;
        void OnProgress(object sender, ProgressReportEventArgs e)
        {
            bool finished = e.Progress == ImpactAreaScenarioSimulation.IMPACT_AREA_SIM_COMPLETED;
            reporter.ReportProgress((int)(100 * (completed + (finished ? 1 : Math.Clamp(e.Progress, 0, 100) / 100f)) / sims.Count));
            if (finished) completed++;
        }
        void OnMessage(object sender, MessageEventArgs e) => log(e.Message.Message);
        foreach (var sim in sims)
        {
            sim.ProgressReport += OnProgress;
            sim.MessageReport += OnMessage;
        }
        try
        {
            ScenarioResults results = scenario.Compute(cc, cancellationToken, computeIsDeterministic: false);
            cancellationToken.ThrowIfCancellationRequested();
            if (results.ResultsList.Count != sims.Count || results.ResultsList.Any(r => r.IsNull))
                throw new InvalidOperationException($"Scenario '{elementName}' did not produce complete results.");
            reporter.ReportProgress(100);
            log($"    Scenario computation complete.");
            return results;
        }
        finally
        {
            foreach (var sim in sims)
            {
                sim.ProgressReport -= OnProgress;
                sim.MessageReport -= OnMessage;
                MessageHub.Unregister(sim);
            }
        }
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
