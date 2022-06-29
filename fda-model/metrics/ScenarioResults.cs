
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;

namespace metrics
{
    public class ScenarioResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields 
        List<IContainImpactAreaScenarioResults> _resultsList;
        #endregion

        #region Properties 
        public List<IContainImpactAreaScenarioResults> ResultsList
        {
            get
            {
                return _resultsList;
            }
        }
        public int AnalysisYear { get; }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructor
        public ScenarioResults(int year)
        {
            _resultsList = new List<IContainImpactAreaScenarioResults>();
            AnalysisYear = year;
        }
        #endregion

        #region Methods
        public List<int> GetImpactAreaIDs()
        {
            List<int> impactAreaIDs = new List<int>();
            if (_resultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in _resultsList)
                {
                    foreach (ConsequenceDistributionResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
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
            List<string> assetCats = new List<string>();
            if (_resultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in _resultsList)
                {
                    foreach (ConsequenceDistributionResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
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
            List<string> damCats = new List<string>();
            if (_resultsList.Count != 0)
            {
                foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in _resultsList)
                {
                    foreach (ConsequenceDistributionResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
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
        public IHistogram AEPHistogram(int impactAreaID, int thresholdID = 0)
        {
            return GetResults(impactAreaID).GetAEPHistogram(thresholdID);
        }
        public double MeanAEP(int impactAreaID, int thresholdID=0)
        {
            return GetResults(impactAreaID).MeanAEP(thresholdID);
        }
        public double MedianAEP(int impactAreaID, int thresholdID=0)
        {
            return GetResults(impactAreaID).MedianAEP(thresholdID);
        }
        public double AssuranceOfAEP(int impactAreaID,  double exceedanceProbability, int thresholdID=0)
        {
            return GetResults(impactAreaID).AssuranceOfAEP(thresholdID, exceedanceProbability);
        }
        public double LongTermExceedanceProbability(int impactAreaID,  int years, int thresholdID = 0)
        {
            return GetResults(impactAreaID).LongTermExceedanceProbability(thresholdID, years);
        }
        public double AssuranceOfEvent(int impactAreaID, double standardNonExceedanceProbability, int thresholdID=0)
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
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of consequences
        public double MeanExpectedAnnualConsequences(int impactAreaID = -999, string damageCategory = null, string assetCategory= null)
        {//TODO: This could probably be more efficient and could use some null checking
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceDistributionResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.MeanExpectedAnnualConsequences();
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                    {
                        if(damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
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
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns> the level of consequences exceeded by the specified probability 
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {//efficiency and null checking 
            double consequenceValue = 0;
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceDistributionResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            consequenceValue += consequenceResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
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
        /// <param name="impactAreaID"></param> The default is a null value (-999)
        /// <returns></returns>        
        public IHistogram GetConsequencesHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            List<IHistogram> histograms = new List<IHistogram>();

            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
            {
                foreach (ConsequenceDistributionResult consequenceResult in impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (damageCategory == null && assetCategory == null && impactAreaID == -999)
                    {
                        histograms.Add(consequenceResult.ConsequenceHistogram);
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID == -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory == null && impactAreaID != -999)
                    {
                        if (impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID == -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory == null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory == null && assetCategory != null && impactAreaID != -999)
                    {
                        if (assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            histograms.Add(consequenceResult.ConsequenceHistogram);
                        }
                    }
                    if (damageCategory != null && assetCategory != null && impactAreaID != -999)
                    {
                        if (damageCategory.Equals(consequenceResult.DamageCategory) && assetCategory.Equals(consequenceResult.AssetCategory) && impactAreaID.Equals(consequenceResult.RegionID))
                        {
                            return consequenceResult.ConsequenceHistogram;
                        }
                    }
                }
            }
            IHistogram aggregateHistogram;
            if (histograms.Count == 0)
            {
                string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
                ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
                ReportMessage(this, new MessageEventArgs(errorMessage));
                aggregateHistogram = new Histogram();
            }
            else
            {
                aggregateHistogram = Histogram.AddHistograms(histograms);
            }
            return aggregateHistogram;
        }
        public void AddResults(IContainImpactAreaScenarioResults resultsToAdd)
        {
            ImpactAreaScenarioResults results = GetResults(resultsToAdd.ImpactAreaID);
            if (results.IsNull)
            {
                _resultsList.Add(resultsToAdd);
            }
        }
        public ImpactAreaScenarioResults GetResults(int impactAreaID)
        {
            foreach(ImpactAreaScenarioResults results in _resultsList)
            {
                if (results.ImpactAreaID.Equals(impactAreaID))
                {
                    return results;
                }
            }
            ImpactAreaScenarioResults dummyResults = new ImpactAreaScenarioResults();
            string message = $"The IMPACT AREA SCENARIO RESULTS could not be found. an arbitrary object is being returned";
            HEC.MVVMFramework.Model.Messaging.ErrorMessage errorMessage = new HEC.MVVMFramework.Model.Messaging.ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyResults;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public bool Equals(ScenarioResults scenarioResultsForComparison)
        {
            bool resultsAreEqual = true;
            foreach (ImpactAreaScenarioResults scenarioResults in _resultsList)
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
            XElement mainElement = new XElement("ScenarioResults");
            mainElement.SetAttributeValue("Year", AnalysisYear);
            foreach (ImpactAreaScenarioResults impactAreaScenarioResults in _resultsList)
            {
                XElement impactAreaScenarioResultsElement = impactAreaScenarioResults.WriteToXml();
                mainElement.Add(impactAreaScenarioResultsElement);
            }
            return mainElement;
        }

        public static ScenarioResults ReadFromXML(XElement xElement)
        {
            int year = Convert.ToInt32(xElement.Attribute("Year").Value);
            ScenarioResults scenarioResults = new ScenarioResults(year);
            foreach (XElement element in xElement.Elements())
            {
                IContainImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(element);
                scenarioResults.AddResults(impactAreaScenarioResults);
            }
            return scenarioResults;
        }
        #endregion

    }
}
