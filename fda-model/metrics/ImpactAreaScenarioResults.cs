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
        public DamageResults DamageResults { get; }
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
            DamageResults = new DamageResults();
            ImpactAreaID = 0;
            _isNull = true;
        }
        public ImpactAreaScenarioResults(int impactAreaID)
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            DamageResults = new DamageResults(impactAreaID);
            ImpactAreaID = impactAreaID;
            _isNull = false;
        }
        private ImpactAreaScenarioResults(PerformanceByThresholds performanceByThresholds, DamageResults expectedAnnualDamageResults, int impactAreaID)
        {
            PerformanceByThresholds = performanceByThresholds;
            DamageResults = expectedAnnualDamageResults;
            ImpactAreaID = impactAreaID;
            _isNull = false;
        }
        #endregion

        #region Methods
        private bool IsEADConverged(bool computeWithDamage)
        {
            if (computeWithDamage == true)
            {   //TODO: these hard-coded strings are TROUBLE
                return DamageResults.GetDamageResult("Total", "Total", ImpactAreaID).DamageHistogram.IsConverged;
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
                eadIsConverged = DamageResults.GetDamageResult("Total", "Total", ImpactAreaID).DamageHistogram.TestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
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
                eadIterationsRemaining = DamageResults.GetDamageResult("Total", "Total", ImpactAreaID).DamageHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);

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
            bool damageResultsMatch = DamageResults.Equals(incomingIContainResults.DamageResults);
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
            XElement expectedAnnualDamageResultsElement = DamageResults.WriteToXML();
            expectedAnnualDamageResultsElement.Name = "Expected_Annual_Damage_Results";
            masterElement.Add(expectedAnnualDamageResultsElement);
            masterElement.SetAttributeValue("ImpactAreaID", ImpactAreaID);
            return masterElement;
        }

        public static IContainResults ReadFromXML(XElement xElement)
        {
            PerformanceByThresholds performanceByThresholds = PerformanceByThresholds.ReadFromXML(xElement.Element("Performance_By_Thresholds"));
            DamageResults expectedAnnualDamageResults = DamageResults.ReadFromXML(xElement.Element("Expected_Annual_Damage_Results"));
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ImpactAreaScenarioResults(performanceByThresholds,expectedAnnualDamageResults,impactAreaID);
        }
        #endregion
    }
}