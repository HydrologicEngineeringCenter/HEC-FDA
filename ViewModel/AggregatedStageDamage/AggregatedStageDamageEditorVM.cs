using paireddata;
using System;
using System.Windows;
using ViewModel.Editors;
using ViewModel.Utilities;


namespace ViewModel.AggregatedStageDamage
{
    public class AggregatedStageDamageEditorVM : BaseLoggingEditorVM
    {
        private bool _IsInEditMode;
        private bool _IsManualRadioSelected = true;
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
        public AggregatedStageDamageEditorVM(UncertainPairedData func, string xLabel, string yLabel, string chartTitle, EditorActionManager actionManager) : base(func, xLabel, yLabel, chartTitle, actionManager)
        {
            _IsInEditMode = false;
            HasChanges = true;
            SetDimensions(800, 600, 400, 400);

            ManualVM = new ManualStageDamageVM();
            CalculatedVM = new CalculatedStageDamageVM();
            CurrentVM = ManualVM;
        }

        public AggregatedStageDamageEditorVM(ChildElement elem, EditorActionManager actionManager) : base(elem, "", "", "", actionManager)
        {
            _IsInEditMode = true;
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
   

        public virtual void SaveWhileEditing()
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

        
        private void SaveCalculatedCurves()
        {
            FdaValidationResult vr = CalculatedVM.ValidateForm();
            if (vr.IsValid)
            {
                int wseID = CalculatedVM.SelectedWaterSurfaceElevation.GetElementID();
                int structID = CalculatedVM.SelectedStructures.GetElementID();
                LastEditDate = DateTime.Now.ToString("G");
                AggregatedStageDamageElement elem = new AggregatedStageDamageElement(Name, LastEditDate, Description, wseID, structID, CalculatedVM.GetStageDamageCurves(), false);
                if (CurrentElement == null)
                {
                    CurrentElement = elem;
                }
                CurrentElement.LastEditDate = LastEditDate;
                Saving.PersistenceManagers.StageDamagePersistenceManager manager = Saving.PersistenceFactory.GetStageDamageManager();

                if (_IsInEditMode)
                {
                    manager.SaveExisting(CurrentElement, elem);
                }
                else
                {
                    manager.SaveNew(elem);
                    _IsInEditMode = true;
                }
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
                AggregatedStageDamageElement elem = new AggregatedStageDamageElement(Name, LastEditDate, Description, -1, -1, ManualVM.GetStageDamageCurves(), true);

                Saving.PersistenceManagers.StageDamagePersistenceManager manager = Saving.PersistenceFactory.GetStageDamageManager();

                if (_IsInEditMode)
                {
                    manager.SaveExisting(CurrentElement, elem);
                }
                else
                {
                    manager.SaveNew(elem);
                    _IsInEditMode = true;
                }
                CurrentElement = elem;
            }
        }

        public override void Save()
        {
            SaveWhileEditing();
        }
        #endregion
    }
}
