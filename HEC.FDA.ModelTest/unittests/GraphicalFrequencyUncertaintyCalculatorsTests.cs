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
            (double[] expandedExceedenceProbs, ContinuousDistribution[] dists) = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(
                exceedanceProbabilities, stagesOrFlows, usingStagesNotFlows);

            // Note: The result is expanded with required probabilities (extrapolation + filling)
            // so it will have more points than the input
            Assert.True(expandedExceedenceProbs.Length >= exceedanceProbabilities.Length);

            // Check that all distributions are of the correct type
            string expectedTypeName = usingStagesNotFlows ? "Normal" : "LogNormal";
            foreach (var distribution in dists)
            {
                Assert.Contains(expectedTypeName, distribution.GetType().Name);
            }
        }
    }
}