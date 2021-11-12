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

namespace ViewModel.AlternativeComparisonReport
{
    public class CreateNewAlternativeComparisonReportVM : BaseEditorVM
    {
        //private string _SelectedWithProjectPlan;
        private ObservableCollection<Increment> _Increments = new ObservableCollection<Increment>();
        private int _SelectedIndex = 0;
        private CustomObservableCollection<AlternativeComboItem> _ProjectAlternatives = new CustomObservableCollection<AlternativeComboItem>();
        public CustomObservableCollection<AlternativeComboItem> ProjectAlternatives
        {
            get { return _ProjectAlternatives; }
        }
        public AlternativeComboItem SelectedWithoutProjectAlternative { get; set; }

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { _SelectedIndex = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<ComparisonRowItemVM> Rows { get; } = new ObservableCollection<ComparisonRowItemVM>();

        public ObservableCollection<Increment> Increments
        {
            get { return _Increments; }
            set { _Increments = value; NotifyPropertyChanged(); }
        }
        public List<string> Plans { get; set; }
        //change from string to "plan" once that object exists
        //public string SelectedWithoutProjectPlan
        //{
        //    get;set;
        //}
        //public string SelectedWithProjectPlan
        //{
        //    get { return _SelectedWithProjectPlan; }
        //    set { _SelectedWithProjectPlan = value; NotifyPropertyChanged(); }
        //}
        //public string SelectedWithProjectPlanSecondIncrement { get; set; }


       // public bool IsUsingSecondIncrement { get; set; }

        public CreateNewAlternativeComparisonReportVM( EditorActionManager actionManager) : base(actionManager)
        {
            LoadRows();
            StudyCache.AlternativeAdded += AddAlternative;
            StudyCache.AlternativeRemoved += RemoveAlternative;
            StudyCache.AlternativeUpdated += UpdateAlternative;
        }

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

        public override void Save()
        {
            //the second increment onward will not have the second plan defined. You need to
            //get it from the first plan of the previous increment.
            int i = 0;
        }

        public override bool RunSpecialValidation()
        {
            bool isFirstValid = IsFirstIncrementValid();
            if(!isFirstValid)
            {
                MessageBox.Show("Not all increments have plans defined.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            foreach (Increment inc in Increments)
            {
                if(!IsFirstPlanDefined(inc))
                {
                    MessageBox.Show("Not all increments have plans defined.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private bool IsFirstPlanDefined(Increment inc)
        {
            if (inc.SelectedPlan1 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsFirstIncrementValid()
        {
            Increment inc = Increments[0];
            if(inc.SelectedPlan1 != null && inc.SelectedPlan2 != null)
            {
                return true;
            }
            else
            {
                return false;
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
