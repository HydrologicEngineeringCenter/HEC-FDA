using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Alternatives.Results.ResultObject
{
    public class AlternativeResult
    {

        public EADResult EADResult{get;set;}
        public AAEQResult AAEQResult { get; set; }

        public AlternativeResult(EADResult eadResult, AAEQResult aaeqResult)
        {
            EADResult = eadResult;
            AAEQResult = aaeqResult;
        }

    }
}
