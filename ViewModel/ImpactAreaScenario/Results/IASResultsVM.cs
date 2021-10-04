using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class IASResultsVM : BaseViewModel
    {

        private ObservableCollection<string> _damageReports = new ObservableCollection<string>() {"Damage with Uncertainty",
                "Damage by Damage Category" };

        private ObservableCollection<string> _performanceReports = new ObservableCollection<string>() {"Annual Exceedance Probability",
                "Long-term Risk",
                "Assurance of Threshold"};

        private string _selectedOutcome;
        private string _selectedReport;
        private ObservableCollection<string> _reports = new ObservableCollection<string>();
        private int _selectedReportIndex = 0;

        private DamageWithUncertaintyVM _damageWithUncertaintyVM;
        private DamageByDamageCategoryVM _damageByDamageCategoryVM;
        private PerformanceAEPVM _performanceAEPVM;
        private PerformanceAssuranceOfThresholdVM _performanceAssuranceOfThresholdVM;
        private PerformanceLongTermRiskVM _performanceLongTermRiskVM;

        private BaseViewModel _currentResultVM;
        private int _counter;
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

        public ObservableCollection<string> Reports
        {
            get { return _reports; }
            set { _reports = value; NotifyPropertyChanged(); }
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

        public int Counter
        {
            get { return _counter; }
            set { _counter = value; NotifyPropertyChanged(); }
        }
       
        public IASResultsVM()
        {
            loadVMs();
            CurrentResultVM = _damageWithUncertaintyVM;

            IASName = "test name";
            Outcomes = new List<string>() { "Damage", "Performance" };
            SelectedOutcome = "Damage";

            Reports = new ObservableCollection<string>();
            foreach (string name in _damageReports)
            {
                Reports.Add(name);
            }
            SelectedReport = _damageReports[0];

        }

        

        private void loadVMs()
        {
            _damageWithUncertaintyVM = new DamageWithUncertaintyVM();
            _damageByDamageCategoryVM = new DamageByDamageCategoryVM();
            _performanceAEPVM = new PerformanceAEPVM();
            _performanceAssuranceOfThresholdVM = new PerformanceAssuranceOfThresholdVM();
            _performanceLongTermRiskVM = new PerformanceLongTermRiskVM();

        }

        public void ButtonNextClick()
        {
            CurrentResultVM = _performanceAEPVM;
            Counter += 1;
        }

        private void ReportChanged()
        {
            switch (SelectedReport)
            {
                case "Damage with Uncertainty":
                    {
                        CurrentResultVM = _damageWithUncertaintyVM;
                        break;
                    }
                case "Damage by Damage Category":
                    {
                        CurrentResultVM = _damageByDamageCategoryVM;
                        break;
                    }
                case "Annual Exceedance Probability":
                    {
                        CurrentResultVM = _performanceAEPVM;
                        break;
                    }
                case "Long-term Risk":
                    {
                        CurrentResultVM = _performanceLongTermRiskVM;
                        break;
                    }
                case "Assurance of Threshold":
                    {
                        CurrentResultVM = _performanceAssuranceOfThresholdVM;
                        break;
                    }
            }

        }


        private void SelectedOutcomeChanged()
        {
            if(_selectedOutcome.Equals("Damage"))
            {
                //Reports = _damageReports;
                Reports.Clear();
                foreach (string name in _damageReports)
                {
                    Reports.Add(name);
                }
                SelectedReport = Reports.First();
            }
            else if(_selectedOutcome.Equals("Performance"))
            {
                //Reports = _performanceReports;
                Reports.Clear();
                foreach (string name in _performanceReports)
                {
                    Reports.Add(name);
                }
                SelectedReport = Reports.First();
            }
        }


        private void loadDummyData()
        {
            
        }

    }
}
