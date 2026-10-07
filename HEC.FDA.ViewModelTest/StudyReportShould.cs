using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Compute;
using Utility;
using Visual.Observables;
using HEC.FDA.ViewModel.Reporting;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics.Distributions;
using Statistics.Histograms;
using Utility.Progress;
using Xunit;

namespace HEC.FDA.ViewModelTest;

// FDA's cache is process-global; do not run these fixtures alongside other cache users.
[CollectionDefinition("Study report", DisableParallelization = true)]
public class StudyReportCollection { }

[Collection("Study report")]
public class StudyReportShould : IDisposable
{
    private const string Edited = "2026-01-01T10:00:00";
    private const string Computed = "2026-01-02T10:00:00";
    private readonly IStudyCache _previous = BaseViewModel.StudyCache;
    private readonly FDACache _study = new();
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "fda-report-tests-" + Guid.NewGuid().ToString("N"));
    private string Output => Path.Combine(_folder, "report.csv");

    public StudyReportShould()
    {
        Directory.CreateDirectory(_folder);
        BaseViewModel.StudyCache = _study;
        _study.AddElement(new ImpactAreaElement("Impact areas", "", [new ImpactAreaRowItem(1, "Downtown")], 1, "Name") { LastEditDate = Edited });
        _study.AddElement(new StudyPropertiesElement("Study", _folder, "", new ConvergenceCriteriaVM(), 1) { LastEditDate = Edited });
    }

    public void Dispose()
    {
        BaseViewModel.StudyCache = _previous;
        Directory.Delete(_folder, true);
    }

    private IASElement Scenario(int id, bool hasResults = true, int stageDamageId = -1)
    {
        var ias = new SpecificIAS(1, -1, -1, -1, -1, -1, [], false, 0,
            new ConsequencesConfig());
        var element = new IASElement("Scenario " + id, "", Edited, "2026", stageDamageId, -1, -1, -1, false, [ias], id);
        if (hasResults) element.Results = Results();
        _study.AddElement(element);
        return element;
    }

    private static ScenarioResults Results()
    {
        var results = new HEC.FDA.Model.scenarios.Scenario([]).Compute(new Statistics.ConvergenceCriteria(1, 1));
        results.ComputeDate = Computed;
        results.AddResults(new ImpactAreaScenarioResults(1));
        return results;
    }

    private static StageDamageCurve Curve()
    {
        var component = new CurveComponentVM("Stage damage");
        component.SetPairedData(new UncertainPairedData([1, 2], [new Deterministic(10), new Deterministic(20)],
            new CurveMetaData("Stage", "Damage", "Stage damage", "Residential", 1, "Structure")));
        return new StageDamageCurve(new ImpactAreaRowItem(1, "Downtown"), "Residential", component, "Structure", StageDamageConstructionType.COMPUTED);
    }

    private Task Generate(CountingOperations operations, CancellationToken token = default) =>
        new StudyReportService(_study, operations).GenerateAsync("Study", Output, ProgressReporter.None(), token);

    private AggregatedStageDamageElement ComputedStageDamage()
    {
        var mappings = new StructureSelectionMapping(true, false, "ID", "OT", "FFE", "Value", "", "", "", "", "", "", "", "", "", "");
        _study.AddElement(new InventoryElement("Inventory", "", mappings, new(), false, 1) { LastEditDate = Edited });
        _study.AddElement(new HydraulicElement("Hydraulics", "", [], HydraulicDataSource.WSEGrid, 1) { LastEditDate = Edited });
        var stageDamage = new AggregatedStageDamageElement("Computed stage damage", Edited, "", 2026, 1, 1,
            [Curve()], [], false, false, 1, Computed);
        _study.AddElement(stageDamage);
        return stageDamage;
    }

    [Fact]
    public async Task ReuseEveryCurrentScenarioWithoutComputing()
    {
        for (int id = 1; id <= 100; id++) Scenario(id);
        ComputedStageDamage();
        var operations = new CountingOperations();
        await Generate(operations);
        Assert.Equal(0, operations.ScenarioComputes);
        Assert.Equal(0, operations.ScenarioSaves);
        Assert.Equal(0, operations.StageDamageComputes);
        Assert.True(File.Exists(Output));
    }

    [Fact]
    public async Task ComputeMissingAndDirtyScenariosAndReuseThemOnNextExport()
    {
        Scenario(1, false);
        var dirty = Scenario(2);
        dirty.UpdateTreeViewHeader(dirty.Name + "*");
        Scenario(3);
        var operations = new CountingOperations();
        await Generate(operations);
        Assert.Equal(new[] { 1, 2 }, operations.SavedScenarioIds);
        await Generate(operations);
        Assert.Equal(2, operations.ScenarioComputes);
        Assert.Equal(2, operations.ScenarioSaves);
    }

    [Fact]
    public async Task RecomputeOnlyScenariosReferencingChangedManualCurves()
    {
        var manual = new AggregatedStageDamageElement("Manual", "2026-01-03T10:00:00", "", 2026, -1, -1,
            [Curve()], [], true, false, 1);
        _study.AddElement(manual);
        Scenario(1, stageDamageId: 1);
        Scenario(2);
        var operations = new CountingOperations();
        await Generate(operations);
        Assert.Equal(new[] { 1 }, operations.SavedScenarioIds);
        Assert.Equal(0, operations.StageDamageComputes);
    }

    [Fact]
    public async Task ChangedInventoryRecomputesStageDamageAndItsScenariosOnlyOnce()
    {
        ComputedStageDamage();
        _study.GetChildElementsOfType<InventoryElement>().Single().LastEditDate = "2026-01-03T10:00:00";
        Scenario(1, stageDamageId: 1);
        Scenario(2);
        var operations = new CountingOperations { AllowStageDamage = true };
        await Generate(operations);
        Assert.Equal(1, operations.StageDamageComputes);
        Assert.Equal(new[] { 1 }, operations.SavedScenarioIds);
        await Generate(operations);
        Assert.Equal(1, operations.StageDamageComputes);
        Assert.Equal(1, operations.ScenarioComputes);
    }

    [Fact]
    public async Task MissingStageDamageResultsComputeBeforeDependentScenario()
    {
        var stageDamage = ComputedStageDamage();
        stageDamage.Curves.Clear();
        Scenario(1, stageDamageId: stageDamage.ID);
        var operations = new CountingOperations { AllowStageDamage = true };
        await Generate(operations);
        Assert.Equal(1, operations.StageDamageComputes);
        Assert.Equal(new[] { 1 }, operations.SavedScenarioIds);
    }

    [Fact]
    public async Task FailureLogStaysOpenAndDoesNotClaimCompletion()
    {
        Scenario(1, false);
        var job = new BatchJob(uiThreadSyncContext: new Utility.SynchronizationContext(action => action()));
        var vm = new StudyReportLogVM(job);
        Assert.False(vm.IsOkToClose());
        await vm.RunAsync(new StudyReportService(_study, new CountingOperations { FailScenarioId = 1 }), "Study", Output);
        Assert.True(vm.IsOkToClose());
        Assert.False(vm.CanCancel);
        Assert.True(job.Progress < 100);
        Assert.Contains(job.Messages, m => m.EvaluatedMessage.Contains("Report generation failed: Compute failed."));
        Assert.False(File.Exists(Output));
    }

    [Fact]
    public async Task LogReasonsForOnDemandComputesAndReachFullProgress()
    {
        Scenario(1);
        _study.AddElement(new AlternativeElement("Without", "", Edited, new AlternativeScenario(1, 2026), null, 1));
        _study.AddElement(new AlternativeElement("With", "", Edited, new AlternativeScenario(1, 2026), null, 2));
        _study.AddElement(new AlternativeComparisonReportElement("Comparison", "", Edited, 1, [2], 1));
        var job = new BatchJob(uiThreadSyncContext: new Utility.SynchronizationContext(action => action()));
        var vm = new StudyReportLogVM(job);
        await vm.RunAsync(new StudyReportService(_study, new CountingOperations()), "Study", Output);
        Assert.False(vm.IsRunning);
        Assert.True(vm.CanClose);
        Assert.Equal(100, job.Progress);
        var messages = job.Messages.Select(m => m.EvaluatedMessage).ToList();
        Assert.Contains(messages, m => m.Contains("Computing alternative 'Without': alternative results are calculated on demand"));
        Assert.Contains(messages, m => m.Contains("Computing comparison 'Comparison': comparison results are calculated on demand"));
        Assert.Contains(messages, m => m.Contains("Reusing scenario 'Scenario 1': saved results appear up to date"));
    }

    [Fact]
    public void SaveResultsAndComputeDatesToRealStudyDatabase()
    {
        string previousPath = Connection.Instance.ProjectFile;
        var previousSavingCache = PersistenceFactory.StudyCacheForSaving;
        try
        {
            Connection.Instance.ProjectFile = Path.Combine(_folder, "study.sqlite");
            PersistenceFactory.StudyCacheForSaving = _study;
            var stageDamage = new AggregatedStageDamageElement("Computed", Edited, "", 2026, -1, -1,
                [], [], false, false, 1);
            PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().SaveNew(stageDamage);
            var scenario = Scenario(1, false);
            _study.RemoveElement(scenario);
            PersistenceFactory.GetIASManager().SaveNew(scenario);
            var operations = new StudyReportOperations();
            var impactArea = _study.GetChildElementsOfType<ImpactAreaElement>().Single();
            var low = new DynamicHistogram(1, new Statistics.ConvergenceCriteria());
            var high = new DynamicHistogram(1, new Statistics.ConvergenceCriteria());
            low.AddObservationsToHistogram([10, 11, 12]);
            high.AddObservationsToHistogram([20, 21, 22]);
            var computedCurve = new UncertainPairedData([1, 2], [low, high],
                new CurveMetaData("Stage", "Damage", "Computed", "Residential", 1, "Structure"));
            operations.SaveStageDamage(stageDamage, new StageDamageResult([computedCurve], [], null, impactArea), _ => { });
            operations.SaveScenario(scenario, Results(), _ => { });
            // Query SQLite directly: SQLiteManager's DataTables are cached snapshots.
            using var stageCommand = new SQLiteCommand("SELECT XML FROM stage_damage_relationships", Connection.Instance.Reader.DbConnection);
            using var scenarioCommand = new SQLiteCommand("SELECT XML FROM impact_area_scenarios", Connection.Instance.Reader.DbConnection);
            string stageXml = (string)stageCommand.ExecuteScalar();
            string scenarioXml = (string)scenarioCommand.ExecuteScalar();
            var loadedStage = new AggregatedStageDamageElement(XElement.Parse(stageXml), stageDamage.ID);
            var loadedScenario = new IASElement(XElement.Parse(scenarioXml), scenario.ID);
            Assert.Single(loadedStage.Curves);
            Assert.NotNull(loadedScenario.Results);
            Assert.Equal(loadedStage.LastEditDate, loadedStage.ComputeDate);
            Assert.Equal(loadedScenario.LastEditDate, loadedScenario.Results.ComputeDate);
            Assert.False(stageDamage.UpdateComputeDate);
            Assert.False(scenario.UpdateComputeDate);
        }
        finally
        {
            Connection.Instance.Close();
            SQLiteConnection.ClearAllPools();
            // Existing persistence helpers leave commands to finalization; release their native handles.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (previousPath != null) Connection.Instance.ProjectFile = previousPath;
            PersistenceFactory.StudyCacheForSaving = previousSavingCache;
        }
    }

    [Fact]
    public void StageDamageComputeHonorsCancellationBeforePublishingAnyResults()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var compute = new HEC.FDA.Model.stageDamage.ScenarioStageDamage([]);
        Assert.ThrowsAny<OperationCanceledException>(() => compute.Compute(cancellationToken: cancellation.Token));
    }

    [Fact]
    public async Task KeepEarlierSavedResultsWhenLaterComputeFailsAndLeaveExistingReportIntact()
    {
        var first = Scenario(1, false);
        var second = Scenario(2, false);
        File.WriteAllText(Output, "Existing report");
        var operations = new CountingOperations { FailScenarioId = 2 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Generate(operations));
        Assert.NotNull(first.Results);
        Assert.Null(second.Results);
        Assert.Equal(new[] { 1 }, operations.SavedScenarioIds);
        Assert.Equal("Existing report", File.ReadAllText(Output));
    }

    [Fact]
    public async Task CancelDiscardsActiveResultButKeepsEarlierSavesAndReport()
    {
        var first = Scenario(1, false);
        var second = Scenario(2, false);
        File.WriteAllText(Output, "Existing report");
        using var cancellation = new CancellationTokenSource();
        var operations = new CountingOperations { CancelScenarioId = 2, Cancellation = cancellation };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Generate(operations, cancellation.Token));
        Assert.NotNull(first.Results);
        Assert.Null(second.Results);
        Assert.Equal(1, operations.ScenarioSaves);
        Assert.Equal("Existing report", File.ReadAllText(Output));
    }

    [Fact]
    public async Task ComputeEachAlternativeOnlyOnceForMultipleComparisonsAndDoNotPersistIt()
    {
        Scenario(1);
        var without = new AlternativeElement("Without", "", Edited, new AlternativeScenario(1, 2026), null, 1);
        var with = new AlternativeElement("With", "", Edited, new AlternativeScenario(1, 2026), null, 2);
        _study.AddElement(without);
        _study.AddElement(with);
        for (int id = 1; id <= 5; id++)
            _study.AddElement(new AlternativeComparisonReportElement("Comparison " + id, "", Edited, 1, [2], id));
        var operations = new CountingOperations();
        await Generate(operations);
        Assert.Equal(2, operations.AlternativeComputes);
        Assert.Equal(5, operations.ComparisonComputes);
        Assert.Null(without.Results);
        Assert.Null(with.Results);
        Assert.Equal(0, operations.ScenarioComputes);
    }

    [Theory]
    [InlineData(false, Edited, Computed, false, "no complete saved results")]
    [InlineData(true, Edited, Computed, true, "the component node is dirty")]
    [InlineData(true, "2026-01-03", Computed, false, "the component was edited since its last compute")]
    [InlineData(true, Edited, null, false, "the last compute date is unavailable")]
    [InlineData(true, null, Computed, false, "the last edit date is unavailable")]
    public void ExplainWhyResultsNeedComputing(bool hasResults, string edited, string computed, bool dirty, string reason)
    {
        Assert.Equal(reason, ReportFreshness.GetComputeReason(hasResults, edited, computed, dirty, []));
    }

    [Fact]
    public void DetectUpdatedDependencyEvenWhenTimestampMatchesWithinSameSecond()
    {
        Assert.Contains("computed during this report run", ReportFreshness.GetComputeReason(true, Edited, Computed,
            false, [new ReportInputState("Stage damage", Computed, WasComputed: true)]));
        Assert.Null(ReportFreshness.GetComputeReason(true, Computed, Computed,
            false, [new ReportInputState("Stage damage", Computed)]));
    }

    [Fact]
    public void RouteCsvWarningsToProvidedLogger()
    {
        var messages = new List<string>();
        var factory = new CsvReportFactory(messages.Add);
        factory.AddStageDamageSummary("Study", "Invalid curves", [null]);
        Assert.True(factory.HasWarnings);
        Assert.Contains(messages, m => m.Contains("Warning:"));
    }

    [Fact]
    public void WriteStageDamageRowsWithMatchingColumnsAndInvariantNumbers()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var factory = new CsvReportFactory(_ => { });
            var upd = StageDamageRunner.ConvertCurvesToUPD([Curve()]).Single();
            factory.AddStageDamageSummary("Study", "Stage damage", [upd]);
            factory.SaveReportAtomically(Output, CancellationToken.None);
            var lines = File.ReadAllLines(Output);
            int header = Array.FindIndex(lines, l => l.StartsWith("Study ID,Element Name"));
            Assert.Equal(lines[header].Split(',').Length, lines[header + 1].Split(',').Length);
            Assert.Contains(",Downtown,", lines[header + 1]);
            Assert.Contains("1.00", lines[header + 1]);
        }
        finally { CultureInfo.CurrentCulture = previousCulture; }
    }

    [Fact]
    public void CanceledWritePreservesExistingReportAndLeavesNoTemporaryFile()
    {
        File.WriteAllText(Output, "Existing report");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var factory = new CsvReportFactory(_ => { });
        Assert.ThrowsAny<OperationCanceledException>(() => factory.SaveReportAtomically(Output, cancellation.Token));
        Assert.Equal("Existing report", File.ReadAllText(Output));
        Assert.Empty(Directory.GetFiles(_folder, "*.tmp"));
    }

    private class CountingOperations : StudyReportOperations
    {
        public int ScenarioComputes, ScenarioSaves, StageDamageComputes, AlternativeComputes, ComparisonComputes;
        public int FailScenarioId = -1, CancelScenarioId = -1;
        public CancellationTokenSource Cancellation;
        public bool AllowStageDamage;
        public List<int> SavedScenarioIds = new();

        public override Task<StageDamageResult> ComputeStageDamageAsync(AggregatedStageDamageElement element,
            ProgressReporter reporter, Action<string> log, CancellationToken token)
        {
            StageDamageComputes++;
            if (AllowStageDamage)
                return Task.FromResult(new StageDamageResult(StageDamageRunner.ConvertCurvesToUPD([Curve()]), [], null,
                    BaseViewModel.StudyCache.GetChildElementsOfType<ImpactAreaElement>().Single()));
            throw new InvalidOperationException("Unexpected stage damage compute.");
        }
        public override void SaveStageDamage(AggregatedStageDamageElement element, StageDamageResult result, Action<string> log)
        {
            element.Curves.Clear();
            element.Curves.Add(Curve());
            element.ComputeDate = DateTime.Now.ToString("G");
            element.LastEditDate = element.ComputeDate;
            element.UpdateTreeViewHeader(element.Name);
        }
        public override Task<ScenarioResults> ComputeScenarioAsync(IASElement element,
            ProgressReporter reporter, Action<string> log, CancellationToken token)
        {
            ScenarioComputes++;
            if (element.ID == FailScenarioId) throw new InvalidOperationException("Compute failed.");
            if (element.ID == CancelScenarioId) Cancellation.Cancel();
            return Task.FromResult(Results());
        }
        public override void SaveScenario(IASElement element, ScenarioResults results, Action<string> log)
        {
            ScenarioSaves++;
            SavedScenarioIds.Add(element.ID);
            element.Results = results;
            element.Results.ComputeDate = DateTime.Now.ToString("G");
            element.LastEditDate = element.Results.ComputeDate;
            element.UpdateTreeViewHeader(element.Name);
        }
        public override Task<AlternativeResults> ComputeAlternativeAsync(AlternativeElement element,
            StudyPropertiesElement properties, ProgressReporter reporter)
        {
            AlternativeComputes++;
            return new HEC.FDA.ViewModel.Reporting.StudyReportOperations()
                .ComputeAlternativeAsync(element, properties, reporter);
        }
        public override Task<AlternativeComparisonReportResults> ComputeComparisonAsync(AlternativeResults without,
            AlternativeResults[] with, ProgressReporter reporter, CancellationToken token)
        {
            ComparisonComputes++;
            return base.ComputeComparisonAsync(without, with, reporter, token);
        }
    }
}
