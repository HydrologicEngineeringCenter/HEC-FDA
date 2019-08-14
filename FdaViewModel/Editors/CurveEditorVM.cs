using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;
using FdaModel.Messaging;

namespace FdaViewModel.Editors
{
    public class CurveEditorVM : BaseEditorVM, Utilities.ISaveUndoRedo, ITransactionsAndMessages, FdaModel.Messaging.IRecieveMessages
    {

        private Statistics.UncertainCurveDataCollection _Curve;
        private string _SavingText;
        private List<MessageRowItem> _MessageRows = new List<MessageRowItem>();

        public ErrorLevel FilterLevel => throw new NotImplementedException();
        public Type SenderTypeFilter => throw new NotImplementedException();
        public Type MessageTypeFilter => throw new NotImplementedException();

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


        public Statistics.UncertainCurveDataCollection Curve
        {
            get { return _Curve; }
            set
            {
                _Curve = value;
                ReportMessage(this, new FdaModel.Messaging.MessageEventArgs(new Message("Curve value changed")));
                NotifyPropertyChanged();
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

        public List<MessageRowItem> MessageRows
        {
            get { return _MessageRows; }
            set { _MessageRows = value; NotifyPropertyChanged(); }
        }
        public bool TransactionsMessagesVisible
        {
            get;
            set;
        }

        public string PlotTitle { get; set; }

    

        #endregion

        #region constructors
        public CurveEditorVM(Statistics.UncertainCurveDataCollection defaultCurve, EditorActionManager actionManager) :base(actionManager)
        {
            _Curve = defaultCurve;
            PlotTitle = "Curve";
            TransactionRows = new ObservableCollection<TransactionRowItem>();
        }

        public CurveEditorVM(Utilities.ChildElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            SavingText = elem.Name + " last saved: " + elem.LastEditDate;
            TransactionHelper.LoadTransactionsAndMessages(this, elem);
            ReportMessage(this, new MessageEventArgs(new Message("openning... testing pub sub")));
            PlotTitle = Name;
            MessageRows = NLogDataBaseHelper.GetMessageRows(NLog.LogLevel.Fatal);
        }

        #endregion


        #region voids
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

        public override void OnClosing(object sender, EventArgs e)
        {
            base.OnClosing(sender, e);
        }
        public override void Dispose()
        {
            base.Dispose();
        }

        public void RecieveMessage(object sender, MessageEventArgs e)
        {
            List<MessageRowItem> msgs = new List<MessageRowItem>();
            foreach(MessageRowItem row in MessageRows)
            {
                msgs.Add(row);
            }
            msgs.Add(new MessageRowItem("date", e.Message.ToString(), "cody"));
            MessageRows = msgs;
        }


        #endregion

    }
}
