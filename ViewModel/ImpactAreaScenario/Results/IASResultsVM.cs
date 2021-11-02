using Model;
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

        private SpecificIASResultVM _selectedResult;

        public List<SpecificIASResultVM> Results { get; set; }
        public SpecificIASResultVM SelectedResult
        {
            get { return _selectedResult; }
            set { _selectedResult = value; NotifyPropertyChanged(); }
        }


        public IASResultsVM(List<SpecificIASResultVM> results)
        {
            Results = results;
            SelectedResult = results[0];
        }


    }
}
