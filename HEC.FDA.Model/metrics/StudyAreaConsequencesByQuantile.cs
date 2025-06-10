using HEC.FDA.Model.metrics.Extensions;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics;

public class StudyAreaConsequencesByQuantile : Validation
{
    #region Properties 
    public List<AggregatedConsequencesByQuantile> ConsequenceResultList { get; }
    public event MessageReportedEventHandler MessageReport;
    public bool IsNull { get; }
    internal int AlternativeID { get; }
    #endregion

    #region Constructors
    public StudyAreaConsequencesByQuantile()
    {
        ConsequenceResultList = [];
        AggregatedConsequencesByQuantile dummyConsequenceDistributionResult = new();
        ConsequenceResultList.Add(dummyConsequenceDistributionResult);
        IsNull = true;
    }
    internal StudyAreaConsequencesByQuantile(int alternativeID)
    {
        ConsequenceResultList = [];
        AlternativeID = alternativeID;
        IsNull = false;
    }
    //public for testing
    internal StudyAreaConsequencesByQuantile(List<AggregatedConsequencesByQuantile> damageResults)
    {
        ConsequenceResultList = damageResults;
        IsNull = false;
    }
    #endregion

    #region Methods 
    //public for testing purposes
    public void AddExistingConsequenceResultObject(AggregatedConsequencesByQuantile consequenceResultToAdd)
    {
        AggregatedConsequencesByQuantile consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
        if (consequenceResult.IsNull)
        {
            ConsequenceResultList.Add(consequenceResultToAdd);
        }
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
    public double SampleMeanDamage(string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
    {
        return ConsequenceResultList
    .FilterByCategories(damageCategory, assetCategory, impactAreaID)
    .Sum(result => result.ConsequenceSampleMean());
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
    public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability, string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
    {
        return ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
            .Sum(result => result.ConsequenceExceededWithProbabilityQ(exceedanceProbability));
    }
    /// <summary>
    /// This method returns a consequence result for the given damage category, asset category, and impact area 
    /// Impact area ID is used for alternative and alternative comparison reports 
    /// Impact area ID is -999 otherwise 
    /// </summary>
    /// <param name="damageCategory"></param>
    /// <param name="assetCategory"></param>
    /// <param name="impactAreaID"></param>
    /// <returns></returns>
    public AggregatedConsequencesByQuantile GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID = -999)
    {
        AggregatedConsequencesByQuantile result = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
            .FirstOrDefault();
        if (result != null)
        {
            return result;
        }
        string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
        ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
        ReportMessage(this, new MessageEventArgs(errorMessage));
        AggregatedConsequencesByQuantile singleEmpiricalDistributionOfConsequences = new();
        return singleEmpiricalDistributionOfConsequences;
    }

    public void ReportMessage(object sender, MessageEventArgs e)
    {
        MessageReport?.Invoke(sender, e);
    }
    /// <summary>
    /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
    /// ThreadsafeInlineHistogram histogram = GetConsequenceResultsHistogram(damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="damageCategory"></param> The default is null 
    /// <param name="assetCategory"></param> The default is null 
    /// <param name="impactAreaID"></param> The default is a null value (-999)
    /// <returns></returns> Aggregated consequences histogram 
    public Empirical GetAggregateEmpiricalDistribution(string damageCategory = null, string assetCategory = null, int impactAreaID = -999)
    {
        var empiricalDistsToStack = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
            .Select(result => result.ConsequenceDistribution)
            .ToList();

        if (empiricalDistsToStack.Count() == 0)
        {
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return new Empirical();
        }
        return Empirical.StackEmpiricalDistributions(empiricalDistsToStack, Empirical.Sum);
    }

    public XElement WriteToXML()
    {
        XElement masterElem = new("EAD_Results");
        foreach (AggregatedConsequencesByQuantile damageResult in ConsequenceResultList)
        {
            XElement damageResultElement = damageResult.WriteToXML();
            damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
            masterElem.Add(damageResultElement);
        }
        return masterElem;
    }

    public static StudyAreaConsequencesByQuantile ReadFromXML(XElement xElement)
    {
        List<AggregatedConsequencesByQuantile> damageResults = [];
        foreach (XElement histogramElement in xElement.Elements())
        {
            damageResults.Add(AggregatedConsequencesByQuantile.ReadFromXML(histogramElement));
        }
        return new StudyAreaConsequencesByQuantile(damageResults);
    }
    #endregion

}
