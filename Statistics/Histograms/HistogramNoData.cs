using Statistics.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramNoData: Histogram
    {
        #region Construtors      
        internal HistogramNoData(double min, double max, double width): base(InitializeBins(new Tuple<double, double>(min, max), width))
        {
        }
        #endregion
        private static IBin[] InitializeBins(Tuple<double, double> range, double width)
        {
            if (!HistogramValidator.IsConstructable(range.Item1, range.Item2, width, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
            else return InitializeEmptyBins(min: range.Item1, width, Increments(range.Item2 - range.Item1, width, Math.Ceiling)).ToArray();
        }

        public override double PDF(double x) => 0;
        public override double CDF(double x) => 0;
        public override double InverseCDF(double p) => throw new InvalidOperationException("The InverseCDF function cannot be performed because the histogram contains 0 binned observations.");
        public override double Sample(Random r = null) => throw new InvalidOperationException("The Sample function cannot be performed because the histogram contains 0 binned observations.");
        public override double[] Sample(int n, Random r = null) => throw new InvalidOperationException("The Sample function cannot be performed because the histogram contains 0 binned observations.");
        public override IDistribution SampleDistribution(Random r) => this;       
    }
    

    //public static class HistogramFactory
    //{
    //    public static IHistogram Factory(double min, double max, double widths) => new HistogramNoData(min, max, widths);
    //    public static IHistogram Factory(double min, double max, int nBins) => new HistogramNoData(min, max, nBins);
    //    public static IHistogram Factory(IEnumerable<double> data, double min, double max, double widths)
    //    {
    //        IData sampleData = new Data(data);
    //        return sampleData.IsValid ? (IHistogram)new HistogramBinnedData(sampleData, min, max, widths) : new HistogramNoData(min, max, widths);
    //    }
    //    public static IHistogram Factory(IEnumerable<double> data, double min, double max, int nBins)
    //    {
    //        if (!HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //        else return Factory(data, (max - min) / nBins);
    //    }
    //    public static IHistogram Factory(IEnumerable<double> data, double widths)
    //    {
    //        IData sampleData = new Data(data);
    //        return sampleData.IsValid ? new HistogramBinnedData(sampleData, widths) : throw new InvalidConstructorArgumentsException("$The histogram could not be sized because the input data contains no finite elements and not histogram minimum or maximum were provided.");
    //    }
    //    public static IHistogram Factory(IEnumerable<double> data, int nBins)
    //    {
    //        IData sampleData = new Data(data);
    //        return sampleData.IsValid ? new HistogramBinnedData(sampleData, (sampleData.Maximum + double.Epsilon - sampleData.Minimum) / (double)nBins) : throw new InvalidConstructorArgumentsException("$The histogram could not be sized because the input data contains no finite elements and not histogram minimum or maximum were provided.");
    //    }
    //}
    
}
