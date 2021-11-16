using ViewModel.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Alternatives;
using ViewModel.Saving;
using HEC.CS.Collections;
using ViewModel.Utilities;

namespace ViewModel.AlternativeComparisonReport
{
    public class CreateNewAlternativeComparisonReportVM : BaseEditorVM
    {
        private AlternativeComparisonReportElement _CurrentElement;
        private bool _IsInEditMode;

        //private string _SelectedWithProjectPlan;
        //private ObservableCollection<Increment> _Increments = new ObservableCollection<Increment>();
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

        public CreateNewAlternativeComparisonReportVM(AlternativeComparisonReportElement elem, EditorActionManager actionManager) : base(actionManager)
        {
            _CurrentElement = elem;
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
            AlternativeComboItem comboToSelect = ProjectAlternatives.FirstOrDefault(comboItem => comboItem.ID == withProjID);
            if (comboToSelect != null)
            {
                row.SelectedAlternative = comboToSelect;
            }
        }

        private void SelectWithoutProjectCombo(int withoutProjID)
        {
            AlternativeComboItem comboToSelect = ProjectAlternatives.FirstOrDefault(comboItem => comboItem.ID == withoutProjID);
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
            
            AlternativeComboItem itemToRemove = ProjectAlternatives.Where(comboItem => comboItem.ID == e.ID).SingleOrDefault();
            if (itemToRemove != null)
            {
                ProjectAlternatives.Remove(itemToRemove);
            }
        }
        private void UpdateAlternative(object sender, ElementUpdatedEventArgs e)
        {

            int idToUpdate = e.ID;

            //AlternativeComboItem itemToUpdate = null;

            //foreach (AlternativeComboItem ci in ProjectAlternatives)
            //{
            //    int id = ci.Alternative.GetElementID();
            //    if(id == idToUpdate)
            //    {
            //        itemToUpdate = ci;
            //        break;
            //    }
            //}

            AlternativeComboItem itemToUpdate = ProjectAlternatives.Where(comboItem => comboItem.ID == idToUpdate).SingleOrDefault();
            if (itemToUpdate != null)
            {
                itemToUpdate.UpdateAlternative((AlternativeElement)e.NewElement);
                //if(ProjectAlternatives.Contains(itemToUpdate))
                //{
                //    int index = ProjectAlternatives.IndexOf(itemToUpdate);
                //    ProjectAlternatives.RemoveAt(index);
                //    ProjectAlternatives.Insert(index, (AlternativeElement)e.NewElement);
                //}
            }
        }

        public void AddComparison()
        {
            Rows.Add(new ComparisonRowItemVM( _ProjectAlternatives));
        }

        private FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            //can't have the same without and with
            AlternativeElement alternative = SelectedWithoutProjectAlternative.Alternative;
            foreach(ComparisonRowItemVM row in Rows)
            {
                if(row.SelectedAlternative.Alternative == alternative)
                {
                    vr.AddErrorMessage("The same selection cannot be used for the without project and the with project.");
                    break;
                }
            }
            //can't have repeat "with" selections
            //create a list of the selected "with" alternatives
            List<AlternativeElement> selectedAlts = Rows.Select(row => row.SelectedAlternative.Alternative).ToList();
            bool isUnique = selectedAlts.Distinct().Count() == selectedAlts.Count();
            if(!isUnique)
            {
                vr.AddErrorMessage("With project selections must be unique.");
            }
            return vr;
        }

        public override void Save()
        {
            FdaValidationResult result = IsValid();
            if(!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage.ToString(), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if(Description == null)
            {
                Description = "";
            }
            List<int> selectedIds = new List<int>();
            foreach(ComparisonRowItemVM row in Rows)
            {
                selectedIds.Add(row.SelectedAlternative.ID);
            }
            AlternativeComparisonReportElement elemToSave = new AlternativeComparisonReportElement(Name, Description, SelectedWithoutProjectAlternative.ID, selectedIds);

            if (_IsInEditMode)
            {
                //todo: create the manager
                PersistenceFactory.GetAlternativeCompReportManager().SaveExisting(_CurrentElement, elemToSave);
            }
            else
            {
                PersistenceFactory.GetAlternativeCompReportManager().SaveNew(elemToSave);
                _IsInEditMode = true;
            }
            _CurrentElement = elemToSave;

        }

        //public override bool RunSpecialValidation()
        //{
        //    bool isFirstValid = IsFirstIncrementValid();
        //    if(!isFirstValid)
        //    {
        //        MessageBox.Show("Not all increments have plans defined.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return false;
        //    }
        //    foreach (Increment inc in Increments)
        //    {
        //        if(!IsFirstPlanDefined(inc))
        //        {
        //            MessageBox.Show("Not all increments have plans defined.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //private bool IsFirstPlanDefined(Increment inc)
        //{
        //    if (inc.SelectedPlan1 != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //private bool IsFirstIncrementValid()
        //{
        //    Increment inc = Increments[0];
        //    if(inc.SelectedPlan1 != null && inc.SelectedPlan2 != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

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
