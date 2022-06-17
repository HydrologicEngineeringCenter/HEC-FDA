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
        private ConsequenceDistributionResults _consequenceResults;
        private bool _isNull;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public ConsequenceDistributionResults ConsequenceResults
        {
            get
            {
                return _consequenceResults;
            }
        }
        public List<int> AnalysisYears { get; }
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
            _consequenceResults = new ConsequenceDistributionResults();
        }
        public AlternativeResults(int id, List<int> analysisYears)
        {
            _alternativeID = id;
            _consequenceResults = new ConsequenceDistributionResults();
            _isNull = false;
            AnalysisYears = analysisYears;
        }
        private AlternativeResults(int id, ConsequenceDistributionResults consequenceResults, List<int> analysisYears)
        {
            _alternativeID = id;
            _consequenceResults = consequenceResults;
            _isNull = false;
            AnalysisYears = analysisYears;

        }
        #endregion
        #region Methods
        public List<string> GetAssetCategories()
        {
            List<string> assetCats = new List<string>();
            if (_consequenceResults.ConsequenceResultList.Count != 0)
            {
                foreach (ConsequenceDistributionResult consequence in _consequenceResults.ConsequenceResultList)
                {
                    if (!assetCats.Contains(consequence.AssetCategory))
                    {
                        assetCats.Add(consequence.AssetCategory);
                    }
                }
            }
            return assetCats;
        }
        public List<string> GetDamageCategories()
        {//TODO: Just get these from the _consequenceResults
            List<string> damageCats = new List<string>();
            if (_consequenceResults.ConsequenceResultList.Count != 0)
            {
                foreach (ConsequenceDistributionResult consequence in _consequenceResults.ConsequenceResultList)
                {
                    if (!damageCats.Contains(consequence.DamageCategory))
                    {
                        damageCats.Add(consequence.DamageCategory);
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
        public IHistogram GetConsequencesHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _consequenceResults.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID);
        }

        //TODO what role will these play
        internal void AddConsequenceResults(int impactAreaID, string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria)
        {
            ConsequenceDistributionResult consequenceResult = ConsequenceResults.GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (consequenceResult.IsNull)
            {
                ConsequenceDistributionResult newConsequenceResult = new ConsequenceDistributionResult(damageCategory,assetCategory,convergenceCriteria ,impactAreaID);
                _consequenceResults.ConsequenceResultList.Add(newConsequenceResult);
            }
        }
        internal void AddConsequenceResults(ConsequenceDistributionResult consequenceResultToAdd)
        {
            ConsequenceDistributionResult consequenceResults = ConsequenceResults.GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
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
            XElement yearsElement = new XElement("Years");
            foreach (int year in AnalysisYears)
            {
                yearsElement.SetAttributeValue($"Year_{year}", year);
            }
            mainElement.Add(yearsElement);
            return mainElement;
        }
        public static AlternativeResults ReadFromXML(XElement xElement)
        {
            int alternativeID = Convert.ToInt32(xElement.Attribute("ID").Value);
            ConsequenceDistributionResults consequenceResults = ConsequenceDistributionResults.ReadFromXML(xElement.Element("Consequences"));
            List<int> years = new List<int>();
            foreach (XAttribute attribute in xElement.Element("Years").Attributes())
            {
                string yearString = attribute.Value;
                int year = Convert.ToInt32(yearString);
                years.Add(year);
            }
            AlternativeResults alternativeResults = new AlternativeResults(alternativeID, consequenceResults, years);
            return alternativeResults;
        }
        #endregion

    }
}
