using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public interface ISaveUndoRedo
    {
        //OwnedElement CurrentElement { get; set; }

          void Redo();
          void Undo();
        void SaveWhileEditing();
        //void AssignValuesFromElementToEditor(object sender, EventArgs e);
        //bool UndoEnabled { get; set; }
        //bool RedoEnabled { get; set; }
        //void LoadInitialStateForUndoRedo();
         //ObservableCollection<Utilities.UndoRedoRowItem> UndoRows { get; set; }
       // ObservableCollection<Utilities.UndoRedoRowItem> RedoRows { get; set; }

        //int SelectedIndexInUndoList { get; set; }
        //int SelectedIndexInRedoList { get; set; }

        //void UpdateTheUndoRedoRowItems();
        //void AssignValuesFromElementToEditor(OwnedElement element);
        //void UpdateUndoRedoButtons();

        // OwnedElement CreateNewElement();
        //Statistics.UncertainCurveDataCollection GetTheElementsCurve();
        //Statistics.UncertainCurveDataCollection GetTheEditorsCurve();
        //void AssignValuesFromEditorToCurrentElement();

        //string Name { get; set; }
        /// <summary>
        /// This is because if the user changes the name through the rename process, the name in the iditor does not get updated.
        /// This method will update the name in the iditor so that it shows the new name the user has entered.
        /// </summary>
        /// <param name="name"></param>
        // void UpdateNameWithNewValue(string name);
        //Action<Utilities.ISaveUndoRedo> SaveAction { get; set; }

    }
}
