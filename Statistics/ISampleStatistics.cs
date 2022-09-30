using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// An interface providing sample statistics for <see cref="double"/> precision sets of finite, numeric data.
    /// </summary>
    public interface ISampleStatistics
    {
        /// <summary>
        /// The arithmetic sample mean, see https://en.wikipedia.org/wiki/Sample_mean_and_covariance.
        /// </summary>
        double Mean { get; }
        /// <summary>
        /// The median or 'middle value' of the sample data, see https://en.wikipedia.org/wiki/Median.
        /// </summary>
        double Median { get; }
        /// <summary>
        /// The sample variance or average squared deviation of the sample values from the mean (calculated with the n - 1 sample correction), see https://en.wikipedia.org/wiki/Variance.
        /// </summary>
        double Variance { get; }
        /// <summary>
        /// The square root of the sample variance or average deviation of the sample values from the mean as an absolute value.
        /// </summary>
        double StandardDeviation { get; }
        /// <summary>
        /// The sample skewness or average cubbed deviation of the sample values from the mean - measuring asymmetry in the distribution of sample values, see https://en.wikipedia.org/wiki/Skewness.
        /// </summary>
        double Skewness { get; }
        /// <summary>
        /// The sample kurtosis or average quartic deviation of the sample values from the mean - measuring the peakedness of the distribution of sample values, see https://en.wikipedia.org/wiki/Kurtosis.
        /// </summary>
        double Kurtosis { get; }
        /// <summary>
        /// The inclusive range of sample value, e.g. [min, max].
        /// </summary>
        Utilities.IRange<double> Range { get; }
        /// <summary>
        /// The number of observations in the sample.
        /// </summary>
        int SampleSize { get; }
    }
}
