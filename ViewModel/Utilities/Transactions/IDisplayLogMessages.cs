using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Utilities.Transactions
{
    public interface IDisplayLogMessages
    {
        void UpdateMessages(bool saving);
        void FilterRowsByLevel(LoggingLevel level);
        void DisplayAllMessages();
        LoggingLevel SaveStatusLevel { get; }
        bool IsExpanded { get; set; }
        ObservableCollection<LogItem> MessageRows { get; set; }

        int MessageCount { get; }
        //bool TransactionsMessagesVisible { get; set; }
        List<LogItem> TempErrors { get; set; }


    }
}
