using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Validation
{
    internal static class Resources
    {
        /// <summary>
        /// A string for invalid parameterization errors.
        /// </summary>
        /// <param name="dataObj"> A string representation of the data object and its parameterization. </param>
        /// <returns> The <paramref name="dataObj"/> parameterization is invalid. </returns>
        internal static string InvalidParameterizationNotice(string dataObj) => $"The {dataObj} parameterization is invalid.";
        /// <summary>
        /// A string for fatal errors usually caused by invalid parameterization of a data object constructor.
        /// </summary>
        /// <param name="dataObj"> A string representation of the data object and its parameterization. </param>
        /// <returns> The <paramref name="dataObj"/> cannot be constructed with the specified parameters. </returns>
        internal static string FatalParameterizationNotice(string dataObj) => $"The {dataObj} cannot be constructed with specified parameters.";
        internal static string SampleSizeSuggestion() => $"The sample size parameter represents number of observations that were used to parameterize the distribution, if the distribution was not estimated from a sample use the default maximum integer: {int.MaxValue.Print()} value for the sample size parameter.";
        internal static string NonFiniteRangeNotice(IDistribution distribution) => $"The {distribution.Print()} generated the invalid range: {distribution.Range.Print()} of values. This is because its parameterization results in infinite or non-numerical extreme values, this usually occurs when one or more parameters measuring dispersion are large or the central tendency of the distribution is approaching the minimum or maximum of the range: [{double.MinValue.ToString("E3")}, {double.MaxValue.ToString("E3")}] of finite values.";
        internal static string DoubleRangeRequirements() => $"range: [{double.MinValue.Print()}, {double.MaxValue.Print()}] with range min < range max";
        internal static string IntRangeRequirements() => $"range: [{int.MinValue.Print()}, {int.MaxValue.Print()}] with range min < range max";
    }
}
