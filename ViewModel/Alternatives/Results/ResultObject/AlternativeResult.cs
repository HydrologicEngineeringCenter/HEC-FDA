namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
{
    public class AlternativeResult
    {
        public EADResult EADResult{get;}
        public AAEQResult AAEQResult { get;  }
        public string Name { get;  }

        public AlternativeResult(string name, EADResult eadResult, AAEQResult aaeqResult)
        {
            Name = name;
            EADResult = eadResult;
            AAEQResult = aaeqResult;
        }

    }
}
