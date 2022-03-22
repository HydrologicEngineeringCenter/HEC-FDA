using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;

namespace StatisticsTests.Convergence
{
    public class ConvergenceTests
    {
        [Fact]
        public void ConvergenceTest()
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(tolerance: 0.05);

            ThreadsafeInlineHistogram threadsafeInlineHistogramNormalDistribution = new ThreadsafeInlineHistogram(convergenceCriteria);
            ThreadsafeInlineHistogram threadsafeInlineHistogramMixedDistributions = new ThreadsafeInlineHistogram(convergenceCriteria);

            int iterations = 3500;
            Random random = new Random();

            for (int i = 0; i < iterations; i++)
            {
                double uniformObservation1 = random.NextDouble();
                double uniformObservation2 = random.NextDouble();
                double messyObservation = random.NextDouble()*random.NextDouble() + random.NextDouble()*random.NextDouble()*random.NextDouble()*1000;

                threadsafeInlineHistogramNormalDistribution.AddObservationToHistogram(uniformObservation1, i);
                threadsafeInlineHistogramNormalDistribution.AddObservationToHistogram(uniformObservation2, i);
                threadsafeInlineHistogramMixedDistributions.AddObservationToHistogram(messyObservation*random.NextDouble(), i);
                threadsafeInlineHistogramMixedDistributions.AddObservationToHistogram(messyObservation, i);

            }

            double upperConfidenceLimitProbability = 0.975;
            double lowerConfidenceLimitProbability = 0.025;
            threadsafeInlineHistogramNormalDistribution.TestForConvergence(upperConfidenceLimitProbability, lowerConfidenceLimitProbability);
            threadsafeInlineHistogramMixedDistributions.TestForConvergence(upperConfidenceLimitProbability, lowerConfidenceLimitProbability);
            
            Assert.True(threadsafeInlineHistogramNormalDistribution.IsConverged);
            Assert.False(threadsafeInlineHistogramMixedDistributions.IsConverged);
        }
    }
}
