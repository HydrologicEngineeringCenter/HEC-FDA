using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class ScenarioRowItem
    {
        public IASElement Element { get; set; }
        public string Name => Element?.Name;

        public ScenarioRowItem(IASElement element = null)
        {
            Element = element;
        }
    }
}
