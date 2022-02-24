using HEC.FDA.ViewModel.Utilities;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Saving
{
    public interface IPersistableWithUndoRedo:IElementManager
    {


        ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(ChildElement element);
        ChildElement Undo(ChildElement element, int changeTableIndex);
        ChildElement Redo(ChildElement element, int changeTableIndex);
    }
}
