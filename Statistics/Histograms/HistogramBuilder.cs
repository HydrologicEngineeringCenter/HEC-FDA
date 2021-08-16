using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramBuilder: IHistogramBuilder
    {
        private List<int> _SampleCounts = new List<int>();

        public int SampleSize { get; }
        public int[] SampleCounts => _SampleCounts.ToArray();
        public int BinCount { get; private set; }
        public double BinWidth { get; private set; }
        public IRange<double> Range { get; private set; }
        public List<IBin> Bins { get; private set; }
        public ISampleStatistics Statistics { get; private set; }

        public IHistogramBuilder InitializeWithDataAndWidth(IEnumerable<double> data, double width)
        {
            if (data.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(data),
                    $"The specified data is invalid because it is null or contains 0 finite, numeric data elements.");
            }
            IData sample = IDataFactory.Factory(data);
            if (sample.SampleSize < 1)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(data),
                    $"The specified data cannot be used to initialize the histogram because it contains 0 finite numerical data elements.");
            }
            if (!width.IsFinite() || width < double.Epsilon)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(width),
                    $"The specified bin width: {width} is invalid. It must be a positive, finite numerical value.");
            }           
            BinWidth = width;
            _SampleCounts.Add(sample.SampleSize);
            Bins = HistogramBins(sample, HistogramMin(sample, width), HistogramMax(sample, width), width);
            Statistics = ISampleStatisticsFactory.Factory(Bins);
            Range = IRangeFactory.Factory(Bins[0].Range.Min, Bins[Bins.Count - 1].Range.Max, true, false, true, true);
            BinCount = Bins.Count;
            return this;
        }
        private double HistogramMin(IData data, double width) => data.Range.Min - (width / 2.0);
        private double HistogramMax(IData data, double width)
        {
            /* If the data only contains one value:
             *   make the max fit (barely) in one bin,
             *   this ensures only one bin is created, and 
             *   the data is binned in it.
             */
            return data.Range.Min == data.Range.Max ? data.Range.Min + (width / 2.0) - double.Epsilon : data.Range.Max;
        }
        private List<IBin> HistogramBins(IData data, double min, double max, double width)
        {
            int i = 0, n = 0;
            List<IBin> bins = new List<IBin>();
            double[] sample = data.Elements.ToArray();
            double binMin = min, binMax = binMin + width;
            if (max < sample.Last()) max = sample.Last();
            do
            {
                /* While loop criteria:
                 * (1) In range of data and
                 * (2) In range of current bin
                 */
                while (i < sample.Length
                    && sample[i] < binMax)
                {
                    n++; //increase bin count
                    i++; //increase data index
                }
                /* After counting the data in the bin range:
                 * (1) create new Bin
                 * (2) increment Bin range
                 * (3) reset Bin count
                 * Note: do NOT increment i (e.g. data index), sample[i] belongs in a future bin.
                 */
                bins.Add(new Bin(binMin, binMax, n));
                binMin = binMax;
                binMax += width;
                n = 0;
                /* Keep going until:
                 * (1) all the data have been binned and
                 * (2) the histogram max is included in a bin
                 * Note: binMin is last bin's max value
                 */
            } while (i < sample.Length && binMin <= max);
            return bins;
        }

        public IHistogramBuilder InitializeWithBinsCountAndRange(int nBins, double min, double max)
        {
            if (nBins < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(nBins),
                    $"The specified number of bins: {nBins} is invalid because it is less than 1.");
            IRange<double> range = IRangeFactory.Factory(min, max, true, false, true, true);
            if (range.State > IMessageLevels.Message)
                throw new ArgumentOutOfRangeException(
                    $"The specified histogram range is invalid and contains the following errors and messages: {range.Messages.PrintTabbedListOfMessages()}");
            return InitializeWithBinsCountWidthAndMin(nBins, min, (max - min) / nBins);
        }
        public IHistogramBuilder InitializeWithBinsCountWidthAndMin(int nBins, double min, double width)
        {
            if (nBins < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(nBins),
                    $"The specified number of bins: {nBins} is invalid because it is less than 1.");
            if (!min.IsFinite())
                throw new ArgumentOutOfRangeException(
                    nameof(min),
                    $"The specified histogram minimum: {min} is invalid because it is not a finite numerical value.");
            if (!width.IsFinite() || width < double.Epsilon)
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    $"The specified bin width: {width} is invalid. It must be a positive, finite numerical value.");
            double binMin = min;
            List<IBin> bins = new List<IBin>();
            for (int i = 0; i < nBins; i++)
            {
                bins.Add(new Bin(binMin, binMin + width, 0));
                binMin += width;
            }
            Bins = bins;
            BinWidth = width;
            BinCount = bins.Count;
            Range = IRangeFactory.Factory(min, bins[bins.Count - 1].Range.Max, true, false, true, true);
            return this;
        }



    }
}
