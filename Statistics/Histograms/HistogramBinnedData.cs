using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramBinnedData : Histogram
    {
        internal HistogramBinnedData(IData data, double min, double max, double width) : base(InitializeBins(min, max, data, width, out IEnumerable<IMessage> msgs))
        {
        }
        internal HistogramBinnedData(IBin[] bins) : base(bins)
        {
        }
        internal HistogramBinnedData(IHistogram histogram, List<IConvergenceResult> convergence) : base(histogram, convergence)
        {
        }

        #region Functions
        private int FindBinCount(double x, bool cummulative = true)
        {
            if (!x.IsOnRange(Range.Min, Range.Max, inclusiveMin: true, inclusiveMax: false))
            {
                return x < Range.Min ? 0 : cummulative ? 1 : 0;
            }
            else
            {
                int n = 0;
                foreach (var bin in Bins)
                {
                    n += bin.Count;
                    if (x.IsOnRange(bin.Range.Min, bin.Range.Max, inclusiveMin: true, inclusiveMax: false)) return cummulative ? n : bin.Count;
                }
                throw new Exception($"An unexpected error occured while attempting to find the histogram bin associated withe value {x}.");
            }
        }
        #region Initialization Functions
        private static IBin[] InitializeBins(double min, double max, IData data, double width, out IEnumerable<IMessage> msgs)
        {
            if (!Validation.HistogramValidator.IsConstructable(min, max, width, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
            else
            {
                List<IMessage> msgsList = data.Messages.ToList();
                var range = GetHistogramRange(min, max, data, ref msgsList);
                List<IBin> bins = InitializeEmptyBins(range.Item1, width, Increments(data.Range.Min - range.Item1, width, Math.Floor));
                bins.AddRange(InitializeDataBins(bins.Count == 0 ? range.Item1 : bins.Last().Range.Max, data, width));
                bins.AddRange(InitializeEmptyBins(bins.Last().Range.Max, width, Increments(data.Range.Max - bins.Last().Range.Max, width, Math.Ceiling)));
                msgs = msgsList.ToArray();
                return bins.ToArray();
            }        
        }
        
        #endregion
        #region IDistribution Functions
        public override double PDF(double x) => (double)FindBinCount(x, false) / (double)SampleSize;
        public override double CDF(double x) => (double)FindBinCount(x) / (double)SampleSize;
        public override double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The provided probability value: {p} is not on the a valid range: [0, 1]");
            else
            {
                int n = 0;
                foreach (var bin in Bins)
                {
                    n += bin.Count;
                    double pAtN = (double)n / SampleSize;
                    if (!(pAtN < p)) return bin.MidPoint;
                }
                throw new Exception($"An unexpected error occured while attempting to find the histogram bin associated with the probability value {p}.");
            }
        }
        public override double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        public override double[] Sample(int sampleSize, Random r = null)
        {
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = Sample(r);
            return sample;
        }
        public override IDistribution SampleDistribution(Random r = null) => (IDistribution)IHistogramFactory.Factory(IDataFactory.Factory(Sample(SampleSize, r)), Range.Min, Range.Max, Bins.Length);
        #endregion
        #endregion
    }
}
