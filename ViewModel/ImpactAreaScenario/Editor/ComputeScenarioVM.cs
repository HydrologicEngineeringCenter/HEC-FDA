using compute;
using HEC.MVVMFramework.Base.Implementations;
using metrics;
using scenarios;
using Statistics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ComputeScenarioVM:BaseViewModel
    {
        private int _Progress;
        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; NotifyPropertyChanged(); }
        }
        public ComputeScenarioVM(int analysisYear, List<SpecificIAS> iasElems, Action<ScenarioResults> callback)
        {
            List<ImpactAreaScenarioSimulation> sims = new List<ImpactAreaScenarioSimulation>();

            foreach (SpecificIAS ias in iasElems)

            {
                ImpactAreaScenarioSimulation sim = ias.CreateSimulation();
                MessageHub.Register(sim);
                sim.ProgressReport += Sim_ProgressReport;
                sims.Add(sim);
            }
            Scenario scenario = new Scenario(analysisYear, sims);
            int seed = 999;
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria();

            Task.Run(() =>
            {
                ScenarioResults scenarioResults = scenario.Compute(randomProvider, cc);
                //Event for when everything has been computed.
                callback?.Invoke(scenarioResults);
            });
        }

        private void Sim_ProgressReport(object sender, MVVMFramework.Base.Events.ProgressReportEventArgs progress)
        {
            Progress = progress.Progress;
        }
    }
}
