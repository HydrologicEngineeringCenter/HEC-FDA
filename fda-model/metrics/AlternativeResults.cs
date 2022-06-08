using System;
using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using Statistics;
using Statistics.Histograms;

namespace metrics
{
    public class AlternativeResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private int _alternativeID;
        private ConsequenceResults _consequenceResults;
        private bool _isNull;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public ConsequenceResults ConsequenceResults
        {
            get
            {
                return _consequenceResults;
            }
        }
        public int BaseYear { get { return BaseYearScenarioResults.AnalysisYear; } }
        public int FutureYear { get { return FutureYearScenarioResults.AnalysisYear; } }
        public event MessageReportedEventHandler MessageReport;
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        internal ScenarioResults BaseYearScenarioResults { get; set; }
        internal ScenarioResults FutureYearScenarioResults { get; set; }
        #endregion

        #region Constructor
        public AlternativeResults()
        {
            _isNull = true;
            _alternativeID = 0;
            _consequenceResults = new ConsequenceResults();
        }
        public AlternativeResults(int id)
        {
            _alternativeID = id;
            _consequenceResults = new ConsequenceResults();
            _isNull = false;
        }
        private AlternativeResults(int id, ConsequenceResults consequenceResults)
        {
            _alternativeID = id;
            _consequenceResults = consequenceResults;
            _isNull = false;
        }
        #endregion
        #region Methods
        public List<string> GetAssetCategories()
        {
            List<string> assetCats = new List<string>();
            foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in BaseYearScenarioResults.ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (!assetCats.Contains(consequenceResult.AssetCategory))
                    {
                        assetCats.Add(consequenceResult.AssetCategory);
                    }
                }

            }
            foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in FutureYearScenarioResults.ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (!assetCats.Contains(consequenceResult.AssetCategory))
                    {
                        assetCats.Add(consequenceResult.AssetCategory);
                    }
                }

            }
            return assetCats;
        }
        public List<string> GetDamageCategories()
        {
            List<string> damageCats = new List<string>();
            foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in BaseYearScenarioResults.ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (!damageCats.Contains(consequenceResult.AssetCategory))
                    {
                        damageCats.Add(consequenceResult.AssetCategory);
                    }
                }

            }
            foreach (IContainImpactAreaScenarioResults containImpactAreaScenarioResults in FutureYearScenarioResults.ResultsList)
            {
                foreach (ConsequenceResult consequenceResult in containImpactAreaScenarioResults.ConsequenceResults.ConsequenceResultList)
                {
                    if (!damageCats.Contains(consequenceResult.AssetCategory))
                    {
                        damageCats.Add(consequenceResult.AssetCategory);
                    }
                }

            }
            return damageCats;
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
        public double MeanConsequence(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _consequenceResults.MeanDamage(damageCategory, assetCategory, impactAreaID);
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
        /// <returns></returns>the level of consequences exceeded by the specified probability 
        public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _consequenceResults.ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetConsequencesHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetConsequencesHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _consequenceResults.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID);
        }

        //TODO what role will these play
        internal void AddConsequenceResults(int impactAreaID, string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria)
        {
            ConsequenceResult consequenceResult = ConsequenceResults.GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (consequenceResult.IsNull)
            {
                ConsequenceResult newConsequenceResult = new ConsequenceResult(damageCategory,assetCategory,convergenceCriteria ,impactAreaID);
                _consequenceResults.ConsequenceResultList.Add(newConsequenceResult);
            }
        }
        internal void AddConsequenceResults(ConsequenceResult consequenceResultToAdd)
        {
            ConsequenceResult consequenceResults = ConsequenceResults.GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResults.IsNull)
            {
                _consequenceResults.ConsequenceResultList.Add(consequenceResultToAdd);
            }
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public XElement WriteToXML()
        {
            XElement mainElement = new XElement("AlternativeResults");
            XElement consequencesEvent = ConsequenceResults.WriteToXML();
            consequencesEvent.Name = "Consequences";
            mainElement.Add(consequencesEvent);
            mainElement.SetAttributeValue("ID", _alternativeID);
            return mainElement;
        }
        public static AlternativeResults ReadFromXML(XElement xElement)
        {
            int alternativeID = Convert.ToInt32(xElement.Attribute("ID").Value);
            ConsequenceResults consequenceResults = ConsequenceResults.ReadFromXML(xElement.Element("Consequences"));
            AlternativeResults alternativeResults = new AlternativeResults(alternativeID, consequenceResults);
            return alternativeResults;
        }
        #endregion

    }
}
