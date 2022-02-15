using FdaLogging;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Saving
{
    public interface IElementManager
    {    
        void SaveNew(ChildElement element);
        void Remove(ChildElement element);
        void SaveExisting(ChildElement oldElement, ChildElement elementToSave);
        void Load();

        void Log(LoggingLevel level, string message, string elementName);
        ObservableCollection<LogItem> GetLogMessages(string elementName);
        ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName);

    }
}
