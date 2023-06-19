using HEC.FDA.Model.stageDamage;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HEC.FDA.ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM : BaseEditorVM
    {
        private bool _IsManualRadioSelected = false;
        private BaseViewModel _CurrentVM;

        #region properties
        public BaseViewModel CurrentVM
        {
            get { return _CurrentVM; }
            set { _CurrentVM = value; NotifyPropertyChanged(); }
        }
        public bool IsManualRadioSelected
        {
            get { return _IsManualRadioSelected; }
            set { _IsManualRadioSelected = value; UpdateVM(); NotifyPropertyChanged(); }
        }

        public ManualStageDamageVM ManualVM { get; set; }

        public CalculatedStageDamageVM CalculatedVM { get; set; }

        #endregion

        #region constructors
        public AggregatedStageDamageEditorVM(EditorActionManager actionManager) : base(actionManager)
        {
            HasChanges = true;
            ManualVM = new ManualStageDamageVM();
            CalculatedVM = new CalculatedStageDamageVM(GetName);
            //this registration is so that fda can detect changes made in child view models
            //and prompt the user if they want to save when closing
            RegisterChildViewModel(ManualVM);
            RegisterChildViewModel(CalculatedVM);
            CurrentVM = CalculatedVM;
        }

        public AggregatedStageDamageEditorVM(ChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            AggregatedStageDamageElement element = (AggregatedStageDamageElement)elem;
            Name = element.Name;
            Description = element.Description;
            IsManualRadioSelected = element.IsManual;
            if (IsManualRadioSelected)
            {
                ManualVM = new ManualStageDamageVM(element.Curves);
                CalculatedVM = new CalculatedStageDamageVM(GetName);
                CurrentVM = ManualVM;
            }
            else
            {
                ManualVM = new ManualStageDamageVM();
                CalculatedVM = new CalculatedStageDamageVM(element.SelectedWSE, element.SelectedStructures, element.AnalysisYear, element.Curves, element.ImpactAreaFrequencyRows,element.WriteDetailsOut, GetName);
                CurrentVM = CalculatedVM;
            }
            //this registration is so that fda can detect changes made in child view models
            //and prompt the user if they want to save when closing
            RegisterChildViewModel(ManualVM);
            RegisterChildViewModel(CalculatedVM);
        }
        #endregion

        #region voids          
        
        private void UpdateVM()
        {
            if(_IsManualRadioSelected)
            {
                CurrentVM = ManualVM;
            }
            else
            {
                CurrentVM = CalculatedVM;
            }
        }
        
        /// <summary>
        /// Updates the name of the UPD
        /// </summary>
        private void UpdateStageDamageMetaData(List<StageDamageCurve> curves)
        {
            foreach(StageDamageCurve curve in curves)
            {
                curve.ComputeComponent.Name = Name + " " + curve.ImpArea.Name + " " + curve.DamCat + " " + curve.AssetCategory;
            }
        }

        private void SaveCalculatedCurves()
        {
            FdaValidationResult vr = CalculatedVM.ValidateForm();
            if (vr.IsValid)
            {
                int wseID = CalculatedVM.SelectedWaterSurfaceElevation.ID;
                int structID = CalculatedVM.SelectedStructures.ID;
                string lastEditDate = DateTime.Now.ToString("G");
                int id = 1;
                if (OriginalElement != null)
                {
                    id = OriginalElement.ID;
                }
                else
                {
                    id = Saving.PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().GetNextAvailableId();
                }
                List<StageDamageCurve> stageDamageCurves = CalculatedVM.GetStageDamageCurves();
                UpdateStageDamageMetaData(stageDamageCurves);
                AggregatedStageDamageElement elemToSave = new AggregatedStageDamageElement(Name, lastEditDate, Description, CalculatedVM.AnalysisYear, wseID, structID, 
                   stageDamageCurves, CalculatedVM.ImpactAreaFrequencyRows, false, CalculatedVM.WriteDetailsFile, id);              
                base.Save(elemToSave);
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage, "Unable to Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveManualEditCurves()
        {
            bool valid = ManualVM.ValidateForm();
            if (valid)
            {
                string lastEditDate = DateTime.Now.ToString("G");
                int id = GetElementID();
                List<ImpactAreaFrequencyFunctionRowItem> impAreaFrequencyRows = new List<ImpactAreaFrequencyFunctionRowItem>();
                List<StageDamageCurve> stageDamageCurves = ManualVM.GetStageDamageCurves();
                UpdateStageDamageMetaData(stageDamageCurves);
                int analysisYear = DateTime.Now.Year;
                AggregatedStageDamageElement elem = new AggregatedStageDamageElement(Name, lastEditDate, Description, analysisYear, - 1, -1, stageDamageCurves,
                    impAreaFrequencyRows, true, false, id);
                base.Save(elem);
            }
        }

        public override void Save()
        {
            if (IsManualRadioSelected)
            {
                SaveManualEditCurves();
            }
            else
            {
                SaveCalculatedCurves();
            }
        }

        public int GetElementID()
        {
            int id = -1;
            if (IsCreatingNewElement)
            {
                id = Saving.PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().GetNextAvailableId();
            }
            else
            {
                id = OriginalElement.ID;
            }
            return id;
        }

        public string GetName()
        {
            return Name;
        }

        #endregion
    }
}
