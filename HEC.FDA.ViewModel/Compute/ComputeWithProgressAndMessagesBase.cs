using HEC.FDA.Model.compute;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.ViewModel.Implementations;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeWithProgressAndMessagesBase : BaseViewModel
    {
        public int _TotalSims;
        public int _IterationsCompleted = 0;
        public Dictionary<int, string> _ImpactAreaIdToName = new Dictionary<int, string>();

        private int _Progress;
        private string _NumberCompleted;
        private string _ProgressLabel;


        private SubscriberMessageViewModel _MessageVM = new SubscriberMessageViewModel();

        public SubscriberMessageViewModel MessageVM
        {
            get { return _MessageVM; }
        }

        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; NotifyPropertyChanged(); }
        }

        public string ProgressLabel
        {
            get { return _ProgressLabel; }
            set { _ProgressLabel = value; NotifyPropertyChanged(); }
        }

        public string NumberCompleted
        {
            get { return _NumberCompleted; }
            set { _NumberCompleted = value; NotifyPropertyChanged(); }
        }


        public ComputeWithProgressAndMessagesBase()
        {

        }

        //todo:  have to unregister in the callback so i know that it is done running?
        private void UnregisterMessages(List<ImpactAreaScenarioSimulation> sims)
        {
            foreach (ImpactAreaScenarioSimulation sim in sims)
            {
                MessageHub.Unregister(sim);
            }
        }

        public void LoadImpactAreaNames(List<SpecificIAS> iasElems)
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
                List<ImpactAreaRowItem> impactAreaRows = impactAreaElems[0].ImpactAreaRows;
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

        public void RegisterProgressAndMessages(List<ImpactAreaScenarioSimulation> sims)
        {
            foreach (ImpactAreaScenarioSimulation sim in sims)
            {
                sim.ProgressReport += Sim_ProgressReport;
                MessageVM.InstanceHash.Add(sim.GetHashCode());
            }
        }
        private void Sim_ProgressReport(object sender, MVVMFramework.Base.Events.ProgressReportEventArgs progress)
        {
            if (sender is ImpactAreaScenarioSimulation sim)
            {
                int impactAreaID = sim.ImpactAreaID;
                if (_ImpactAreaIdToName.ContainsKey(impactAreaID))
                {
                    ProgressLabel = StringConstants.SCENARIO_PROGRESS_LABEL + " " + _ImpactAreaIdToName[impactAreaID];
                }
            }
            Progress = progress.Progress;
            if (Progress == ImpactAreaScenarioSimulation.IMPACT_AREA_SIM_COMPLETED)
            {
                Progress = 100;
                UpdateTotalCompleted();
            }
        }

        private void UpdateTotalCompleted()
        {
            _IterationsCompleted++;
            NumberCompleted = _IterationsCompleted + "/" + _TotalSims;
        }

    }
}
