using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;

namespace FdaViewModel.Editors
{
    public class CurveEditorVM : BaseEditorVM, Utilities.ISaveUndoRedo, Utilities.Transactions.ITransactionsAndMessages
    {

        private Statistics.UncertainCurveDataCollection _Curve;
        private string _SavingText;

        #region properties
        public int UndoRowsSelectedIndex
        {
            set
            {
                if (value == -1)
                {
                    return;
                }
                OwnedElement prevElement = ActionManager.SaveUndoRedoHelper.SelectedIndexInUndoList(value, CurrentElement);
                AssignValuesFromElementToEditor(prevElement);
                UndoRowsSelectedIndex = -1;//this should clear the selection after the choice is made

            }
        }

        public int RedoRowsSelectedIndex
        {
            set
            {
                if (value == -1)
                {
                    return;
                }
                OwnedElement nextElement = ActionManager.SaveUndoRedoHelper.SelectedIndexInRedoList(value, CurrentElement);
                AssignValuesFromElementToEditor(nextElement);
                RedoRowsSelectedIndex = -1;//this should clear the selection after the choice is made

            }
        }


        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }

        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }

        public List<TransactionRowItem> TransactionRows
        {
            get;set;
        }

        public List<MessageRowItem> MessageRows
        {
            get;set;
        }
        public bool TransactionsMessagesVisible { get; set; }

        public string PlotTitle { get; set; }

        #endregion

        #region constructors
        public CurveEditorVM(Statistics.UncertainCurveDataCollection defaultCurve, EditorActionManager actionManager) :base(actionManager)
        {

            Curve = defaultCurve;
            PlotTitle = "Curve";
        }

        public CurveEditorVM(Utilities.OwnedElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            SavingText = " Last saved at: " + elem.LastEditDate;
            TransactionHelper.LoadTransactionsAndMessages(this, elem);
            PlotTitle = Name;
        }

        #endregion


        #region voids
        public override void Undo()
        {
            OwnedElement prevElement = ActionManager.SaveUndoRedoHelper.UndoElement(CurrentElement);
            if (prevElement != null)
            {
                AssignValuesFromElementToEditor(prevElement);
            }
        }

        public override void Redo()
        {
            OwnedElement nextElement = ActionManager.SaveUndoRedoHelper.RedoElement(CurrentElement);
            if(nextElement != null)
            {
                AssignValuesFromElementToEditor(nextElement);
            }
        }

        public override void SaveWhileEditing()
        {
            SavingText = " Saving...";
            OwnedElement elementToSave = ActionManager.CreateElementFromEditorAction(this);
            if(CurrentElement == null)
            {
                CurrentElement = elementToSave;
            }
            LastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = LastEditDate;
            CurrentElement.LastEditDate = LastEditDate;
            ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name,CurrentElement.Curve, elementToSave);
            //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
            // i need to update those properties here
            AssignValuesFromEditorToCurrentElement();

            //update the rules to exclude the new name from the banned list
            //OwnerValidationRules.Invoke(this, _CurrentElement.Name);  
            SavingText = " Saved at " + DateTime.Now.ToShortTimeString();
        }

        private void AssignValuesFromEditorToCurrentElement()
        {
            ActionManager.AssignValuesFromEditorToElementAction(this,CurrentElement);
        }

        public void AssignValuesFromElementToEditor(OwnedElement element)
        {
            ActionManager.AssignValuesFromElementToEditorAction(this, element);
        }
        public override void Save()
        {
            SaveWhileEditing();
        }

        

        #endregion 

    }
}
