using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
using System;
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
            IsCreatingNewElement = true;
            HasChanges = true;
            SetDimensions(800, 600, 400, 400);
            ManualVM = new ManualStageDamageVM();
            CalculatedVM = new CalculatedStageDamageVM();
            CurrentVM = CalculatedVM;
        }

        public AggregatedStageDamageEditorVM(ChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            IsCreatingNewElement = false;
            AggregatedStageDamageElement element = (AggregatedStageDamageElement)elem;
            Name = element.Name;
            Description = element.Description;
            IsManualRadioSelected = element.IsManual;
            if (IsManualRadioSelected)
            {
                ManualVM = new ManualStageDamageVM(element.Curves);
                CalculatedVM = new CalculatedStageDamageVM();
                CurrentVM = ManualVM;
            }
            else
            {
                ManualVM = new ManualStageDamageVM();
                CalculatedVM = new CalculatedStageDamageVM(element.SelectedWSE, element.SelectedStructures, element.Curves);
                CurrentVM = CalculatedVM;
            }
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
        
        private void SaveCalculatedCurves()
        {
            FdaValidationResult vr = CalculatedVM.ValidateForm();
            if (vr.IsValid)
            {
                int wseID = CalculatedVM.SelectedWaterSurfaceElevation.ID;
                int structID = CalculatedVM.SelectedStructures.ID;
                LastEditDate = DateTime.Now.ToString("G");
                int id = Saving.PersistenceFactory.GetStageDamageManager().GetNextAvailableId();
                AggregatedStageDamageElement elemToSave = new AggregatedStageDamageElement(Name, LastEditDate, Description, wseID, structID, CalculatedVM.GetStageDamageCurves(), false, id);              
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
                LastEditDate = DateTime.Now.ToString("G");
                int id = GetElementID();
                AggregatedStageDamageElement elem = new AggregatedStageDamageElement(Name, LastEditDate, Description, -1, -1, ManualVM.GetStageDamageCurves(), true, id);
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
                id = Saving.PersistenceFactory.GetStageDamageManager().GetNextAvailableId();
            }
            else
            {
                id = OriginalElement.ID;
            }
            return id;
        }

        #endregion
    }
}
