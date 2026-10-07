using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Utility.Progress;

namespace HEC.FDA.ViewModel.Reporting;

/// <summary>Existing compute and save operations, separated from report scheduling.</summary>
public class StudyReportOperations
{
    public virtual Task<StageDamageResult> ComputeStageDamageAsync(AggregatedStageDamageElement element,
        ProgressReporter reporter, Action<string> log, CancellationToken token) =>
        Task.Run(() => StageDamageRunner.RunStageDamage(element, reporter, log, token), token);

    public virtual Task<ScenarioResults> ComputeScenarioAsync(IASElement element,
        ProgressReporter reporter, Action<string> log, CancellationToken token) =>
        Task.Run(() => ScenarioRunner.RunScenario(element, token, reporter, log), token);

    public virtual Task<AlternativeResults> ComputeAlternativeAsync(AlternativeElement element,
        StudyPropertiesElement properties, ProgressReporter reporter)
    {
        var validation = element.RunPreComputeValidation();
        if (!validation.IsValid)
            throw new InvalidOperationException($"Alternative '{element.Name}' cannot compute: {validation.ErrorMessage}");
        return AlternativeComputer.RunAnnualizationCompute(element, properties, reporter);
    }

    public virtual Task<AlternativeComparisonReportResults> ComputeComparisonAsync(AlternativeResults without,
        AlternativeResults[] with, ProgressReporter reporter, CancellationToken token) => Task.Run(() =>
            Model.alternativeComparisonReport.AlternativeComparisonReport.ComputeAlternativeComparisonReport(without, with, reporter), token);

    public virtual void SaveStageDamage(AggregatedStageDamageElement element, StageDamageResult result, Action<string> log)
    {
        var curves = result.StageDamageFunctions.Select(upd =>
        {
            var component = new CurveComponentVM(StringConstants.STAGE_DAMAGE, StringConstants.STAGE,
                StringConstants.DAMAGE, DistributionOptions.HISTOGRAM_ONLY);
            component.SetPairedData(upd);
            var impactArea = result.ImpactAreaElement.GetImpactAreaRow(upd.ImpactAreaID)
                ?? throw new InvalidOperationException($"Impact area {upd.ImpactAreaID} no longer exists.");
            return new StageDamageCurve(impactArea, upd.DamageCategory, component, upd.AssetCategory, StageDamageConstructionType.COMPUTED);
        }).ToList();
        var previousCurves = element.Curves.ToList();
        string previousDate = element.ComputeDate, previousVersion = element.SoftwareVersion, previousEdit = element.LastEditDate;
        try
        {
            element.Curves.Clear();
            element.Curves.AddRange(curves);
            element.ComputeDate = DateTime.Now.ToString("G");
            element.SoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            element.UpdateComputeDate = true;
            PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().SaveExisting(element);
            element.UpdateTreeViewHeader(element.Name);
            element.CustomTreeViewHeader.Decoration = "";
            element.UpdateTooltip();
        }
        catch
        {
            element.Curves.Clear();
            element.Curves.AddRange(previousCurves);
            element.ComputeDate = previousDate;
            element.SoftwareVersion = previousVersion;
            element.LastEditDate = previousEdit;
            throw;
        }
        finally { element.UpdateComputeDate = false; }
        log($"Saved stage damage '{element.Name}' to the study database.");
    }

    public virtual void SaveScenario(IASElement element, ScenarioResults results, Action<string> log)
    {
        var previous = element.Results;
        string previousEdit = element.LastEditDate;
        try
        {
            element.Results = results;
            element.UpdateComputeDate = true;
            PersistenceFactory.GetIASManager().SaveExisting(element);
            IASTooltipHelper.UpdateTooltip(element);
        }
        catch
        {
            element.Results = previous;
            element.LastEditDate = previousEdit;
            throw;
        }
        finally { element.UpdateComputeDate = false; }
        log($"Saved scenario '{element.Name}' to the study database.");
    }
}
