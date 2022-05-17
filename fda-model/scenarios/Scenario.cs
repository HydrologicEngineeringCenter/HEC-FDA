using System;
using System.Collections.Generic;
using impactarea;
using metrics;
using System.Xml.Linq;
using compute;
using Statistics;
namespace scenarios
{
    public class Scenario
    {
        private Int64 _year;
        private IList<ImpactAreaScenarioSimulation> _impactAreaSimulations;
        //probably need getters and setters
        public IList<ImpactAreaScenarioSimulation> ImpactAreaSimulations
        {
            get
            {
                return _impactAreaSimulations;
            }
        }
        public Int64 Year{
            get{return _year;}
        }
        public IList<ImpactAreaScenarioSimulation> ImpactAreas
        {
            get { return _impactAreaSimulations;  }
        }
        public Scenario(Int64 year, IList<ImpactAreaScenarioSimulation> impactAreaSimulations){
            _year = year;
            _impactAreaSimulations = impactAreaSimulations;
        }
        public ScenarioResults Compute(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, bool computeDefaultThreshold = true, bool giveMeADamageFrequency = false)
        {
            //probably instantiate a rng to seed each impact area differently
            ScenarioResults scenarioResults = new ScenarioResults();
            foreach(ImpactAreaScenarioSimulation impactArea in _impactAreaSimulations){
                scenarioResults.AddResults(impactArea.Compute(randomProvider, convergenceCriteria));
            }
            return scenarioResults;
        }

        //public XElement WriteToXML()
        //{
        //    XElement masterElement = new XElement("Scenario");
        //    foreach (ImpactAreaScenarioSimulation impactAreaSimulation in ImpactAreaSimulations)
        //    {
        //        XElement impactAreaSimulationElement = impactAreaSimulation.write
        //    }
        //}
    }
}