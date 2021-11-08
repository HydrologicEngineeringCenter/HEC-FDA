
namespace Base.Interfaces
{
    public interface IProgressReport: IReportMessage
    {
        event Events.ProgressReportedEventHandler ProgressReport;
        void ReportProgress(object sender, Events.ProgressReportEventArgs e);
    }
}
