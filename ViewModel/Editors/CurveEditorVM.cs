using FdaLogging;
using HEC.Plotting.Core.ViewModel;
using HEC.Plotting.SciChart2D.ViewModel;
using paireddata;
using System;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Editors
{
    public class CurveEditorVM : BaseLoggingEditorVM, ISaveWhileEditing
    {

        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("CurveEditorVM");

        public ChartViewModel MixedViewModel { get; } = new SciChart2DChartViewModel("Test Title");

        private UncertainPairedData _Curve;
        private string _SavingText;
        private TableWithPlotVM _TableWithPlot;

        #region properties

        public TableWithPlotVM TableWithPlot
        {
            get { return _TableWithPlot; }
            set { _TableWithPlot = value; NotifyPropertyChanged(); }
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

           

        public string PlotTitle { get; set; }


        #endregion

        #region constructors
        public CurveEditorVM(UncertainPairedData defaultCurve, string xLabel,string yLabel,string chartTitle, EditorActionManager actionManager) :base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            PlotTitle = "Curve";
            SetDimensions(800, 600, 400, 400);
            ComputeComponentVM vm = new ComputeComponentVM( "testName", "testXLabel", "testYLabel");
            TableWithPlot = new TableWithPlotVM(vm);
        }
 

        public CurveEditorVM(ChildElement elem, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) :base(elem, xLabel, yLabel, chartTitle, actionManager)
        {
            PlotTitle = Name;
            SetDimensions(800, 600, 400, 400);
            ComputeComponentVM vm = new ComputeComponentVM( "testName", "testXLabel", "testYLabel");
            TableWithPlot = new TableWithPlotVM(vm);
        }

        #endregion


        #region voids       
   

        public virtual UncertainPairedData GetCoordinatesFunction()
        {
            return TableWithPlot.GetUncertainPairedData();
            //todo: this will be the curve from the table.
            //return UncertainPairedDataFactory.CreateDefaultDeterminateData("", "", ""); 
        }

        public virtual void SaveWhileEditing()
        {
            XElement tableXML = TableWithPlot.ToXML();


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
                UncertainPairedData coordFunc = GetCoordinatesFunction();
                EditorVM.Function = coordFunc;
                Curve = coordFunc;
            }
            catch(Exception ex)
            {
                //we were unsuccessful in creating the function                
                TempErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, ex.Message));
                UpdateMessages(true);
                return;
            }

            InTheProcessOfSaving = true;
            ChildElement elementToSave = ActionManager.SaveHelper.CreateElementFromEditorAction(this);
            if(CurrentElement == null)
            {
                CurrentElement = elementToSave;
            }

            LastEditDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = LastEditDate;
            CurrentElement.LastEditDate = LastEditDate;
            elementToSave.Curve = Curve;

            ActionManager.SaveHelper.Save(CurrentElement.Name,CurrentElement, elementToSave);
            //saving puts all the right values in the db but does not update the owned element in the tree. (in memory values)
            // i need to update those properties here
            AssignValuesFromEditorToCurrentElement();

            UpdateSave(elementToSave);
        }
    
        public override void Save()
        {
            SaveWhileEditing();
        }

        private void AssignValuesFromEditorToCurrentElement()
        {
            ActionManager.SaveHelper.AssignValuesFromEditorToElementAction(this,CurrentElement);
        }

        /// <summary>
        /// This is used with the undo redo stuff. The undo/redo returns an element, and then this is able to load
        /// the editor with its values
        /// </summary>
        /// <param name="element"></param>
        public void AssignValuesFromElementToEditor(ChildElement element)
        {
            ActionManager.SaveHelper.AssignValuesFromElementToEditorAction(this, element);
        }

        #endregion

    }
}
