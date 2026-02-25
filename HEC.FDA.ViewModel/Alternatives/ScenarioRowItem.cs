using HEC.FDA.ViewModel.ImpactAreaScenario;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class ScenarioRowItem
    {
        public const string NONE = "";
        public IASElement Element { get; set; }
        public string Name => Element?.Name ?? NONE;

        public ScenarioRowItem(IASElement element = null)
        {
            Element = element;
        }
    }
}
