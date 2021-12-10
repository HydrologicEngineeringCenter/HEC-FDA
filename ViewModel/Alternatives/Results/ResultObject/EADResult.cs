using System.Collections.Generic;

namespace ViewModel.Alternatives.Results.ResultObject
{
    public class EADResult
    {
        public List<YearResult> YearResults { get; set; }

        public EADResult(List<YearResult> yearResults)
        {
            YearResults = yearResults;
        }

    }
}
