namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class AlternativeResult
    {
        public EADResult EADResult{get;}
        public EqadResult AAEQResult { get;  }
        public string Name { get;  }

        public AlternativeResult(string name, EADResult eadResult, EqadResult aaeqResult)
        {
            Name = name;
            EADResult = eadResult;
            AAEQResult = aaeqResult;
        }

    }
}
