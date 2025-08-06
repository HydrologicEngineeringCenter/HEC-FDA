namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class AlternativeResult
    {
        public EADResult EADResult{get;}
        public EqadResult EqadResult { get;  }
        public string Name { get;  }

        public AlternativeResult(string name, EADResult eadResult, EqadResult eqadResult)
        {
            Name = name;
            EADResult = eadResult;
            EqadResult = eqadResult;
        }

    }
}
