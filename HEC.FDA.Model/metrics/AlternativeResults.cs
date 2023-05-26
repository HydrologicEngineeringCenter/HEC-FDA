using System;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;

namespace HEC.FDA.Model.metrics
{
    public class AlternativeResults : ValidationErrorLogger, IProgressReport
    {
        #region Properties
        public int AlternativeID {get;}
        public ManyEmpiricalDistributionsOfConsequences AAEQDamageResults { get; }
        public List<int> AnalysisYears { get; }
        public int PeriodOfAnalysis { get; }
        public event ProgressReportedEventHandler ProgressReport;

        public bool IsNull { get; }
        internal ScenarioResults BaseYearScenarioResults { get; set; }
        internal ScenarioResults FutureYearScenarioResults { get; set; }
        #endregion

        #region Constructor
        public AlternativeResults()
        {
            IsNull = true;
            AlternativeID = 0;
            AAEQDamageResults = new ManyEmpiricalDistributionsOfConsequences(AlternativeID);
            BaseYearScenarioResults = new ScenarioResults();
            FutureYearScenarioResults = new ScenarioResults();
            AnalysisYears = new List<int>() { 2030, 2049 };
            PeriodOfAnalysis = 50;
            AddRules();
        }


        public AlternativeResults(int id, List<int> analysisYears, int periodOfAnalysis)
        {
            AlternativeID = id;
            PeriodOfAnalysis = periodOfAnalysis;
            AAEQDamageResults = new ManyEmpiricalDistributionsOfConsequences(id);
            IsNull = false;
            AnalysisYears = analysisYears;
            AddRules();
        }
        internal AlternativeResults(int id, List<int> analysisYears, int periodOfAnalysis, bool isNull)
        {
            AlternativeID = id;
            AAEQDamageResults = new ManyEmpiricalDistributionsOfConsequences(id);
            IsNull = isNull;
            AnalysisYears = analysisYears;
            PeriodOfAnalysis = periodOfAnalysis;
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
            List<int> impactAreaIDs = new();
            if (AAEQDamageResults.ConsequenceResultList.Count != 0)
            {
                foreach (SingleEmpiricalDistributionOfConsequences consequence in AAEQDamageResults.ConsequenceResultList)
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
            List<string> assetCats = new();
            if (AAEQDamageResults.ConsequenceResultList.Count != 0)
            {
                foreach (SingleEmpiricalDistributionOfConsequences consequence in AAEQDamageResults.ConsequenceResultList)
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
            List<string> damageCats = new();
            if (AAEQDamageResults.ConsequenceResultList.Count != 0)
            {
                foreach (SingleEmpiricalDistributionOfConsequences consequence in AAEQDamageResults.ConsequenceResultList)
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
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of aaeq damage
        public double MeanAAEQDamage(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return AAEQDamageResults.MeanDamage(damageCategory, assetCategory, impactAreaID);
        }
        /// <summary>
        /// This method returns the mean of base year expected annual damage for the given damage category, asset category, impact area combination 
        /// The level of aggregation of the mean is determined by the arguments used in the method
        /// For example, if you wanted mean EAD damage for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = MeanBaseYearEAD(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of ead damage for base year 
        public double MeanBaseYearEAD(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
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
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>The mean of ead damage for future year 
        public double MeanFutureYearEAD(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
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
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>the level of AAEQ damage exceeded by the specified probability 
        public double AAEQDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return AAEQDamageResults.ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
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
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>the level of EAD damage exceeded by the specified probability 
        public double BaseYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
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
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns></returns>the level of EAD damage exceeded by the specified probability 
        public double FutureYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
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
        public Empirical GetAAEQDamageDistribution(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return AAEQDamageResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
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
        public Empirical GetBaseYearEADDistribution(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return BaseYearScenarioResults.GetConsequencesDistribution(impactAreaID, damageCategory, assetCategory);
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
        public Empirical GetFutureYearEADDistribution(int impactAreaID = utilities.IntegerConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.GetConsequencesDistribution(impactAreaID, damageCategory, assetCategory);
        }
        internal void AddConsequenceResults(SingleEmpiricalDistributionOfConsequences consequenceResultToAdd)
        {
            SingleEmpiricalDistributionOfConsequences consequenceResults = AAEQDamageResults.GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResults.IsNull)
            {
                AAEQDamageResults.ConsequenceResultList.Add(consequenceResultToAdd);
            }
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }

        #endregion

    }
}
