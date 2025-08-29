using System.Collections.Generic;
using HEC.FDA.ViewModel.Alternatives.Results.ResultObject;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class AlternativeResultsVM : BaseViewModel
    {
        #region String Constants
        private const string DAMAGE_WITH_UNCERTAINTY = "Damage with Uncertainty";
        private const string DAMAGE_BY_IMPACT_AREA = "Damage by Impact Area";
        private const string DAMAGE_BY_DAMAGE_CATEGORY = "Damage by Damage Category";
        #endregion

        #region Backing Fields
        private IAlternativeResult _selectedResult;
        private string _SelectedReport;
        #endregion

        #region Properties
        public IAlternativeResult CurrentResultVM
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }
        public List<string> Reports { get; } = new List<string>() { DAMAGE_WITH_UNCERTAINTY, DAMAGE_BY_IMPACT_AREA, DAMAGE_BY_DAMAGE_CATEGORY };
        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; SelectedReportChanged(); NotifyPropertyChanged(); }
        }
        public AlternativeResult AlternativeResult { get; }
        #endregion

        #region Constructors
        public AlternativeResultsVM(AlternativeResult altResult)
        {
            AlternativeResult = altResult;
            //set the starting state of the combos.
            SelectedReport = DAMAGE_WITH_UNCERTAINTY;
        }
        #endregion

        private void SelectedReportChanged()
        {
            if (DAMAGE_WITH_UNCERTAINTY.Equals(SelectedReport))
            {
                CurrentResultVM = AlternativeResult.EqadResult.DamageWithUncertaintyVM;

            }
            else if (DAMAGE_BY_IMPACT_AREA.Equals(SelectedReport))
            {
                CurrentResultVM = AlternativeResult.EqadResult.DamageByImpactAreaVM;
            }
            else if (DAMAGE_BY_DAMAGE_CATEGORY.Equals(SelectedReport))
            {
                CurrentResultVM = AlternativeResult.EqadResult.DamageByDamCatVM;

            }
        }
    }
}
