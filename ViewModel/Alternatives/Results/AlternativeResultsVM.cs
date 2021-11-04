using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives.Results
{
    public class AlternativeResultsVM : BaseViewModel
    {

        private AlternativeResultBase _selectedResult;
        private List<string> _Reports = new List<string>() { "Damage with Uncertainty", "Damage by Impact Area",
        "Damage by Damage Category"};
        private List<string> _DamageMeasure = new List<string>() { "EAD", "AAEQ"};
        private string _SelectedDamageMeasure;
        private string _SelectedReport;
        private int _SelectedYear;
        private bool _YearsVisible;
        private AAEQDamageByDamCatVM _AAEQDamageByDamCatVM;
        private AAEQDamageByImpactAreaVM _AAEQDamageByImpactAreaVM;
        private AAEQDamageWithUncertaintyVM _AAEQDamageWithUncertaintyVM;

        private EADDamageByDamCatVM _EADDamageByDamCatVM;
        private EADDamageByImpactAreaVM _EADDamageByImpactAreaVM;
        private EADDamageWithUncertaintyVM _EADDamageWithUncertaintyVM;

        public bool YearsVisible
        {
            get { return _YearsVisible; }
            set { _YearsVisible = value; NotifyPropertyChanged(); }
        }
        public List<int> Years { get; set; }
        public int SelectedYear
        {
            get { return _SelectedYear; }
            set { _SelectedYear = value;  }
        }
        
        public List<AlternativeResultBase> Results { get; set; }
        public AlternativeResultBase CurrentResultVM
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }

        public List<string> DamageMeasure
        {
            get { return _DamageMeasure; }
            set { _DamageMeasure = value; NotifyPropertyChanged(); }
        }
        public string SelectedDamageMeasure
        {
            get { return _SelectedDamageMeasure; }
            set { _SelectedDamageMeasure = value; SelectedDamageMeasureChanged(); NotifyPropertyChanged(); }
        }

        public List<string> Reports
        {
            get { return _Reports; }
            set { _Reports = value; NotifyPropertyChanged(); }
        }
        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; SelectedReportChanged(); NotifyPropertyChanged(); }
        }


        public AlternativeResultsVM(List<AlternativeResultBase> results)
        {
            LoadVMs();
            Results = results;
            CurrentResultVM = results[0];

            _SelectedDamageMeasure = _DamageMeasure[0];
            //_SelectedYear = 2021;
            _SelectedReport = _Reports[0];


        }

        private void LoadVMs()
        {
            _AAEQDamageByDamCatVM = new AAEQDamageByDamCatVM();
            _AAEQDamageByImpactAreaVM = new AAEQDamageByImpactAreaVM();
            _AAEQDamageWithUncertaintyVM = new AAEQDamageWithUncertaintyVM();
        }

        private void SelectedDamageMeasureChanged()
        {
            if("EAD".Equals(_SelectedDamageMeasure))
            {

            }
        }
        private void SelectedReportChanged()
        {
            //private List<string> _Reports = new List<string>() { "Damage with Uncertainty", "Damage by Impact Area",
        //"Damage by Damage Category"};
            if (_Reports[0].Equals(SelectedReport))
            {
                //ead
                if (_SelectedDamageMeasure.Equals(_DamageMeasure[0]))
                {
                    CurrentResultVM = _EADDamageWithUncertaintyVM;
                }
                //aaeq
                else
                {
                    CurrentResultVM = _AAEQDamageWithUncertaintyVM;
                }
            }
            else if (_Reports[1].Equals(SelectedReport))
            {
                //ead
                if (_SelectedDamageMeasure.Equals(_DamageMeasure[0]))
                {
                    CurrentResultVM = _EADDamageByImpactAreaVM;
                }
                //aaeq
                else
                {
                    CurrentResultVM = _AAEQDamageByImpactAreaVM;
                }
            }
            else if (_Reports[2].Equals(SelectedReport))
            {
                //ead
                if (_SelectedDamageMeasure.Equals(_DamageMeasure[0]))
                {
                    CurrentResultVM = _EADDamageByDamCatVM;
                }
                //aaeq
                else
                {
                    CurrentResultVM = _AAEQDamageByDamCatVM;
                }
            }
        }


    }
}
