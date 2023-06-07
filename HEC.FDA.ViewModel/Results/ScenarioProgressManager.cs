using HEC.CS.Collections;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.MVVMFramework.Base.Events;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioProgressManager: ComputeWithProgressAndMessagesBase
    {
        public Dictionary<IASElement, List<ImpactAreaScenarioSimulation>> ElemToSims = new Dictionary<IASElement, List<ImpactAreaScenarioSimulation>>();
        public CustomObservableCollection<ScenarioProgressVM> ProgressControls { get; } = new CustomObservableCollection<ScenarioProgressVM>();
        public Dictionary<IASElement, Scenario> Scenarios { get; } = new Dictionary<IASElement, Scenario>();

        public ScenarioProgressManager()
        {
        }

        public void Update(List<IASElement> elems)
        {
            ElemToSims.Clear();
            ProgressControls.Clear();
            Scenarios.Clear();

            foreach (IASElement elem in elems)
            {
                List<ImpactAreaScenarioSimulation> sims = ComputeScenarioVM.CreateSimulations(elem.SpecificIASElements);
                ElemToSims.Add(elem, sims);
                RegisterProgressAndMessages(sims);
                Scenarios.Add(elem, new Scenario( sims));
                ProgressControls.Add(new ScenarioProgressVM(elem, sims));
            }
        }


        public override void Sim_ProgressReport(object sender, ProgressReportEventArgs progress)
        {
            if (sender is ImpactAreaScenarioSimulation sim)
            {
                foreach(ScenarioProgressVM scenarioProgressVM in ProgressControls)
                {
                    scenarioProgressVM.UpdateSimProgress(sim, progress.Progress);
                }
            }
        }

    }
}
