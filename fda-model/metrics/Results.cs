using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Linq;
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
            
            List<bool> convergedList = new List<bool>();
            //dont like this
            foreach (var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                convergedList.Add(PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityIsConverged());
            }
            foreach (var convergenceResult in convergedList)
            {
                if (convergenceResult)
                {
                    //do nothing
                }
                else
                {
                    return false;
                } 
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
            bool cnepIsConverged = true;
            List<bool> convergedList = new List<bool>();

            //dont like this.
            foreach (var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                convergedList.Add(PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb));
          
            }
            foreach (var convergenceResult in convergedList)
            {
                if (convergenceResult)
                {
                    //do nothing
                }
                else
                {
                    cnepIsConverged = false;
                }
            }
            return eadIsConverged && cnepIsConverged;
        }
        public Int64 RemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            Int64 eadIterationsRemaining = ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);

            List<Int64> performanceIterationsRemaining = new List<Int64>();

            //i do not like this, but the keys are frustrating.
            foreach (var key in PerformanceByThresholds.ThresholdsDictionary)
            {
                performanceIterationsRemaining.Add(PerformanceByThresholds.ThresholdsDictionary[key.Key].ProjectPerformanceResults.ConditionalNonExceedanceProbabilityRemainingIterations(upperConfidenceLimitProb, lowerConfidenceLimitProb));
            }
            return Math.Max(eadIterationsRemaining, performanceIterationsRemaining.Max());
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