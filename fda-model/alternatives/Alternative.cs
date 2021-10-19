using System;
using System.Collections.Generic;
using scenarios;
namespace alternatives
{
    public class Alternative
    {
        private Scenario _currentYear;
        private Scenario _futureYear;
        //probably need getters and setters
        public Alternative(Scenario currentYear, Scenario futureYear){
            _currentYear = currentYear;
            _futureYear = futureYear;
        }
        public IList<metrics.IContainResults> ComputeEEAD(interfaces.IProvideRandomNumbers rp, Int64 iterations, double discountRate){
            //probably instantiate a rng to seed each impact area differently
            IList<metrics.IContainResults> currentEAD = _currentYear.Compute(rp,iterations);
            IList<metrics.IContainResults> futureEAD = _futureYear.Compute(rp, iterations);
            //discoiunt future ead
            IList<metrics.IContainResults> discountedEAD = discount(futureEAD,discountRate, _futureYear.Year - _currentYear.Year);
            //combine the future and current ead
            return currentEAD;//not right, fix this.
        }
        private IList<metrics.IContainResults> discount(IList<metrics.IContainResults> eads, double discountRate, Int64 years){
            //discount the eads
            return eads;
        }
    }
}