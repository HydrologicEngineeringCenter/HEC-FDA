using HEC.CS.Collections;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.MVVMFramework.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    //todo: i don't think i want to inherit this class, but maybe. 
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
                Scenarios.Add(elem, new Scenario(elem.AnalysisYear, sims));
                ProgressControls.Add(new ScenarioProgressVM(elem, sims));
            }

        }


        public override void Sim_ProgressReport(object sender, ProgressReportEventArgs progress)
        {
            //todo: handle the individual progress bars.
            if (sender is ImpactAreaScenarioSimulation sim)
            {
                foreach(ScenarioProgressVM scenarioProgressVM in ProgressControls)
                {
                    scenarioProgressVM.UpdateSimProgress(sim, progress.Progress);
                }

                //IASElement elem = GetElementForSim(sim);
                //if(elem != null)
                //{

                //}

                //int impactAreaID = sim.ImpactAreaID;
                //if (_ImpactAreaIdToName.ContainsKey(impactAreaID))
                //{
                //    //ProgressLabel = StringConstants.SCENARIO_PROGRESS_LABEL + " " + _ImpactAreaIdToName[impactAreaID];
                //}
                //ProgressRowItem progRow = GetRowFromElem(elem);
                //progRow.Progress = progress.Progress;
            }
            //Progress = progress.Progress;
            //if (Progress == ImpactAreaScenarioSimulation.IMPACT_AREA_SIM_COMPLETED)
            //{
            //    Progress = 100;
            //    UpdateTotalCompleted();
            //}
        }

        //private IASElement GetElementForSim(ImpactAreaScenarioSimulation sim)
        //{
        //    IASElement returnElement = null;
        //    foreach(KeyValuePair<IASElement, List<ImpactAreaScenarioSimulation>> keyValue in ElemToSims)
        //    {
        //        IASElement elem = keyValue.Key;
        //        List<ImpactAreaScenarioSimulation> sims = keyValue.Value;
        //        foreach(ImpactAreaScenarioSimulation s in sims)
        //        {
        //            if(s == sim)
        //            {
        //                returnElement = elem;
        //                break;
        //            }
        //        }
        //    }
        //    return returnElement;

        //}

    }
}
