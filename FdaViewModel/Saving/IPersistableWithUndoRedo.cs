using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public interface IPersistableWithUndoRedo:IElementManager
    {


        ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(ChildElement element);
        ChildElement Undo(ChildElement element, int changeTableIndex);
        ChildElement Redo(ChildElement element, int changeTableIndex);
    }
}
