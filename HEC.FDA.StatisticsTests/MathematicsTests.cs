using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Statistics;

namespace StatisticsTests
{
    [ExcludeFromCodeCoverage]
    [Trait("RunsOn", "Remote")]
    public class MathematicsTests
    {
        private const double TOLERANCE = 1e-10;

        #region IntegrateTrapezoidal Validation Tests

        [Fact]
        public void IntegrateTrapezoidal_SimpleRectangle_ReturnsCorrectArea()
        {
            // Arrange - Rectangle with constant y = 0.5 from x = 0 to x = 1
            double[] xVals = { 0.25, 0.5, 0.75 };
            double[] yVals = { 0.5, 0.5, 0.5 };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert - IntegrateTrapezoidal assumes starting from (0,0)
            // From (0,0) to (0.25,0.5): triangle (0.25 * 0) + trapezoid (0.25 * 0.5/2) = 0.0625
            // From (0.25,0.5) to (0.5,0.5): rectangle = 0.25 * 0.5 = 0.125
            // From (0.5,0.5) to (0.75,0.5): rectangle = 0.25 * 0.5 = 0.125
            // From (0.75,0.5) to (1,0.5): rectangle = 0.25 * 0.5 = 0.125
            // Total = 0.0625 + 0.125 + 0.125 + 0.125 = 0.4375
            Assert.Equal(0.4375, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateTrapezoidal_SimpleTriangle_ReturnsCorrectArea()
        {
            // Arrange - Triangle from (0,0) to (1,1)
            double[] xVals = { 0.25, 0.5, 0.75, 1.0 };
            double[] yVals = { 0.25, 0.5, 0.75, 1.0 };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert - Area of triangle with base 1 and height 1 = 0.5
            Assert.Equal(0.5, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateTrapezoidal_LinearFunction_ReturnsCorrectArea()
        {
            // Arrange - Linear function y = x
            double[] xVals = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            double[] yVals = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert - IntegrateTrapezoidal assumes starting from (0,0)
            // From (0,0) to (0.1,0.1): triangle = 0.1 * 0.1 / 2 = 0.005
            // Then trapezoidal segments between points
            // From (0.9,0.9) to (1,0.9): rectangle = 0.1 * 0.9 = 0.09
            // The exact calculation gives 0.495
            Assert.Equal(0.495, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateTrapezoidal_QuadraticFunction_ReturnsCorrectArea()
        {
            // Arrange - Quadratic function y = x^2
            double[] xVals = new double[11];
            double[] yVals = new double[11];
            for (int i = 0; i < 11; i++)
            {
                xVals[i] = i * 0.1;
                yVals[i] = xVals[i] * xVals[i];
            }

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert - Integral of x^2 from 0 to 1 = 1/3 H 0.333...
            Assert.Equal(1.0 / 3.0, result, 0.01); // Less precision due to trapezoidal approximation
        }

        [Fact]
        public void IntegrateTrapezoidal_EmptyArrays_ReturnsZero()
        {
            // Arrange
            double[] xVals = { };
            double[] yVals = { };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert
            Assert.Equal(0.0, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateTrapezoidal_SinglePoint_HandlesCorrectly()
        {
            // Arrange
            double[] xVals = { 0.5 };
            double[] yVals = { 0.75 };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert - Should integrate from 0 to 0.5 and then 0.5 to 1
            // Area = (0.5 * 0.75 / 2) + (0.5 * 0.75) = 0.1875 + 0.375 = 0.5625
            Assert.Equal(0.5625, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateTrapezoidal_LastPointAtOne_NoExtraPadding()
        {
            // Arrange - Points ending exactly at x = 1
            double[] xVals = { 0.5, 1.0 };
            double[] yVals = { 0.5, 0.8 };

            // Act
            double result = Mathematics.IntegrateTrapezoidal(xVals, yVals);

            // Assert
            // From 0 to 0.5: triangle = 0.5 * 0.5 / 2 = 0.125
            // From 0.5 to 1.0: trapezoid = 0.5 * (0.5 + 0.8) / 2 = 0.325
            Assert.Equal(0.45, result, TOLERANCE);
        }

        #endregion

        #region IntegrateCDF Validation Tests

        [Fact]
        public void IntegrateCDF_SimpleRectangle_ReturnsCorrectArea()
        {
            // Arrange - Rectangle with constant y = 0.5
            double[] xVals = { 0.25, 0.5, 0.75 };
            double[] yVals = { 0.5, 0.5, 0.5 };

            // Act
            double result = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            // Padding from 0 to 0.25: triangle = 0.25 * 0.5 / 2 = 0.0625
            // Main area from 0.25 to 0.75: 0.5 * 0.5 = 0.25
            // Padding from 0.75 to 1: rectangle = 0.25 * 0.5 = 0.125
            // Total = 0.0625 + 0.25 + 0.125 = 0.4375
            Assert.Equal(0.4375, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateCDF_LinearFunction_ReturnsCorrectArea()
        {
            // Arrange - Linear CDF
            double[] xVals = { 0.2, 0.4, 0.6, 0.8 };
            double[] yVals = { 0.2, 0.4, 0.6, 0.8 };

            // Act
            double result = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            // Padding from 0 to 0.2: triangle = 0.2 * 0.2 / 2 = 0.02
            // Main area: trapezoidal from 0.2 to 0.8
            // Padding from 0.8 to 1: rectangle = 0.2 * 0.8 = 0.16
            double expectedMainArea = 0.2 * (0.2 + 0.4) / 2 + 0.2 * (0.4 + 0.6) / 2 + 0.2 * (0.6 + 0.8) / 2;
            Assert.Equal(0.02 + expectedMainArea + 0.16, result, TOLERANCE);
        }

        [Fact]
        public void IntegrateCDF_FloatValues_ReturnsCorrectArea()
        {
            // Arrange
            float[] xVals = { 0.25f, 0.5f, 0.75f };
            float[] yVals = { 0.25f, 0.5f, 0.75f };

            // Act
            float result = Mathematics.IntegrateCDF<float>(xVals, yVals);

            // Assert
            // Padding from 0 to 0.25: triangle = 0.25 * 0.25 / 2 = 0.03125
            // Main area from 0.25 to 0.75: trapezoids
            // Padding from 0.75 to 1: rectangle = 0.25 * 0.75 = 0.1875
            float expectedMainArea = 0.25f * (0.25f + 0.5f) / 2 + 0.25f * (0.5f + 0.75f) / 2;
            Assert.Equal(0.03125f + expectedMainArea + 0.1875f, result, 1e-6f);
        }

        [Fact]
        public void IntegrateCDF_FullRange_NoPadding()
        {
            // Arrange - CDF from 0 to 1 exactly
            double[] xVals = { 0.0, 0.5, 1.0 };
            double[] yVals = { 0.0, 0.5, 1.0 };

            // Act
            double result = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert - No padding needed, just trapezoidal integration
            Assert.Equal(0.5, result, TOLERANCE);
        }

        #endregion

        #region RealIntegrateTrapezoidal Validation Tests

        [Fact]
        public void RealIntegrateTrapezoidal_SimpleRectangle_ReturnsCorrectArea()
        {
            // Arrange
            double[] xVals = { 0.0, 1.0 };
            double[] yVals = { 2.0, 2.0 };

            // Act
            double result = Mathematics.RealIntegrateTrapezoidal<double>(xVals, yVals);

            // Assert - Area = 1.0 * 2.0 = 2.0
            Assert.Equal(2.0, result, TOLERANCE);
        }

        [Fact]
        public void RealIntegrateTrapezoidal_Triangle_ReturnsCorrectArea()
        {
            // Arrange
            double[] xVals = { 0.0, 1.0 };
            double[] yVals = { 0.0, 2.0 };

            // Act
            double result = Mathematics.RealIntegrateTrapezoidal<double>(xVals, yVals);

            // Assert - Area = 0.5 * 1.0 * 2.0 = 1.0
            Assert.Equal(1.0, result, TOLERANCE);
        }

        [Fact]
        public void RealIntegrateTrapezoidal_MismatchedArrayLengths_ThrowsException()
        {
            // Arrange
            double[] xVals = { 0.0, 1.0 };
            double[] yVals = { 0.0, 1.0, 2.0 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Mathematics.RealIntegrateTrapezoidal<double>(xVals, yVals));
        }

        [Fact]
        public void RealIntegrateTrapezoidal_SinglePoint_ReturnsZero()
        {
            // Arrange
            double[] xVals = { 0.5 };
            double[] yVals = { 1.0 };

            // Act
            double result = Mathematics.RealIntegrateTrapezoidal<double>(xVals, yVals);

            // Assert - No area with single point
            Assert.Equal(0.0, result, TOLERANCE);
        }

        #endregion

        #region Equivalence Tests Between IntegrateCDF and IntegrateTrapezoidal

        [Fact]
        public void IntegrationMethods_LinearCDF_ProduceEquivalentResults()
        {
            // Arrange - Linear CDF from 0 to 1
            double[] xVals = new double[11];
            double[] yVals = new double[11];
            for (int i = 0; i < 11; i++)
            {
                xVals[i] = i * 0.1;
                yVals[i] = i * 0.1;
            }

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert - Both methods should give same result for data from 0 to 1
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_PartialCDF_ProduceEquivalentResults()
        {
            // Arrange - CDF data that doesn't span full [0,1] range
            double[] xVals = { 0.2, 0.4, 0.6, 0.8 };
            double[] yVals = { 0.3, 0.5, 0.7, 0.9 };

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            
            // For equivalent comparison, we need to manually calculate what IntegrateTrapezoidal does
            // It assumes starting from (0,0) and ending at (1, last_y)
            double manualTrapezoidalArea = 0.0;
            
            // From (0,0) to (0.2, 0.3)
            manualTrapezoidalArea += 0.2 * 0.3 / 2;
            
            // From (0.2, 0.3) to (0.4, 0.5)
            manualTrapezoidalArea += 0.2 * (0.3 + 0.5) / 2;
            
            // From (0.4, 0.5) to (0.6, 0.7)
            manualTrapezoidalArea += 0.2 * (0.5 + 0.7) / 2;
            
            // From (0.6, 0.7) to (0.8, 0.9)
            manualTrapezoidalArea += 0.2 * (0.7 + 0.9) / 2;
            
            // From (0.8, 0.9) to (1.0, 0.9)
            manualTrapezoidalArea += 0.2 * 0.9;
            
            // Verify our understanding of IntegrateTrapezoidal
            Assert.Equal(manualTrapezoidalArea, trapezoidalResult, TOLERANCE);

            // Now calculate IntegrateCDF result
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);
            
            // Assert - Both should be equivalent
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_StepFunction_ProduceEquivalentResults()
        {
            // Arrange - Step function CDF
            double[] xVals = { 0.0, 0.25, 0.25, 0.5, 0.5, 0.75, 0.75, 1.0 };
            double[] yVals = { 0.0, 0.0, 0.33, 0.33, 0.67, 0.67, 1.0, 1.0 };

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_SmoothCDF_ProduceEquivalentResults()
        {
            // Arrange - Smooth S-curve CDF (logistic function approximation)
            double[] xVals = new double[21];
            double[] yVals = new double[21];
            for (int i = 0; i < 21; i++)
            {
                xVals[i] = i * 0.05;
                // Approximate logistic curve
                double x = (xVals[i] - 0.5) * 10; // Scale and center
                yVals[i] = 1.0 / (1.0 + Math.Exp(-x));
            }

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_RandomCDF_ProduceEquivalentResults()
        {
            // Arrange - Random but valid CDF (monotonically increasing)
            Random rng = new Random(42); // Fixed seed for reproducibility
            int n = 15;
            double[] xVals = new double[n];
            double[] yVals = new double[n];
            
            // Generate sorted x values between 0 and 1
            for (int i = 0; i < n; i++)
            {
                xVals[i] = (double)i / (n - 1);
            }
            
            // Generate monotonically increasing y values
            yVals[0] = rng.NextDouble() * 0.1; // Start low
            for (int i = 1; i < n; i++)
            {
                yVals[i] = yVals[i - 1] + rng.NextDouble() * (1.0 - yVals[i - 1]) / (n - i);
            }
            yVals[n - 1] = Math.Min(yVals[n - 1], 1.0); // Ensure last value doesn't exceed 1

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_EdgeCase_SinglePointAtOrigin_ProduceEquivalentResults()
        {
            // Arrange
            double[] xVals = { 0.0 };
            double[] yVals = { 0.0 };

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_EdgeCase_SinglePointAtEnd_ProduceEquivalentResults()
        {
            // Arrange
            double[] xVals = { 1.0 };
            double[] yVals = { 1.0 };

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        [Fact]
        public void IntegrationMethods_EdgeCase_DenseCDF_ProduceEquivalentResults()
        {
            // Arrange - Very dense CDF sampling
            int n = 1000;
            double[] xVals = new double[n];
            double[] yVals = new double[n];
            for (int i = 0; i < n; i++)
            {
                xVals[i] = (double)i / (n - 1);
                yVals[i] = Math.Pow(xVals[i], 2); // Quadratic CDF
            }

            // Act
            double trapezoidalResult = Mathematics.IntegrateTrapezoidal(xVals, yVals);
            double cdfResult = Mathematics.IntegrateCDF<double>(xVals, yVals);

            // Assert
            Assert.Equal(trapezoidalResult, cdfResult, TOLERANCE);
        }

        #endregion

        #region Float Precision Tests

        [Fact]
        public void IntegrateCDF_FloatPrecision_HandlesCorrectly()
        {
            // Arrange
            float[] xVals = { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
            float[] yVals = { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };

            // Act
            float resultFloat = Mathematics.IntegrateCDF<float>(xVals, yVals);
            
            // Convert to double for comparison
            double[] xValsDouble = Array.ConvertAll(xVals, x => (double)x);
            double[] yValsDouble = Array.ConvertAll(yVals, y => (double)y);
            double resultDouble = Mathematics.IntegrateCDF<double>(xValsDouble, yValsDouble);

            // Assert - Results should be close within float precision
            Assert.Equal(resultDouble, resultFloat, 1e-6);
        }

        #endregion
    }
}