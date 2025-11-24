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
    public int RegionID { get; } = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
    public bool IsNull { get; }
    public ConvergenceCriteria ConvergenceCriteria { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// This constructor is only used for handling errors. 
    /// </summary>
    public AggregatedConsequencesBinned(int impactAreaID)
    {
        DamageCategory = "UNASSIGNED";
        AssetCategory = "UNASSIGNED";
        RegionID = impactAreaID;
        ConvergenceCriteria = new ConvergenceCriteria();
        ConsequenceHistogram = new DynamicHistogram();
        DamagedElementQuantityHistogram = new DynamicHistogram();
        IsNull = true;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];
    }
    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConvergenceCriteria = convergenceCriteria;
        IsNull = false;
        RegionID = impactAreaID;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];
        _HistogramNotConstructed = true;

    }

    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, IHistogram histogram, int impactAreaID)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConsequenceHistogram = histogram;
        ConvergenceCriteria = ConsequenceHistogram.ConvergenceCriteria;
        RegionID = impactAreaID;
        IsNull = false;
        _TempResults = new double[ConvergenceCriteria.IterationCount];
        _TempCounts = new double[ConvergenceCriteria.IterationCount];

    }

    public AggregatedConsequencesBinned(string damageCategory, string assetCategory, int impactAreaID)
    {
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
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
                binWidth = 1;
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
        return new AggregatedConsequencesByQuantile(consequenceDistributionResult.DamageCategory, consequenceDistributionResult.AssetCategory, empirical, consequenceDistributionResult.RegionID);
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
        return masterElement;
    }

    public static AggregatedConsequencesBinned ReadFromXML(XElement xElement)
    {
        IHistogram damageHistogram = DynamicHistogram.ReadFromXML(xElement.Element("DamageHistogram"));
        string damageCategory = xElement.Attribute("DamageCategory").Value;
        string assetCategory = xElement.Attribute("AssetCategory").Value;
        int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
        return new AggregatedConsequencesBinned(damageCategory, assetCategory, damageHistogram, id);
    }
    #endregion
}
