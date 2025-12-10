using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.utilities;
using Statistics;
using System;
using Xunit;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class GraphicalFrequencyUncertaintyCalculatorsTests
    {
        [Theory]
        [InlineData(new double[] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 },
                    new double[] { 13.388, 15.484, 16.702, 17.973, 18.849, 19.524, 20.173, 20.979 },
                    48, true)]
        [InlineData(new double[] { 0.99, 0.5, 0.1, 0.02, 0.01, 0.002 },
                    new double[] { 500, 2000, 34900, 66900, 86000, 146000 },
                    5, false)]
        [InlineData(new double[] { 0.99, 0.95, 0.90, 0.85, 0.8, 0.75, 0.7, 0.65, 0.6, 0.55, 0.5, 0.45, 0.4, 0.35, 0.3, 0.25, 0.2, 0.15, 0.1, 0.05, 0.02, 0.01, 0.005, 0.0025 },
                    new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
                    20, true)]
        [InlineData(new double[] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 },
                    new double[] { 1000, 2000, 5000, 10000, 20000, 30000, 50000, 80000 },
                    20, false)]  // Another flows test case
        public void LessSimpleMethod_ProducesEquivalentResultsToGraphicalDistribution(
            double[] exceedanceProbabilities,
            double[] stagesOrFlows,
            int equivalentRecordLength,
            bool usingStagesNotFlows)
        {
            // Arrange
            var curveMetaData = new CurveMetaData("x label", "name", "residential");

            // Act - Create using the new static method
            var uncertainPairedData = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                exceedanceProbabilities,
                stagesOrFlows,
                usingStagesNotFlows,
                equivalentRecordLength,
                curveMetaData);

            // Act - Create using the existing GraphicalUncertainPairedData (which uses GraphicalDistribution internally)
            var graphicalUncertain = new GraphicalUncertainPairedData(
                exceedanceProbabilities,
                stagesOrFlows,
                equivalentRecordLength,
                curveMetaData,
                usingStagesNotFlows);

            // Assert - Both implementations should produce the same number of probability points
            // (after extrapolation and filling with required probabilities)
            Assert.Equal(graphicalUncertain.CombinedExceedanceProbabilities.Length, uncertainPairedData.Xvals.Length);
            Assert.Equal(graphicalUncertain.CombinedExceedanceProbabilities.Length, uncertainPairedData.Yvals.Length);

            // Compare X values (non-exceedance probabilities)
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                double expectedX = 1 - graphicalUncertain.CombinedExceedanceProbabilities[i];
                Assert.Equal(expectedX, uncertainPairedData.Xvals[i], 0.00001);
            }

            // Compare distributions directly - this tests that both implementations produce
            // identical probability distributions at each point (same means and standard deviations)
            double[] testProbs = { 0.5, 0.85, 0.15 };
            foreach (double curveSampleProb in testProbs)
            {
                for (int i = 0; i < uncertainPairedData.Yvals.Length; i++)
                {
                    // Sample directly from distributions to avoid monotonicity enforcement differences
                    double newValue = uncertainPairedData.Yvals[i].InverseCDF(curveSampleProb);
                    double origValue = graphicalUncertain.GraphicalDistributionWithLessSimple.StageOrLogFlowDistributions[i].InverseCDF(curveSampleProb);

                    // Y values should match with tight tolerance
                    double tolerance = Math.Max(Math.Abs(origValue) * 0.0001, 0.0001);
                    Assert.Equal(origValue, newValue, tolerance);
                }
            }
        }

        [Fact]
        public void LessSimpleMethod_WithNullExceedanceProbabilities_ThrowsArgumentNullException()
        {
            // Arrange
            double[] exceedanceProbabilities = null;
            double[] stagesOrFlows = { 1.0, 2.0 };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                    exceedanceProbabilities, stagesOrFlows, true));
        }

        [Fact]
        public void LessSimpleMethod_WithNullStagesOrFlows_ThrowsArgumentNullException()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5, 0.1 };
            double[] stagesOrFlows = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                    exceedanceProbabilities, stagesOrFlows, true));
        }

        [Fact]
        public void LessSimpleMethod_WithMismatchedArrayLengths_ThrowsArgumentException()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5, 0.1, 0.02 };
            double[] stagesOrFlows = { 1.0, 2.0 }; // Different length

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                    exceedanceProbabilities, stagesOrFlows, true));
            Assert.Contains("same length", exception.Message);
        }

        [Fact]
        public void LessSimpleMethod_WithInsufficientData_ThrowsArgumentException()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5 }; // Only one point
            double[] stagesOrFlows = { 1.0 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                    exceedanceProbabilities, stagesOrFlows, true));
            Assert.Contains("At least 2 data points", exception.Message);
        }

        [Fact]
        public void LessSimpleMethod_WithInvalidEquivalentRecordLength_ThrowsArgumentException()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5, 0.1 };
            double[] stagesOrFlows = { 1.0, 2.0 };
            int invalidERL = 0;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                    exceedanceProbabilities, stagesOrFlows, true, equivalentRecordLength: invalidERL));
            Assert.Contains("at least 1 year", exception.Message);
        }

        [Theory]
        [InlineData(true)]  // Test with stages (Normal distribution)
        [InlineData(false)] // Test with flows (LogNormal distribution)
        public void LessSimpleMethod_CreatesCorrectDistributionType(bool usingStagesNotFlows)
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5, 0.2, 0.1, 0.02 };
            double[] stagesOrFlows = { 10.0, 15.0, 20.0, 30.0 };

            // Act
            var result = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                exceedanceProbabilities, stagesOrFlows, usingStagesNotFlows);

            // Assert
            Assert.NotNull(result);
            // Note: The result is expanded with required probabilities (extrapolation + filling)
            // so it will have more points than the input
            Assert.True(result.Yvals.Length >= exceedanceProbabilities.Length);

            // Check that all distributions are of the correct type
            string expectedTypeName = usingStagesNotFlows ? "Normal" : "LogNormal";
            foreach (var distribution in result.Yvals)
            {
                Assert.Contains(expectedTypeName, distribution.GetType().Name);
            }
        }

        [Fact]
        public void LessSimpleMethod_WithCustomThresholds_AppliesCorrectly()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.9, 0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005, 0.001 };
            double[] stagesOrFlows = { 5.0, 10.0, 15.0, 20.0, 25.0, 30.0, 35.0, 40.0, 45.0 };
            double customFrequentThreshold = 0.3;  // Different from default
            double customRareThreshold = 0.01;      // Different from default

            // Act
            var result = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                exceedanceProbabilities,
                stagesOrFlows,
                true,
                equivalentRecordLength: 20,
                frequentEventThreshold: customFrequentThreshold,
                rareEventThreshold: customRareThreshold);

            // Assert
            Assert.NotNull(result);
            // Note: The result is expanded with required probabilities
            Assert.True(result.Yvals.Length >= exceedanceProbabilities.Length);

            // The result should still be valid UncertainPairedData
            // We can sample from it without errors
            var sampledData = result.SamplePairedData(0.5);
            Assert.NotNull(sampledData);
        }

        [Fact]
        public void LessSimpleMethod_WithDefaultCurveMetadata_CreatesValidResult()
        {
            // Arrange
            double[] exceedanceProbabilities = { 0.5, 0.1, 0.02 };
            double[] stagesOrFlows = { 10.0, 20.0, 30.0 };

            // Act - Don't provide curve metadata (uses default)
            var result = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                exceedanceProbabilities,
                stagesOrFlows,
                true,
                curveMetaData: null); // Explicitly passing null

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.CurveMetaData);
            // Note: The result is expanded with required probabilities
            Assert.True(result.Xvals.Length >= exceedanceProbabilities.Length);
        }
    }
}