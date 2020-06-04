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
using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using FunctionsView.ViewModel;
using Utilities;
using FdaViewModel.Saving;
using FdaLogging;
using Model.Inputs.Functions.ImpactAreaFunctions;
using HEC.Plotting.Core.ViewModel;
using HEC.Plotting.SciChart2D.ViewModel;

namespace FdaViewModel.Editors
{
    public class CurveEditorVM : BaseLoggingEditorVM, ISaveUndoRedo
    {

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("CurveEditorVM");

        public ChartViewModel MixedViewModel { get; } = new SciChart2DChartViewModel("Test Title");

        private IFdaFunction _Curve;
        private string _SavingText;
        //private ObservableCollection<FdaLogging.LogItem> _MessageRows = new ObservableCollection<FdaLogging.LogItem>();
  

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
                SavingText = CreateLastSavedText(prevElement);
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
                SavingText = CreateLastSavedText(nextElement);

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

        //public ObservableCollection<TransactionRowItem> TransactionRows
        //{
        //    get;
        //    set;
        //}

        //public ObservableCollection<FdaLogging.LogItem> MessageRows
        //{
        //    get { return _MessageRows; }
        //    set { _MessageRows = value; NotifyPropertyChanged("MessageRows"); NotifyPropertyChanged("MessageCount"); }
        //}

        //public int MessageCount
        //{
        //    get { return _MessageRows.Count; }
        //}

        //public bool TransactionsMessagesVisible
        //{
        //    get;
        //    set;
        //}

        public string PlotTitle { get; set; }

    

        #endregion

        #region constructors
        public CurveEditorVM(IFdaFunction defaultCurve,string xLabel,string yLabel,string chartTitle, EditorActionManager actionManager) :base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            
           // _Curve = defaultCurve;
            PlotTitle = "Curve";
            //TransactionRows = new ObservableCollection<TransactionRowItem>();
        }

        

        public CurveEditorVM(Utilities.ChildElement elem, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) :base(elem, xLabel, yLabel, chartTitle, actionManager)
        {
            
            //TransactionHelper.LoadTransactionsAndMessages(this, elem);
            PlotTitle = Name;

            //add the temp errors from the curve


            //MessageRows = FdaLogging.RetrieveFromDB.GetMessageRowsForType(elem.GetType(), FdaLogging.LoggingLevel.Fatal);
            //Saving.PersistenceFactory.GetElementManager(elem).;
            //Storage.Connection.Instance.GetElementId()
            //MessageRows = elem.Logs;
            //EditorLogAdded += UpdateMessages;
        }

        #endregion


        #region voids
        
        //public override void AddErrorMessage(string error)
        //{

        //    FdaLogging.LogItem mri = new FdaLogging.LogItem(DateTime.Now, error, "", "Fatal", "", "");
        //    InsertMessage(mri);
        //}
        //public override void UpdateMessages()
        //{
        //    MessageRows = FdaLogging.RetrieveFromDB.GetMessageRows(FdaLogging.LoggingLevel.Fatal);
        //}

        //private void InsertMessage(FdaLogging.LogItem mri)
        //{
        //    ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
        //    tempList.Add(mri);
        //    foreach (FdaLogging.LogItem row in MessageRows)
        //    {
        //        tempList.Add(row);
        //    }
        //    MessageRows = tempList;
        //}
        //private void UpdateMessages(object sender, EventArgs e)
        //{
        //    FdaLogging.LogItem mri = (FdaLogging.LogItem)sender;
        //    ObservableCollection<FdaLogging.LogItem> tempList = new ObservableCollection<FdaLogging.LogItem>();
        //    foreach (FdaLogging.LogItem row in MessageRows)
        //    {
        //        tempList.Add(row);
        //    }
        //    tempList.Add(mri);
        //    MessageRows = tempList;
        //}

            /// <summary>
            /// I wanted this here so that the text could live in one place.
            /// That way if we want to change it, it should change all the places that use it.
            /// </summary>
            /// <param name="elem"></param>
            /// <returns></returns>
        private string CreateLastSavedText(ChildElement elem)
        {
            return "Last Saved: " + elem.LastEditDate;
        }

        public  void Undo()
        {
            ChildElement prevElement = ActionManager.SaveUndoRedoHelper.UndoElement(CurrentElement);
            if (prevElement != null)
            {
                AssignValuesFromElementToEditor(prevElement);
                SavingText = CreateLastSavedText(prevElement);
                ReloadMessages();
                EditorVM.UpdateChartViewModel();
            }
        }

        public  void Redo()
        {
            ChildElement nextElement = ActionManager.SaveUndoRedoHelper.RedoElement(CurrentElement);
            if(nextElement != null)
            {
                AssignValuesFromElementToEditor(nextElement);
                SavingText = CreateLastSavedText(nextElement);
                ReloadMessages();
                EditorVM.UpdateChartViewModel();
            }
        }

        public  void SaveWhileEditing()
        {
            if(!HasChanges)
            {
                //todo: it looks like this never gets hit. It always has changes.
                String time = DateTime.Now.ToString();
                
                LogItem li =LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time );
                MessageRows.Insert(0, li);
                SaveStatusLevel = LoggingLevel.Debug;
                //TempErrors = new List<LogItem>() { li };
                //UpdateMessages();
                return;
            }

            try
            {
                //try to construct the new coordinates function
                ICoordinatesFunction coordFunc = EditorVM.CreateFunctionFromTables();
                EditorVM.Function = coordFunc;
                Curve = ImpactAreaFunctionFactory.Factory(coordFunc, IFdaFunctionEnum.Rating);


            }
            catch(Exception ex)
            {
                //we were unsuccessful in creating the coordinates function                
                TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, ex.Message));
                UpdateMessages(true);
                return;
            }
            //update the messages that show up in the expander
            //if (!EditorVM.IsValid)
            //{
                //since we aren't saving then there will be nothing new in the database.
                //just need to do an update in case there are new messages from the coordinates function.
                //SaveStatusVisible = true;
                //UpdateMessages();
                //return;
            //}

            InTheProcessOfSaving = true;
            ChildElement elementToSave = ActionManager.SaveUndoRedoHelper.CreateElementFromEditorAction(this);
            if(CurrentElement == null)
            {
                CurrentElement = elementToSave;
            }
            //IElementManager manager = Saving.PersistenceFactory.GetElementManager(CurrentElement);
            //ICoordinatesFunction coordFunc = EditorVM.CreateFunctionFromTables();
           // IFdaFunction newFunction = ImpactAreaFunctionFactory.Factory(coordFunc, ImpactAreaFunctionEnum.Rating);
            //Curve = newFunction;
            //EditorVM.Function = newFunction.Function;
            //SavingText = " Saving...";
            LastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = LastEditDate;
            CurrentElement.LastEditDate = LastEditDate;
            elementToSave.Curve = Curve;
            ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name,CurrentElement, elementToSave);
            //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
            // i need to update those properties here
            AssignValuesFromEditorToCurrentElement();

            //update the rules to exclude the new name from the banned list
            //OwnerValidationRules.Invoke(this, _CurrentElement.Name);  
            SavingText = CreateLastSavedText(elementToSave);
            //refresh the log messages
            //TempErrors = GetTempLogsFromCoordinatesFunctionEditor();
            //MessageRows = manager.GetLogMessages(CurrentElement.Name);
            //UpdateMessages();

            ReloadMessages(true);
            HasChanges = false;
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

        //public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        //{   

        //    MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessagesByLevel(level, CurrentElement.Name);
        //}

        //public void DisplayAllMessages()
        //{
        //    MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
        //}

        #endregion

    }
}
