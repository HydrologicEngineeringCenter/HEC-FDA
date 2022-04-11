using metrics;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class SpecificIASResultVM : BaseViewModel
    {
        private const string DAMAGE = "Damage";
        private const string PERFORMANCE = "Performance";
        private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
        private const string DAMAGE_BY_DAMCAT = "Damage by Damage Category";
        private const string ANNUAL_EXC_PROB = "Annual Exceedance Probability";
        private const string LONG_TERM_RISK = "Long-term Risk";
        private const string ASSURANCE_OF_THRESHOLD = "Assurance of Threshold";

        private readonly List<string> _outcomes = new List<string>() { DAMAGE, PERFORMANCE };

        private readonly List<string> _damageReports = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_DAMCAT };

        private readonly List<string> _performanceReports = new List<string>() { ANNUAL_EXC_PROB, LONG_TERM_RISK, ASSURANCE_OF_THRESHOLD };
        private readonly metrics.Results _IASResult;
        private string _selectedOutcome;
        private string _selectedReport;
        private ThresholdComboItem _selectedThreshold;

        private List<string> _reports = new List<string>();
        private int _selectedReportIndex = 0;
        private bool _thresholdComboVisible;

        private DamageWithUncertaintyVM _damageWithUncertaintyVM;
        private DamageByDamageCategoryVM _damageByDamageCategoryVM;
        private PerformanceVMBase _performanceAEPVM;
        private PerformanceVMBase _performanceAssuranceOfThresholdVM;
        private PerformanceVMBase _performanceLongTermRiskVM;

        private BaseViewModel _currentResultVM;

        #region Properties
        public string IASName { get; set; }
        public List<string> Outcomes { get; set; }
        public int SelectedReportIndex
        {
            get { return _selectedReportIndex; }
            set { _selectedReportIndex = value; NotifyPropertyChanged(); }
        }
        public string SelectedOutcome
        {
            get { return _selectedOutcome; }
            set { _selectedOutcome = value; SelectedOutcomeChanged(); }
        }

        public List<string> Reports
        {
            get { return _reports; }
            set { _reports = value; NotifyPropertyChanged(); }
        }

        public List<ThresholdComboItem> Thresholds { get; } = new List<ThresholdComboItem>();
        

        public ThresholdComboItem SelectedThreshold
        {
            get { return _selectedThreshold; }
            set { _selectedThreshold = value; NotifyPropertyChanged(); ThresholdChanged(); }
        }

        public string SelectedReport
        {
            get { return _selectedReport; }
            set { _selectedReport = value; NotifyPropertyChanged(); ReportChanged(); }
        }

        public BaseViewModel CurrentResultVM
        {
            get { return _currentResultVM; }
            set { _currentResultVM = value; NotifyPropertyChanged(); }
        }

        public bool ThresholdComboVisible
        {
            get { return _thresholdComboVisible; }
            set { _thresholdComboVisible = value; NotifyPropertyChanged(); }
        }

        #endregion

        public SpecificIASResultVM(string iasName, List<ThresholdRowItem> thresholds, metrics.Results iasResult)
        {
            _IASResult = iasResult;
            LoadThresholdData(iasResult);
            loadVMs();
            CurrentResultVM = _damageWithUncertaintyVM;

            IASName = iasName;
            Outcomes = _outcomes;
            SelectedOutcome = DAMAGE;

            Reports = _damageReports;
            SelectedReport = DAMAGE_WITH_UNCERTAINTY;

        }

        private void LoadThresholdData(metrics.Results iasResult)
        {
            Dictionary<int, Threshold> thresholdsDictionary = iasResult.PerformanceByThresholds.ThresholdsDictionary;
            foreach(KeyValuePair<int, Threshold> threshold in thresholdsDictionary)
            {
                 ThresholdRowItem row = new ThresholdRowItem(threshold.Key, threshold.Value.ThresholdType, threshold.Value.ThresholdValue);
                Thresholds.Add(new ThresholdComboItem(row.GetMetric()));
            }

            if(Thresholds.Count > 0)
            {
                SelectedThreshold = Thresholds.First();
            }
        }


        private void loadVMs()
        {
            //todo: do i pass the results into all of these?
            _damageWithUncertaintyVM = new DamageWithUncertaintyVM(_IASResult);
            _damageByDamageCategoryVM = new DamageByDamageCategoryVM(_IASResult);
            _performanceAEPVM = new PerformanceAEPVM(_IASResult, Thresholds);
            _performanceAEPVM.updateSelectedMetric(SelectedThreshold);
            _performanceAssuranceOfThresholdVM = new PerformanceAssuranceOfThresholdVM(_IASResult, Thresholds);
            _performanceAssuranceOfThresholdVM.updateSelectedMetric(SelectedThreshold);
            _performanceLongTermRiskVM = new PerformanceLongTermRiskVM(_IASResult, Thresholds);
            _performanceLongTermRiskVM.updateSelectedMetric(SelectedThreshold);
        }

        private void ThresholdChanged()
        {
            if (_currentResultVM is PerformanceVMBase currentResult)
            {
                currentResult.updateSelectedMetric(SelectedThreshold);
            }
        }

        /// <summary>
        /// This method swaps out the viewmodel for the content control in the view.
        /// </summary>
        private void ReportChanged()
        {
            switch(SelectedReport)
            {
                case DAMAGE_WITH_UNCERTAINTY:
                    {
                        CurrentResultVM = _damageWithUncertaintyVM;
                        break;
                    }
                case DAMAGE_BY_DAMCAT:
                    {
                        CurrentResultVM = _damageByDamageCategoryVM;
                        break;
                    }
                case ANNUAL_EXC_PROB:
                    {
                        CurrentResultVM = _performanceAEPVM;
                        break;
                    }
                case LONG_TERM_RISK:
                    {
                        CurrentResultVM = _performanceLongTermRiskVM;
                        break;
                    }
                case ASSURANCE_OF_THRESHOLD:
                    {
                        CurrentResultVM = _performanceAssuranceOfThresholdVM;
                        break;
                    }
            }

        }

        private void SelectedOutcomeChanged()
        {
            switch(_selectedOutcome)
            {
                case DAMAGE:
                    {
                        ThresholdComboVisible = false;
                        Reports = _damageReports;
                        SelectedReport = Reports.First();
                        break;
                    }
                case PERFORMANCE:
                    {
                        ThresholdComboVisible = true;
                        Reports = _performanceReports;
                        SelectedReport = Reports.First();
                        break;
                    }
            }

        }

    }
}
