using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using Xunit;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class UncertainConsequenceFrequencyCurveShould
    {
        private static readonly double[] TestXvals = { 0.01, 0.1, 0.5, 0.9, 0.99 };
        private static readonly string TestDamageCategory = "Residential";
        private static readonly string TestAssetCategory = "Structure";

        [Fact]
        public void AccumulateYValuesIntoHistograms()
        {
            // Arrange
            int batchSize = 100;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            CategoriedUncertainPairedData uncertainCurve = new(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Act - add a batch of curves with known y-values
            for (int i = 0; i < batchSize; i++)
            {
                double[] yvals = { 100 + i, 200 + i, 300 + i, 400 + i, 500 + i };
                PairedData curve = new(TestXvals, yvals);
                uncertainCurve.AddCurveRealization(curve, i);
            }
            uncertainCurve.PutDataIntoHistograms();

            // Assert - histograms should have been created for each x-value position
            Assert.Equal(TestXvals.Length, uncertainCurve.YHistograms.Count);
            foreach (var histogram in uncertainCurve.YHistograms)
            {
                Assert.Equal(batchSize, histogram.SampleSize);
            }
        }

        [Fact]
        public void ReturnCorrectMeanCurve()
        {
            // Arrange
            int batchSize = 100;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            CategoriedUncertainPairedData uncertainCurve = new(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Add curves with constant y-values of 100, 200, 300, 400, 500
            for (int i = 0; i < batchSize; i++)
            {
                double[] yvals = { 100.0, 200.0, 300.0, 400.0, 500.0 };
                PairedData curve = new(TestXvals, yvals);
                uncertainCurve.AddCurveRealization(curve, i);
            }
            uncertainCurve.PutDataIntoHistograms();

            // Act
            UncertainPairedData upd = uncertainCurve.GetUncertainPairedData();
            double[] meanYvals = new double[upd.Yvals.Length];
            for (int i = 0; i < upd.Yvals.Length; i++)
            {
                meanYvals[i] = ((IHistogram)upd.Yvals[i]).SampleMean;
            }

            // Assert - mean should be very close to the constant values
            double tolerance = 0.01;
            Assert.Equal(100.0, meanYvals[0], tolerance);
            Assert.Equal(200.0, meanYvals[1], tolerance);
            Assert.Equal(300.0, meanYvals[2], tolerance);
            Assert.Equal(400.0, meanYvals[3], tolerance);
            Assert.Equal(500.0, meanYvals[4], tolerance);
        }

        [Fact]
        public void ReturnCorrectQuantileCurve()
        {
            // Arrange
            int batchSize = 100;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            CategoriedUncertainPairedData uncertainCurve = new(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Add curves with y-values that range from 0 to 99 for each position
            for (int i = 0; i < batchSize; i++)
            {
                double[] yvals = { i * 1.0, i * 2.0, i * 3.0, i * 4.0, i * 5.0 };
                PairedData curve = new(TestXvals, yvals);
                uncertainCurve.AddCurveRealization(curve, i);
            }
            uncertainCurve.PutDataIntoHistograms();

            // Act - get 50th percentile (median)
            UncertainPairedData upd = uncertainCurve.GetUncertainPairedData();
            PairedData medianCurve = upd.SamplePairedDataRaw(0.5);

            // Assert - median should be approximately at 50% of the range
            // For values 0-99, median is around 49.5
            double tolerance = 5.0; // Allow some tolerance due to histogram binning
            Assert.True(Math.Abs(medianCurve.Yvals[0] - 49.5) < tolerance);
            Assert.True(Math.Abs(medianCurve.Yvals[1] - 99.0) < tolerance);
            Assert.True(Math.Abs(medianCurve.Yvals[2] - 148.5) < tolerance);
        }

        [Fact]
        public void AccumulateMultipleBatchesCorrectly()
        {
            // Arrange - use batchSize = 100 to match default IterationCount
            int batchSize = 100;
            int numBatches = 3;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            CategoriedUncertainPairedData uncertainCurve = new(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Act - add multiple batches
            for (int batch = 0; batch < numBatches; batch++)
            {
                for (int i = 0; i < batchSize; i++)
                {
                    double[] yvals = { 100.0, 200.0, 300.0, 400.0, 500.0 };
                    PairedData curve = new(TestXvals, yvals);
                    uncertainCurve.AddCurveRealization(curve, i);
                }
                uncertainCurve.PutDataIntoHistograms();
            }

            // Assert - total sample size should be batchSize * numBatches
            int expectedSampleSize = batchSize * numBatches;
            foreach (var histogram in uncertainCurve.YHistograms)
            {
                Assert.Equal(expectedSampleSize, histogram.SampleSize);
            }
        }

        [Fact]
        public void PreserveMetadata()
        {
            // Arrange
            ConvergenceCriteria convergenceCriteria = new(minIterations: 100, maxIterations: 1000);

            // Act
            CategoriedUncertainPairedData uncertainCurve = new(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.LifeLoss,
                RiskType.Non_Fail,
                convergenceCriteria);

            // Assert
            Assert.Equal(TestDamageCategory, uncertainCurve.DamageCategory);
            Assert.Equal(TestAssetCategory, uncertainCurve.AssetCategory);
            Assert.Equal(ConsequenceType.LifeLoss, uncertainCurve.ConsequenceType);
            Assert.Equal(RiskType.Non_Fail, uncertainCurve.RiskType);
            Assert.Equal(TestXvals.Length, uncertainCurve.Xvals.Count);
        }

        [Fact]
        public void CreateFromConsequenceFrequencyCurve()
        {
            // Arrange
            double[] yvals = { 100.0, 200.0, 300.0, 400.0, 500.0 };
            PairedData pairedData = new(TestXvals, yvals);
            CategoriedPairedData initialCurve = new(
                pairedData,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail);
            ConvergenceCriteria convergenceCriteria = new(minIterations: 100, maxIterations: 1000);

            // Act
            CategoriedUncertainPairedData uncertainCurve = new(initialCurve, convergenceCriteria);

            // Assert
            Assert.Equal(TestDamageCategory, uncertainCurve.DamageCategory);
            Assert.Equal(TestAssetCategory, uncertainCurve.AssetCategory);
            Assert.Equal(ConsequenceType.Damage, uncertainCurve.ConsequenceType);
            Assert.Equal(RiskType.Fail, uncertainCurve.RiskType);
            Assert.Equal(TestXvals.Length, uncertainCurve.Xvals.Count);
        }

        [Fact]
        public void GetOrCreateFindsExistingCurve()
        {
            // Arrange
            ConvergenceCriteria convergenceCriteria = new(minIterations: 100, maxIterations: 1000);
            ImpactAreaScenarioResults results = new(impactAreaID: 1);

            // Act - create first curve
            CategoriedUncertainPairedData curve1 = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Act - get same curve again
            CategoriedUncertainPairedData curve2 = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals,
                TestDamageCategory,
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            // Assert - should be the same object
            Assert.Same(curve1, curve2);
            Assert.Single(results.UncertainConsequenceFrequencyCurves);
        }

        [Fact]
        public void GetOrCreateCreatesNewCurveForDifferentMetadata()
        {
            // Arrange
            ConvergenceCriteria convergenceCriteria = new(minIterations: 100, maxIterations: 1000);
            ImpactAreaScenarioResults results = new(impactAreaID: 1);

            // Act - create curves with different metadata
            CategoriedUncertainPairedData residentialCurve = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals,
                "Residential",
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            CategoriedUncertainPairedData commercialCurve = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals,
                "Commercial",
                TestAssetCategory,
                ConsequenceType.Damage,
                RiskType.Fail,
                convergenceCriteria);

            CategoriedUncertainPairedData lifeLossCurve = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals,
                "Residential",
                TestAssetCategory,
                ConsequenceType.LifeLoss,
                RiskType.Fail,
                convergenceCriteria);

            // Assert - should have three separate curves
            Assert.Equal(3, results.UncertainConsequenceFrequencyCurves.Count);
            Assert.NotSame(residentialCurve, commercialCurve);
            Assert.NotSame(residentialCurve, lifeLossCurve);
        }

        [Fact]
        public void PutUncertainFrequencyCurvesIntoHistogramsFlushesAllCurves()
        {
            // Arrange - use batchSize = 100 to match default IterationCount
            int batchSize = 100;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            ImpactAreaScenarioResults results = new(impactAreaID: 1);

            // Create two curves
            CategoriedUncertainPairedData curve1 = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals, "Residential", "Structure", ConsequenceType.Damage, RiskType.Fail, convergenceCriteria);
            CategoriedUncertainPairedData curve2 = results.GetOrCreateUncertainConsequenceFrequencyCurve(
                TestXvals, "Commercial", "Structure", ConsequenceType.Damage, RiskType.Fail, convergenceCriteria);

            // Add data to both curves
            for (int i = 0; i < batchSize; i++)
            {
                double[] yvals = { 100.0, 200.0, 300.0, 400.0, 500.0 };
                PairedData curve = new(TestXvals, yvals);
                curve1.AddCurveRealization(curve, i);
                curve2.AddCurveRealization(curve, i);
            }

            // Act
            results.PutUncertainFrequencyCurvesIntoHistograms();

            // Assert - both curves should have histograms with the correct sample size
            foreach (var histogram in curve1.YHistograms)
            {
                Assert.Equal(batchSize, histogram.SampleSize);
            }
            foreach (var histogram in curve2.YHistograms)
            {
                Assert.Equal(batchSize, histogram.SampleSize);
            }
        }

        [Fact]
        public void EnsureDeterministicResultsWithBatchedAdds()
        {
            // This test verifies that adding curves in batches produces deterministic results
            // by running the same sequence twice and comparing results

            // Arrange
            int batchSize = 100;
            ConvergenceCriteria convergenceCriteria = new(minIterations: batchSize, maxIterations: batchSize * 10);
            Random random = new(42); // Fixed seed for reproducibility

            // First run
            CategoriedUncertainPairedData curve1 = new(
                TestXvals, TestDamageCategory, TestAssetCategory,
                ConsequenceType.Damage, RiskType.Fail, convergenceCriteria);

            double[][] randomYvals = new double[batchSize][];
            for (int i = 0; i < batchSize; i++)
            {
                randomYvals[i] = new double[] {
                    random.NextDouble() * 100,
                    random.NextDouble() * 200,
                    random.NextDouble() * 300,
                    random.NextDouble() * 400,
                    random.NextDouble() * 500
                };
            }

            // Add in order 0, 1, 2, ...
            for (int i = 0; i < batchSize; i++)
            {
                PairedData curve = new(TestXvals, randomYvals[i]);
                curve1.AddCurveRealization(curve, i);
            }
            curve1.PutDataIntoHistograms();

            // Second run with same data added in same order
            CategoriedUncertainPairedData curve2 = new(
                TestXvals, TestDamageCategory, TestAssetCategory,
                ConsequenceType.Damage, RiskType.Fail, convergenceCriteria);

            for (int i = 0; i < batchSize; i++)
            {
                PairedData curve = new(TestXvals, randomYvals[i]);
                curve2.AddCurveRealization(curve, i);
            }
            curve2.PutDataIntoHistograms();

            // Assert - both curves should have identical means
            UncertainPairedData upd1 = curve1.GetUncertainPairedData();
            UncertainPairedData upd2 = curve2.GetUncertainPairedData();

            for (int i = 0; i < TestXvals.Length; i++)
            {
                double mean1 = ((IHistogram)upd1.Yvals[i]).SampleMean;
                double mean2 = ((IHistogram)upd2.Yvals[i]).SampleMean;
                Assert.Equal(mean1, mean2, precision: 10);
            }
        }
    }
}
