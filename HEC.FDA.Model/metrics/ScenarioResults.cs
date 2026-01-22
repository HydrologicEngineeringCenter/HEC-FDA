using HEC.FDA.Model.metrics.Extensions;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics;

public class ScenarioResults : ValidationErrorLogger
{
    private const string SCENARIO_RESULTS = "ScenarioResults";
    private const string COMPUTE_DATE = "ComputeDate";
    private const string SOFTWARE_VERSION = "SoftwareVersion";
    #region Properties 
    public string ComputeDate { get; set; }
    public string SoftwareVersion { get; set; }

    public List<ImpactAreaScenarioResults> ResultsList { get; } = new List<ImpactAreaScenarioResults>();

    #endregion

    #region Constructor
    internal ScenarioResults()
    {
    }

    #endregion

    #region Methods
    public List<int> GetImpactAreaIDs(ConsequenceType consequenceType)
    {
        return ResultsList
            .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
            .Where(c => c.ConsequenceType == consequenceType)
            .Select(c => c.RegionID)
            .Distinct()
            .ToList();
    }
    public List<string> GetAssetCategories(ConsequenceType consequenceType = ConsequenceType.Damage)
    {
        return ResultsList
            .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
            .Where(c => c.ConsequenceType == consequenceType)
            .Select(c => c.AssetCategory)
            .Distinct()
            .ToList();
    }
    public List<string> GetDamageCategories(ConsequenceType consequenceType = ConsequenceType.Damage)
    {
        return ResultsList
            .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
            .Where(c => c.ConsequenceType == consequenceType)
            .Select(c => c.DamageCategory)
            .Distinct()
            .ToList();
    }
    public List<RiskType> GetRiskTypes()
    {
        return ResultsList
            .SelectMany(r => r.ConsequenceResults.ConsequenceResultList)
            .Select(r => r.RiskType)
            .Distinct()
            .ToList();
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
    public double SampleMeanExpectedAnnualConsequences(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {//TODO: This could probably be more efficient and could use some null checking
        double consequenceValue = 0;
        foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
        {
            consequenceValue += impactAreaScenarioResults.ConsequenceResults.SampleMeanDamage(damageCategory, assetCategory, impactAreaID, consequenceType, riskType);
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
    public double ConsequencesExceededWithProbabilityQ(double exceedanceProbability,
        int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE,
        string damageCategory = null,
        string assetCategory = null,
        ConsequenceType consequenceType = ConsequenceType.Damage,
        RiskType riskType = RiskType.Total)
    {
        double consequenceValue = 0;
        foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
        {
            consequenceValue += impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList
               .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
               .Sum((x) => x.ConsequenceHistogram.InverseCDF(exceedanceProbability));
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
    public Empirical GetConsequencesDistribution(int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, string damageCategory = null, string assetCategory = null, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Total)
    {
        List<Empirical> empiricalDistsToStack = [];

        foreach (ImpactAreaScenarioResults impactAreaScenarioResults in ResultsList)
        {
            var filtered = impactAreaScenarioResults.ConsequenceResults.ConsequenceResultList
                .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
                .Select((h) => DynamicHistogram.ConvertToEmpiricalDistribution(h.ConsequenceHistogram));

            foreach (var consequenceResult in filtered)
            {
                empiricalDistsToStack.Add(consequenceResult);
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
    public void AddResults(ImpactAreaScenarioResults resultsToAdd)
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
        XElement mainElement = new(SCENARIO_RESULTS);
        mainElement.SetAttributeValue(COMPUTE_DATE, ComputeDate);
        mainElement.SetAttributeValue(SOFTWARE_VERSION, SoftwareVersion);
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
            ImpactAreaScenarioResults impactAreaScenarioResults = ImpactAreaScenarioResults.ReadFromXML(element);
            scenarioResults.AddResults(impactAreaScenarioResults);
        }

        if (xElement.Attribute(COMPUTE_DATE) != null)
        {
            string computeDate = xElement.Attribute(COMPUTE_DATE).Value;
            scenarioResults.ComputeDate = computeDate;
        }

        if (xElement.Attribute(SOFTWARE_VERSION) != null)
        {
            string softwareVersion = xElement.Attribute(SOFTWARE_VERSION).Value;
            scenarioResults.SoftwareVersion = softwareVersion;
        }

        return scenarioResults;
    }
    #endregion

}
