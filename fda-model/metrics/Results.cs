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
        public ThreadsafeInlineHistogram[] ToList()
        {
            List<ThreadsafeInlineHistogram> histos = new List<ThreadsafeInlineHistogram>();
            foreach(ThreadsafeInlineHistogram h in ExpectedAnnualDamageResults.HistogramsOfEADs.Values)
            {
                histos.Add(h);
            }
            foreach (Threshold t in PerformanceByThresholds.ThresholdsDictionary.Values)
            {
                histos.Add(t.ProjectPerformanceResults.HistogramOfAEPs);
            }
            return histos.ToArray();
        }

    }
}