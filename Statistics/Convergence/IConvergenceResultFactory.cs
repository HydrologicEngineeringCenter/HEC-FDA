using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Provides public and internal factory methods for the construction of <see cref="IConvergenceResult"/>s.
    /// </summary>
    /// <remarks> Objects constructed by the public method will alway constructed <see cref="IConvergenceResult.Passed"/> = <see langword="false"/>, <see cref="IConvergenceResult.TestMessage"/> = 'Not Tested' <see cref="IConvergenceResult"/>s. </remarks>
    public static class IConvergenceResultFactory
    {
        internal static IConvergenceResult Factory(bool result, Utilities.IMessage report) => new Convergence.ConvergenceResult(result, report);
        /// <summary>
        /// Constructs not tested, not converged <see cref="IConvergenceResult"/>s.
        /// </summary>
        /// <returns> An <see cref="IConvergenceResult"/>. </returns>
        public static IConvergenceResult Factory() => new Convergence.ConvergenceResult();
    }
}
