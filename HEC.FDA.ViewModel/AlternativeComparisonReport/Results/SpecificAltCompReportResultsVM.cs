using System.Collections.Generic;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using HEC.FDA.ViewModel.Study;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class SpecificAltCompReportResultsVM : BaseViewModel
    {
        private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
        private const string DAMAGE_BY_IMPACT_AREA = "Damage by Impact Area";
        private const string DAMAGE_BY_DAMAGE_CATEGORY = "Damage by Damage Category";
        private const string EAD = "EAD";
        private const string EqAD = "EqAD";

        private IAlternativeResult _selectedResult;
        private string _SelectedDamageMeasure;
        private string _SelectedReport;
        private bool _YearsVisible;
        private YearResult _SelectedYear;


        public string Name { get;}
        public bool YearsVisible
        {
            get { return _YearsVisible; }
            set { _YearsVisible = value; NotifyPropertyChanged(); }
        }
        public List<int> Years { get; } = new List<int>();

        public YearResult SelectedYear
        {
            get { return _SelectedYear; }
            set { _SelectedYear = value; SelectedYearChanged(); NotifyPropertyChanged(); }
        }

        public IAlternativeResult CurrentResultVM
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }
        public List<string> DamageMeasure { get; } = new List<string>() { EAD, EqAD };

        public string SelectedDamageMeasure
        {
            get { return _SelectedDamageMeasure; }
            set { _SelectedDamageMeasure = value; SelectedDamageMeasureChanged(); NotifyPropertyChanged(); }
        }

        public List<string> Reports { get; } = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_IMPACT_AREA, DAMAGE_BY_DAMAGE_CATEGORY };

        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; SelectedReportChanged(); NotifyPropertyChanged(); }
        }

        public AlternativeResult AlternativeResult { get; }

        public SpecificAltCompReportResultsVM(AlternativeResult altResult)
        {
            Name = altResult.Name;       
            AlternativeResult = altResult;
            YearsVisible = true;

            foreach (YearResult yr in altResult.EADResult.YearResults)
            {
                Years.Add(yr.Year);
            }

            //set the starting state of the combos.
            _SelectedDamageMeasure = EAD;
            SelectedYear = altResult.EADResult.YearResults[0];
            _SelectedReport = DAMAGE_WITH_UNCERTAINTY;
        }

        public SpecificAltCompReportResultsVM()
        {
            Name = "Summary";
        }

        private void SelectedYearChanged()
        {
            //i can assume we are on EAD if a year is changing.
            //we want to select the same vm when switching years
            if(CurrentResultVM == null || CurrentResultVM is DamageWithUncertaintyVM)
            {
                CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
            }
            else if(CurrentResultVM is DamageByDamCatVM)
            {
                CurrentResultVM = SelectedYear.DamageByDamCatVM;
                SelectedReport = DAMAGE_BY_DAMAGE_CATEGORY;
            }
            else if(CurrentResultVM is DamageByImpactAreaVM)
            {
                CurrentResultVM = SelectedYear.DamageByImpactAreaVM;
                SelectedReport = DAMAGE_BY_IMPACT_AREA;
            }
        }

        private void SelectedDamageMeasureChanged()
        {
            if (EAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = true;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
            }
            else if (EqAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = false;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                CurrentResultVM = AlternativeResult.AAEQResult.DamageWithUncertaintyVM;
            }
        }
        private void SelectedReportChanged()
        {
            switch(SelectedReport)
            {
                case DAMAGE_WITH_UNCERTAINTY:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.AAEQResult.DamageWithUncertaintyVM;
                    }
                    break;
                case DAMAGE_BY_IMPACT_AREA:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageByImpactAreaVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.AAEQResult.DamageByImpactAreaVM;
                    }
                    break;
                case DAMAGE_BY_DAMAGE_CATEGORY:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageByDamCatVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.AAEQResult.DamageByDamCatVM;
                    }
                    break;
            }

        }
    }
}
