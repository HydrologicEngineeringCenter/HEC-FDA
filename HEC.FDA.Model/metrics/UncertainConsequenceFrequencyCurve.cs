using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.metrics;

/// <summary>
/// Represents an uncertain consequence-frequency curve that aggregates multiple
/// ConsequenceFrequencyCurve samples into histograms for each ordinate position.
/// Curves must be added in batches to ensure deterministic results when using parallel computation.
/// </summary>
public class UncertainConsequenceFrequencyCurve
{
    #region Fields
    private const int INITIAL_BIN_QUANTITY = 500;
    private readonly double[][] _TempYValues;
    private readonly int _BatchSize;
    private bool _HistogramsNotConstructed = true;
    private double[] _Xvals;
    #endregion

    #region Properties
    /// <summary>
    /// The x-values (typically exceedance probabilities) shared by all curves.
    /// </summary>
    public IReadOnlyList<double> Xvals => _Xvals;

    /// <summary>
    /// Histograms for each y-value position in the curves.
    /// </summary>
    public List<DynamicHistogram> YHistograms { get; private set; } = [];

    /// <summary>
    /// The type of consequence (Damage or LifeLoss).
    /// </summary>
    public ConsequenceType ConsequenceType { get; }

    /// <summary>
    /// The type of risk (Fail, Non_Fail, or Total).
    /// </summary>
    public RiskType RiskType { get; }

    /// <summary>
    /// The damage category (e.g., "Residential", "Commercial").
    /// </summary>
    public string DamageCategory { get; }

    /// <summary>
    /// The asset category (e.g., "Structure", "Content").
    /// </summary>
    public string AssetCategory { get; }

