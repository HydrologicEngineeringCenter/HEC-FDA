using Model.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    /// <summary>
    /// Provides static properties and functions for <see cref="IFdaFunction"/>s.
    /// </summary>
    public static class IFdaFunctionUtilities
    {
        internal static IFunction Sample(this IFdaFunction fx, double nonexceedanceProbability) => ((FdaFunctionBase)fx)._Function.Sample(nonexceedanceProbability);
    }
}
