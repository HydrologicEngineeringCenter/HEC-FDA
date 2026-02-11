using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics;

public class AggregatedConsequencesBinned
{
    #region Fields
    private const int INITIAL_BIN_QUANTITY = 500;
    private readonly double[] _TempResults;
    private readonly double[] _TempCounts;
    private bool _HistogramNotConstructed = false;
    #endregion

    #region Properties
    public IHistogram ConsequenceHistogram { get; private set; }
    public IHistogram DamagedElementQuantityHistogram { get; private set; }
    public string DamageCategory { get; }
    public string AssetCategory { get; }
    public ConsequenceType ConsequenceType { get; }
    public RiskType RiskType { get; }
    public int RegionID { get; } = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
    public bool IsNull { get; }
    public ConvergenceCriteria ConvergenceCriteria { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// This constructor is only used for handling errors. 
    /// </summary>
    public AggregatedConsequencesBinned(int impactAreaID, ConsequenceType consequenceType, RiskType riskType)
    {
        DamageCategory = "UNASSIGNED";
        AssetCategory = "UNASSIGNED";
        ConsequenceType = consequenceType;
        RiskType = riskType;
        RegionID = impactAreaID;
        ConvergenceCriteria = new ConvergenceCriteria();
        ConsequenceHistogram = new DynamicHistogram();
        DamagedElementQuantityHistogram = new DynamicHistogram();
        IsNull = true;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];
    }
    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, ConsequenceType consequenceType, RiskType riskType = RiskType.Fail)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        ConvergenceCriteria = convergenceCriteria;
        IsNull = false;
        RegionID = impactAreaID;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];
        _HistogramNotConstructed = true;

    }

    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, IHistogram histogram, int impactAreaID, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        ConsequenceHistogram = histogram;
        ConvergenceCriteria = ConsequenceHistogram.ConvergenceCriteria;
        RegionID = impactAreaID;
        IsNull = false;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];

    }

    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, int impactAreaID, ConsequenceType consequenceType = ConsequenceType.Damage, RiskType riskType = RiskType.Fail)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        RegionID = impactAreaID;
        ConvergenceCriteria = new ConvergenceCriteria();
        ConsequenceHistogram = new DynamicHistogram();
        DamagedElementQuantityHistogram = new DynamicHistogram();
        IsNull = true;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];
    }
    #endregion

    #region Methods
    internal void PutDataIntoHistogram()
    {
        if (_HistogramNotConstructed)
        {
            double max = _TempResults.Max();
            double min = _TempResults.Min();
            double range = max - min;
            double binWidth;
            if (range < INITIAL_BIN_QUANTITY)
            {
                //this is cranked down from 1 to .001 because that's a more reasonable number for AALL. Potentially should be
                //even lower. Needs investigation. 
                binWidth = .001;
            }
            else
            {
                binWidth = range / INITIAL_BIN_QUANTITY;
            }
            ConsequenceHistogram = new DynamicHistogram(binWidth, ConvergenceCriteria);
            DamagedElementQuantityHistogram = new DynamicHistogram(binWidth: 1, ConvergenceCriteria);
            _HistogramNotConstructed = false;
        }
        ConsequenceHistogram.AddObservationsToHistogram(_TempResults);
        DamagedElementQuantityHistogram.AddObservationsToHistogram(_TempCounts);
        Array.Clear(_TempResults);
    }

    internal void AddConsequenceRealization(double damageRealization, long iteration = 1, int damagedElementsCount = 0)
    {
        _TempResults[iteration] = (damageRealization);
        _TempCounts[iteration] = (damagedElementsCount);

    }

    internal double SampleMeanExpectedAnnualConsequences()
    {
        return ConsequenceHistogram.SampleMean;
    }

    internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
    {
        double nonExceedanceProbability = 1 - exceedanceProbability;
        double quantile = ConsequenceHistogram.InverseCDF(nonExceedanceProbability);
        return quantile;
    }

    internal double QuantityExceededWithProbabilityQ(double exceedanceProbability)
    {
        double nonExceedanceProbability = 1 - exceedanceProbability;
        double quantile = DamagedElementQuantityHistogram.InverseCDF(nonExceedanceProbability);
        return quantile;
    }

    internal bool Equals(AggregatedConsequencesBinned damageResult)
    {
        bool histogramsMatch = ConsequenceHistogram.Equals(damageResult.ConsequenceHistogram);
        if (!histogramsMatch)
        {
            return false;
        }
        return true;
    }
    public static AggregatedConsequencesByQuantile ConvertToSingleEmpiricalDistributionOfConsequences(AggregatedConsequencesBinned consequenceDistributionResult)
    {
        Empirical empirical = DynamicHistogram.ConvertToEmpiricalDistribution(consequenceDistributionResult.ConsequenceHistogram);
        return new AggregatedConsequencesByQuantile(consequenceDistributionResult.DamageCategory, consequenceDistributionResult.AssetCategory, empirical, consequenceDistributionResult.RegionID, consequenceDistributionResult.ConsequenceType);
    }
    public XElement WriteToXML()
    {
        XElement masterElement = new("ConsequenceResult");
        XElement histogramElement = ConsequenceHistogram.ToXML();
        histogramElement.Name = "DamageHistogram";
        masterElement.Add(histogramElement);
        masterElement.SetAttributeValue("DamageCategory", DamageCategory);
        masterElement.SetAttributeValue("AssetCategory", AssetCategory);
        masterElement.SetAttributeValue("ImpactAreaID", RegionID);
        masterElement.SetAttributeValue("ConsequenceType", ConsequenceType);
        masterElement.SetAttributeValue("RiskType", RiskType);
        return masterElement;
    }

    public static AggregatedConsequencesBinned ReadFromXML(XElement xElement)
    {
        IHistogram damageHistogram = DynamicHistogram.ReadFromXML(xElement.Element("DamageHistogram"));
        string damageCategory = xElement.Attribute("DamageCategory").Value;
        string assetCategory = xElement.Attribute("AssetCategory").Value;
        int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);

        // This allows for backward compatibility -- if we are loading an object saved before the enum existed,
        // it will default to damage because that is all the used to exist
        // anything saved after the enum was introduced will have the attribute and be parsed accordingly
        ConsequenceType consequenceType = ConsequenceType.Damage;
        var typeAttr = xElement.Attribute("ConsequenceType");
        if (typeAttr != null && Enum.TryParse<ConsequenceType>(typeAttr.Value, out var parsed))
            consequenceType = parsed;

        // Backward compatibility for RiskType - defaults to Unassigned for old saved results
        RiskType riskType = RiskType.Unassigned;
        var riskTypeAttr = xElement.Attribute("RiskType");
        if (riskTypeAttr != null && Enum.TryParse<RiskType>(riskTypeAttr.Value, out var parsedRiskType))
            riskType = parsedRiskType;

        return new AggregatedConsequencesBinned(damageCategory, assetCategory, damageHistogram, id, consequenceType, riskType);
    }
    #endregion
}
