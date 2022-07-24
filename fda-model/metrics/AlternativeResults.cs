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
    public class AlternativeResults : Validation, IReportMessage
    {
        #region Fields
        private int _alternativeID;
        private ConsequenceDistributionResults _aaeqResults;
        private bool _isNull;
        #endregion

        #region Properties
        public int AlternativeID
        {
            get { return _alternativeID; }
        }
        public ConsequenceDistributionResults AAEQDamageResults
        {
            get
            {
                return _aaeqResults;
            }
        }
        public List<int> AnalysisYears { get; }
        public int PeriodOfAnalysis { get; }
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
            _aaeqResults = new ConsequenceDistributionResults();
            BaseYearScenarioResults = new ScenarioResults();
            FutureYearScenarioResults = new ScenarioResults();
            AnalysisYears = new List<int>() { 2030, 2049 };
            PeriodOfAnalysis = 50;
            AddRules();
        }



        public AlternativeResults(int id, List<int> analysisYears, int periodOfAnalysis)
        {
            _alternativeID = id;
            PeriodOfAnalysis = periodOfAnalysis;
            _aaeqResults = new ConsequenceDistributionResults(false);
            _isNull = false;
            AnalysisYears = analysisYears;
            AddRules();
        }
        internal AlternativeResults(int id, List<int> analysisYears, int periodOfAnalysis, bool isNull)
        {
            _alternativeID = id;
            _aaeqResults = new ConsequenceDistributionResults();
            _isNull = isNull;
            AnalysisYears = analysisYears;
            PeriodOfAnalysis = periodOfAnalysis;
            AddRules();
        }
        private AlternativeResults(int id, ConsequenceDistributionResults consequenceResults, List<int> analysisYears, int periodOfAnalysis)
        {
            _alternativeID = id;
            _aaeqResults = consequenceResults;
            PeriodOfAnalysis = periodOfAnalysis;
            _isNull = false;
            AnalysisYears = analysisYears;
            AddRules();

        }
        #endregion
        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(AnalysisYears), new Rule(() => AnalysisYears[1] - AnalysisYears[0] >= 1, "The most likely future year must be at least 1 year greater then the base year"));
            AddSinglePropertyRule(nameof(PeriodOfAnalysis), new Rule(() => PeriodOfAnalysis >= Math.Abs(AnalysisYears[0] - AnalysisYears[1]) + 1, "The period of analysis must be greater than or equal to the difference between the analysis years, inclusive."));
        }
        public List<int> GetImpactAreaIDs()
        {
            List<int> impactAreaIDs = new List<int>();
            if (_aaeqResults.ConsequenceResultList.Count != 0)
            {
                foreach (ConsequenceDistributionResult consequence in _aaeqResults.ConsequenceResultList)
                {
                    if (!impactAreaIDs.Contains(consequence.RegionID))
                    {
                        impactAreaIDs.Add(consequence.RegionID);
                    }
                }
            }
            return impactAreaIDs;
        }
        public List<string> GetAssetCategories()
        {
            List<string> assetCats = new List<string>();
            if (_aaeqResults.ConsequenceResultList.Count != 0)
            {
                foreach (ConsequenceDistributionResult consequence in _aaeqResults.ConsequenceResultList)
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
        {
            List<string> damageCats = new List<string>();
            if (_aaeqResults.ConsequenceResultList.Count != 0)
            {
                foreach (ConsequenceDistributionResult consequence in _aaeqResults.ConsequenceResultList)
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
        /// This method returns the mean of the average annual equivalent damage for the given damage category, asset category, impact area combination 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean AAEQ damage for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanAAEQDamage(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of aaeq damage
        public double MeanAAEQDamage(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _aaeqResults.MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method returns the mean of base year expected annual damage for the given damage category, asset category, impact area combination 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD damage for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanBaseYearEAD(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of ead damage for base year 
        public double MeanBaseYearEAD(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return BaseYearScenarioResults.MeanExpectedAnnualConsequences(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method returns the mean of future year expected annual damage for the given damage category, asset category, impact area combination 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD damage for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanFutureYearEAD(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value -999
        /// <returns></returns>The mean of ead damage for future year 
        public double MeanFutureYearEAD(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.MeanExpectedAnnualConsequences(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method calls the inverse CDF of the AAEQ damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  damage is determined by the arguments used in the method
        /// For example, if you wanted the AAEQ damage exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = AAEQDamageExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns>the level of AAEQ damage exceeded by the specified probability 
        public double AAEQDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _aaeqResults.ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method calls the inverse CDF of the base year EAD damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  damage is determined by the arguments used in the method
        /// For example, if you wanted the EAD exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = BaseYearEADDamageExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns>the level of EAD damage exceeded by the specified probability 
        public double BaseYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return BaseYearScenarioResults.ConsequencesExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method calls the inverse CDF of the future year EAD damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  damage is determined by the arguments used in the method
        /// For example, if you wanted the EAD exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = FutureYearEADDamageExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value -999
        /// <returns></returns>the level of EAD damage exceeded by the specified probability 
        public double FutureYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.ConsequencesExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of aaeq damage for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetAAEQDamageHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public IHistogram GetAAEQDamageHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return _aaeqResults.GetConsequenceResultsHistogram(damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of base year ead for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetBaseYearEADHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public IHistogram GetBaseYearEADHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return BaseYearScenarioResults.GetConsequencesHistogram(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of future year ead for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetFutureYearEADHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="impactAreaID"></param>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <returns></returns>
        public IHistogram GetFutureYearEADHistogram(int impactAreaID = -999, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.GetConsequencesHistogram(impactAreaID, damageCategory, assetCategory);
        }


        //TODO what role will these play
        internal void AddConsequenceResults(int impactAreaID, string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria)
        {
            ConsequenceDistributionResult consequenceResult = AAEQDamageResults.GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (consequenceResult.IsNull)
            {
                ConsequenceDistributionResult newConsequenceResult = new ConsequenceDistributionResult(damageCategory,assetCategory,convergenceCriteria ,impactAreaID);
                _aaeqResults.ConsequenceResultList.Add(newConsequenceResult);
            }
        }
        internal void AddConsequenceResults(ConsequenceDistributionResult consequenceResultToAdd)
        {
            ConsequenceDistributionResult consequenceResults = AAEQDamageResults.GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResults.IsNull)
            {
                _aaeqResults.ConsequenceResultList.Add(consequenceResultToAdd);
            }
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        #endregion

    }
}
