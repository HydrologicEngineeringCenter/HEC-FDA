using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    /// <summary>
    /// Extension methods for objects implementing the <see cref="IDistribution"/> interface.
    /// </summary>
    public static class IDistributionExtensions
    {
        /// <summary>
        /// Calculates the number of random values required to sample the distribution.
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns> An integer equal to the <see cref="IDistribution.SampleSize"/>. </returns>
        public static int SampleParameters(this IDistribution distribution) => !distribution.IsNull() ? distribution.SampleSize : throw new ArgumentNullException();
        /// <summary>
        /// Generates a parametric bootstrap sample of the distribution.
        /// </summary>
        /// <param name="distribution"></param>
        /// <param name="packetOfRandomNumbers"> Random numbers used to generate the bootstrap sample values, the array length must be equal to or longer than <see cref="IDistribution.SampleSize"/>. </param>
        /// <returns> A new <see cref="IDistribution"/> constructed from a bootstrap sample from the underlying distribution. </returns>
        public static IDistribution Sample(this IDistribution distribution, double[] packetOfRandomNumbers)
        {
            if (distribution.IsNull()) throw new ArgumentNullException($"The sample distribution cannot be constructed because the input distribution is null.");
            if (!(distribution.State < Utilities.IMessageLevels.Error)) throw new ArgumentException($"The specified distribution cannot be sampled because it is being held in an invalid state with the following messages: {distribution.Messages.PrintTabbedListOfMessages()}");           
            if (!packetOfRandomNumbers.IsItemsOnRange(IRangeFactory.Factory(0d, 1d))) throw new ArgumentException($"The sample distribution cannot be sampled because the provided packet or random numbers is null, empty or contains members outside the valid range of: [0, 1].");
            if (packetOfRandomNumbers.Length < distribution.SampleSize) throw new ArgumentException($"The parametric bootstrap sample cannot be constructed using the {distribution.Print(true)} distribution. It requires at least {distribution.SampleSize} random value but only {packetOfRandomNumbers.Length} were provided.");
            double[] X = new double[distribution.SampleSize];
            for (int i = 0; i < distribution.SampleSize; i++) X[i] = distribution.InverseCDF(packetOfRandomNumbers[i]);
            return IDistributionFactory.Fit(X, distribution.Type);
        }
        private static bool IsItemsOnRange<T>(this IEnumerable<T> input, IRange<T> range)
        {
            if (range.IsNull()) throw new ArgumentNullException(nameof(range), "The input IEnumerable items cannot not be checked for membership on the specified range because the range is null.");
            if (input.IsNullOrEmpty()) return false;
            else
            {
                foreach (var p in input) if (!range.IsOnRange(p)) return false;
                return true;
            }
        }
        /// <summary>
        /// Checks if two <see cref="IDistribution"/>s converge before and after the addition of extra observations, based on the <paramref name="criteria"/> specified <see cref="IConvergenceCriteria"/>.
        /// </summary>
        /// <param name="criteria"> The criteria for convergence, <see cref="IConvergenceCriteria"/>. </param>
        /// <param name="before"> The <see cref="IDistribution"/> before the additional sample observations were added. </param>
        /// <param name="after"> The <see cref="IDistribution"/> after the additional sample observation were added. </param>
        /// <returns> An assessment of the <see cref="IConvergenceCriteria.Test(double, double, int, int)"/> stored as a <see cref="IConvergenceResult"/>. </returns>
        public static IConvergenceResult Test(this IConvergenceCriteria criteria, IDistribution before, IDistribution after)
        {
            if (after.IsNull()) throw new ArgumentNullException(nameof(after));
            if (before.IsNull()) throw new ArgumentNullException(nameof(before));
            if (criteria.IsNull()) throw new ArgumentNullException(nameof(criteria));
            return criteria.Test(before.InverseCDF(criteria.Quantile), after.InverseCDF(criteria.Quantile), after.SampleSize - before.SampleSize, after.SampleSize);
        }
    }
}
