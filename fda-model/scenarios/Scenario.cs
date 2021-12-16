using System;
using System.Collections.Generic;
using impactarea;
namespace scenarios
{
    public class Scenario
    {
        private Int64 _year;
        private IList<ImpactAreaSimulation> _impactAreas;
        //probably need getters and setters
        public Int64 Year{
            get{return _year;}
        }
        public Scenario(Int64 year, IList<ImpactAreaSimulation> impactAreas){
            _year = year;
            _impactAreas = impactAreas;
        }
        public List<metrics.Results> Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            //probably instantiate a rng to seed each impact area differently
            System.Collections.Generic.List<metrics.Results> ret = new System.Collections.Generic.List<metrics.Results>();

            foreach(ImpactAreaSimulation ia in _impactAreas){
                ret.Add(ia.Compute(rp, iterations));
            }
            return ret;
        }
    }
}