using compute;
using HEC.FDA.ViewModel.ImpactArea;
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

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ComputeScenarioVM:BaseViewModel
    {
        private string _SimName;
        private int _Progress;
        private string _NumberCompleted;
        private int _IterationsCompleted = 0;
        private int _TotalSims;

        private Dictionary<int, string> _ImpactAreaIdToName = new Dictionary<int, string>();

        private SubscriberMessageViewModel _MessageVM = new SubscriberMessageViewModel();


        public SubscriberMessageViewModel MessageVM
        {
            get { return _MessageVM; }
        }
        public string NumberCompleted
        {
            get { return _NumberCompleted; }
            set { _NumberCompleted = value; NotifyPropertyChanged(); }
        }
        public string SimName
        {
            get { return _SimName; }
            set { _SimName = value; NotifyPropertyChanged(); }
        }
        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; NotifyPropertyChanged(); }
        }
        public ComputeScenarioVM(int analysisYear, List<SpecificIAS> iasElems, Action<ScenarioResults> callback)
        {
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
                    SimName = _ImpactAreaIdToName[impactAreaID];
                }
            }
            Progress = progress.Progress;
            if(_Progress == 100)
            {
                UpdateTotalCompleted();
            }
        }
    }
}
