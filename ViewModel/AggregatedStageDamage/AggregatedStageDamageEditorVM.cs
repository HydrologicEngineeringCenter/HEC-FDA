using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FdaLogging;
using ViewModel.Editors;
using ViewModel.Utilities;
using Functions;
using FunctionsView.ViewModel;
using HEC.Plotting.Core.ViewModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;


namespace ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM : BaseLoggingEditorVM, ISaveUndoRedo
    {
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("CurveEditorVM");

        private string _SavingText;
        private bool _IsManualRadioSelected = true;
        #region properties
        public bool IsManualRadioSelected
        {
            get { return _IsManualRadioSelected; }
            set { _IsManualRadioSelected = value; }
        }

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
                //this should clear the selection after the choice is made
                UndoRowsSelectedIndex = -1;
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
                //this should clear the selection after the choice is made
                RedoRowsSelectedIndex = -1;
            }
        }

        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }

        public string PlotTitle { get; set; }


        public ManualStageDamageVM ManualVM { get; set; }

        public CalculatedStageDamageVM CalculatedVM { get; set; }

        private IParameterEnum _ParameterType;
        #endregion

        #region constructors
        public AggregatedStageDamageEditorVM(IFdaFunction defaultCurve, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) : base(defaultCurve, xLabel, yLabel, chartTitle, actionManager)
        {
            HasChanges = true;
            _ParameterType = defaultCurve.ParameterType;
            PlotTitle = "Curve";
            SetDimensions(800, 600, 400, 400);

            ManualVM = new ManualStageDamageVM();
            CalculatedVM = new CalculatedStageDamageVM();
        }

        public AggregatedStageDamageEditorVM(ChildElement elem, EditorActionManager actionManager) : base(elem, "", "", "", actionManager)
        {
            AggregatedStageDamageElement element = (AggregatedStageDamageElement)elem;
            Name = element.Name;
            Description = element.Description;
            IsManualRadioSelected = element.IsManual;
            if (IsManualRadioSelected)
            {
                ManualVM = new ManualStageDamageVM(element.Curves);
                CalculatedVM = new CalculatedStageDamageVM();
            }
        }

        #endregion


        #region voids       

        /// <summary>
        /// This gets used when using the previous and next buttons.
        /// </summary>
        /// <param name="element"></param>
        public void Fill(AggregatedStageDamageElement element)
        {
            //todo are we doing the undo redo for this element? if not, then we don't need this.
        }

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

        public void Undo()
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

        public void Redo()
        {
            ChildElement nextElement = ActionManager.SaveUndoRedoHelper.RedoElement(CurrentElement);
            if (nextElement != null)
            {
                AssignValuesFromElementToEditor(nextElement);
                SavingText = CreateLastSavedText(nextElement);
                ReloadMessages();
                EditorVM.UpdateChartViewModel();
            }
        }

        public virtual ICoordinatesFunction GetCoordinatesFunction()
        {
            return EditorVM.CreateFunctionFromTables();
        }

        private bool ValidateManualRows()
        {
            bool areRowsUnique = AreManualRowsUniqueCombinations();
            bool curvesValid = AreManualCurvesValid();
            return areRowsUnique && curvesValid;
        }

        private bool AreManualCurvesValid()
        {
            ObservableCollection<ManualStageDamageRowItem> rows = ManualVM.Rows;
            foreach (ManualStageDamageRowItem r in rows)
            {
                try
                {
                    r.EditorVM.CreateFunctionFromTables();
                }
                catch (Exception ex)
                {
                    //we have an invalid curve
                    String msg = "An invalid curve was detected." + Environment.NewLine +
                        "Invalid curve: " + r.ID + Environment.NewLine + ex.Message;
                    MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private bool AreManualRowsUniqueCombinations()
        {
            HashSet<String> rowPairs = new HashSet<string>();
            ObservableCollection<ManualStageDamageRowItem> rows = ManualVM.Rows;
            foreach (ManualStageDamageRowItem row in rows)
            {
                //todo do we enforce unique names on the impact area names? I hope so or this won't work
                String uniqueCombo = row.SelectedImpArea.Name + " | " + row.SelectedDamCat;
                bool added = rowPairs.Add(uniqueCombo);
                if (!added)
                {
                    //then it was a duplicate
                    String msg = "Stage-Damage curves must have unique impact area and damage category combinations." + Environment.NewLine +
                        "Repeat curve: " + row.ID;
                    MessageBox.Show(msg, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        public virtual void SaveWhileEditing()
        {
            if (IsManualRadioSelected)
            {
                bool valid = ValidateManualRows();
                if (valid)
                {
                    //continue saving
                    int wseID = CalculatedVM.SelectedWaterSurfaceElevation.GetElementID();
                    int structID = CalculatedVM.SelectedStructures.GetElementID();
                    LastEditDate = DateTime.Now.ToString("G");
                    AggregatedStageDamageElement elem = new AggregatedStageDamageElement(Name, LastEditDate, Description, wseID, structID, ManualVM.GetStageDamageCurves(), true);
                    CurrentElement.LastEditDate = LastEditDate;
                    Saving.PersistenceManagers.StageDamagePersistenceManager manager = Saving.PersistenceFactory.GetStageDamageManager();
                    manager.SaveNew(elem);

                }
            }  
        }

        public override void Save()
        {
            SaveWhileEditing();
        }

        private void AssignValuesFromEditorToCurrentElement()
        {
            ActionManager.SaveUndoRedoHelper.AssignValuesFromEditorToElementAction(this, CurrentElement);
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
