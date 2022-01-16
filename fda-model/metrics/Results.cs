using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using paireddata;
namespace metrics
{
    public class Results: IContainResults
    {
        public PerformanceByThresholds PerformanceByThresholds { get; }
        public ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; }
        public Results()
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ExpectedAnnualDamageResults = new ExpectedAnnualDamageResults();
        }

    }
}