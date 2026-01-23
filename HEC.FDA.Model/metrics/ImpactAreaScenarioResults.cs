using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics
{
    public class ImpactAreaScenarioResults
    {
        #region Properties 
        public PerformanceByThresholds PerformanceByThresholds { get; set; } //exposed publicly for testing
        public StudyAreaConsequencesBinned ConsequenceResults { get; }
        public int ImpactAreaID { get; }
        public bool IsNull { get; }
        public List<ConsequenceFrequencyCurve> ConsequenceFrequencyFunctions { get; set; } = [];
        #endregion
        #region Constructors 
        public ImpactAreaScenarioResults(int impactAreaID, bool isNull)
        {
            PerformanceByThresholds = new PerformanceByThresholds(true);
            ConsequenceResults = new StudyAreaConsequencesBinned(impactAreaID);
            ImpactAreaID = impactAreaID;
            IsNull = isNull;
        }
        public ImpactAreaScenarioResults(int impactAreaID)
        {
            PerformanceByThresholds = new PerformanceByThresholds();
            ConsequenceResults = new StudyAreaConsequencesBinned(false);
            ImpactAreaID = impactAreaID;
            IsNull = false;
        }
        private ImpactAreaScenarioResults(PerformanceByThresholds performanceByThresholds, StudyAreaConsequencesBinned expectedAnnualDamageResults, int impactAreaID)
        {
            PerformanceByThresholds = performanceByThresholds;
            ConsequenceResults = expectedAnnualDamageResults;
            ImpactAreaID = impactAreaID;
            IsNull = false;
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
        public double AEPWithGivenAssurance(int thresholdID, double assurance)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.AEPWithGivenAssurance(assurance);
        }
        public double AssuranceOfAEP(int thresholdID, double exceedanceProbability)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.AssuranceOfAEP(exceedanceProbability);
        }
        public DynamicHistogram GetAEPHistogramForPlotting(int thresholdID)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.GetAEPHistogram();
        }
        public double LongTermExceedanceProbability(int thresholdID, int years)
        {
            return PerformanceByThresholds.GetThreshold(thresholdID).SystemPerformanceResults.LongTermExceedanceProbability(years);
        }
        public double AssuranceOfEvent(int thresholdID, double standardNonExceedanceProbability)
        {
            Threshold thresh = PerformanceByThresholds.GetThreshold(thresholdID);
            return thresh.SystemPerformanceResults.AssuranceOfEvent(standardNonExceedanceProbability, thresh.ThresholdValue);
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
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of consequences
        public double MeanExpectedAnnualConsequences(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Total)
        {
            return ConsequenceResults.SampleMeanDamage(damageCategory, assetCategory, impactAreaID, consequenceType, riskType);
        }

        public IHistogram GetSpecificHistogram(int impactAreaID, string damageCategory, string assetCategory)
        {
            return ConsequenceResults.GetSpecificHistogram(damageCategory, assetCategory, impactAreaID);
        }
        public bool ResultsAreConverged(double upperConfidenceLimitProb, double lowerConfidenceLimitProb, bool checkConsequenceResults)
        {
            bool consequenceConverged = true;
            if (checkConsequenceResults == true)
                consequenceConverged = ConsequenceResultsAreConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            bool performanceConverged = PerformanceResultsAreConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            return consequenceConverged && performanceConverged;
        }

        /// <summary>
        /// Determines whether all system performance results across thresholds have converged based on the specified
        /// upper and lower confidence limit probabilities.
        /// </summary>
        private bool PerformanceResultsAreConverged(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                bool thresholdAssuranceIsConverged = threshold.SystemPerformanceResults.AssuranceTestForConvergence(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                if (!thresholdAssuranceIsConverged)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks to see if all consequence results are converged based on the provided confidence limit probabilities. Zero valued histograms are skipped.
        /// </summary>
        private bool ConsequenceResultsAreConverged(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResults.ConsequenceResultList)
            {
                if (consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                {
                    continue;
                }
                if (consequenceDistributionResult.ConsequenceHistogram.IsHistogramConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public long RemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb, bool computeWithDamage)
        {
            List<long> eadIterationsRemaining = new();
            if (computeWithDamage == true)
            {
                foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResults.ConsequenceResultList)
                {
                    if (consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
                    {
                        eadIterationsRemaining.Add(0);
                    }
                    else
                    {
                        long itsRemaining = consequenceDistributionResult.ConsequenceHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                        eadIterationsRemaining.Add(itsRemaining);
                    }
                }
            }
            else
            {
                eadIterationsRemaining.Add(0);
            }

            List<long> performanceIterationsRemaining = new();
            foreach (var threshold in PerformanceByThresholds.ListOfThresholds)
            {
                long itsRemaining = threshold.SystemPerformanceResults.AssuranceRemainingIterations(upperConfidenceLimitProb, lowerConfidenceLimitProb);
                performanceIterationsRemaining.Add(itsRemaining);
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
            XElement masterElement = new("Results");
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
            StudyAreaConsequencesBinned expectedAnnualDamageResults = StudyAreaConsequencesBinned.ReadFromXML(xElement.Element("Expected_Annual_Damage_Results"));
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ImpactAreaScenarioResults(performanceByThresholds, expectedAnnualDamageResults, impactAreaID);
        }

        // this method is called to add a consequences result with zero damages to an impact area scenario which has no damages but has a levee
        // still want to compute performance statistics on the levee, but want to show the user that there are also zero damages
        // previously, the damages zero damages would not be reported to the user
        public void AddZeroConsequencesResult(int impactAreaId)
        {
            var zeroResult = new AggregatedConsequencesBinned(impactAreaId);
            ConsequenceResults.ConsequenceResultList.Add(zeroResult);
        }
        #endregion
    }
}