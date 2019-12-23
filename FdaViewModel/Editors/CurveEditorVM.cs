using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;
using FdaViewModel.StageTransforms;
using Model;

namespace FdaViewModel.Editors
{
    public class CurveEditorVM : BaseEditorVM, ISaveUndoRedo, ITransactionsAndMessages
    {

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("CurveEditorVM");


        private IFdaFunction _Curve;
        private string _SavingText;
        private ObservableCollection<FdaLogging.LogItem> _MessageRows = new ObservableCollection<FdaLogging.LogItem>();

  

        #region properties
        public int UndoRowsSelectedIndex
        {
            set
            {
                if (value == -1)
                {
                    return;
                }
                ChildElement prevElement = ActionManager.SaveUndoRedoHelper.SelectedIndexInUndoList(value, CurrentElement);
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
                ChildElement nextElement = ActionManager.SaveUndoRedoHelper.SelectedIndexInRedoList(value, CurrentElement);
                AssignValuesFromElementToEditor(nextElement);
                RedoRowsSelectedIndex = -1;//this should clear the selection after the choice is made

            }
        }


        public IFdaFunction Curve
        {
            get { return _Curve; }
            set
            {
                _Curve = value;
                NotifyPropertyChanged();
                //Saving.PersistenceFactory.GetElementManager(CurrentElement).Log(FdaLogging.LoggingLevel.Info, "CurveChanged", CurrentElement.Name);
                //MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
            }
        }

        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<TransactionRowItem> TransactionRows
        {
            get;
            set;
        }

        public ObservableCollection<FdaLogging.LogItem> MessageRows
        {
            get { return _MessageRows; }
            set { _MessageRows = value; NotifyPropertyChanged("MessageRows"); NotifyPropertyChanged("MessageCount"); }
        }

        public int MessageCount
        {
            get { return _MessageRows.Count; }
        }

        public bool TransactionsMessagesVisible
        {
            get;
            set;
        }

        public string PlotTitle { get; set; }

    

        #endregion

        #region constructors
        public CurveEditorVM(IFdaFunction defaultCurve, EditorActionManager actionManager) :base(actionManager)
        {
            _Curve = defaultCurve;
            PlotTitle = "Curve";
            TransactionRows = new ObservableCollection<TransactionRowItem>();
        }

        public CurveEditorVM(Utilities.ChildElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {

            TransactionHelper.LoadTransactionsAndMessages(this, elem);
            PlotTitle = Name;

            //MessageRows = FdaLogging.RetrieveFromDB.GetMessageRowsForType(elem.GetType(), FdaLogging.LoggingLevel.Fatal);
            //Saving.PersistenceFactory.GetElementManager(elem).;
            //Storage.Connection.Instance.GetElementId()
            MessageRows = Saving.PersistenceFactory.GetElementManager(elem).GetLogMessages(elem.Name);
            //MessageRows = elem.Logs;
            //EditorLogAdded += UpdateMessages;
        }

        #endregion


        #region voids

        public override void AddErrorMessage(string error)
        {

            FdaLogging.LogItem mri = new FdaLogging.LogItem(DateTime.Now, error, "", "Fatal", "", "");
            InsertMessage(mri);
        }
        public override void UpdateMessages()
        {
            MessageRows = FdaLogging.RetrieveFromDB.GetMessageRows(FdaLogging.LoggingLevel.Fatal);
        }

        private void InsertMessage(FdaLogging.LogItem mri)
        {
            ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
            tempList.Add(mri);
            foreach (FdaLogging.LogItem row in MessageRows)
            {
                tempList.Add(row);
            }
            MessageRows = tempList;
        }
        private void UpdateMessages(object sender, EventArgs e)
        {
            FdaLogging.LogItem mri = (FdaLogging.LogItem)sender;
            ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
            foreach (FdaLogging.LogItem row in MessageRows)
            {
                tempList.Add(row);
            }
            tempList.Add(mri);
            MessageRows = tempList;
        }

        public  void Undo()
        {
            ChildElement prevElement = ActionManager.SaveUndoRedoHelper.UndoElement(CurrentElement);
            if (prevElement != null)
            {
                AssignValuesFromElementToEditor(prevElement);
                SavingText = prevElement.Name + " last saved: " + prevElement.LastEditDate;
                TransactionRows.Insert(0, new TransactionRowItem(DateTime.Now.ToString("G"), "Previously saved values", "me"));
            }
        }

        public  void Redo()
        {
            ChildElement nextElement = ActionManager.SaveUndoRedoHelper.RedoElement(CurrentElement);
            if(nextElement != null)
            {
                AssignValuesFromElementToEditor(nextElement);
                SavingText = nextElement.Name + " last saved: " + nextElement.LastEditDate;

            }
        }

        public  void SaveWhileEditing()
        {
            //SavingText = " Saving...";
            ChildElement elementToSave = ActionManager.SaveUndoRedoHelper.CreateElementFromEditorAction(this);
            if(CurrentElement == null)
            {
                CurrentElement = elementToSave;
            }
            LastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = LastEditDate;
            CurrentElement.LastEditDate = LastEditDate;
            ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name,CurrentElement, elementToSave);
            //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
            // i need to update those properties here
            AssignValuesFromEditorToCurrentElement();

            //update the rules to exclude the new name from the banned list
            //OwnerValidationRules.Invoke(this, _CurrentElement.Name);  
            SavingText = elementToSave.Name + " last saved: " + elementToSave.LastEditDate;
            MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);

        }

        public override void Save()
        {
            SaveWhileEditing();
        }

        private void AssignValuesFromEditorToCurrentElement()
        {
            ActionManager.SaveUndoRedoHelper.AssignValuesFromEditorToElementAction(this,CurrentElement);
        }

        /// <summary>
        /// This is used with the undo redo stuff. The undo/redo returns an element, and then this is able to load
        /// the editor with its values
        /// </summary>
        /// <param name="element"></param>
        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            ActionManager.SaveUndoRedoHelper.AssignValuesFromElementToEditorAction(this, element);
        }

        public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        {   

            MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessagesByLevel(level, CurrentElement.Name);
        }

        public void DisplayAllMessages()
        {
            MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
        }

        #endregion

    }
}
