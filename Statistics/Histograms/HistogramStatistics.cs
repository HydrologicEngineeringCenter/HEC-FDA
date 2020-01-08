using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramStatistics : ISummaryStatistics
    {
        #region Properties
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public IRange<double> Range { get; }
        //public double Minimum { get; }
        //public double Maximum { get; }
        public int SampleSize { get; }
        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        internal HistogramStatistics(IBin[] bins)
        {
            var _ = GetSumAndN(bins);
            double sum = _.Item1;
            SampleSize = _.Item2;
            if (SampleSize > 1)
            {
                Range = IRangeFactory.Factory(bins[0].Range.Min, bins[bins.Length - 1].Range.Max);
                Mean = SampleSize > 0 ? sum / (double)SampleSize : double.NaN;
                int medInt = SampleSize / 2, medMod = SampleSize % 2;
                double deviations2 = 0, deviations3 = 0, n = 0;
                for (int i = 0; i < bins.Length; i++)
                {
                    n += bins[i].Count;
                    double deviation = (bins[i].MidPoint - Mean);
                    deviations2 += deviation * deviation * bins[i].Count;
                    deviations3 += deviation * deviation * deviation * bins[i].Count;
                    if (!(n < medInt)) Median = GetMedian(bins, i, SampleSize, medInt, medMod, SampleSize);
                }
                var dispersion = GetVarianceStandardDeviationAndSkew(deviations2, deviations3, SampleSize);
                Variance = dispersion.Item1;
                StandardDeviation = dispersion.Item2;
                Skewness = dispersion.Item3;
                
            }
            else
            {
                if (SampleSize == 1)
                {
                    Mean = sum;
                    Median = Mean;                   
                    Variance = 0;
                    Skewness = 0;
                    StandardDeviation = 0;
                    Range = IRangeFactory.Factory(bins[0].Range.Min, bins[bins.Length - 1].Range.Max);
                    //Minimum = bins[0].Minimum;
                    //Maximum = bins[bins.Length - 1].Maximum;
                }
                else
                {
                    Mean = double.NaN;
                    Median = double.NaN;
                    Variance = double.NaN;
                    Skewness = double.NaN;
                    StandardDeviation = double.NaN;
                    Range = IRangeFactory.Factory(bins[0].Range.Min, bins[bins.Length - 1].Range.Max);
                    //Minimum = bins[0].Minimum;
                    //Maximum = bins[bins.Length - 1].Maximum;
                }
            }
            IsValid = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<IMessage> messages);
            Messages = messages;
        }
        #endregion

        #region Functions
        #region Initialization Functions
        /// <summary>
        /// Computes the sum and sample size of the binned elements.
        /// </summary>
        /// <param name="bins"></param>
        /// <returns> A Tuple containing the sum and sample size, respectively. </returns>
        private Tuple<double, int> GetSumAndN(IBin[] bins)
        {
            int n = 0;
            double sum = 0;
            foreach (var bin in bins)
            {
                n += bin.Count;
                sum += bin.MidPoint * bin.Count;
            }
            return new Tuple<double, int>(sum, n);
        }
        /// <summary>
        /// Computes the median of the binned elements.
        /// </summary>
        /// <param name="bins"></param>
        /// <param name="iBin"> The index of the bin containing the first of the median elements. </param>
        /// <param name="nBin"> The count of the binned elements in the first <paramref name="iBin"/> bins. </param>
        /// <param name="medInt"> The reverse rank order first of the median elements. </param>
        /// <param name="medMod"> If <paramref name="medMod"/> is 0 only one binned element is needed to compute the median, otherwise the median is the average value of two binned elements. </param>
        /// <returns> The median value of the binned elements. <see cref="Double.NaN"/> is returned if there are no binned elements. </returns>
        private double GetMedian(IBin[] bins, int iBin, int nBin, int medInt, int medMod, int sampleSize)
        {
            if (sampleSize < 1) return double.NaN;
            else return medInt > 0 && medMod > 0 && nBin < medInt + 1 ? (bins[iBin].MidPoint + bins[iBin + 1].MidPoint) / 2.0 : bins[iBin].MidPoint;
        }
        /// <summary>
        /// Computes the variance, standard deviation and skewness of the binned elements.
        /// </summary>
        /// <param name="deviations2"> The sum of the binned elements squared deviations from the mean. </param>
        /// <param name="deviations3"> The sum of the binned elements cubbed deviations from the mean. </param>
        /// <param name="sampleSize"> The number of binned elements. </param>
        /// <returns> A Tuple containing the variance, standard deviation and skewness of the binned elements, respectively. <see cref="double.NaN"/> is returned if there are no binned elements. </returns>
        private Tuple<double, double, double> GetVarianceStandardDeviationAndSkew(double deviations2, double deviations3, double sampleSize)
        {
            double variance, stdev, skew;
            if (sampleSize > 2)
            {
                variance = deviations2 / (sampleSize - 1);
                stdev = Math.Sqrt(variance);
                skew = (deviations3 / sampleSize) / (stdev * stdev * stdev);
            }
            else if (sampleSize == 1)
            {
                variance = 0;
                stdev = 0;
                skew = 0;
            }
            else
            {
                variance = double.NaN;
                stdev = double.NaN;
                skew = double.NaN;
            }
            return new Tuple<double, double, double>(variance, stdev, skew);
        }
        #endregion
        public bool Validate(IValidator<ISummaryStatistics> validator, out IEnumerable<IMessage> messages)
        {
            return validator.IsValid(this, out messages);
        }
        #endregion
    }
}
