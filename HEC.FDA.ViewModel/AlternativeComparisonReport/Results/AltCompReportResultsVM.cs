using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class AltCompReportResultsVM : BaseViewModel
    {
        private SpecificAltCompReportResultsVM _SelectedResult;
        public List<SpecificAltCompReportResultsVM> ResultVMs { get; }
        public SpecificAltCompReportResultsVM SelectedResult
        {
            get { return _SelectedResult; }
            set { _SelectedResult = value; NotifyPropertyChanged(); }
        }

        public AltCompReportResultsVM( List<SpecificAltCompReportResultsVM> resultVMs)
        {
            ResultVMs = resultVMs;
            if(resultVMs.Count > 0)
            {
                SelectedResult = resultVMs[0];
            }

        }

    }
}
