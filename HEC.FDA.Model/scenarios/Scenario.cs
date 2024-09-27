using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Statistics;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.compute;
using System.Threading;
using System.Reflection;

namespace HEC.FDA.Model.scenarios
{
    public class Scenario : IReportMessage
    {
        #region Fields 
        private readonly IList<ImpactAreaScenarioSimulation> _impactAreaSimulations;
        #endregion
        #region Properties 
        public IList<ImpactAreaScenarioSimulation> ImpactAreaSimulations
        {
            get
            {
                return _impactAreaSimulations;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        public IList<ImpactAreaScenarioSimulation> ImpactAreas
        {
            get { return _impactAreaSimulations; }
        }
        #endregion
        #region Constructors
        internal Scenario()
        {
            _impactAreaSimulations = new List<ImpactAreaScenarioSimulation>();
        }
        public Scenario( IList<ImpactAreaScenarioSimulation> impactAreaSimulations)
        {
            _impactAreaSimulations = impactAreaSimulations;
        }
        #endregion
        #region Methods
        public ScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            return Compute(randomProvider, convergenceCriteria, new CancellationToken());
        }
        public ScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, CancellationToken cancellationToken)
        {
            //probably instantiate a rng to seed each impact area differently
            ScenarioResults scenarioResults = new();
            foreach (ImpactAreaScenarioSimulation impactArea in _impactAreaSimulations)
            {
                scenarioResults.AddResults(impactArea.Compute(randomProvider, convergenceCriteria, cancellationToken));
            }
            scenarioResults.ComputeDate = DateTime.Now.ToString("G");
            scenarioResults.SoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return scenarioResults;
        }
        public ImpactAreaScenarioSimulation GetImpactAreaScenarioSimulation(int impactAreaID)
        {
            foreach (ImpactAreaScenarioSimulation impactAreaScenarioSimulation in _impactAreaSimulations)
            {
                if (impactAreaScenarioSimulation.ImpactAreaID.Equals(impactAreaID))
                {
                    return impactAreaScenarioSimulation;
                }
            }
            ImpactAreaScenarioSimulation dummyScenario = new(impactAreaID);
            ReportMessage(this, new MessageEventArgs(new Message("The requested scenario could not be found. An arbitrary object is being returned.")));
            return dummyScenario;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public bool Equals(Scenario scenarioToCompare)
        {
            foreach (ImpactAreaScenarioSimulation impactAreaScenarioSimulation in _impactAreaSimulations)
            {
                ImpactAreaScenarioSimulation impactAreaScenarioSimulationToCompare = scenarioToCompare.GetImpactAreaScenarioSimulation(impactAreaScenarioSimulation.ImpactAreaID);
                bool impactAreaScenariosAreTHeSame = impactAreaScenarioSimulation.Equals(impactAreaScenarioSimulationToCompare);
                if (!impactAreaScenariosAreTHeSame)
                {
                    return false;
                }
            }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement mainElement = new("Scenario");
            foreach (ImpactAreaScenarioSimulation impactAreaScenarioSimulation in _impactAreaSimulations)
            {
                XElement iasElement = impactAreaScenarioSimulation.WriteToXML();
                mainElement.Add(iasElement);
            }
            return mainElement;
        }

        public static Scenario ReadFromXML(XElement xElement)
        {
            IList<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            foreach (XElement element in xElement.Elements())
            {
                ImpactAreaScenarioSimulation iasFromXML = ImpactAreaScenarioSimulation.ReadFromXML(element);
                impactAreaScenarioSimulations.Add(iasFromXML);
            }
            Scenario scenario = new( impactAreaScenarioSimulations);
            return scenario;
        }
        #endregion
    }
}