using HEC.CS.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class CreateNewAlternativeComparisonReportVM : BaseEditorVM
    {
        private bool _IsInEditMode;

        private int _SelectedIndex = 0;
        private CustomObservableCollection<AlternativeComboItem> _ProjectAlternatives = new CustomObservableCollection<AlternativeComboItem>();
        public CustomObservableCollection<AlternativeComboItem> ProjectAlternatives
        {
            get { return _ProjectAlternatives; }
        }
        public AlternativeComboItem SelectedWithoutProjectAlternative { get; set; }
        public string Description { get; set; }
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { _SelectedIndex = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ComparisonRowItemVM> Rows { get; } = new ObservableCollection<ComparisonRowItemVM>();

        public CreateNewAlternativeComparisonReportVM( EditorActionManager actionManager) : base(actionManager)
        {
            LoadRows();
            ListenToAlternativeEvents();
        }

        public CreateNewAlternativeComparisonReportVM(AlternativeComparisonReportElement elem, EditorActionManager actionManager) : base((ChildElement)elem, actionManager)
        {
            _IsInEditMode = true;
            FillForm(elem);
            ListenToAlternativeEvents();
        }

        private void ListenToAlternativeEvents()
        {
            StudyCache.AlternativeAdded += AddAlternative;
            StudyCache.AlternativeRemoved += RemoveAlternative;
            StudyCache.AlternativeUpdated += UpdateAlternative;
        }

        private void FillForm(AlternativeComparisonReportElement elem)
        {
            Name = elem.Name;
            Description = elem.Description;
            LoadRows();
            //select the correct combo items in the first row.
            SelectWithoutProjectCombo(elem.WithoutProjAltID);
            List<int> withProjAltIDs = elem.WithProjAltIDs;
            SelectWithProjectCombo(Rows[0], withProjAltIDs[0]);
            //the first row is taken care of, now add rows and select correctly for the other with proj alts
            for(int i = 1;i<withProjAltIDs.Count;i++)
            {
                ComparisonRowItemVM newRow = new ComparisonRowItemVM(_ProjectAlternatives);
                Rows.Add(newRow);
                SelectWithProjectCombo(newRow, withProjAltIDs[i]);
            }
        }

        private void SelectWithProjectCombo(ComparisonRowItemVM row, int withProjID)
        {
            AlternativeComboItem comboToSelect = ProjectAlternatives.FirstOrDefault(comboItem => comboItem.Alternative.ID == withProjID);
            if (comboToSelect != null)
            {
                row.SelectedAlternative = comboToSelect;
            }
        }

        private void SelectWithoutProjectCombo(int withoutProjID)
        {
            AlternativeComboItem comboToSelect = ProjectAlternatives.FirstOrDefault(comboItem => comboItem.Alternative.ID == withoutProjID);
            if(comboToSelect != null)
            {
                SelectedWithoutProjectAlternative = comboToSelect;
            }
        }

        /// <summary>
        /// Adds the first row and fills the comboboxes with the available alternatives.
        /// </summary>
        private void LoadRows()
        {
            List<AlternativeElement> alts = StudyCache.GetChildElementsOfType<AlternativeElement>();
            List<AlternativeComboItem> comboItems = new List<AlternativeComboItem>();
            foreach(AlternativeElement elem in alts)
            {
                comboItems.Add(new AlternativeComboItem(elem));
            }
            ProjectAlternatives.AddRange(comboItems);
            Rows.Add(new ComparisonRowItemVM( _ProjectAlternatives));
            if(alts.Count>0)
            {
                SelectedWithoutProjectAlternative = comboItems[0];
            }
        }

        private void AddAlternative(object sender, ElementAddedEventArgs e)
        {
            AlternativeElement newAlt = e.Element as AlternativeElement;
            if(newAlt != null)
            {
                ProjectAlternatives.Add(new AlternativeComboItem(newAlt));
            }
        }
        private void RemoveAlternative(object sender, ElementAddedEventArgs e)
        {
            AlternativeComboItem itemToRemove = ProjectAlternatives.Where(comboItem => comboItem.Alternative.ID == e.Element.ID).SingleOrDefault();
            if (itemToRemove != null)
            {
                ProjectAlternatives.Remove(itemToRemove);
            }
        }
        private void UpdateAlternative(object sender, ElementUpdatedEventArgs e)
        {
            int idToUpdate = e.NewElement.ID;
            AlternativeComboItem itemToUpdate = ProjectAlternatives.Where(comboItem => comboItem.Alternative.ID == idToUpdate).SingleOrDefault();
            if (itemToUpdate != null)
            {
                itemToUpdate.UpdateAlternative((AlternativeElement)e.NewElement);
            }
        }

        public void AddComparison()
        {
            Rows.Add(new ComparisonRowItemVM( _ProjectAlternatives));
        }

        private FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            string errorMsg = "'Without Project' and 'With Project' selections must be unique.";
            //can't have the same without and with
            AlternativeElement alternative = SelectedWithoutProjectAlternative.Alternative;
            foreach(ComparisonRowItemVM row in Rows)
            {
                if(row.SelectedAlternative.Alternative == alternative)
                {
                    vr.AddErrorMessage(errorMsg);
                    break;
                }
            }
            //if we are still valid then do the next validation
            if (vr.IsValid)
            {
                //can't have repeat "with" selections
                List<AlternativeElement> selectedAlts = Rows.Select(row => row.SelectedAlternative.Alternative).ToList();
                bool isUnique = selectedAlts.Distinct().Count() == selectedAlts.Count();
                if (!isUnique)
                {
                    vr.AddErrorMessage(errorMsg);
                }
            }
            return vr;
        }

        public override void Save()
        {
            FdaValidationResult result = IsValid();
            if(!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage.ToString(), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(Description == null)
            {
                Description = "";
            }
            List<int> selectedIds = new List<int>();
            foreach(ComparisonRowItemVM row in Rows)
            {
                selectedIds.Add(row.SelectedAlternative.Alternative.ID);
            }

            if (_IsInEditMode)
            {
                AlternativeComparisonReportElement elemToSave = new AlternativeComparisonReportElement(Name, Description, DateTime.Now.ToString("G"), SelectedWithoutProjectAlternative.Alternative.ID, selectedIds, OriginalElement.ID);
                PersistenceFactory.GetAlternativeCompReportManager().SaveExisting(elemToSave);
            }
            else
            {
                int id = PersistenceFactory.GetAlternativeCompReportManager().GetNextAvailableId();
                AlternativeComparisonReportElement elemToSave = new AlternativeComparisonReportElement(Name, Description, DateTime.Now.ToString("G"), SelectedWithoutProjectAlternative.Alternative.ID, selectedIds, id);

                PersistenceFactory.GetAlternativeCompReportManager().SaveNew(elemToSave);
                _IsInEditMode = true;
            }
        }

        public void RemoveSelectedRow()
        {
            if(SelectedIndex>0)
            {
                int newIndex = SelectedIndex - 1;
                Rows.RemoveAt(SelectedIndex);
                SelectedIndex = newIndex;
            }
        }

    }
}
