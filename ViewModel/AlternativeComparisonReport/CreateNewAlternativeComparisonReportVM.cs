using ViewModel.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Alternatives;

namespace ViewModel.AlternativeComparisonReport
{
    public class CreateNewAlternativeComparisonReportVM : BaseEditorVM
    {
        private string _SelectedWithProjectPlan;
        private ObservableCollection<Increment> _Increments = new ObservableCollection<Increment>();
        private int _SelectedIndex = 0;

        public List<AlternativeElement> ProjectAlternatives { get; } = new List<AlternativeElement>();
        public AlternativeElement SelectedWithoutProjectAlternative { get; set; }

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
            //StudyCache.StageDamageAdded += AddStageDamageElement;

        }

        private void LoadRows()
        {
            List<AlternativeElement> alts = StudyCache.GetChildElementsOfType<AlternativeElement>();
            ProjectAlternatives.AddRange(alts);
            Rows.Add(new ComparisonRowItemVM(ProjectAlternatives));
            if(alts.Count>0)
            {
                SelectedWithoutProjectAlternative = alts[0];
            }
        }

        public void AddComparison()
        {
            Rows.Add(new ComparisonRowItemVM(ProjectAlternatives));
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
