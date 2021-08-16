using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramStatistics : ISampleStatistics
    {
        #region Properties
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Skewness { get; }
        public double Kurtosis { get; }
        public IRange<double> Range { get; }
        public int SampleSize { get; }
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        internal HistogramStatistics(IEnumerable<IBin> bins)
        {
            if (!Validation.SummaryStatisticsValidator.IsConstructable(bins, out string msg)) throw new Utilities.InvalidConstructorArgumentsException(msg);
            /* This is a 2 pass function:
             *      (1) First, calculate the:
             *          - sample size
             *          - sample mean
             *          - min and max (not the same as the histogram min and max - the binned data min and max)
             *      (2) Second, calculate the:
             *          - median
             *          - sample variance
             *          - sample skewness
             *          - sample kurtosis
             */
            int n = 0;
            bool needmin = true;
            double sum = 0, min = double.NaN, max = double.NaN;
            foreach (IBin bin in bins) // First pass
            {
                n += bin.Count;
                sum += bin.MidPoint * bin.Count;
                if (bin.Count > 0)
                {
                    if (needmin)
                    {
                        min = bin.MidPoint; 
                        needmin = false;
                    }
                    max = bin.MidPoint;
                }
            }            
            SampleSize = n;
            Mean = SampleSize > 0 ? sum / SampleSize: double.NaN;
            Range = IRangeFactory.Factory(min, max, true, true, true, false);
            /* median:
             *  Is even or odd number of observations in the histogram?
             *      IF odd median observation = n / 2 + 1
             *      ELSE (even) median observation is between n / 2 and n / 2 + 1 observation     
             */
            bool oddn = n % 2 != 0, needMed = true;
            int medIndex = oddn ? n / 2 : n / 2 + 1, m = 0;
            /* sample statistics:
             *      mean = sum(x_i) / n
             *      variance = sum((x_i - mean) ^ 2) / (n - 1)
             *      skewness = sum((x_i - mean) ^ 3) / n / sample variance ^ (3 * 1/2)
             *      kurtosis = sum((x_i - mean) ^ 4) / n / (sum((x_i - mean) ^ 2 / n) ^ 2 (note: NOT excess kurtosis)
             */
            IBin[] _bins = bins.ToArray();
            double deviation = 0, deviation2 = 0, deviation3 = 0, deviation4 = 0;
            for (int i = 0; i < _bins.Length; i++)
            {
                deviation += _bins[i].MidPoint - Mean;
                deviation2 += deviation * deviation;
                deviation3 += deviation2 * deviation;
                if (needMed)
                {
                    if (m < medIndex && medIndex <= m + _bins[i].Count)
                    {
                        if (oddn || medIndex + 1 <= m + _bins[i].Count) Median = _bins[i].MidPoint;
                        else Median = (_bins[i].MidPoint + _bins[i + 1].MidPoint) / 2;
                        needMed = false;
                    }
                    else m += _bins[i].Count;
                }
            }
            /* If an the data contains a insufficient sample size (observations, degrees of freedom)
             *      variance = 0
             *      skewness = 0
             *      kurtosis = 0 (note: this would be -3 if excess kurtosis were being computed)
             */
            Variance = SampleSize > 1 ? deviation2 / (n - 1) : 0;
            Skewness = SampleSize > 2 ? deviation3 / n / Math.Pow(Variance, 3 / 2) : 0;
            Kurtosis = SampleSize > 3 ? deviation4 / n / ((deviation2 / n) * (deviation2 / n)) : 0;
            StandardDeviation = Math.Sqrt(Variance);
            State = Validate(new Validation.SummaryStatisticsValidator(), out IEnumerable<IMessage> messages);
            Messages = messages;
        }
        #endregion

        #region Functions
        #region Initialization Functions
        ///// <summary>
        ///// Computes the sum and sample size of the binned elements.
        ///// </summary>
        ///// <param name="bins"></param>
        ///// <returns> A Tuple containing the sum and sample size, respectively. </returns>
        //private Tuple<double, int> GetSumAndN(IBin[] bins)
        //{
        //    int n = 0;
        //    double sum = 0;
        //    foreach (var bin in bins)
        //    {
        //        n += bin.Count;
        //        sum += bin.MidPoint * bin.Count;
        //    }
        //    return new Tuple<double, int>(sum, n);
        //}
        ///// <summary>
        ///// Computes the median of the binned elements.
        ///// </summary>
        ///// <param name="bins"></param>
        ///// <param name="iBin"> The index of the bin containing the first of the median elements. </param>
        ///// <param name="nBin"> The count of the binned elements in the first <paramref name="iBin"/> bins. </param>
        ///// <param name="medInt"> The reverse rank order first of the median elements. </param>
        ///// <param name="medMod"> If <paramref name="medMod"/> is 0 only one binned element is needed to compute the median, otherwise the median is the average value of two binned elements. </param>
        ///// <returns> The median value of the binned elements. <see cref="Double.NaN"/> is returned if there are no binned elements. </returns>
        //private double GetMedian(IBin[] bins, int iBin, int nBin, int medInt, int medMod, int sampleSize)
        //{
        //    if (sampleSize < 1) return double.NaN;
        //    else return medInt > 0 && medMod > 0 && nBin < medInt + 1 ? (bins[iBin].MidPoint + bins[iBin + 1].MidPoint) / 2.0 : bins[iBin].MidPoint;
        //}
        ///// <summary>
        ///// Computes the variance, standard deviation and skewness of the binned elements.
        ///// </summary>
        ///// <param name="deviations2"> The sum of the binned elements squared deviations from the mean. </param>
        ///// <param name="deviations3"> The sum of the binned elements cubbed deviations from the mean. </param>
        ///// <param name="sampleSize"> The number of binned elements. </param>
        ///// <returns> A Tuple containing the variance, standard deviation and skewness of the binned elements, respectively. <see cref="double.NaN"/> is returned if there are no binned elements. </returns>
        //private Tuple<double, double, double> GetVarianceStandardDeviationAndSkew(double deviations2, double deviations3, double sampleSize)
        //{
        //    double variance, stdev, skew;
        //    if (sampleSize > 2)
        //    {
        //        variance = deviations2 / (sampleSize - 1);
        //        stdev = Math.Sqrt(variance);
        //        skew = (deviations3 / sampleSize) / (stdev * stdev * stdev);
        //    }
        //    else if (sampleSize == 1)
        //    {
        //        variance = 0;
        //        stdev = 0;
        //        skew = 0;
        //    }
        //    else
        //    {
        //        variance = double.NaN;
        //        stdev = double.NaN;
        //        skew = double.NaN;
        //    }
        //    return new Tuple<double, double, double>(variance, stdev, skew);
        //}
        #endregion
        public IMessageLevels Validate(IValidator<ISampleStatistics> validator, out IEnumerable<IMessage> messages)
        {
            return validator.IsValid(this, out messages);
        }
        #endregion
    }
}
