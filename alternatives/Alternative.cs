using System;
using System.Collections;
using scenarios;
namespace alternatives
{
    public class Alternative
    {
        private Scenario _currentYear;
        private Scenario _futureYear;
        //probably need getters and setters
        public Alternative(Scenario currentYear, Scenario futureYear){
            _currentYear = CurrentYear;
            _futureYear = impactAreas;
        }
        public IList<double> ComputeEEAD(Int64 seed, Int64 iterations, double discountRate){
            //probably instantiate a rng to seed each impact area differently
            IList<double> currentEAD = _currentYear.Compute(seed,iterations);
            IList<double> futureEAD = _futureYear.Compute(seed, iterations);
            //discoiunt future ead
            IList<double> discountedEAD = discount(futureEAD,discountRate, _futureYear.Year - _currentYear.Year);
            //combine the future and current ead
            return currentEAD;//not right, fix this.
        }
        private IList<double> discount(IList<double> eads, double discountRate, int64 years){
            //discount the eads
            return eads;
        }
    }
}