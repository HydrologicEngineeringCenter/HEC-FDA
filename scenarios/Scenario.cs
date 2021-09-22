using System;
using System.Collections;
using impactarea;
namespace scenarios
{
    public class Scenario
    {
        private Int64 _year;
        private IList<ImpactArea> _impactAreas;
        //probably need getters and setters
        public Int64 Year{
            get{return _year;}
        }
        public Scenario(Int64 year, IList<ImpactArea> impactAreas){
            _year = year;
            _impactAreas = impactAreas;
        }
        public IList<double> Compute(Int64 seed, Int64 iterations){
            //probably instantiate a rng to seed each impact area differently
            foreach(ImpactArea ia in _impactAreas){
                ia.Compute(seed, iterations);
            }
        }
    }
}