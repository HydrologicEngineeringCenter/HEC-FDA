using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class SpecificAltCompReportResultsVM : BaseViewModel
    {
        private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
        private const string LIFE_LOSS_WITH_UNCERTAINTY = "Life Loss with Uncertainty";
        private const string LIFE_LOSS_BY_IMPACT_AREA = "Life Loss by Impact Area";
        private const string DAMAGE_BY_IMPACT_AREA = "Damage by Impact Area";
        private const string DAMAGE_BY_DAMAGE_CATEGORY = "Damage by Damage Category";
        private const string EAD = "EAD";
        private const string EALL = "EALL";
        private const string EqAD = "EqAD";

        private IAlternativeResult _selectedResult;
        private string _SelectedDamageMeasure;
        private string _SelectedReport;
        private List<string> _reports = [];
        private List<string> _damageReports = [DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_IMPACT_AREA, DAMAGE_BY_DAMAGE_CATEGORY];
        private List<string> _lifeLossReports = [LIFE_LOSS_WITH_UNCERTAINTY, LIFE_LOSS_BY_IMPACT_AREA];
        private bool _YearsVisible;
        private YearResult _SelectedYear;


        public string Name { get; }
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
        public List<string> DamageMeasure { get; } = new List<string>();

        public string SelectedDamageMeasure
        {
            get { return _SelectedDamageMeasure; }
            set { _SelectedDamageMeasure = value; SelectedDamageMeasureChanged(); NotifyPropertyChanged(); }
        }

        public List<string> Reports
        {
            get { return _reports; }
            set { _reports = value; NotifyPropertyChanged(); }
        }

        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; SelectedReportChanged(); NotifyPropertyChanged(); }
        }

        public AlternativeResult AlternativeResult { get; }

        public SpecificAltCompReportResultsVM(AlternativeResult altResult, bool hasDamageResults, bool hasLifeLossResults)
        {
            Name = altResult.Name;
            AlternativeResult = altResult;
            YearsVisible = true;

            foreach (YearResult yr in altResult.EADResult.YearResults)
            {
                Years.Add(yr.Year);
            }

            // Dynamically populate DamageMeasure based on available data
            if (hasDamageResults)
            {
                DamageMeasure.Add(EAD);
                DamageMeasure.Add(EqAD);
            }
            if (hasLifeLossResults)
            {
                DamageMeasure.Add(EALL);
            }

            // Set the starting state of the combos based on available data
            if (hasDamageResults)
            {
                _SelectedDamageMeasure = EAD;
                SelectedYear = altResult.EADResult.YearResults[0];
                Reports = _damageReports;
                _SelectedReport = DAMAGE_WITH_UNCERTAINTY;
            }
            else if (hasLifeLossResults)
            {
                _SelectedDamageMeasure = EALL;
                SelectedYear = altResult.EADResult.YearResults[0];
                Reports = _lifeLossReports;
                _SelectedReport = LIFE_LOSS_WITH_UNCERTAINTY;
            }
        }

        public SpecificAltCompReportResultsVM()
        {
            Name = "Summary";
        }

        private void SelectedYearChanged()
        {
            if (CurrentResultVM == null || CurrentResultVM is DamageWithUncertaintyVM)
            {
                if (EAD.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
                    SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                }
                else if (EALL.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.LifeLossWithUncertaintyVM;
                    SelectedReport = LIFE_LOSS_WITH_UNCERTAINTY;
                }
            }
            else if (CurrentResultVM is DamageByDamCatVM)
            {
                CurrentResultVM = SelectedYear.DamageByDamCatVM;
                SelectedReport = DAMAGE_BY_DAMAGE_CATEGORY;
            }
            else if (CurrentResultVM is DamageByImpactAreaVM)
            {
                if (EAD.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.DamageByImpactAreaVM;
                    SelectedReport = DAMAGE_BY_IMPACT_AREA;
                }
                else if (EALL.Equals(_SelectedDamageMeasure))
                {
                    CurrentResultVM = SelectedYear.LifeLossByImpactAreaVM;
                    SelectedReport = LIFE_LOSS_BY_IMPACT_AREA;
                }
            }
        }

        private void SelectedDamageMeasureChanged()
        {
            if (EAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = true;
                Reports = _damageReports;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
            }
            else if (EqAD.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = false;
                Reports = _damageReports;
                SelectedReport = DAMAGE_WITH_UNCERTAINTY;
                CurrentResultVM = AlternativeResult.EqadResult.DamageWithUncertaintyVM;
            }
            else if (EALL.Equals(_SelectedDamageMeasure))
            {
                YearsVisible = true;
                Reports = _lifeLossReports;
                SelectedReport = LIFE_LOSS_WITH_UNCERTAINTY;
                CurrentResultVM = SelectedYear.LifeLossWithUncertaintyVM;
            }
        }
        private void SelectedReportChanged()
        {
            switch (SelectedReport)
            {
                case DAMAGE_WITH_UNCERTAINTY:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageWithUncertaintyVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.EqadResult.DamageWithUncertaintyVM;
                    }
                    break;
                case DAMAGE_BY_IMPACT_AREA:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageByImpactAreaVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.EqadResult.DamageByImpactAreaVM;
                    }
                    break;
                case DAMAGE_BY_DAMAGE_CATEGORY:
                    if (EAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = SelectedYear.DamageByDamCatVM;
                    }
                    else if (EqAD.Equals(_SelectedDamageMeasure))
                    {
                        CurrentResultVM = AlternativeResult.EqadResult.DamageByDamCatVM;
                    }
                    break;
                case LIFE_LOSS_WITH_UNCERTAINTY:
                    CurrentResultVM = SelectedYear.LifeLossWithUncertaintyVM;
                    break;
                case LIFE_LOSS_BY_IMPACT_AREA:
                    CurrentResultVM = SelectedYear.LifeLossByImpactAreaVM;
                    break;
            }

        }
    }
}
