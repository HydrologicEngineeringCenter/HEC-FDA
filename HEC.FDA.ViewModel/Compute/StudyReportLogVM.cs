using System;
using System.Threading;
using System.Threading.Tasks;
using HEC.FDA.ViewModel.Reporting;
using Visual.Observables;

namespace HEC.FDA.ViewModel.Compute;

public class StudyReportLogVM : BaseViewModel
{
    private readonly CancellationTokenSource _cancellation = new();
    private bool _isRunning = true;
    private bool _cancelRequested;
    public BatchJob Job { get; }
    public bool IsRunning
    {
        get => _isRunning;
        private set { _isRunning = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(CanCancel)); NotifyPropertyChanged(nameof(CanClose)); }
    }
    public bool CanCancel => IsRunning && !_cancelRequested;
    public bool CanClose => !IsRunning;

    public StudyReportLogVM(BatchJob job) => Job = job;

    public void Cancel()
    {
        if (!CanCancel) return;
        _cancelRequested = true;
        _cancellation.Cancel();
        NotifyPropertyChanged(nameof(CanCancel));
        Job.Reporter.ReportMessage("Cancellation requested. Waiting for the active compute to stop; stage damage stops between impact areas. Its result will not be saved. Earlier saved results will remain.");
    }

    public async Task RunAsync(StudyReportService service, string studyName, string outputPath)
    {
        try
        {
            await service.GenerateAsync(studyName, outputPath, Job.Reporter, _cancellation.Token);
        }
        catch (OperationCanceledException) when (_cancellation.IsCancellationRequested)
        {
            Job.Reporter.ReportMessage("Report generation canceled. No new report was published. Previously saved compute results remain in the study database.");
        }
        catch (Exception ex)
        {
            Job.Reporter.ReportMessage($"Report generation failed: {ex.Message}");
            Job.Reporter.ReportMessage("No new report was published. Previously saved compute results remain in the study database.");
        }
        finally
        {
            IsRunning = false;
            _cancellation.Dispose();
        }
    }

    public override bool IsOkToClose() => !IsRunning;
}
