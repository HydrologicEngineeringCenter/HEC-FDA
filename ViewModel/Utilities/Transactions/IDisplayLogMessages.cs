using FdaLogging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Utilities.Transactions
{
    public interface IDisplayLogMessages
    {
        void UpdateMessages(bool saving);
        LoggingLevel SaveStatusLevel { get; }
        bool IsExpanded { get; set; }
        ObservableCollection<LogItem> MessageRows { get; set; }
        int MessageCount { get; }
        List<LogItem> TempErrors { get; set; }
    }
}
