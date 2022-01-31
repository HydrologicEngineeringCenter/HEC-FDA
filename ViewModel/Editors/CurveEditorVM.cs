using ViewModel.Utilities;
using System;
using Model;
using Functions;
using FdaLogging;
using HEC.Plotting.Core.ViewModel;
using HEC.Plotting.SciChart2D.ViewModel;
using paireddata;

namespace ViewModel.Editors
{
    public class CurveEditorVM : BaseLoggingEditorVM, ISaveUndoRedo
    {

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("CurveEditorVM");

        public ChartViewModel MixedViewModel { get; } = new SciChart2DChartViewModel("Test Title");

        private UncertainPairedData _Curve;
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


        public UncertainPairedData Curve
        {
            get { return _Curve; }
            set
            {
                _Curve = value;
                NotifyPropertyChanged();                
            }
        }

        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }      

        public string PlotTitle { get; set; }


        private IParameterEnum _ParameterType;
        #endregion

        #region constructors
        public CurveEditorVM(UncertainPairedData defaultCurve,string xLabel,string yLabel,string chartTitle, EditorActionManager actionManager) :base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            PlotTitle = "Curve";
            SetDimensions(800, 600, 400, 400);
        }

       

        public CurveEditorVM(ChildElement elem, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) :base(elem, xLabel, yLabel, chartTitle, actionManager)
        {
            if (elem.Curve != null)
            {
                //the curve is null for the conditions editor
            }
            PlotTitle = Name;
            SetDimensions(800, 600, 400, 400);         
        }

        #endregion


        #region voids       

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
                //EditorVM.UpdateChartViewModel();
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
                //EditorVM.UpdateChartViewModel();
            }
        }

        public virtual UncertainPairedData GetCoordinatesFunction()
        {
            return null;// EditorVM.CreateFunctionFromTables(); 
        }

        public virtual void SaveWhileEditing()
        {
            if(!HasChanges)
            {
                //todo: it looks like this never gets hit. It always has changes.
                String time = DateTime.Now.ToString();
                LogItem li =LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time );
                MessageRows.Insert(0, li);
                SaveStatusLevel = LoggingLevel.Debug;
                return;
            }

            try
            {
                //try to construct the new coordinates function
                UncertainPairedData coordFunc = GetCoordinatesFunction();
                EditorVM.Function = coordFunc;
                //IFunction function = coordFunc.Sample(.5);

                Curve = coordFunc;
            }
            catch(Exception ex)
            {
                //we were unsuccessful in creating the coordinates function                
                TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, ex.Message));
                UpdateMessages(true);
                return;
            }

            InTheProcessOfSaving = true;
            ChildElement elementToSave = ActionManager.SaveUndoRedoHelper.CreateElementFromEditorAction(this);
            if(CurrentElement == null)
            {
                CurrentElement = elementToSave;
            }

            LastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = LastEditDate;
            CurrentElement.LastEditDate = LastEditDate;
            elementToSave.Curve = Curve;

            ActionManager.SaveUndoRedoHelper.Save(CurrentElement.Name,CurrentElement, elementToSave);
            //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
            // i need to update those properties here
            AssignValuesFromEditorToCurrentElement();

            SavingText = CreateLastSavedText(elementToSave);

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

        #endregion

    }
}
