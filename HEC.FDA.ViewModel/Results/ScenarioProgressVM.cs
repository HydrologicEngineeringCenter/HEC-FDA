using HEC.CS.Collections;
using HEC.FDA.Model.compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioProgressVM
    {
        public CustomObservableCollection<ProgressRowItem> ProgressRows { get; } = new CustomObservableCollection<ProgressRowItem>();
        public string ProgressLabel { get; }

        public ScenarioProgressVM(IASElement elem, List<ImpactAreaScenarioSimulation> sims)
        {
            foreach (ImpactAreaScenarioSimulation sim in sims)
            {
                ProgressRows.Add(new ProgressRowItem(sim));
            }
            ProgressLabel = elem.Name;
        }

        public void UpdateSimProgress(ImpactAreaScenarioSimulation sim, int progress)
        {
            foreach (ProgressRowItem row in ProgressRows)
            {
                if (row.Simulation == sim)
                {
                    if (progress == ImpactAreaScenarioSimulation.IMPACT_AREA_SIM_COMPLETED)
                    {
                        progress = 100;
                    }
                    row.Progress = progress;
                    
                }
            }
        }


    }
}
