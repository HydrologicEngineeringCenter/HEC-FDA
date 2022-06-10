using compute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ComputeScenarioVM:BaseViewModel
    {

        public ComputeScenarioVM(int year, List<SpecificIAS> iasElems, Action callback)
        {
            List<Task> tasks = new List<Task>();
            List<ImpactAreaScenarioSimulation> simulations = new List<ImpactAreaScenarioSimulation>();
            foreach (SpecificIAS ias in iasElems)
            {
                tasks.Add( ias.ComputeScenario(this, new EventArgs()));
            }

            Task.Run(() =>
            {
                Task.WaitAll(tasks.ToArray());

                //Event for when everything has been computed.
                callback?.Invoke();
            });
        }

    }
}
