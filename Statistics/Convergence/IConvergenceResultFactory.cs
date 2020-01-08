using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    internal static class IConvergenceResultFactory
    {
        internal static IConvergenceResult Factory(bool result, Utilities.IMessage report) => new Convergence.ConvergenceResult(result, report);
    }
}
