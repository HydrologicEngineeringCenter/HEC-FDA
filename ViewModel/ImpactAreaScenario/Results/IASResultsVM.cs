using System.Collections.Generic;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class IASResultsVM : BaseViewModel
    {

        private SpecificIASResultVM _selectedResult;

        public List<SpecificIASResultVM> Results { get; } = new List<SpecificIASResultVM>();
        public SpecificIASResultVM SelectedResult
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }


        public IASResultsVM(List<SpecificIASResultVM> results)
        {
            Results.AddRange( results);
            SelectedResult = results[0];
        }


    }
}
