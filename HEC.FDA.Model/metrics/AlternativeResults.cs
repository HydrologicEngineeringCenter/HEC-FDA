using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.Model.metrics
{
    public class AlternativeResults : ValidationErrorLogger
    {
        #region Properties
        internal bool ScenariosAreIdentical { get; set; } = false;
        public int AlternativeID { get; }
        public StudyAreaConsequencesByQuantile EqadResults { get; internal set; }
        public List<int> AnalysisYears { get; }
        public int PeriodOfAnalysis { get; }
        public bool IsNull { get; }
        internal ScenarioResults BaseYearScenarioResults { get; set; }
        internal ScenarioResults FutureYearScenarioResults { get; set; }
        #endregion

        #region Constructor
        public AlternativeResults()
        {
            IsNull = true;
            AlternativeID = 0;
            EqadResults = new StudyAreaConsequencesByQuantile(AlternativeID);
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
            EqadResults = new StudyAreaConsequencesByQuantile(id);
            IsNull = false;
            AnalysisYears = analysisYears;
            AddRules();
        }
        internal AlternativeResults(StudyAreaConsequencesByQuantile studyAreaConsequencesByQuantile, int id, List<int> analysisYears, int periodOfAnalysis, bool isNull)
        {
            AlternativeID = id;
            EqadResults = studyAreaConsequencesByQuantile;
            IsNull = isNull;
            AnalysisYears = analysisYears;
            PeriodOfAnalysis = periodOfAnalysis;
            AddRules();
        }
        #endregion

        private void AddRules()
        {
            AddSinglePropertyRule(nameof(AnalysisYears), new Rule(() => AnalysisYears[1] - AnalysisYears[0] >= 1, "The most likely future year must be at least 1 year greater then the base year"));
            AddSinglePropertyRule(nameof(PeriodOfAnalysis), new Rule(() => PeriodOfAnalysis >= Math.Abs(AnalysisYears[0] - AnalysisYears[1]) + 1, "The period of analysis must be greater than or equal to the difference between the analysis years, inclusive."));
        }
        public List<int> GetImpactAreaIDs(ConsequenceType consequenceType = ConsequenceType.Damage)
        {
            List<int> impactAreaIDs = new();
            if (EqadResults.ConsequenceResultList.Count != 0)
            {
                foreach (AggregatedConsequencesByQuantile consequence in EqadResults.ConsequenceResultList)
                {
                    if (!impactAreaIDs.Contains(consequence.RegionID) && consequence.ConsequenceType == consequenceType)
                    {
                        impactAreaIDs.Add(consequence.RegionID);
                    }
                }
            }
            return impactAreaIDs;
        }
        public List<string> GetAssetCategories(ConsequenceType consequenceType = ConsequenceType.Damage)
        {
            List<string> assetCats = new();
            if (EqadResults.ConsequenceResultList.Count != 0)
            {
                foreach (AggregatedConsequencesByQuantile consequence in EqadResults.ConsequenceResultList)
                {
                    if (!assetCats.Contains(consequence.AssetCategory) && consequence.ConsequenceType == consequenceType)
                    {
                        assetCats.Add(consequence.AssetCategory);
                    }
                }
            }
            return assetCats;
        }
        public List<string> GetDamageCategories(ConsequenceType consequenceType = ConsequenceType.Damage)
        {
            List<string> damageCats = new();
            if (EqadResults.ConsequenceResultList.Count != 0)
            {
                foreach (AggregatedConsequencesByQuantile consequence in EqadResults.ConsequenceResultList)
                {
                    if (!damageCats.Contains(consequence.DamageCategory) && consequence.ConsequenceType == consequenceType)
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
        /// For example, if you wanted mean eqad damage for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double meanEAD = SampleMeanEqad(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc...the default is null
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns>The mean of EqAD</returns> 
        public double SampleMeanEqad(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            if (ScenariosAreIdentical)
            {
                return BaseYearScenarioResults.SampleMeanExpectedAnnualConsequences(impactAreaID, damageCategory, assetCategory);
            }
            else
            {
                return EqadResults.SampleMeanDamage(damageCategory, assetCategory, impactAreaID);
            }
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
        /// <returns>The mean of ead damage for base year </returns>
        public double SampleMeanBaseYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null, ConsequenceType consequenceType = ConsequenceType.Damage)
        {
            return BaseYearScenarioResults.SampleMeanExpectedAnnualConsequences(impactAreaID, damageCategory, assetCategory, consequenceType);
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
        /// <returns>The mean of ead damage for future year </returns>
        public double SampleMeanFutureYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null, ConsequenceType consequenceType = ConsequenceType.Damage)
        {
            return FutureYearScenarioResults.SampleMeanExpectedAnnualConsequences(impactAreaID, damageCategory, assetCategory, consequenceType);

        }
        /// <summary>
        /// This method calls the inverse CDF of the EqAD damage histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
        /// The level of aggregation of  damage is determined by the arguments used in the method
        /// For example, if you wanted the EqAD damage exceeded with probability .98 for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// double consequenceValue = EqadExceededWithProbabilityQ(.98, damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        /// <param name="damageCategory"></param> either residential, commercial, etc....the default is null
        /// <param name="exceedanceProbability"></param>
        /// <param name="assetCategory"></param> either structure, content, etc...the default is null
        /// <param name="impactAreaID"></param>the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
        /// <returns>the level of EqAD damage exceeded by the specified probability</returns> 
        public double EqadExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            if (ScenariosAreIdentical)
            {
                return BaseYearScenarioResults.ConsequencesExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damageCategory, assetCategory);
            }
            else
            {
                return EqadResults.ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
            }
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
        /// <returns>the level of EAD damage exceeded by the specified probability </returns>
        public double BaseYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
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
        /// <returns>the level of EAD damage exceeded by the specified probability</returns> 
        public double FutureYearEADDamageExceededWithProbabilityQ(double exceedanceProbability, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.ConsequencesExceededWithProbabilityQ(exceedanceProbability, impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of eqad damage for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetEqadDistribution(damageCategory: "residential", impactAreaID: 2);
        public Empirical GetEqadDistribution(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            if (ScenariosAreIdentical)
            {
                return BaseYearScenarioResults.GetConsequencesDistribution(impactAreaID, damageCategory, assetCategory);
            }
            else
            {
                return EqadResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
            }
        }
        /// <summary>
        /// This method gets the histogram (distribution) of base year ead for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetBaseYearEADHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        public Empirical GetBaseYearEADDistribution(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return BaseYearScenarioResults.GetConsequencesDistribution(impactAreaID, damageCategory, assetCategory);
        }
        /// <summary>
        /// This method gets the histogram (distribution) of future year ead for the given damage category(ies), asset category(ies), and impact area(s)
        /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
        /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
        /// ThreadsafeInlineHistogram histogram = GetFutureYearEADHistogram(damageCategory: "residential", impactAreaID: 2);
        /// </summary>
        public Empirical GetFutureYearEADDistribution(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
        {
            return FutureYearScenarioResults.GetConsequencesDistribution(impactAreaID, damageCategory, assetCategory);
        }
        internal void AddConsequenceResults(AggregatedConsequencesByQuantile consequenceResultToAdd)
        {
            AggregatedConsequencesByQuantile consequenceResults = EqadResults.GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID, consequenceResultToAdd.ConsequenceType);
            if (consequenceResults.IsNull)
            {
                EqadResults.ConsequenceResultList.Add(consequenceResultToAdd);
            }
        }

    }
}
