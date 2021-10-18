using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    /// <summary>
    /// A factory for the creation of object implementing the <see cref="ISampleStatistics"/> interface.
    /// </summary>
    public static class ISampleStatisticsFactory
    {
        /// <summary>
        /// Returns summary statistics describing the <see cref="double"/> precision finite numeric <see cref="IData.Elements"/> dataset.
        /// </summary>
        /// <param name="data"> An object implementing the <see cref="IData"/> interface containing a set of <see cref="double"/> precision numeric values in the <see cref="IData.Elements"/> property. </param>
        /// <returns> An instance of an object implementing the <see cref="ISampleStatistics"/> interface, containing sample statistics describing the dataset. </returns>
        public static ISampleStatistics Factory(IData data) => data.IsNull() ? throw new ArgumentNullException(nameof(data)) : new SampleStatistics(data);
        /// <summary>
        /// Returns summary statistics describing the <see cref="double"/> precision finite numeric data elements, contained in the <paramref name="data"/> parameter's dataset.   
        /// </summary>
        /// <param name="data"> The dataset from which sample statistics are calculated on the finite numeric value. </param>
        /// <remarks> Summary statistics are computed after generating an object implementing the <see cref="IData"/> interface, which separates the <paramref name="data"/> dataset into a set of <see cref="double"/> precision finite numeric <see cref="IData.Elements"/> and infinite or non-numeric data. </remarks>
        /// <returns> An object implementing the <see cref="ISampleStatistics"/> interface, containing the descriptive statistics for the <see cref="double"/> precision finite numeric element in the <paramref name="data"/> parameter's dataset. </returns>
        public static ISampleStatistics Factory(IEnumerable<double> data) => data.IsNullOrEmpty() ? throw new ArgumentNullException(nameof(data)) : Factory(IDataFactory.Factory(data));
        ///// <summary>
        ///// Returns summary statistics describing the binned data.
        ///// </summary>
        ///// <param name="data"> A set of <see cref="IHistogram.Bins"/>. </param>
        ///// <returns> Descriptive statistics for the binned data. </returns>
        //public static ISampleStatistics Factory(IEnumerable<IBin> data) => data.IsNullItem() ? throw new ArgumentNullException(nameof(data)) : new Histograms.HistogramStatistics(data);
    }
}