    /// <summary>
    /// The convergence criteria used for the histograms.
    /// </summary>
    public ConvergenceCriteria ConvergenceCriteria { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates an UncertainConsequenceFrequencyCurve with the specified metadata and convergence criteria.
    /// </summary>
    /// <param name="xvals">The x-values (exceedance probabilities) for all curves.</param>
    /// <param name="damageCategory">The damage category.</param>
    /// <param name="assetCategory">The asset category.</param>
    /// <param name="consequenceType">The type of consequence.</param>
    /// <param name="riskType">The type of risk.</param>
    /// <param name="convergenceCriteria">The convergence criteria for the histograms.</param>
    public UncertainConsequenceFrequencyCurve(
        double[] xvals,
        string damageCategory,
        string assetCategory,
        ConsequenceType consequenceType,
        RiskType riskType,
        ConvergenceCriteria convergenceCriteria)
    {
        _Xvals = xvals;
        DamageCategory = damageCategory;
        AssetCategory = assetCategory;
        ConsequenceType = consequenceType;
        RiskType = riskType;
        ConvergenceCriteria = convergenceCriteria;
        _BatchSize = convergenceCriteria.IterationCount;

        // Initialize temp arrays for each y-value position
        _TempYValues = new double[xvals.Length][];
        for (int i = 0; i < xvals.Length; i++)
        {
            _TempYValues[i] = new double[_BatchSize];
        }
    }

    /// <summary>
    /// Creates an UncertainConsequenceFrequencyCurve from an initial ConsequenceFrequencyCurve.
    /// The x-values are extracted from the curve.
    /// </summary>
    /// <param name="initialCurve">The first curve to use for initialization.</param>
    /// <param name="convergenceCriteria">The convergence criteria for the histograms.</param>
    public UncertainConsequenceFrequencyCurve(
        ConsequenceFrequencyCurve initialCurve,
        ConvergenceCriteria convergenceCriteria)
        : this(
            initialCurve.FrequencyCurve.Xvals.ToArray(),
            initialCurve.DamageCategory,
            initialCurve.AssetCategory,
            initialCurve.ConsequenceType,
            initialCurve.RiskType,
            convergenceCriteria)
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// Adds a ConsequenceFrequencyCurve to the batch at the specified iteration index.
    /// The curve's y-values are stored in temp arrays until PutDataIntoHistograms is called.
    /// </summary>
    /// <param name="curve">The curve to add.</param>
    /// <param name="iterationIndex">The index within the current batch (0 to BatchSize-1).</param>
    /// <exception cref="ArgumentException">Thrown if the curve's x-values don't match.</exception>
    public void AddCurveRealization(ConsequenceFrequencyCurve curve, long iterationIndex)
    {
        ValidateCurve(curve);

        IReadOnlyList<double> yvals = curve.FrequencyCurve.Yvals;
        for (int i = 0; i < yvals.Count; i++)
        {
            _TempYValues[i][iterationIndex] = yvals[i];
        }
    }

    /// <summary>
    /// Adds a PairedData curve to the batch at the specified iteration index.
    /// </summary>
    /// <param name="frequencyCurve">The paired data curve to add.</param>
    /// <param name="iterationIndex">The index within the current batch.</param>
    public void AddCurveRealization(PairedData frequencyCurve, long iterationIndex)
    {
        IReadOnlyList<double> yvals = frequencyCurve.Yvals;
        for (int i = 0; i < yvals.Count; i++)
        {
            _TempYValues[i][iterationIndex] = yvals[i];
        }
    }

    /// <summary>
    /// Flushes the batch of curves from temp arrays into the histograms.
    /// Should be called after each batch of iterations completes.
    /// </summary>
    public void PutDataIntoHistograms()
    {
        if (_HistogramsNotConstructed)
        {
            InitializeHistograms();
            _HistogramsNotConstructed = false;
        }

        for (int i = 0; i < _TempYValues.Length; i++)
        {
            YHistograms[i].AddObservationsToHistogram(_TempYValues[i]);
            Array.Clear(_TempYValues[i]);
        }
    }

    /// <summary>
    /// Gets the mean y-value at each x-value position.
    /// </summary>
    /// <returns>A PairedData representing the mean consequence-frequency curve.</returns>
    public PairedData GetMeanCurve()
    {
        double[] meanYvals = new double[YHistograms.Count];
        for (int i = 0; i < YHistograms.Count; i++)
        {
            meanYvals[i] = YHistograms[i].SampleMean;
        }
        return new PairedData(_Xvals, meanYvals);
    }

    /// <summary>
    /// Gets the y-value at a specified quantile for each x-value position.
    /// </summary>
    /// <param name="probability">The non-exceedance probability (0 to 1).</param>
    /// <returns>A PairedData representing the quantile consequence-frequency curve.</returns>
    public PairedData GetQuantileCurve(double probability)
    {
        double[] quantileYvals = new double[YHistograms.Count];
        for (int i = 0; i < YHistograms.Count; i++)
        {
            quantileYvals[i] = YHistograms[i].InverseCDF(probability);
        }
        return new PairedData(_Xvals, quantileYvals);
    }

    private void InitializeHistograms()
    {
        YHistograms = new List<DynamicHistogram>(_TempYValues.Length);
        for (int i = 0; i < _TempYValues.Length; i++)
        {
            double max = _TempYValues[i].Max();
            double min = _TempYValues[i].Min();
            double range = max - min;
            double binWidth;
            if (range < INITIAL_BIN_QUANTITY)
            {
                binWidth = 0.001;
            }
            else
            {
                binWidth = range / INITIAL_BIN_QUANTITY;
            }
            YHistograms.Add(new DynamicHistogram(binWidth, ConvergenceCriteria));
        }
    }

    private void ValidateCurve(ConsequenceFrequencyCurve curve)
    {
        if (curve.DamageCategory != DamageCategory)
        {
            throw new ArgumentException($"Damage category mismatch: expected '{DamageCategory}', got '{curve.DamageCategory}'");
        }
        if (curve.AssetCategory != AssetCategory)
        {
            throw new ArgumentException($"Asset category mismatch: expected '{AssetCategory}', got '{curve.AssetCategory}'");
        }
        if (curve.ConsequenceType != ConsequenceType)
        {
            throw new ArgumentException($"Consequence type mismatch: expected '{ConsequenceType}', got '{curve.ConsequenceType}'");
        }
        if (curve.RiskType != RiskType)
        {
            throw new ArgumentException($"Risk type mismatch: expected '{RiskType}', got '{curve.RiskType}'");
        }
        if (curve.FrequencyCurve.Xvals.Count != _Xvals.Length)
        {
            throw new ArgumentException($"X-values count mismatch: expected {_Xvals.Length}, got {curve.FrequencyCurve.Xvals.Count}");
        }
    }
    #endregion
}
