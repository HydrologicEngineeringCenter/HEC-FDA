using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.scenarios;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeScenarioVM:ComputeWithProgressAndMessagesBase
    {

        public ComputeScenarioVM(IASElement elem, Action<IASElement, ScenarioResults> callback):base()
        {
            List<SpecificIAS> iasElems = elem.SpecificIASElements;
            ProgressLabel = StringConstants.SCENARIO_PROGRESS_LABEL;
            _TotalSims = iasElems.Count;
            NumberCompleted = _IterationsCompleted + "/" + _TotalSims;

            LoadImpactAreaNames(iasElems);

            FdaValidationResult canComputeVr = elem.CanCompute();

            if (canComputeVr.IsValid)
            {
                List<ImpactAreaScenarioSimulation> sims = CreateSimulations(iasElems);

                RegisterProgressAndMessages(sims);

                Scenario scenario = new( sims);

                CancellationTokenSource _CancellationToken = new();
                ComputeScenario(elem, scenario, callback, _CancellationToken.Token);
                //UnregisterMessages(sims);
            }
            else
            {
                canComputeVr.AddErrorMessage("Edit the scenario to resolve the issue.");
                MessageBox.Show(canComputeVr.ErrorMessage, "Cannot Compute Scenario", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        public static Task ComputeScenario(IASElement elem, Scenario scenario, Action<IASElement, ScenarioResults> callback, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                int seed = 1234;
                ConvergenceCriteria convergenceCriteria = StudyCache.GetStudyPropertiesElement().GetStudyConvergenceCriteria();              
                ScenarioResults scenarioResults = scenario.Compute(convergenceCriteria, cancellationToken, computeIsDeterministic:false);       
                //Event for when everything has been computed.
                callback?.Invoke(elem, scenarioResults);
            }, cancellationToken);
        }

        /// <summary>
        /// Assumes the iasElems have been validated and are good to go.
        /// </summary>
        /// <param name="iasElems"></param>
        public static List<ImpactAreaScenarioSimulation> CreateSimulations(List<SpecificIAS> iasElems)
        {
            List<ImpactAreaScenarioSimulation> sims = new();

            foreach (SpecificIAS ias in iasElems)
            { 
                ImpactAreaScenarioSimulation sim = ias.CreateSimulation();
                if (sim != null)
                {
                    sims.Add(sim);
                }
            }
            return sims;
        }


    }
}
