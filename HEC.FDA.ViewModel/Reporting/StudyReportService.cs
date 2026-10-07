using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using Utility.Progress;

namespace HEC.FDA.ViewModel.Reporting;

/// <summary>Updates saved results only when necessary, then exports the open study.</summary>
public class StudyReportService
{
    private readonly IStudyCache _study;
    private readonly StudyReportOperations _operations;
    private readonly ReportDependencies _dependencies;
    private readonly HashSet<int> _updatedStageDamage = new();
    private ReportResultCache<int, AlternativeResults> _alternatives;

    public StudyReportService(IStudyCache study, StudyReportOperations operations = null)
    {
        _study = study;
        _operations = operations ?? new StudyReportOperations();
        _dependencies = new ReportDependencies(study);
    }

    // Called on the UI thread: computations run in workers, persistence resumes on the UI thread.
    public async Task GenerateAsync(string studyName, string outputPath, ProgressReporter reporter,
        CancellationToken cancellationToken)
    {
        Action<string> log = message => reporter.ReportMessage(message);
        _alternatives = new ReportResultCache<int, AlternativeResults>();
        _updatedStageDamage.Clear();
        var factory = new CsvReportFactory(log);
        var stageDamages = _study.GetChildElementsOfType<AggregatedStageDamageElement>().ToList();
        var scenarios = _study.GetChildElementsOfType<IASElement>().ToList();
        var alternatives = _study.GetChildElementsOfType<AlternativeElement>().ToList();
        var comparisons = _study.GetChildElementsOfType<AlternativeComparisonReportElement>().ToList();
        int total = stageDamages.Count + scenarios.Count + alternatives.Count + comparisons.Count + 1;
        int completed = 0;
        ProgressReporter Next(string name) => reporter.SubTask(name, (float)completed / total, 1f / total);
        void Complete(ProgressReporter task) { task.ReportProgress(100); completed++; }

        log($"Generating report for study '{studyName}'.");
        log("Stage damage and scenario results will be saved after each successful compute. Alternatives and comparisons are calculated on demand.");
        log("The report uses saved component definitions. Save editor changes before generating a report.");

        foreach (var element in stageDamages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var task = Next("Stage damage: " + element.Name);
            if (element.IsManual)
            {
                if (element.Curves.Count == 0)
                    throw new InvalidOperationException($"Manual stage damage '{element.Name}' has no curves. Supply curves before generating a report.");
                log($"Reusing stage damage '{element.Name}': manually supplied curves.");
            }
            else
            {
                string reason = StageDamageReason(element);
                if (reason == null) log($"Reusing stage damage '{element.Name}': saved results appear up to date.");
                else
                {
                    log($"Computing stage damage '{element.Name}': {reason}.");
                    await ReportExecution.ComputeAndSaveAsync(
                        () => _operations.ComputeStageDamageAsync(element, task, log, cancellationToken),
                        result => _operations.SaveStageDamage(element, result, log), cancellationToken);
                    _updatedStageDamage.Add(element.ID);
                }
            }
            await Task.Run(() => factory.AddStageDamageSummary(studyName, element.Name,
                StageDamageRunner.ConvertCurvesToUPD(element.Curves)), cancellationToken);
            Complete(task);
        }

        foreach (var element in scenarios)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var task = Next("Scenario: " + element.Name);
            string reason = ScenarioReason(element);
            if (reason == null) log($"Reusing scenario '{element.Name}': saved results appear up to date.");
            else
            {
                log($"Computing scenario '{element.Name}': {reason}.");
                await ReportExecution.ComputeAndSaveAsync(
                    () => _operations.ComputeScenarioAsync(element, task, log, cancellationToken),
                    result => _operations.SaveScenario(element, result, log), cancellationToken);
            }
            await Task.Run(() => factory.AddScenarioResults(studyName, element), cancellationToken);
            Complete(task);
        }

