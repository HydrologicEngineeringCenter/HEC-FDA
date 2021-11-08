using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Alternatives.Results.ResultObject;

namespace ViewModel.Alternatives.Results
{
    public class AlternativeResultsVM : BaseViewModel
    {

        private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
        private const string DAMAGE_BY_IMPACT_AREA = "Damage by Impact Area";
        private const string DAMAGE_BY_DAMAGE_CATEGORY = "Damage by Damage Category";
        private const string EAD = "EAD";
        private const string AAEQ = "AAEQ";

        //private AlternativeResult _AlternativeResult;

        private AlternativeResultBase _selectedResult;
        private List<string> _Reports = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_IMPACT_AREA,
        DAMAGE_BY_DAMAGE_CATEGORY};
        private List<string> _DamageMeasure = new List<string>() { EAD, AAEQ};
        private string _SelectedDamageMeasure;
        private string _SelectedReport;
        private bool _YearsVisible;
        private AAEQDamageByDamCatVM _AAEQDamageByDamCatVM;
        private AAEQDamageByImpactAreaVM _AAEQDamageByImpactAreaVM;
        private AAEQDamageWithUncertaintyVM _AAEQDamageWithUncertaintyVM;

        private DamageByDamCatVM _EADDamageByDamCatVM;
        private DamageByImpactAreaVM _EADDamageByImpactAreaVM;
        private DamageWithUncertaintyVM _EADDamageWithUncertaintyVM;

        private List<int> _Years = new List<int>() { 2021, 2022 };
        private YearResult _SelectedYear;
       

        public bool YearsVisible
        {
            get { return _YearsVisible; }
            set { _YearsVisible = value; NotifyPropertyChanged(); }
        }
        public List<int> Years
        {
            get { return _Years; }
            set { _Years = value; NotifyPropertyChanged(); }
        }
        public YearResult SelectedYear
        {
            get { return _SelectedYear; }
            set { _SelectedYear = value;  SelectedYearChanged(); NotifyPropertyChanged(); }
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

        public AlternativeResult AlternativeResult { get; set; }
        

        public AlternativeResultsVM(AlternativeResult altResult)
        {
            AlternativeResult = altResult;
            YearsVisible = true;
            //LoadVMs();
            //Results = results;
            //CurrentResultVM = results[0];
            Years = new List<int>();
            foreach(YearResult yr in altResult.EADResult.YearResults)
            {
                Years.Add(yr.Year);
            }

            _SelectedDamageMeasure = EAD;
            SelectedYear = altResult.EADResult.YearResults[0];
            //_SelectedReport = DAMAGE_WITH_UNCERTAINTY;

            //CurrentResultVM = altResult.EADResult.YearResults[0].DamageWithUncertaintyVM;


        }

        //private void LoadVMs()
        //{
        //    _AAEQDamageByDamCatVM = new AAEQDamageByDamCatVM();
        //    _AAEQDamageByImpactAreaVM = new AAEQDamageByImpactAreaVM();
        //    _AAEQDamageWithUncertaintyVM = new AAEQDamageWithUncertaintyVM();
        //}

        private void SelectedYearChanged()
        {
            //i can assume we are on EAD if a year is changing.
           // List<YearResult> yearResults = _AlternativeResult.EADResult.YearResults;
            CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;

        }

        private void SelectedDamageMeasureChanged()
        {
            if(EAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = true;
                CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
            }
            else if(AAEQ.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = false;
                CurrentResultVM = AlternativeResult.AAEQResult.DamageWithUncertaintyVM;
            }
        }
        private void SelectedReportChanged()
        {
            if (DAMAGE_WITH_UNCERTAINTY.Equals(SelectedReport))
            {
                if (EAD.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
                }
                else if (AAEQ.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = AlternativeResult.AAEQResult.DamageWithUncertaintyVM;
                }
            }
            else if (DAMAGE_BY_IMPACT_AREA.Equals(SelectedReport))
            {
                if (EAD.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.DamageByImpactAreaVM ;
                }
                else if (AAEQ.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = AlternativeResult.AAEQResult.DamageByImpactAreaVM;
                }
            }
            else if (DAMAGE_BY_DAMAGE_CATEGORY.Equals(SelectedReport))
            {
                if (EAD.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.DamageByDamCatVM;
                }
                else if (AAEQ.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = AlternativeResult.AAEQResult.DamageByDamCatVM;
                }
            }
        }


    }
}
