using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Linq;
using System.Xml.Linq;

namespace metrics
{
    public class ImpactAreaScenarioResults: IContainImpactAreaScenarioResults
    {//TODO: I want to make this class internal. We should access this logic through ScenarioResults. 
        #region Fields
        //TODO: this is not working quite like I expect. 
        //if blank results are returned from a compute, isNull is false
        bool _isNull;
        #endregion

        #region Properties 
        public PerformanceByThresholds PerformanceByThresholds { get; set; } //exposed publicly for testing
        public ConsequenceDistributionResults ConsequenceResults { get; }
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
        public ImpactAreaScenarioResults(int impactAreaID, bool isNull)
        {
            PerformanceByThresholds = new PerformanceByThresholds(true);
            ConsequenceResults = new ConsequenceDistributionResults();
            ImpactAreaID = impactAreaID;
            _isNull = isNull;
        }
        public ImpactAreaScenarioResults(int impactAreaID)
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ConsequenceResults = new ConsequenceDistributionResults(false);
            ImpactAreaID = impactAreaID;
            _isNull = false;
        }
        private ImpactAreaScenarioResults(PerformanceByThresholds performanceByThresholds, ConsequenceDistributionResults expectedAnnualDamageResults, int impactAreaID)
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
        public ThreadsafeInlineHistogram GetAEPHistogram(int thresholdID)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.GetAEPHistogram();
        }
        public double LongTermExceedanceProbability(int thresholdID, int years)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.LongTermExceedanceProbability(years);
        }
        public double AssuranceOfEvent(int thresholdID, double standardNonExceedanceProbability)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.AssuranceOfEvent(standardNonExceedanceProbability);
        }
        /// <summary>
        /// This method returns the mean of the consequences measure of the consequence result object for the given damage category, asset category, impact area combination 
        /// Damage measures could be EAD or other measures of consequences 
        /// Note that when working with impact area scenario results, there is only 1 impact area 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanDamage(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of consequences
        public double MeanExpectedAnnualConsequences(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return ConsequenceResults.MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method calls the inverse CDF of the damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  consequences is determined by the arguments used in the method
        /// For example, if you wanted the EAD exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = ConsequenceExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns> the level of consequences exceeded by the specified probability 
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return ConsequenceResults.ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetConsequencesHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary> aggregated consequences histogram 
        /// <param name="damageCategory"></param> The default is null 
        /// <param name="assetCategory"></param> The default is null 
        /// <param name="impactAreaID"></param> The default is a null value (-999)
        /// <returns></returns>
        public IHistogram GetConsequencesHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return ConsequenceResults.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID);
        }
        private bool IsEADConverged(bool computeWithDamage)
        { 
            if (computeWithDamage == true)
            {   
                foreach (ConsequenceDistributionResult consequenceDistributionResult in ConsequenceResults.ConsequenceResultList)
                {
                    if(!consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                    {
                        if(consequenceDistributionResult.ConsequenceHistogram.IsConverged == false)
                        {
                            return false;
                        }
                    }
                }
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
        public bool ResultsAreConverged(double upperConfidenceLimitProb, double lowerConfidenceLimitProb, bool computeWithDamage)
        {
            bool eadIsConverged = true;
                if (computeWithDamage == true)
                {
                    foreach (ConsequenceDistributionResult consequenceDistributionResult in ConsequenceResults.ConsequenceResultList)
                    {   
                        if(consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                        {
                            eadIsConverged = true;
                        } else
                        {
                            if (consequenceDistributionResult.ConsequenceHistogram.IsHistogramConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb) == false)
                            {
                                eadIsConverged = false;
                            }
                        }
                    }
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
            List<Int64> eadIterationsRemaining = new List<Int64>();
            if (computeWithDamage == true)
            {
                foreach (ConsequenceDistributionResult consequenceDistributionResult in ConsequenceResults.ConsequenceResultList)
                {
                    if (consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                    {
                        eadIterationsRemaining.Add(0);
                    }
                    else
                    {
                    eadIterationsRemaining.Add(consequenceDistributionResult.ConsequenceHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb));
                    }
                }
            }

            List<Int64> performanceIterationsRemaining = new List<Int64>();
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                performanceIterationsRemaining.Add(threshold.SystemPerformanceResults.AssuranceRemainingIterations(upperConfidenceLimitProb, lowerConfidenceLimitProb));
            }
            return Math.Max(eadIterationsRemaining.Max(), performanceIterationsRemaining.Max());
        }
        public void ParallelResultsAreConverged(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                threshold.SystemPerformanceResults.ParallelResultsAreConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb);
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

        public static ImpactAreaScenarioResults ReadFromXML(XElement xElement)
        {
            PerformanceByThresholds performanceByThresholds = PerformanceByThresholds.ReadFromXML(xElement.Element("Performance_By_Thresholds"));
            ConsequenceDistributionResults expectedAnnualDamageResults = ConsequenceDistributionResults.ReadFromXML(xElement.Element("Expected_Annual_Damage_Results"));
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ImpactAreaScenarioResults(performanceByThresholds,expectedAnnualDamageResults,impactAreaID);
        }

        internal void ForceDeQueue()
        {
            PerformanceByThresholds.ForceDeQueue();
            ConsequenceResults.ForceDeQueue();
         }
        #endregion
    }
}