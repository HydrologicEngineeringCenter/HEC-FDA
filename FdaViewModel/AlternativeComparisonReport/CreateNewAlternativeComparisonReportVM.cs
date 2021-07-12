using FdaViewModel.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FdaViewModel.AlternativeComparisonReport
{
    public class CreateNewAlternativeComparisonReportVM : BaseEditorVM
    {
        private string _SelectedWithProjectPlan;
        private ObservableCollection<Increment> _Increments = new ObservableCollection<Increment>();
        //private ObservableCollection<string> _SelectedPlan1 = new ObservableCollection<string>();
        //private ObservableCollection<string> _SelectedPlan2 = new ObservableCollection<string>();

        //public ObservableCollection<string> SelectedPlan1
        //{
        //    get { return _SelectedPlan1; }
        //    set { _SelectedPlan1 = value; NotifyPropertyChanged(); }
        //}
        //public ObservableCollection<string> SelectedPlan2
        //{
        //    get { return _SelectedPlan2; }
        //    set { _SelectedPlan2 = value; NotifyPropertyChanged(); }
        //}
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

        public CreateNewAlternativeComparisonReportVM(List<string> plans, EditorActionManager actionManager) : base(actionManager)
        {
            Plans = plans;
        }

        public void AddIncrement()
        {
            Increment inc = new Increment("Increment " + Increments.Count, Plans);
            Increments.Add(inc);
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
    }
}
