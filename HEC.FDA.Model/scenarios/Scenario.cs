using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using Statistics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace HEC.FDA.Model.scenarios
{
    public class Scenario : IReportMessage
    {
        private readonly IList<ImpactAreaScenarioSimulation> _impactAreaSimulations;
        public event MessageReportedEventHandler MessageReport;

        internal Scenario()
        {
            _impactAreaSimulations = [];
        }
        public Scenario(IList<ImpactAreaScenarioSimulation> impactAreaSimulations)
        {
            _impactAreaSimulations = impactAreaSimulations;
        }

        #region Methods
        public ScenarioResults Compute(ConvergenceCriteria convergenceCriteria, bool computeIsDeterministic = false)
        {
            return Compute(convergenceCriteria, new CancellationToken(), computeIsDeterministic);
        }
        public ScenarioResults Compute(ConvergenceCriteria convergenceCriteria, CancellationToken cancellationToken, bool computeIsDeterministic)
        {
            //probably instantiate a rng to seed each impact area differently
            ScenarioResults scenarioResults = new();
            foreach (ImpactAreaScenarioSimulation impactArea in _impactAreaSimulations)
            {
                ImpactAreaScenarioResults res = impactArea.Compute(convergenceCriteria, cancellationToken, computeIsDeterministic);
                scenarioResults.AddResults(res);
            }
            scenarioResults.ComputeDate = DateTime.Now.ToString("G");
            scenarioResults.SoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return scenarioResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        #endregion
    }
}