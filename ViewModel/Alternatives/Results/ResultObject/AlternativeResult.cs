namespace HEC.FDA.ViewModel.Alternatives.Results.ResultObject
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
