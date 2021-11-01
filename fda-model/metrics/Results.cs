using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class Results
    {
        public Thresholds thresholds { get; set; }
        public ExpectedAnnualDamageResults expectedAnnualDamageResults { get; set; }

        public Results()
        {
            thresholds = new Thresholds();
            expectedAnnualDamageResults = new ExpectedAnnualDamageResults();
        }


    }
}