        foreach (var element in alternatives)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var task = Next("Alternative: " + element.Name);
            AlternativeResults result = await GetAlternativeAsync(element, task, log, cancellationToken);
            // Results are transient; use a report copy so existing result tabs keep their own state.
            var reportElement = new AlternativeElement(element.Name, element.Description, element.LastEditDate,
                element.BaseScenario, element.FutureScenario, element.ID) { Results = result };
            await Task.Run(() => factory.AddAlternativeResults(studyName, reportElement), cancellationToken);
            Complete(task);
        }

        foreach (var element in comparisons)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var task = Next("Comparison: " + element.Name);
            log($"Computing comparison '{element.Name}': comparison results are calculated on demand and are not saved to the study database.");
            var without = _dependencies.Find<AlternativeElement>(element.WithoutProjAltID);
            var with = element.WithProjAltIDs.Select(_dependencies.Find<AlternativeElement>).ToList();
            if (with.Count == 0) throw new InvalidOperationException($"Comparison '{element.Name}' has no with-project alternatives.");
            AlternativeResults withoutResults = await GetAlternativeAsync(without, task, log, cancellationToken);
            var withResults = new List<AlternativeResults>();
            foreach (var alternative in with)
                withResults.Add(await GetAlternativeAsync(alternative, task, log, cancellationToken));
            var results = await _operations.ComputeComparisonAsync(withoutResults, withResults.ToArray(), task, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (results == null) throw new InvalidOperationException($"Comparison '{element.Name}' produced no results.");
            await Task.Run(() => factory.AddAlternativeComparisonResults(studyName, element.Name, results,
                with.Select(a => (a.ID, a.Name)).ToList()), cancellationToken);
            Complete(task);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var writeTask = Next("Write report");
        log($"Writing report to '{outputPath}'.");
        await Task.Run(() => factory.SaveReportAtomically(outputPath, cancellationToken), cancellationToken);
        Complete(writeTask);
        log(factory.HasWarnings ? "Report completed with extraction warnings. Review the messages above."
            : "Study report completed successfully.");
    }

    public string StageDamageReason(AggregatedStageDamageElement element) =>
        element.IsManual ? null : ReportFreshness.GetComputeReason(element.Curves.Count > 0,
            element.LastEditDate, element.ComputeDate, ReportDependencies.IsDirty(element),
            _dependencies.StageDamageInputs(element).Select(State));

    public string ScenarioReason(IASElement element)
    {
        bool complete = element.Results != null && element.SpecificIASElements.Count > 0 &&
            element.SpecificIASElements.All(ias => element.Results.ResultsList.Any(r =>
                r.ImpactAreaID == ias.ImpactAreaID && !r.IsNull));
        return ReportFreshness.GetComputeReason(complete, element.LastEditDate,
            element.Results?.ComputeDate, ReportDependencies.IsDirty(element),
            _dependencies.ScenarioInputs(element).Distinct().Select(State));
    }

    private ReportInputState State(ChildElement element) => new(element.Name, element.LastEditDate,
        ReportDependencies.IsDirty(element), element is AggregatedStageDamageElement && _updatedStageDamage.Contains(element.ID));

    private async Task<AlternativeResults> GetAlternativeAsync(AlternativeElement element, ProgressReporter reporter,
        Action<string> log, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _alternatives.GetOrComputeAsync(element.ID, async () =>
        {
            log($"Computing alternative '{element.Name}': alternative results are calculated on demand and are not saved to the study database.");
            var props = _study.GetStudyPropertiesElement()
                ?? throw new InvalidOperationException("Study properties are required to compute alternatives.");
            var result = await _operations.ComputeAlternativeAsync(element, props, reporter);
            cancellationToken.ThrowIfCancellationRequested();
            if (result == null || result.IsNull) throw new InvalidOperationException($"Alternative '{element.Name}' produced no results.");
            return result;
        });
    }

}
