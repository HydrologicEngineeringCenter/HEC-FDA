using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Saving
{
    public interface IElementManager
    {
        
        void SaveNew(Utilities.ChildElement element);
        void Remove(Utilities.ChildElement element);
        void SaveExisting(Utilities.ChildElement oldElement, ChildElement elementToSave, int changeTableIndex);
        void Load();

        void Log(FdaLogging.LoggingLevel level, string message, string elementName);
        ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName);
        ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName);





    }
}
