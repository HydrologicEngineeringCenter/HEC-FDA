using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;
using HEC.FDA.Model.paireddata;
using System;
using System.Linq;
using HEC.FDA.Model.metrics.Extensions;
using Statistics.Distributions;

namespace HEC.FDA.Model.metrics;

public class StudyAreaConsequencesBinned : ValidationErrorLogger
{
    public List<AggregatedConsequencesBinned> ConsequenceResultList { get; }
    //this needs to be an error report
    public bool IsNull { get; }
    internal int AlternativeID { get; }

    #region Constructors
    public StudyAreaConsequencesBinned()
    {
        ConsequenceResultList = [];
        AggregatedConsequencesBinned dummyConsequenceDistributionResult = new();
        ConsequenceResultList.Add(dummyConsequenceDistributionResult);
        IsNull = true;

        //create an array to collect data the side of the convergence criteria iteration count 
    }
    public StudyAreaConsequencesBinned(bool isNull)
    {
        ConsequenceResultList = [];
        IsNull = isNull;
    }
    internal StudyAreaConsequencesBinned(int alternativeID)
    {
        ConsequenceResultList = [];
        AlternativeID = alternativeID;
        IsNull = false;
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
    internal void AddNewConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
    {
        AggregatedConsequencesBinned damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
        if (damageResult.IsNull)
        {
            AggregatedConsequencesBinned newDamageResult = new(damageCategory, assetCategory, convergenceCriteria, impactAreaID);
            ConsequenceResultList.Add(newDamageResult);
        }
    }
    //public for testing purposes
    public void AddExistingConsequenceResultObject(AggregatedConsequencesBinned consequenceResultToAdd)
    {
        AggregatedConsequencesBinned consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
        if (consequenceResult.IsNull)
        {
            ConsequenceResultList.Add(consequenceResultToAdd);
        }
    }
    //This approach is used in binning EAD results
    internal void AddConsequenceRealization(double damageEstimate, string damageCategory, string assetCategory, int impactAreaID, long iteration)
    {
        AggregatedConsequencesBinned damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
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
            AggregatedConsequencesBinned inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID);
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

    public static StudyAreaConsequencesByQuantile ConvertToStudyAreaConsequencesByQuantile(StudyAreaConsequencesBinned studyAreaConsequencesBinned)
    {
        List<AggregatedConsequencesByQuantile> aggregatedConsequencesByQuantiles = [];
        foreach (AggregatedConsequencesBinned aggregatedConsequencesBinned in studyAreaConsequencesBinned.ConsequenceResultList)
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
    public double MeanDamage(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
    {
        return ConsequenceResultList
    .FilterByCategories(damageCategory, assetCategory, impactAreaID)
    .Sum(result => result.MeanExpectedAnnualConsequences());
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
    public double ConsequenceExceededWithProbabilityQ(double exceedanceProbability, string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
    {
        return ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
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
    /// <returns></returns>
    public AggregatedConsequencesBinned GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
    {
        AggregatedConsequencesBinned result = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
            .FirstOrDefault();
        if (result != null)
        {
            return result;
        }
        string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
        ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
        ReportMessage(this, new MessageEventArgs(errorMessage));
        AggregatedConsequencesBinned dummyResult = new();
        return dummyResult;
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
    public Empirical GetAggregateEmpiricalDistribution(string damageCategory = null, string assetCategory = null, int impactAreaID = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE)
    {
        var empiricalDistsToStack = ConsequenceResultList
            .FilterByCategories(damageCategory, assetCategory, impactAreaID)
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