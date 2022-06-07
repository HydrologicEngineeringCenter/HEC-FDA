using compute;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ComputeScenarioVM:BaseViewModel
    {
        //public List<SpecificIAS> SpecificIASElements { get; } = new List<SpecificIAS>();



        public ComputeScenarioVM(int year, List<SpecificIAS> iasElems, Action callback)
        {
            List<Task> tasks = new List<Task>();
            List<ImpactAreaScenarioSimulation> simulations = new List<ImpactAreaScenarioSimulation>();
            foreach (SpecificIAS ias in iasElems)
            {
                tasks.Add( ias.ComputeScenario(this, new EventArgs()));
            }

            //scenarios.Scenario scenario = new scenarios.Scenario(year, simulations);
            //scenario.Compute()

            Task.Run(() =>
            {
                Task.WaitAll(tasks.ToArray());

                //Add logic for when everything has been computed.
                callback?.Invoke();
            });
        }

    }
}
