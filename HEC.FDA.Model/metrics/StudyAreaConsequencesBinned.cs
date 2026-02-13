using HEC.FDA.Model.metrics.Extensions;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics;

public class StudyAreaConsequencesBinned : ValidationErrorLogger
{
    public List<AggregatedConsequencesBinned> ConsequenceResultList { get; }
    //this needs to be an error report
    public bool IsNull { get; }

    #region Constructors
    public StudyAreaConsequencesBinned(int impactAreaID)
    {
        ConsequenceResultList = [];
        AggregatedConsequencesBinned dummyConsequenceDistributionResult = new(impactAreaID, ConsequenceType.UNASSIGNED, RiskType.Fail);
        ConsequenceResultList.Add(dummyConsequenceDistributionResult);
        IsNull = true;

        //create an array to collect data the side of the convergence criteria iteration count 
    }
    public StudyAreaConsequencesBinned(bool isNull)
    {
        ConsequenceResultList = [];
        IsNull = isNull;
    }

    //public for testing
    public StudyAreaConsequencesBinned(List<AggregatedConsequencesBinned> damageResults)
    {
        ConsequenceResultList = damageResults;
        IsNull = false;
    }
    #endregion

    #region Methods 
    //This constructor is used in the simulation parallel compute and creates a threadsafe inline histogram inside consequence distribution result 
    internal void AddNewConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, ConsequenceType consequenceType, RiskType riskType)
    {
        AggregatedConsequencesBinned existingResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID, consequenceType, riskType);
        if (existingResult == null)
        {
            AggregatedConsequencesBinned newResult = new(damageCategory, assetCategory, convergenceCriteria, impactAreaID, consequenceType, riskType);
            ConsequenceResultList.Add(newResult);
        }
    }
    //public for testing purposes
    public void AddExistingConsequenceResultObject(AggregatedConsequencesBinned consequenceResultToAdd)
    {
        AggregatedConsequencesBinned consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID, consequenceResultToAdd.ConsequenceType, consequenceResultToAdd.RiskType);
        if (consequenceResult == null)
        {
            ConsequenceResultList.Add(consequenceResultToAdd);
        }
    }
    //This approach is used in binning EAD results
    internal void AddConsequenceRealization(double damageEstimate, string damageCategory, string assetCategory, int impactAreaID, long iteration, ConsequenceType consequenceType, RiskType riskType)
    {
        AggregatedConsequencesBinned damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID, consequenceType, riskType);
        damageResult.AddConsequenceRealization(damageEstimate, iteration);

    }
    //This approach is used in binning stage damage results 
    internal void AddConsequenceRealization(ConsequenceResult consequenceResult, string damageCategory, int impactAreaID, int iteration)
    {
        GetConsequenceResult(damageCategory, utilities.StringGlobalConstants.STRUCTURE_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.StructureDamage, iteration, consequenceResult.DamagedStructuresQuantity);
        GetConsequenceResult(damageCategory, utilities.StringGlobalConstants.CONTENT_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.ContentDamage, iteration, consequenceResult.DamagedContentsQuantity);
        GetConsequenceResult(damageCategory, utilities.StringGlobalConstants.VEHICLE_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.VehicleDamage, iteration, consequenceResult.DamagedVehiclesQuantity);
        GetConsequenceResult(damageCategory, utilities.StringGlobalConstants.OTHER_ASSET_CATEGORY, impactAreaID).AddConsequenceRealization(consequenceResult.OtherDamage, iteration, consequenceResult.DamagedOthersQuantity);
    }
    public void PutDataIntoHistograms()
    {
        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            consequenceDistributionResult.PutDataIntoHistogram();
        }
    }



    //TODO: This needs to confirm that the histograms inside each ConsequenceDistributionResult Match. 
    public bool Equals(StudyAreaConsequencesBinned inputDamageResults)
    {
        foreach (AggregatedConsequencesBinned damageResult in ConsequenceResultList)
        {
            AggregatedConsequencesBinned inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID, damageResult.ConsequenceType);
            if (inputDamageResult == null)
                return false;

            bool resultsMatch = damageResult.Equals(inputDamageResult);
            if (!resultsMatch)
            {
                return false;
            }
        }
        return true;
    }


    public IHistogram GetSpecificHistogram(string damageCategory, string assetCategory, int impactAreaID, bool getQuantityHistogram = false)
    {
        IHistogram returnHistogram = null;
        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            if (consequenceDistributionResult.DamageCategory == damageCategory)
            {
                if (consequenceDistributionResult.AssetCategory == assetCategory)
                {
                    if (consequenceDistributionResult.RegionID == impactAreaID)
                    {
                        if (getQuantityHistogram)
                        {
                            returnHistogram = consequenceDistributionResult.DamagedElementQuantityHistogram;
                        }
                        else
                        {
                            returnHistogram = consequenceDistributionResult.ConsequenceHistogram;
                        }
                    }
                }
            }
        }
        if (returnHistogram == null)
        {
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            returnHistogram = new DynamicHistogram();
        }
        return returnHistogram;
    }

    internal long RemainingIterations(double upperProb, double lowerProb)
    {
        List<long> stageDamageIterationsRemaining = [];

        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            if (consequenceDistributionResult.ConsequenceHistogram.HistogramIsZeroValued)
            {
                stageDamageIterationsRemaining.Add(0);
            }
            else
            {
                stageDamageIterationsRemaining.Add(consequenceDistributionResult.ConsequenceHistogram.EstimateIterationsRemaining(upperProb, lowerProb));
            }
        }
        return stageDamageIterationsRemaining.Max();
    }

    public bool ResultsAreConverged(double upperConfidenceLimit, double lowerConfidenceLimit)
    {
        bool allHistogramsAreConverged = true;
        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            bool histogramIsConverged = consequenceDistributionResult.ConsequenceHistogram.IsHistogramConverged(upperConfidenceLimit, lowerConfidenceLimit);
            if (!histogramIsConverged)
            {
                allHistogramsAreConverged = false;
                break;
            }
        }
        return allHistogramsAreConverged;
    }
    public static (List<UncertainPairedData>, List<UncertainPairedData>) ToUncertainPairedData(List<double> xValues, List<StudyAreaConsequencesBinned> yValues, int impactAreaID)
    {
        (List<UncertainPairedData>, List<UncertainPairedData>) uncertainPairedDataList = new([], []);
        List<string> damageCategories = yValues[^1].GetDamageCategories();
        List<string> assetCategories = yValues[^1].GetAssetCategories();


        foreach (string damageCategory in damageCategories)
        {
            foreach (string assetCategory in assetCategories)
            {
                CurveMetaData damageCurveMetaData = new("X Values", "Consequences", "Consequences Uncertain Paired Data", damageCategory, impactAreaID, assetCategory);
                List<IHistogram> damageHistograms = [];

                CurveMetaData quantityCurveMetaData = new("X Values", "Damaged Elements Quantity", "Damaged Elements Quantity Uncertain Paired Data", damageCategory, impactAreaID, assetCategory);
                List<IHistogram> quantityHistograms = [];

                foreach (StudyAreaConsequencesBinned consequenceDistributions in yValues)
                {
                    IHistogram histogram = consequenceDistributions.GetSpecificHistogram(damageCategory, assetCategory, impactAreaID);
                    damageHistograms.Add(histogram);

                    IHistogram quantityHistogram = consequenceDistributions.GetSpecificHistogram(damageCategory, assetCategory, impactAreaID, getQuantityHistogram: true);
                    quantityHistograms.Add(quantityHistogram);
                }

                UncertainPairedData damageUncertainPairedData = new(xValues.ToArray(), damageHistograms.ToArray(), damageCurveMetaData);
                UncertainPairedData quantityUncertainPairedData = new(xValues.ToArray(), quantityHistograms.ToArray(), quantityCurveMetaData);

                uncertainPairedDataList.Item1.Add(damageUncertainPairedData);
                uncertainPairedDataList.Item2.Add(quantityUncertainPairedData);
            }
        }
        return uncertainPairedDataList;
    }

    private List<string> GetAssetCategories()
    {
        List<string> assetCategories = [];
        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            if (!assetCategories.Contains(consequenceDistributionResult.AssetCategory))
            {
                assetCategories.Add(consequenceDistributionResult.AssetCategory);
            }
        }
        return assetCategories;
    }

    private List<string> GetDamageCategories()
    {
        List<string> damageCategories = [];
        foreach (AggregatedConsequencesBinned consequenceDistributionResult in ConsequenceResultList)
        {
            if (!damageCategories.Contains(consequenceDistributionResult.DamageCategory))
            {
                damageCategories.Add(consequenceDistributionResult.DamageCategory);
            }
        }
        return damageCategories;
    }

    /// <summary>
    /// Converts binned consequence results to quantile-based results, optionally filtering by consequence type.
    /// </summary>
    /// <param name="studyAreaConsequencesBinned"></param>
    /// <param name="filterByConsequenceType">will filter to the input type. "All" will apply no filter. </param>
    /// <returns></returns>
    public static StudyAreaConsequencesByQuantile ConvertToStudyAreaConsequencesByQuantile(StudyAreaConsequencesBinned studyAreaConsequencesBinned, ConsequenceType filterByConsequenceType)
    {
        List<AggregatedConsequencesByQuantile> aggregatedConsequencesByQuantiles = [];

        //here we apply the filter.  
        var res = studyAreaConsequencesBinned.ConsequenceResultList.Where((r) => r.ConsequenceType == filterByConsequenceType).ToArray();

        foreach (AggregatedConsequencesBinned aggregatedConsequencesBinned in res)
        {
            AggregatedConsequencesByQuantile aggregatedConsequencesByQuantile = AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(aggregatedConsequencesBinned);
            aggregatedConsequencesByQuantiles.Add(aggregatedConsequencesByQuantile);
        }
        StudyAreaConsequencesByQuantile studyAreaConsequencesByQuantile = new(aggregatedConsequencesByQuantiles);
        return studyAreaConsequencesByQuantile;
    }

    public XElement WriteToXML()
    {
        XElement masterElem = new("EAD_Results");
        foreach (AggregatedConsequencesBinned damageResult in ConsequenceResultList)
        {
            XElement damageResultElement = damageResult.WriteToXML();
            masterElem.Add(damageResultElement);
        }
        return masterElem;
    }

    public static StudyAreaConsequencesBinned ReadFromXML(XElement xElement)
    {
        List<AggregatedConsequencesBinned> damageResults = [];
        foreach (XElement histogramElement in xElement.Elements())
        {
            damageResults.Add(AggregatedConsequencesBinned.ReadFromXML(histogramElement));
        }
        return new StudyAreaConsequencesBinned(damageResults);
    }

    #endregion

    #region Aggregation
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
    /// <param name="impactAreaID"></param> the default is the null value utilities.IntegerConstants.DEFAULT_MISSING_VALUE
    /// <returns></returns>The mean of consequences
    public double SampleMeanDamage(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        return ConsequenceResultList
    .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
    .Sum(result => result.SampleMeanExpectedAnnualConsequences());
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
    /// <returns></returns>the level of consequences exceeded by the specified probability 
    public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability, string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        return ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
            .Sum(result => result.ConsequenceExceededWithProbabilityQ(exceedanceProbability));
    }
    /// <summary>
    /// This method returns a consequence result for the given damage category, asset category, and impact area 
    /// Impact area ID is used for alternative and alternative comparison reports 
    /// Impact area ID is utilities.IntegerConstants.DEFAULT_MISSING_VALUE otherwise 
    /// </summary>
    /// <param name="damageCategory"></param>
    /// <param name="assetCategory"></param>
    /// <param name="impactAreaID"></param>
    /// <returns>returns the existing result, else null if no result exists for that combination.</returns>
    public AggregatedConsequencesBinned GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        //Try to get specific result from the list. combination of damcat, asscat, imparea
        AggregatedConsequencesBinned result = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
            .FirstOrDefault();
        return result;
    }

    /// <summary>
    /// This method gets the histogram (distribution) of consequences for the given damage category(ies), asset category(ies), and impact area(s)
    /// The level of aggregation of the distribution of consequences is determined by the arguments used in the method
    /// For example, if you wanted a histogram for residential, impact area 2, all asset categories, then the method call would be as follows:
    /// ThreadsafeInlineHistogram histogram = GetConsequenceResultsHistogram(damageCategory: "residential", impactAreaID: 2);
    /// </summary>
    /// <param name="damageCategory"></param> The default is null 
    /// <param name="assetCategory"></param> The default is null 
    /// <param name="impactAreaID"></param> The default is a null value (utilities.IntegerConstants.DEFAULT_MISSING_VALUE)
    /// <returns></returns> Aggregated consequences histogram 
    public Empirical GetAggregateEmpiricalDistribution(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        var empiricalDistsToStack = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID, consequenceType, riskType)
            .Select(result => DynamicHistogram.ConvertToEmpiricalDistribution(result.ConsequenceHistogram))
            .ToList();
        if (empiricalDistsToStack.Count == 0)
        {
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return new Empirical();
        }
        return Empirical.StackEmpiricalDistributions(empiricalDistsToStack, Empirical.Sum);
    }
    #endregion
}