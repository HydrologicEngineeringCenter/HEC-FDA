using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class Results: IContainResults
    {
        public Thresholds Thresholds { get; set; }
        public ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; set; }

        public Results()
        {
            Thresholds = new Thresholds();
            ExpectedAnnualDamageResults = new ExpectedAnnualDamageResults();
        }


    }
}