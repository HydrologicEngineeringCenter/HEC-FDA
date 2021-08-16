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
        internal static IFrequencyFunction Sample(this IFrequencyFunction fx, double nonexceedanceProbability) => IFrequencyFunctionFactory.Factory(((FdaFunctionBase)fx)._Function.Sample(nonexceedanceProbability), fx.ParameterType, fx.Label, fx.XSeries.Label, fx.YSeries.Label, fx.YSeries.Units);
        internal static ITransformFunction Sample(this ITransformFunction fx, double nonexceedanceProbability) => ITransformFunctionFactory.Factory(((FdaFunctionBase)fx)._Function.Sample(nonexceedanceProbability), fx.ParameterType, fx.Label, fx.XSeries.Units, fx.XSeries.Label, fx.YSeries.Units, fx.YSeries.Label);

        /// <summary>
        /// Prints a label for the <see cref="IFdaFunction"/> x axis.
        /// </summary>
        /// <param name="fx"> The <see cref="IFdaFunction"/> to be evaluated. </param>
        /// <param name="abbreviate"> <see langword="true"/> if an abbreviated label should be printed, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintXLabel(IFdaFunction fx, bool abbreviate) => fx.XSeries.ParameterType.PrintLabel(fx.XSeries.Units, abbreviate);
        /// <summary>
        /// Prints a label for the <see cref="IFdaFunction"/> y axis.
        /// </summary>
        /// <param name="fx"> The <see cref="IFdaFunction"/> to be evaluated. </param>
        /// <param name="abbreviate"> <see langword="true"/> if an abbreviated label should be printed, <see langword="false"/> otherwise. </param>
        /// <returns> A <see cref="string"/> label. </returns>
        public static string PrintYLabel(IFdaFunction fx, bool abbreviate) => fx.XSeries.ParameterType.PrintLabel(fx.XSeries.Units, abbreviate);
    }
}
