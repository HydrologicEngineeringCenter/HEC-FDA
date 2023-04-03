using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Alternatives.Results;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.Model.metrics;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
  public class SpecificIASResultVM : NameValidatingVM
  {
    #region String Constants
    private const string DAMAGE = "Damage";
    private const string PERFORMANCE = "Performance";
    private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
    private const string DAMAGE_BY_DAMCAT = "Damage by Damage Category";
    private const string ANNUAL_EXC_PROB = "Annual Exceedance Probability";
    private const string LONG_TERM_EXCEEDANCE_PROBABILITY = "Long-Term Exceedance Probability";
    private const string ASSURANCE_OF_THRESHOLD = "Assurance of Threshold";
    #endregion

    #region Property Backing Fields
    private readonly List<string> _damageReports = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_DAMCAT };
    private readonly List<string> _performanceReports = new List<string>() { ANNUAL_EXC_PROB, LONG_TERM_EXCEEDANCE_PROBABILITY, ASSURANCE_OF_THRESHOLD };
    private readonly ImpactAreaScenarioResults _IASResult;
    private string _selectedOutcome;
    private string _selectedReport;
    private ThresholdComboItem _selectedThreshold;
    private List<string> _reports = new List<string>();
    private int _selectedReportIndex = 0;
    private bool _thresholdComboVisible;
    private DamageWithUncertaintyVM _damageWithUncertaintyVM;
    private DamageByDamCatVM _damageByDamageCategoryVM;
    private PerformanceVMBase _performanceAEPVM;
    private PerformanceVMBase _performanceAssuranceOfThresholdVM;
    private PerformanceVMBase _performanceLongTermRiskVM;
    private BaseViewModel _currentResultVM;
    #endregion

    #region Properties
    public string IASName { get; set; }
    public List<string> Outcomes { get; set; } = new List<string>() { DAMAGE, PERFORMANCE };
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

    #region Constructors
    public SpecificIASResultVM(string iasName, int impactAreaID, ScenarioResults scenarioResults, List<string> damCats)
    {
      ImpactAreaScenarioResults results = scenarioResults.GetResults(impactAreaID);
      _IASResult = results;
      LoadThresholdData(results);

      loadVMs(damCats, scenarioResults, impactAreaID);
      CurrentResultVM = _damageWithUncertaintyVM;

      IASName = iasName;
      SelectedOutcome = DAMAGE;

      Reports = _damageReports;
      SelectedReport = DAMAGE_WITH_UNCERTAINTY;
    }
    #endregion

    #region Methods
    private void LoadThresholdData(ImpactAreaScenarioResults iasResult)
    {
      foreach (Threshold threshold in iasResult.PerformanceByThresholds.ListOfThresholds)
      {
        ThresholdRowItem row = new ThresholdRowItem(threshold.ThresholdID, threshold.ThresholdType, threshold.ThresholdValue);
        Thresholds.Add(new ThresholdComboItem(row.GetThreshold()));
      }

      if (Thresholds.Count > 0)
      {
        SelectedThreshold = Thresholds.First();
      }
    }

    private void loadVMs(List<string> damCats, ScenarioResults scenarioResults, int impactAreaID)
    {
      StudyPropertiesElement studyPropElem = StudyCache.GetStudyPropertiesElement();
      double discountRate = studyPropElem.DiscountRate;
      int period = studyPropElem.PeriodOfAnalysis;

      _damageWithUncertaintyVM = new DamageWithUncertaintyVM(scenarioResults,impactAreaID);
      _damageByDamageCategoryVM = new DamageByDamCatVM(_IASResult, damCats, discountRate, period);
      _performanceAEPVM = new PerformanceAEPVM(scenarioResults, impactAreaID, Thresholds);
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
      switch (SelectedReport)
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
        case LONG_TERM_EXCEEDANCE_PROBABILITY:
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
      switch (_selectedOutcome)
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
    #endregion
  }
}
