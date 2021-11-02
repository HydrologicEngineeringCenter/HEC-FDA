using Model;
using System.Collections.Generic;
using System.Linq;
using ViewModel.ImpactAreaScenario.Editor;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class SpecificIASResultVM : BaseViewModel
    {

        private readonly List<string> _outcomes = new List<string>() { "Damage", "Performance" };

        private readonly List<string> _damageReports = new List<string>() { "Damage with Uncertainty", "Damage by Damage Category" };

        private readonly List<string> _performanceReports = new List<string>() { "Annual Exceedance Probability", "Long-term Risk", "Assurance of Threshold" };

        private string _selectedOutcome;
        private string _selectedReport;
        private ThresholdComboItem _selectedThreshold;

        private List<ThresholdComboItem> _thresholds;
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

        public List<ThresholdComboItem> Thresholds
        {
            get { return _thresholds; }
            set { _thresholds = value; NotifyPropertyChanged(); }
        }

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

        //todo: once we have the actual results object, that will be passed in here
        public SpecificIASResultVM(string iasName, List<ThresholdRowItem> thresholds)
        {
            LoadThresholdData(thresholds);
            loadVMs();
            CurrentResultVM = _damageWithUncertaintyVM;

            IASName = iasName;
            Outcomes = _outcomes;
            SelectedOutcome = _outcomes[0];

            Reports = _damageReports;
            SelectedReport = _damageReports[0];

        }

        private void LoadThresholdData(List<ThresholdRowItem> thresholdRows)
        {            
            //the thresholds being passed in from the specific ias doesn't include the "default"
            //threshold. That needs to be added here.
            List<ThresholdComboItem> comboItems = new List<ThresholdComboItem>();
            //todo: After talking with Richard it sounds like this default threshold might not
            //always be a constant value (it might depend on the study?). Update this when the new
            //model and compute are finished.
            IMetric defaultMetric = IMetricFactory.Factory(IMetricEnum.ExteriorStage, 5);
            comboItems.Add(new ThresholdComboItem(defaultMetric));
            foreach(ThresholdRowItem row in thresholdRows)
            {              
                comboItems.Add(new ThresholdComboItem(row.GetMetric()));
            }

            Thresholds = comboItems;
            SelectedThreshold = comboItems[0];
        }


        private void loadVMs()
        {
            _damageWithUncertaintyVM = new DamageWithUncertaintyVM();
            _damageByDamageCategoryVM = new DamageByDamageCategoryVM();
            _performanceAEPVM = new PerformanceAEPVM(Thresholds);
            _performanceAssuranceOfThresholdVM = new PerformanceAssuranceOfThresholdVM(Thresholds);
            _performanceLongTermRiskVM = new PerformanceLongTermRiskVM(Thresholds);

        }


        private void ThresholdChanged()
        {
            if (_currentResultVM != null && _currentResultVM is PerformanceVMBase)
            {
                ((PerformanceVMBase)_currentResultVM).updateSelectedMetric(SelectedThreshold);
            }
        }

        /// <summary>
        /// This method swaps out the viewmodel for the content control in the view.
        /// </summary>
        private void ReportChanged()
        {
            string d = _damageReports[0];
            if (_damageReports[0].Equals(SelectedReport))
            {
                CurrentResultVM = _damageWithUncertaintyVM;
            }
            else if (_damageReports[1].Equals(SelectedReport))
            {
                CurrentResultVM = _damageByDamageCategoryVM;
            }
            else if (_performanceReports[0].Equals(SelectedReport))
            {
                CurrentResultVM = _performanceAEPVM;
            }
            else if (_performanceReports[1].Equals(SelectedReport))
            {
                CurrentResultVM = _performanceLongTermRiskVM;
            }
            else if (_performanceReports[2].Equals(SelectedReport))
            {
                CurrentResultVM = _performanceAssuranceOfThresholdVM;
            }
        }


        private void SelectedOutcomeChanged()
        {
            if (_selectedOutcome.Equals("Damage"))
            {
                ThresholdComboVisible = false;
                Reports = _damageReports;
                SelectedReport = Reports.First();
            }
            else if (_selectedOutcome.Equals("Performance"))
            {
                ThresholdComboVisible = true;
                Reports = _performanceReports;
                SelectedReport = Reports.First();
            }
        }



    }
}
