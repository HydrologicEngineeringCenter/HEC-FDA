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
        private bool IsEADConverged()
        {
            return ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].IsConverged;
        }
        private bool IsPerformanceConverged()
        {
            //dont like this.
            foreach (var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                return PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityIsConverged();
            }
            return true;
        }
        public bool IsConverged()
        {
            return IsEADConverged() && IsPerformanceConverged();
        }
        public bool TestResultsForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            bool eadIsConverged = ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].TestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            bool cnepIsConverged = false;
            //dont like this.
            foreach(var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                cnepIsConverged = PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                break;
            }
            
            return eadIsConverged && cnepIsConverged;
        }
        public Int64 RemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            Int64 ead = ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            Int64 performance = 0;
            //i do not like this, but the keys are frustrating.
            foreach (var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                performance = PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityRemainingIterations(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                break;
            }
            return Math.Max(ead, performance);
        }
        public void ParalellTestForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            foreach (var keyvaluepair in PerformanceByThresholds.ThresholdsDictionary)
            {
                keyvaluepair.Value.ProjectPerformanceResults.ParallelTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            }
        }

    }
}