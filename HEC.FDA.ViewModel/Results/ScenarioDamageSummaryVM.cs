using HEC.FDA.ViewModel.ImpactAreaScenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioDamageSummaryVM : BaseViewModel
    {
        public List<ScenarioDamageRowItem> Rows { get; } = new List<ScenarioDamageRowItem>();
        public ScenarioDamageSummaryVM(List<IASElement> scenarioElems)
        {
            foreach(IASElement element in scenarioElems)
            {
                Rows.Add(new ScenarioDamageRowItem(element));
            }
        }

    }
}
