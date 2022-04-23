using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Linq;
using System.Xml.Linq;

namespace metrics
{
    public class Results: IContainResults
    {
        public PerformanceByThresholds PerformanceByThresholds { get; set; } //exposed publicly for testing
        public ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; }
        public Results()
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ExpectedAnnualDamageResults = new ExpectedAnnualDamageResults();
        }
        private Results(PerformanceByThresholds performanceByThresholds, ExpectedAnnualDamageResults expectedAnnualDamageResults)
        {
            PerformanceByThresholds = performanceByThresholds;
            ExpectedAnnualDamageResults = expectedAnnualDamageResults;
        }
        private bool IsEADConverged()
        {
            return ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].IsConverged;
        }
        public bool IsPerformanceConverged() //exposed publicly for testing cnep convergence logic
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

        public XElement WriteToXml()
        {
            XElement masterElement = new XElement("Results");
            XElement performanceByThresholdsElement = PerformanceByThresholds.WriteToXML();
            performanceByThresholdsElement.Name = "Performance_By_Thresholds";
            masterElement.Add(performanceByThresholdsElement);
            XElement expectedAnnualDamageResultsElement = ExpectedAnnualDamageResults.WriteToXML();
            expectedAnnualDamageResultsElement.Name = "Expected_Annual_Damage_Results";
            masterElement.Add(expectedAnnualDamageResultsElement);
            return masterElement;
        }

        public static IContainResults ReadFromXML(XElement xElement)
        {
            PerformanceByThresholds performanceByThresholds = PerformanceByThresholds.ReadFromXML(xElement.Element("Performance_By_Thresholds"));
            ExpectedAnnualDamageResults expectedAnnualDamageResults = ExpectedAnnualDamageResults.ReadFromXML(xElement.Element("Expected_Annual_Damage_Results"));
            return new Results(performanceByThresholds,expectedAnnualDamageResults);
        }
    }
}