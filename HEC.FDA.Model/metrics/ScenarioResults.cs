
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Distributions;
using Statistics.Histograms;

namespace HEC.FDA.Model.metrics
{
    public class ScenarioResults : ValidationErrorLogger
    {
        #region Properties 
        public string ComputeDate { get; set; }

        public List<IContainImpactAreaScenarioResults> ResultsList { get; } = new List<IContainImpactAreaScenarioResults>();

        #endregion

        #region Constructor
        internal ScenarioResults()
        {
        }

        #endregion

        #region Methods
        public List<int> GetImpactAreaIDs()
        {
            List<int> impactAreaIDs = new();
            if (ResultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in ResultsList)
                {
                    foreach (AggregatedConsequencesBinned consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                    {
                        if (!impactAreaIDs.Contains(consequenceResult.RegionID))
                        {
                            impactAreaIDs.Add(consequenceResult.RegionID);
                        }
                    }

                }
            }
            return impactAreaIDs;
        }
        public List<string> GetAssetCategories()
        {
            List<string> assetCats = new();
            if (ResultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in ResultsList)
                {
                    foreach (AggregatedConsequencesBinned consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                    {
                        if (!assetCats.Contains(consequenceResult.AssetCategory))
                        {
                            assetCats.Add(consequenceResult.AssetCategory);
                        }
                    }

                }
            }

            return assetCats;
        }
        public List<string> GetDamageCategories()
        {
            List<string> damCats = new();
            if (ResultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in ResultsList)
                {
                    foreach (AggregatedConsequencesBinned consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                    {
                        if (!damCats.Contains(consequenceResult.DamageCategory))
                        {
                            damCats.Add(consequenceResult.DamageCategory);
                        }
                    }

                }
            }

            return damCats;
        }
        public IHistogram GetAEPHistogramForPlotting(int impactAreaID, int thresholdID = 0)
        {
            return GetResults(impactAreaID).GetAEPHistogramForPlotting(thresholdID);
        }
        public double MeanAEP(int impactAreaID, int thresholdID = 0)
        {
            return GetResults(impactAreaID).MeanAEP(thresholdID);
        }
        public double MedianAEP(int impactAreaID, int thresholdID = 0)
        {
            return GetResults(impactAreaID).MedianAEP(thresholdID);
        }
        public double AssuranceOfAEP(int impactAreaID, double exceedanceProbability, int thresholdID = 0)
        {
            return GetResults(impactAreaID).AssuranceOfAEP(thresholdID, exceedanceProbability);
        }
        public double AEPWithGivenAssurance(int impactAreaID, double assurance, int thresholdID = 0)
        {
            return GetResults(impactAreaID).AEPWithGivenAssurance(thresholdID, assurance);
        }
        public double LongTermExceedanceProbability(int impactAreaID, int years, int thresholdID = 0)
        {
            return GetResults(impactAreaID).LongTermExceedanceProbability(thresholdID, years);
        }
        public double AssuranceOfEvent(int impactAreaID, double standardNonExceedanceProbability, int thresholdID = 0)
        {
            return GetResults(impactAreaID).AssuranceOfEvent(thresholdID, standardNonExceedanceProbability);
        }
        /// <summary>
        /// This method returns the mean of the consequences measure of the consequence result object for the given damage category, asset category, impact area combination 
        /// Damage measures could be EAD or other measures of consequences 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanDamage(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of consequences
        public double MeanExpectedAnnualConsequences(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {//TODO: This could probably be more efficient and could use some null checking
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                foreach (AggregatedConsequencesBinned consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.MeanExpectedAnnualConsequences();
                        }

                    }
                }
            }
            return consequenceValue;
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
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns> the level of consequences exceeded by the specified probability 
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {//efficiency and null checking 
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                foreach (AggregatedConsequencesBinned consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                }
            }
            return consequenceValue;
        }
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetConsequencesHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary> aggregated consequences histogram 
        /// <param name="damageCategory"></param> The default is null 
        /// <param name="assetCategory"></param> The default is null 
        /// <param name="impactAreaID"></param> The default is a null value (utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
        /// <returns></returns>        
        public Empirical GetConsequencesDistribution(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            List<Empirical> empiricalDistsToStack = new();

            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                foreach (AggregatedConsequencesBinned consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            empiricalDistsToStack.Add(Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram));
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return Histogram.ConvertToEmpiricalDistribution(consequenceResult.ConsequenceHistogram);
                        }
                    }
                }
            }
            if (empiricalDistsToStack.Count == 0)
            {
                string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
                ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                return new Empirical();
            }
            else
            {
                return Empirical.StackEmpiricalDistributions(empiricalDistsToStack, Empirical.Sum);
            }
        }
        public void AddResults(IContainImpactAreaScenarioResults resultsToAdd)
        { 
            ResultsList.Add(resultsToAdd);
        }
        public ImpactAreaScenarioResults GetResults(int impactAreaID)
        {
            foreach (ImpactAreaScenarioResults results in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                if (results.ImpactAreaID.Equals(impactAreaID))
                {
                    return results;
                }
            }
            int dummyImpactAreaID = 9999;
            ImpactAreaScenarioResults dummyResults = new(dummyImpactAreaID, true);
            string message = $"The IMPACT AREA SCENARIO RESULTS could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyResults;
        }

        public static StudyAreaConsequencesByQuantile ConvertToStudyAreaConsequencesByQuantile(ScenarioResults results)
        {
            List<AggregatedConsequencesByQuantile> aggregatedConsequencesByQuantiles = new();
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in results.ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                StudyAreaConsequencesByQuantile studyAreaConsequencesByQuantile = StudyAreaConsequencesBinned.ConvertToStudyAreaConsequencesByQuantile(impactAreaScenarioResults.ConsequenceResults);
                aggregatedConsequencesByQuantiles.AddRange(studyAreaConsequencesByQuantile.ConsequenceResultList);
            }
            StudyAreaConsequencesByQuantile allImpactAreas = new(aggregatedConsequencesByQuantiles);
            return allImpactAreas;

        }

        public bool Equals(ScenarioResults scenarioResultsForComparison)
        {
            bool resultsAreEqual = true;
            foreach (ImpactAreaScenarioResults scenarioResults in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                ImpactAreaScenarioResults impactAreaScenarioResultsToCompare = scenarioResultsForComparison.GetResults(scenarioResults.ImpactAreaID);
                resultsAreEqual = scenarioResults.Equals(impactAreaScenarioResultsToCompare);
                if (!resultsAreEqual)
                {
                    break;
                }
            }
            return resultsAreEqual;
        }
        public XElement WriteToXML()
        {
            XElement mainElement = new("ScenarioResults");
            mainElement.SetAttributeValue("ComputeDate", ComputeDate);
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                XElement impactAreaScenarioResultsElement = impactAreaScenarioResults.WriteToXml();
                mainElement.Add(impactAreaScenarioResultsElement);
            }
            return mainElement;
        }

        public static ScenarioResults ReadFromXML(XElement xElement)
        {
            ScenarioResults scenarioResults = new();
            
            foreach (XElement element in xElement.Elements())
            {
                IContainImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(element);
                scenarioResults.AddResults(impactAreaScenarioResults);
            }

            if(xElement.Attribute("ComputeDate") != null)
            {
                string computeDate = xElement.Attribute("ComputeDate").Value;
                scenarioResults.ComputeDate = computeDate;
            }

            return scenarioResults;
        }
        #endregion

    }
}
