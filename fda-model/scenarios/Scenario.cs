using System;
using System.Collections.Generic;
using impactarea;
using metrics;

namespace scenarios
{
    public class Scenario
    {
        private Int64 _year;
        private IList<ImpactAreaSimulation> _impactAreaSimulations;
        //probably need getters and setters
        public IList<ImpactAreaSimulation> ImpactAreaSimulations
        {
            get
            {
                return _impactAreaSimulations;
            }
        }
        public Int64 Year{
            get{return _year;}
        }
        public IList<ImpactAreaSimulation> ImpactAreas
        {
            get { return _impactAreaSimulations;  }
        }
        public Scenario(Int64 year, IList<ImpactAreaSimulation> impactAreas){
            _year = year;
            _impactAreaSimulations = impactAreas;
        }
        public ScenarioResults Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            //probably instantiate a rng to seed each impact area differently
            ScenarioResults scenarioResults = new ScenarioResults();
            foreach(ImpactAreaSimulation impactArea in _impactAreaSimulations){
                scenarioResults.AddResults(impactArea.Compute(rp, iterations));
            }
            return scenarioResults;
        }
    }
}