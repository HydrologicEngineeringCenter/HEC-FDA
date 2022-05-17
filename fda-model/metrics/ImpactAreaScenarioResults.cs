using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Linq;
using System.Xml.Linq;

namespace metrics
{
    public class ImpactAreaScenarioResults: IContainResults
    {
        #region Fields
        bool _isNull;
        #endregion

        #region Properties 
        public PerformanceByThresholds PerformanceByThresholds { get; set; } //exposed publicly for testing
        public ConsequenceResults ConsequenceResults { get; }
        public int ImpactAreaID { get; }
        #endregion
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        #region Constructors 
        public ImpactAreaScenarioResults()
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ConsequenceResults = new ConsequenceResults();
            ImpactAreaID = 0;
            _isNull = true;
        }
        public ImpactAreaScenarioResults(int impactAreaID)
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ConsequenceResults = new ConsequenceResults(impactAreaID);
            ImpactAreaID = impactAreaID;
            _isNull = false;
        }
        private ImpactAreaScenarioResults(PerformanceByThresholds performanceByThresholds, ConsequenceResults expectedAnnualDamageResults, int impactAreaID)
        {
            PerformanceByThresholds = performanceByThresholds;
            ConsequenceResults = expectedAnnualDamageResults;
            ImpactAreaID = impactAreaID;
            _isNull = false;
        }
        #endregion

        #region Methods
        public double MeanAEP(int thresholdID)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.MeanAEP();
        }
        public double MedianAEP(int thresholdID)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.MedianAEP();
        }
        public double AssuranceOfAEP(int thresholdID, double exceedanceProbability)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.AssuranceOfAEP(exceedanceProbability);
        }
        public double LongTermExceedanceProbability(int thresholdID, int years)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.LongTermExceedanceProbability(years);
        }
        public double AssuranceOfEvent(int thresholdID, double standardNonExceedanceProbability)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.AssuranceOfEvent(standardNonExceedanceProbability);
        }
        public double MeanExpectedAnnualConsequences(int impactAreaID, string damageCategory, string assetCategory)
        {
            return ConsequenceResults.MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID, string damageCategory, string assetCategory)
        {
            return ConsequenceResults.ConsequenceExceededWithProbabilityQ(damageCategory, exceedanceProbability, assetCategory, impactAreaID);
        }
        private bool IsEADConverged(bool computeWithDamage)
        {
            if (computeWithDamage == true)
            {   //TODO: these hard-coded strings are TROUBLE
                return ConsequenceResults.GetConsequenceResult("Total", "Total", ImpactAreaID).ConsequenceHistogram.IsConverged;
            }
            return true;
        }
        public bool IsPerformanceConverged() //exposed publicly for testing cnep convergence logic
        {
            
            List<bool> convergedList = new List<bool>();
            //dont like this
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                convergedList.Add(threshold.SystemPerformanceResults.AssuranceIsConverged());
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
        public bool IsConverged(bool computeWithDamage)
        {
            return IsEADConverged(computeWithDamage) && IsPerformanceConverged();
        }
        public bool TestResultsForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb, bool computeWithDamage)
        {
            bool eadIsConverged = true;
            if (computeWithDamage)
            {//TODO: Hard-coded strings are TROUBLE
                eadIsConverged = ConsequenceResults.GetConsequenceResult("Total", "Total", ImpactAreaID).ConsequenceHistogram.TestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            }
            bool cnepIsConverged = true;
            List<bool> convergedList = new List<bool>();

            //dont like this.
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                bool thresholdAssuranceIsConverged = threshold.SystemPerformanceResults.AssuranceTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                convergedList.Add(thresholdAssuranceIsConverged);
          
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
        public Int64 RemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb, bool computeWithDamage)
        {
            Int64 eadIterationsRemaining = 0;
            if (computeWithDamage)
            {
                eadIterationsRemaining = ConsequenceResults.GetConsequenceResult("Total", "Total", ImpactAreaID).ConsequenceHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);

            }
            List<Int64> performanceIterationsRemaining = new List<Int64>();

            //i do not like this, but the keys are frustrating 
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                performanceIterationsRemaining.Add(threshold.SystemPerformanceResults.AssuranceRemainingIterations(upperConfidenceLimitProb, lowerConfidenceLimitProb));
            }
            return Math.Max(eadIterationsRemaining, performanceIterationsRemaining.Max());
        }
        public void ParalellTestForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                threshold.SystemPerformanceResults.ParallelTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            }
        }
        public bool Equals(ImpactAreaScenarioResults incomingIContainResults)
        {
            bool performanceMatches = PerformanceByThresholds.Equals(incomingIContainResults.PerformanceByThresholds);
            bool damageResultsMatch = ConsequenceResults.Equals(incomingIContainResults.ConsequenceResults);
            if (!performanceMatches || !damageResultsMatch)
            {
                return false;
            }
            return true;
        }
        public XElement WriteToXml()
        {
            XElement masterElement = new XElement("Results");
            XElement performanceByThresholdsElement = PerformanceByThresholds.WriteToXML();
            performanceByThresholdsElement.Name = "Performance_By_Thresholds";
            masterElement.Add(performanceByThresholdsElement);
            XElement expectedAnnualDamageResultsElement = ConsequenceResults.WriteToXML();
            expectedAnnualDamageResultsElement.Name = "Expected_Annual_Damage_Results";
            masterElement.Add(expectedAnnualDamageResultsElement);
            masterElement.SetAttributeValue("ImpactAreaID", ImpactAreaID);
            return masterElement;
        }

        public static IContainResults ReadFromXML(XElement xElement)
        {
            PerformanceByThresholds performanceByThresholds = PerformanceByThresholds.ReadFromXML(xElement.Element("Performance_By_Thresholds"));
            ConsequenceResults expectedAnnualDamageResults = ConsequenceResults.ReadFromXML(xElement.Element("Expected_Annual_Damage_Results"));
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ImpactAreaScenarioResults(performanceByThresholds,expectedAnnualDamageResults,impactAreaID);
        }
        #endregion
    }
}