using HEC.FDA.Model.alternativeComparisonReport;
using HEC.FDA.Model.metrics;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HEC.FDA.ModelTest.unittests;

/// <summary>
/// Tests that the consolidated EAD methods correctly return life loss (AALL) reduced results
/// alongside damage reduced results, verifying that the separate AALL computation path is no longer needed.
/// </summary>
public class AlternativeComparisonReportConsolidationTests
{
    private const int ImpactAreaID = 1;
    private const int WithoutProjectAltID = 1;
    private const int WithProjectAltID = 2;
    private const string DamageCategory = "residential";
    private const string AssetCategory = "structure";
    private const int BaseYear = 2025;
    private const int FutureYear = 2075;
    private const int PeriodOfAnalysis = 50;

    /// <summary>
    /// Creates an AlternativeResults with base and future year scenario results containing
    /// both Damage and LifeLoss consequences with known histogram values.
    /// </summary>
    private static AlternativeResults CreateAlternativeResults(int altID, double baseDamageValue, double futureDamageValue, double baseLifeLossValue, double futureLifeLossValue)
    {
        ConvergenceCriteria cc = new(minIterations: 1, maxIterations: 1);

        // Base year scenario
        ScenarioResults baseScenario = new();
        ImpactAreaScenarioResults baseImpactArea = new(ImpactAreaID);
        baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
            CreateBinnedConsequence(baseDamageValue, ConsequenceType.Damage, cc));
        baseImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
            CreateBinnedConsequence(baseLifeLossValue, ConsequenceType.LifeLoss, cc));
        baseScenario.AddResults(baseImpactArea);

        // Future year scenario
        ScenarioResults futureScenario = new();
        ImpactAreaScenarioResults futureImpactArea = new(ImpactAreaID);
        futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
            CreateBinnedConsequence(futureDamageValue, ConsequenceType.Damage, cc));
        futureImpactArea.ConsequenceResults.AddExistingConsequenceResultObject(
            CreateBinnedConsequence(futureLifeLossValue, ConsequenceType.LifeLoss, cc));
        futureScenario.AddResults(futureImpactArea);

        AlternativeResults altResults = new(altID, [BaseYear, FutureYear], PeriodOfAnalysis);
        altResults.BaseYearScenarioResults = baseScenario;
        altResults.FutureYearScenarioResults = futureScenario;
        return altResults;
    }

    private static AggregatedConsequencesBinned CreateBinnedConsequence(double value, ConsequenceType consequenceType, ConvergenceCriteria cc)
    {
        // Create a histogram from a list of identical values so the mean is deterministic
        List<double> data = Enumerable.Repeat(value, 100).ToList();
        DynamicHistogram histogram = new(data, cc);
        return new AggregatedConsequencesBinned(DamageCategory, AssetCategory, histogram, ImpactAreaID, consequenceType);
    }

    private static AlternativeComparisonReportResults ComputeResults(
        double wopBaseDamage, double wopFutureDamage, double wopBaseLL, double wopFutureLL,
        double wpBaseDamage, double wpFutureDamage, double wpBaseLL, double wpFutureLL)
    {
        AlternativeResults withoutProject = CreateAlternativeResults(WithoutProjectAltID, wopBaseDamage, wopFutureDamage, wopBaseLL, wopFutureLL);
        AlternativeResults withProject = CreateAlternativeResults(WithProjectAltID, wpBaseDamage, wpFutureDamage, wpBaseLL, wpFutureLL);
        return AlternativeComparisonReport.ComputeAlternativeComparisonReport(withoutProject, [withProject]);
    }

    [Fact]
    public void BaseYearDamageReduced_ReturnsCorrectMean()
    {
        // Without project base damage = 1000, with project = 300
        // Expected reduced = 1000 - 300 = 700
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        double reduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.Damage);
        Assert.Equal(700, reduced, 1);
    }

    [Fact]
    public void FutureYearDamageReduced_ReturnsCorrectMean()
    {
        // Without project future damage = 2000, with project = 500
        // Expected reduced = 2000 - 500 = 1500
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        double reduced = results.SampleMeanFutureYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.Damage);
        Assert.Equal(1500, reduced, 1);
    }

    [Fact]
    public void BaseYearLifeLossReduced_ReturnsCorrectMean()
    {
        // Without project base life loss = 10, with project = 3
        // Expected reduced = 10 - 3 = 7
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        double reduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.LifeLoss);
        Assert.Equal(7, reduced, 1);
    }

    [Fact]
    public void FutureYearLifeLossReduced_ReturnsCorrectMean()
    {
        // Without project future life loss = 20, with project = 5
        // Expected reduced = 20 - 5 = 15
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        double reduced = results.SampleMeanFutureYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.LifeLoss);
        Assert.Equal(15, reduced, 1);
    }

    [Fact]
    public void HasReducedResultsOfType_ReturnsTrueForLifeLoss()
    {
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        Assert.True(results.HasReducedResultsOfType(ConsequenceType.LifeLoss));
    }

    [Fact]
    public void HasReducedResultsOfType_ReturnsTrueForDamage()
    {
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        Assert.True(results.HasReducedResultsOfType(ConsequenceType.Damage));
    }

    [Fact]
    public void GetReducedAlternativeIDs_ReturnsCorrectIDs()
    {
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        List<int> lifeLossAltIDs = results.GetReducedAlternativeIDs(ConsequenceType.LifeLoss);
        Assert.Contains(WithProjectAltID, lifeLossAltIDs);
    }

    [Fact]
    public void GetReducedImpactAreaIDs_ReturnsCorrectIDs()
    {
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        List<int> lifeLossImpactAreaIDs = results.GetReducedImpactAreaIDs(ConsequenceType.LifeLoss);
        Assert.Contains(ImpactAreaID, lifeLossImpactAreaIDs);
    }

    [Fact]
    public void DamageAndLifeLossReduced_AreIndependent()
    {
        // Verify that querying damage vs life loss returns different values
        var results = ComputeResults(
            wopBaseDamage: 1000, wopFutureDamage: 2000, wopBaseLL: 10, wopFutureLL: 20,
            wpBaseDamage: 300, wpFutureDamage: 500, wpBaseLL: 3, wpFutureLL: 5);

        double damageReduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.Damage);
        double lifeLossReduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.LifeLoss);

        Assert.NotEqual(damageReduced, lifeLossReduced);
        Assert.Equal(700, damageReduced, 1);
        Assert.Equal(7, lifeLossReduced, 1);
    }

    [Fact]
    public void ZeroReduction_WhenWithAndWithoutProjectAreEqual()
    {
        // Same values for with and without project = zero reduction
        var results = ComputeResults(
            wopBaseDamage: 500, wopFutureDamage: 500, wopBaseLL: 5, wopFutureLL: 5,
            wpBaseDamage: 500, wpFutureDamage: 500, wpBaseLL: 5, wpFutureLL: 5);

        double damageReduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.Damage);
        double lifeLossReduced = results.SampleMeanBaseYearEADReduced(WithProjectAltID, ImpactAreaID, DamageCategory, AssetCategory, ConsequenceType.LifeLoss);

        Assert.Equal(0, damageReduced, 1);
        Assert.Equal(0, lifeLossReduced, 1);
    }
}
