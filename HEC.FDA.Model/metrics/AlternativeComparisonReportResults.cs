using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using Statistics.Distributions;
using HEC.MVVMFramework.Model.Messaging;
using System.Linq;

namespace HEC.FDA.Model.metrics;

public class AlternativeComparisonReportResults : ValidationErrorLogger
{   //TODO: save a year 

    //results of the alternative comparison
    private readonly List<StudyAreaConsequencesByQuantile> _EqadReducedResultsList; // somehow this guy is coming in with calculated mean 0. zero for all quantiles, and a negative sample mean. 
    private readonly List<StudyAreaConsequencesByQuantile> _BaseYearEADReducedResultsList;
    private readonly List<StudyAreaConsequencesByQuantile> _FutureYearEADReducedResultsList;

    //input to the alternative comparison. 
    private readonly List<AlternativeResults> _WithProjectAlternativeResults;
    private readonly AlternativeResults _WithoutProjectAlternativeResults;

    public List<int> Years => _WithoutProjectAlternativeResults.AnalysisYears;

    #region Constructor
    internal AlternativeComparisonReportResults(IEnumerable<AlternativeResults> withProjectAlternativeResults, AlternativeResults withoutProjectAlternativeResults, List<StudyAreaConsequencesByQuantile> eqadResults, List<StudyAreaConsequencesByQuantile> baseYearEADResults, List<StudyAreaConsequencesByQuantile> futureYearEADResults)
    {
        _WithProjectAlternativeResults = [.. withProjectAlternativeResults];
        _WithoutProjectAlternativeResults = withoutProjectAlternativeResults;
        _EqadReducedResultsList = eqadResults;
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
        return _EqadReducedResultsList
        .SelectMany(x => x.ConsequenceResultList.Select(r => r.RegionID))
        .Distinct()
        .ToList();
    }
    public List<string> GetAssetCategories()
    {
        return _EqadReducedResultsList
        .SelectMany(x => x.ConsequenceResultList.Select(r => r.AssetCategory))
        .Distinct()
        .ToList();
    }
    public List<string> GetDamageCategories()
    {
        return _EqadReducedResultsList
        .SelectMany(x => x.ConsequenceResultList.Select(r => r.DamageCategory))
        .Distinct()
        .ToList();
    }
    /// <summary>
    /// This method gets the mean EqAD damage reduced between the with- and without-project conditions for a given with-project condition, 
    /// impact area, damage category, and asset category combination. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the mean EqAD damage reduced for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = MeanEqADDamageReduced(1, damageCategory: "residential", impactAreaID: 2);        /// </summary>
    /// <param name="alternativeID"></param>
    /// <param name="impactAreaID"></param>
    /// <param name="damageCategory"></param> either residential, commercial, etc...
    /// <param name="assetCategory"></param> either structure, content, etc...
    /// <returns></returns>
    public double SampleMeanEqadReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID).SampleMeanDamage(damageCategory, assetCategory, impactAreaID);
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
    public double SampleMeanBaseYearEADReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        StudyAreaConsequencesByQuantile results = GetConsequencesReducedResultsForGivenAlternative(alternativeID, true, true);
        return results.SampleMeanDamage(damageCategory, assetCategory, impactAreaID);
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
    public double SampleMeanFutureYearEADReduced(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true).SampleMeanDamage(damageCategory, assetCategory, impactAreaID);
    }
    public double SampleMeanWithoutProjectBaseYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.SampleMeanBaseYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double SampleMeanWithoutProjectFutureYearEAD(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.SampleMeanFutureYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double SampleMeanWithProjectBaseYearEAD(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.SampleMeanBaseYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double SampleMeanWithProjectFutureYearEAD(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.SampleMeanFutureYearEAD(impactAreaID, damageCategory, assetCategory);
    }
    public double SampleMeanWithProjectEqad(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        AlternativeResults alternativeResults = GetAlternativeResults(alternativeID);
        return alternativeResults.SampleMeanEqad(impactAreaID, damageCategory, assetCategory);
    }
    public double SampleMeanWithoutProjectEqad(int impactArea = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return _WithoutProjectAlternativeResults.SampleMeanEqad(impactArea, damageCategory, assetCategory);
    }

    /// <summary>
    /// This method calls the inverse CDF of EqAD damage reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the EqAD damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = EqadReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    public double EqadReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID).ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method calls the inverse CDF of base year ead reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the EqAD damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = BaseYearEADReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <returns></returns>
    public double BaseYearEADReducedExceededWithProbabilityQ(double exceedanceProbability, int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        return GetConsequencesReducedResultsForGivenAlternative(alternativeID, true, true).ConsequenceExceededWithProbabilityQ(exceedanceProbability, damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method calls the inverse CDF of future year ead reduced histogram up to the non-exceedance probabilty. The method accepts exceedance probability as an argument. 
    ///  The level of aggregation of  consequences is determined by the arguments used in the method
    /// For example, if you wanted the EqAD damage exceeded with probability .98 for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// double consequenceValue = FutureYearEADReducedExceededWithProbabilityQ(.98, 1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
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
    public Empirical GetEqadReducedResultsHistogram(int alternativeID, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null)
    {
        StudyAreaConsequencesByQuantile eqadResults = GetConsequencesReducedResultsForGivenAlternative(alternativeID);
        return eqadResults.GetAggregateEmpiricalDistribution(damageCategory, assetCategory, impactAreaID);
    }
    /// <summary>
    /// This method gets the histogram (distribution) of base year ead reduced for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for alternative 1, residential, impact area 2, all asset categories, then the method call would be as follows:
    /// IHistogram histogram = GetBaseYearEADReducedResultsHistogram(1, damageCategory: "residential", impactAreaID: 2);
    /// </summary>
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
            _EqadReducedResultsList.Add(consequenceDistributionResults);
        }
    }
    internal StudyAreaConsequencesByQuantile GetConsequencesReducedResultsForGivenAlternative(int alternativeID, bool getEADResults = false, bool getBaseYearResults = false)
    {
        //Three logical cases here: We want EqAD results, we want base year EAD results, or we want future year EAD results.
        List<StudyAreaConsequencesByQuantile> listToSearch;
        if (!getEADResults) 
        { 
            listToSearch = _EqadReducedResultsList;
        }
        else if (getEADResults && getBaseYearResults)
        {
            listToSearch = _BaseYearEADReducedResultsList;
        } 
        else if (getEADResults && !getBaseYearResults) 
        { 
            listToSearch = _FutureYearEADReducedResultsList;
        } 
        else
        {
            throw new System.ArgumentException("An illogical combination of arguments was provided");
        }
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
