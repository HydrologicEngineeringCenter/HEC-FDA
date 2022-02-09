using System.Collections.ObjectModel;
using ViewModel.Utilities;

namespace ViewModel.Saving
{
    public interface IElementManager
    {
        
        void SaveNew(ChildElement element);
        void Remove(ChildElement element);
        void SaveExisting(ChildElement oldElement, ChildElement elementToSave);
        void Load();

        void Log(FdaLogging.LoggingLevel level, string message, string elementName);
        ObservableCollection<FdaLogging.LogItem> GetLogMessages(string elementName);
        ObservableCollection<FdaLogging.LogItem> GetLogMessagesByLevel(FdaLogging.LoggingLevel level, string elementName);





    }
}
