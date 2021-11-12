using System.Collections.Generic;
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

        private AlternativeResultBase _selectedResult;
        private string _SelectedDamageMeasure;
        private string _SelectedReport;
        private bool _YearsVisible;
        private YearResult _SelectedYear;
       
        public bool YearsVisible
        {
            get { return _YearsVisible; }
            set { _YearsVisible = value; NotifyPropertyChanged(); }
        }
        public List<int> Years { get; } = new List<int>();

        public YearResult SelectedYear
        {
            get { return _SelectedYear; }
            set { _SelectedYear = value;  SelectedYearChanged(); NotifyPropertyChanged(); }
        }
        
        public AlternativeResultBase CurrentResultVM
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }
        public List<string> DamageMeasure { get; } = new List<string>() { EAD, AAEQ };

        public string SelectedDamageMeasure
        {
            get { return _SelectedDamageMeasure; }
            set { _SelectedDamageMeasure = value; SelectedDamageMeasureChanged(); NotifyPropertyChanged(); }
        }

        public List<string> Reports { get; } = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_IMPACT_AREA, DAMAGE_BY_DAMAGE_CATEGORY};

        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; SelectedReportChanged(); NotifyPropertyChanged(); }
        }

        public AlternativeResult AlternativeResult { get;  }
        
        public AlternativeResultsVM(AlternativeResult altResult)
        {
            AlternativeResult = altResult;
            YearsVisible = true;
            
            foreach(YearResult yr in altResult.EADResult.YearResults)
            {
                Years.Add(yr.Year);
            }

            //set the starting state of the combos.
            _SelectedDamageMeasure = EAD;
            SelectedYear = altResult.EADResult.YearResults[0];
            _SelectedReport = DAMAGE_WITH_UNCERTAINTY;
        }

        private void SelectedYearChanged()
        {
            //i can assume we are on EAD if a year is changing.
            // List<YearResult> yearResults = _AlternativeResult.EADResult.YearResults;
            SelectedReport = DAMAGE_WITH_UNCERTAINTY;
            CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
        }

        private void SelectedDamageMeasureChanged()
        {
            if(EAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = true;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
            }
            else if(AAEQ.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = false;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
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
                    CurrentResultVM = SelectedYear.DamageByImpactAreaVM;
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
