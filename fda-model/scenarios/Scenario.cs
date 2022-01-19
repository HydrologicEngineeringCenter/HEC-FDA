using System;
using System.Collections.Generic;
using impactarea;
using metrics;

namespace scenarios
{
    public class Scenario
    {
        private Int64 _year;
        private IList<ImpactAreaSimulation> _impactAreas;
        //probably need getters and setters
        public IList<ImpactAreaSimulation> ImpactAreaSimulations
        {
            get
            {
                return _impactAreas;
            }
        }
        public Int64 Year{
            get{return _year;}
        }
        public IList<ImpactAreaSimulation> ImpactAreas
        {
            get { return _impactAreas;  }
        }
        public Scenario(Int64 year, IList<ImpactAreaSimulation> impactAreas){
            _year = year;
            _impactAreas = impactAreas;
        }
        public Dictionary<int, Results> Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            //probably instantiate a rng to seed each impact area differently
            Dictionary<int,Results> returnDictionary = new Dictionary<int, Results>();

            foreach(ImpactAreaSimulation impactArea in _impactAreas){
                returnDictionary.Add(impactArea.ImpactArea.ID, impactArea.Compute(rp, iterations));
            }
            return returnDictionary;
        }
    }
}