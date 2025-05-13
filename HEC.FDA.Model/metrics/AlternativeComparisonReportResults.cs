using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using Statistics.Distributions;
using HEC.MVVMFramework.Model.Messaging;

namespace HEC.FDA.Model.metrics;

public class AlternativeComparisonReportResults : ValidationErrorLogger
{   //TODO: save a year 
    #region Fields

    private readonly List<StudyAreaConsequencesByQuantile> _AaeqReducedResultsList;
    private readonly List<StudyAreaConsequencesByQuantile> _BaseYearEADReducedResultsList;
    private readonly List<StudyAreaConsequencesByQuantile> _FutureYearEADReducedResultsList;
    private readonly List<AlternativeResults> _WithProjectAlternativeResults;
    private readonly AlternativeResults _WithoutProjectAlternativeResults;

    #endregion

    #region Properties 
    internal bool IsNull { get; }
    public List<int> Years
    {
        get
        {
            return _WithoutProjectAlternativeResults.AnalysisYears;
        }
    }

    #endregion

    #region Constructor
    public AlternativeComparisonReportResults()
    {
        IsNull = true;
        _AaeqReducedResultsList = new List<StudyAreaConsequencesByQuantile>();
        StudyAreaConsequencesByQuantile dummyAaeqResults = new();
        _AaeqReducedResultsList.Add(dummyAaeqResults);
        _BaseYearEADReducedResultsList = new List<StudyAreaConsequencesByQuantile>();
        StudyAreaConsequencesByQuantile dummyBaseYearResults = new();
        _BaseYearEADReducedResultsList.Add(dummyBaseYearResults);
        _FutureYearEADReducedResultsList = new List<StudyAreaConsequencesByQuantile>();
        StudyAreaConsequencesByQuantile dummyFutureYearResults = new();
        _FutureYearEADReducedResultsList.Add(dummyFutureYearResults);
    }
    internal AlternativeComparisonReportResults(IEnumerable<AlternativeResults> withProjectAlternativeResults, AlternativeResults withoutProjectAlternativeResults, List<StudyAreaConsequencesByQuantile> aaeqResults, List<StudyAreaConsequencesByQuantile> baseYearEADResults, List<StudyAreaConsequencesByQuantile> futureYearEADResults)
    {
        _WithProjectAlternativeResults = [.. withProjectAlternativeResults];
        _WithoutProjectAlternativeResults = withoutProjectAlternativeResults;
        _AaeqReducedResultsList = aaeqResults;
        _BaseYearEADReducedResultsList = baseYearEADResults;
        _FutureYearEADReducedResultsList = futureYearEADResults;
    }
    #endregion

    #region Methods 
    //These methods now assume that the same impact areas are in all three results lists - a reasonable assumption 
    //How could they not? What could happen that could cause different impact areas or damage categories to be in different results?  
    //We could cycle through each of the three lists, but that seems unnecessary 
    public List<int> GetImpactAreaIDs()
    {
        List<int> impactAreaIDs = new();
        foreach (StudyAreaConsequencesByQuantile consequenceReducedResults in _AaeqReducedResultsList)
        {
            foreach (AggregatedConsequencesByQuantile consequenceResult in consequenceReducedResults.ConsequenceResultList)
            {
                if (!impactAreaIDs.Contains(consequenceResult.RegionID))
                {
                    impactAreaIDs.Add(consequenceResult.RegionID);
                }
            }

        }
        return impactAreaIDs;
    }
    public List<string> GetAssetCategories()
    {
        List<string> assetCats = new();
        foreach (StudyAreaConsequencesByQuantile consequenceReducedResults in _AaeqReducedResultsList)
        {
            foreach (AggregatedConsequencesByQuantile consequenceResult in consequenceReducedResults.ConsequenceResultList)
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
        List<string> damCats = new();
        foreach (StudyAreaConsequencesByQuantile consequenceReducedResults in _AaeqReducedResultsList)
        {
            foreach (AggregatedConsequencesByQuantile consequenceResult in consequenceReducedResults.ConsequenceResultList)
            {
                if (!damCats.Contains(consequenceResult.DamageCategory))
                {
                    damCats.Add(consequenceResult.DamageCategory);
                }
            }

        }
        return damCats;
    }
    /// <summary>
    /// This method gets the mean aaeq damage reduced between the with- and without-project conditions for a given with-project condition, 
    /// impact area, damage category, and asset category combination. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the mean aaeq damage reduced for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = MeanAAEQDamageReduced(1, damageCategory: "residential", impactAreaID: 2);        /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double MeanAAEQDamageReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID).MeanDamage(damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the mean base year ead reduced between the with- and without-project conditions for a given with-project condition, 
    /// impact area, damage category, and asset category combination. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the mean EAD reduced for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = MeanBaseYearEADReduced(1, damageCategory: "residential", impactAreaID: 2);        /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double MeanBaseYearEADReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true, true).MeanDamage(damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the mean future year ead reduced between the with- and without-project conditions for a given with-project condition, 
    /// impact area, damage category, and asset category combination. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the mean EAD for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = MeanFutureYearEADReduced(1, damageCategory: "residential", impactAreaID: 2);        /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double MeanFutureYearEADReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true).MeanDamage(damageCategory, assetCategory, impactAreaID);
    }

    public double MeanWithoutProjectBaseYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.MeanBaseYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double MeanWithoutProjectFutureYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.MeanFutureYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double MeanWithProjectBaseYearEAD(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.MeanBaseYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double MeanWithProjectFutureYearEAD(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.MeanFutureYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double MeanWithProjectAAEQDamage(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.MeanAAEQDamage(impactAreaID, damageCategory, assetCategory);
    }
    public double MeanWithoutProjectAAEQDamage(int impactArea = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.MeanAAEQDamage(impactArea, damageCategory, assetCategory);
    }

    /// <summary>
    /// This method calls the inverse CDF of aaeq damage reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the aaeq damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = AAEQDamageReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="exceedanceProbability"></param>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double AAEQDamageReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID).ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method calls the inverse CDF of base year ead reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the aaeq damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = BaseYearEADReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="exceedanceProbability"></param>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double BaseYearEADReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true, true).ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method calls the inverse CDF of future year ead reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the aaeq damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = FutureYearEADReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="exceedanceProbability"></param>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double FutureYearEADReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true).ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// IHistogram histogram = GetAlternativeResultsHistogram(1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param>
    /// <param name="assetCategory"></param>
    /// <returns></returns>
    public Empirical GetAAEQReducedResultsHistogram(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        StudyAreaConsequencesByQuantile aaeqResults = GetConsequencesReducedResultsForGivenAlternative(alternativeID);
        return aaeqResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the histogram (distribution) of base year ead reduced for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// IHistogram histogram = GetBaseYearEADReducedResultsHistogram(1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param>
    /// <param name="assetCategory"></param>
    /// <returns></returns>
    public Empirical GetBaseYearEADReducedResultsHistogram(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        StudyAreaConsequencesByQuantile eadResults = GetConsequencesReducedResultsForGivenAlternative(alternativeID, true, true);
        return eadResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the histogram (distribution) of future year ead reduced for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// IHistogram histogram = GetFutureYearEADReducedResultsHistogram(1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param>
    /// <param name="assetCategory"></param>
    /// <returns></returns>
    public Empirical GetFutureYearEADReducedResultsHistogram(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        StudyAreaConsequencesByQuantile eadResults = GetConsequencesReducedResultsForGivenAlternative(alternativeID, true);
        return eadResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
    }
    internal void AddAlternativeResults(StudyAreaConsequencesByQuantile consequenceDistributionResults, bool isEADResults = false, bool isBaseYearResults = false)
    {
        StudyAreaConsequencesByQuantile consequenceResults = GetConsequencesReducedResultsForGivenAlternative(consequenceDistributionResults.AlternativeID, isEADResults, isBaseYearResults);
        if (consequenceResults.IsNull)
        {
            _AaeqReducedResultsList.Add(consequenceDistributionResults);
        }
    }
    internal StudyAreaConsequencesByQuantile GetConsequencesReducedResultsForGivenAlternative(int alternativeID, bool getEADResults = false, bool getBaseYearResults = false)
    {
        List<StudyAreaConsequencesByQuantile> listToSearch;
        if (!getEADResults) { listToSearch = _AaeqReducedResultsList; }
        else if (getEADResults && getBaseYearResults) { listToSearch = _BaseYearEADReducedResultsList; }
        else if (getEADResults && !getBaseYearResults) { listToSearch = _FutureYearEADReducedResultsList; }
        else { throw new System.ArgumentException("An illogical combination of arguments was provided"); }
        foreach (StudyAreaConsequencesByQuantile consequenceDistResults in listToSearch)
        {
            if (consequenceDistResults.AlternativeID.Equals(alternativeID))
            {
                return consequenceDistResults;
            }
        }
        StudyAreaConsequencesByQuantile dummyConsequenceDistributionResults = new();
        ReportMessage(this, new MessageEventArgs(new Message("The requested alternative could not be found. An arbitrary object is being returned.")));
        return dummyConsequenceDistributionResults;
    }

    private AlternativeResults GetAlternativeResults(int alternativeID)
    {
        foreach (AlternativeResults alternativeResults in _WithProjectAlternativeResults)
        {
            if (alternativeResults.AlternativeID.Equals(alternativeID))
            {
                return alternativeResults;
            }
        }
        AlternativeResults dummyAlternativeResults = new();
        ReportMessage(this, new MessageEventArgs(new Message("The requested alternative could not be found. An artibtrary object is being returned.")));
        return dummyAlternativeResults;
    }

    #endregion
}
