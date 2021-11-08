using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
