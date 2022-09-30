using System;
using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.compute;
using HEC.FDA.Statistics.Convergence;

namespace HEC.FDA.Model.scenarios
{
    public class Scenario : IReportMessage
    {
        #region Fields 
        private int _year;
        private IList<ImpactAreaScenarioSimulation> _impactAreaSimulations;
        //probably need getters and setters
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
        public int Year
        {
            get { return _year; }
        }
        public IList<ImpactAreaScenarioSimulation> ImpactAreas
        {
            get { return _impactAreaSimulations; }
        }
        #endregion
        #region Constructors
        internal Scenario()
        {
            _year = 0;
            _impactAreaSimulations = new List<ImpactAreaScenarioSimulation>();
        }
        public Scenario(int year, IList<ImpactAreaScenarioSimulation> impactAreaSimulations)
        {
            _year = year;
            _impactAreaSimulations = impactAreaSimulations;
        }
        #endregion
        #region Methods
        public ScenarioResults Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false)
        {
            //probably instantiate a rng to seed each impact area differently
            ScenarioResults scenarioResults = new ScenarioResults(_year);
            foreach (ImpactAreaScenarioSimulation impactArea in _impactAreaSimulations)
            {
                scenarioResults.AddResults(impactArea.Compute(randomProvider, convergenceCriteria));
            }
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
            ImpactAreaScenarioSimulation dummyScenario = new ImpactAreaScenarioSimulation(impactAreaID);
            ReportMessage(this, new MessageEventArgs(new Message("The requested scenario could not be found. An arbitrary object is being returned.")));
            return dummyScenario;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public bool Equals(Scenario scenarioToCompare)
        {
            bool yearIsSame = _year.Equals(scenarioToCompare._year);
            if (!yearIsSame)
            {
                return false;
            }
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
            XElement mainElement = new XElement("Scenario");
            foreach (ImpactAreaScenarioSimulation impactAreaScenarioSimulation in _impactAreaSimulations)
            {
                XElement iasElement = impactAreaScenarioSimulation.WriteToXML();
                mainElement.Add(iasElement);
            }
            mainElement.SetAttributeValue("Year", _year);
            return mainElement;
        }

        public static Scenario ReadFromXML(XElement xElement)
        {
            string yearString = xElement.Attribute("Year").Value;
            int year = Convert.ToInt32(yearString);
            IList<ImpactAreaScenarioSimulation> impactAreaScenarioSimulations = new List<ImpactAreaScenarioSimulation>();
            foreach (XElement element in xElement.Elements())
            {
                ImpactAreaScenarioSimulation iasFromXML = ImpactAreaScenarioSimulation.ReadFromXML(element);
                impactAreaScenarioSimulations.Add(iasFromXML);
            }
            Scenario scenario = new Scenario(year, impactAreaScenarioSimulations);
            return scenario;
        }
        #endregion
    }
}