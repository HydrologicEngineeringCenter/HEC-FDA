namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class ResultRowItem:BaseViewModel
    {
        public string Name{get;}
        public SpecificIASResultVM Result { get; }
        public ResultRowItem(SpecificIASResultVM result)
        {
            Name = result.Name;
            Result = result;
        }

    }
}
