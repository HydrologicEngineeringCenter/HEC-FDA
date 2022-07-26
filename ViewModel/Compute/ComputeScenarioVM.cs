using compute;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.ViewModel.Implementations;
using metrics;
using scenarios;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeScenarioVM:ComputeBase
    {
        // private string _SimName;
        private int _TotalSims;
        private int _IterationsCompleted = 0;
        private Dictionary<int, string> _ImpactAreaIdToName = new Dictionary<int, string>();








        //public string SimName
        //{
        //    get { return _SimName; }
        //    set { _SimName = value; NotifyPropertyChanged(); }
        //}

        public ComputeScenarioVM(int analysisYear, List<SpecificIAS> iasElems, Action<ScenarioResults> callback):base()
        {
            ProgressLabel = StringConstants.SCENARIO_PROGRESS_LABEL;
            _TotalSims = iasElems.Count;
            NumberCompleted = _IterationsCompleted + "/" + _TotalSims;

            List<ImpactAreaScenarioSimulation> sims = new List<ImpactAreaScenarioSimulation>();

            LoadImpactAreaNames(iasElems);

            foreach (SpecificIAS ias in iasElems)
            {
                ImpactAreaScenarioSimulation sim = ias.CreateSimulation();             
                MessageHub.Register(sim);
                sim.ProgressReport += Sim_ProgressReport;
                sims.Add(sim);

                MessageVM.InstanceHash.Add( sim.GetHashCode());
            }

            Scenario scenario = new Scenario(analysisYear, sims);

            int seed = 1234;
            RandomProvider randomProvider = new RandomProvider(seed);
            ConvergenceCriteria cc = new ConvergenceCriteria();

            Task.Run(() =>
            {
                ScenarioResults scenarioResults = scenario.Compute(randomProvider, cc);                    

                foreach(ImpactAreaScenarioSimulation sim in sims)
                {
                    MessageHub.Unregister(sim);
                }
                //Event for when everything has been computed.
                callback?.Invoke(scenarioResults);
            });
        }

        private void LoadImpactAreaNames(List<SpecificIAS> iasElems)
        {
            foreach (SpecificIAS ias in iasElems)
            {
                string name = GetImpactAreaFromID(ias.ImpactAreaID);
                _ImpactAreaIdToName.Add(ias.ImpactAreaID, name);
            }
        }

        private string GetImpactAreaFromID(int id)
        {
            string impactName = null;
            List<ImpactAreaElement> impactAreaElems = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElems.Count > 0)
            {
                //there only ever be one or zero
                ObservableCollection<ImpactAreaRowItem> impactAreaRows = impactAreaElems[0].ImpactAreaRows;
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    if (row.ID == id)
                    {
                        impactName = row.Name;
                        break;
                    }
                }
            }
            return impactName;
        }

        private void UpdateTotalCompleted()
        {
            _IterationsCompleted++;
            NumberCompleted = _IterationsCompleted + "/" + _TotalSims;
        }

        private void Sim_ProgressReport(object sender, MVVMFramework.Base.Events.ProgressReportEventArgs progress)
        {
            if(sender is ImpactAreaScenarioSimulation sim)
            {
                int impactAreaID = sim.ImpactAreaID;
                if(_ImpactAreaIdToName.ContainsKey(impactAreaID))
                {
                    ProgressLabel =  StringConstants.SCENARIO_PROGRESS_LABEL + " " + _ImpactAreaIdToName[impactAreaID];
                }
            }
            Progress = progress.Progress;
            if(Progress == 100)
            {
                UpdateTotalCompleted();
            }
        }
    }
}
