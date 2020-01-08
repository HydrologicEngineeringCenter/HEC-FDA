using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Utilities;

using Statistics.Validation;

namespace Statistics
{
    public static class IHistogramFactory
    {
        public static IHistogram Factory(double min, double max, int nBins) => HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors) ? Factory(min, max, (max - min) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(double min, double max, double width) => HistogramValidator.IsConstructable(min, max, width, out IList<string> errors) ? new Histograms.HistogramNoData(min, max, width) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, double width) => HistogramValidator.IsConstructable(data.Range.Min, data.Range.Max, width, out IList<string> errors) ? Factory(data, data.Range.Min, data.Range.Max, width) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, double min, double max, double width) => HistogramValidator.IsConstructable(min, max, width, out IList<string> errors) ? new Histograms.HistogramBinnedData(data, min, max, width) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, int nBins) => HistogramValidator.IsConstructable(data.Range.Min, data.Range.Max, nBins, out IList<string> errors) ? Factory(data, data.Range.Min, data.Range.Max, (data.Range.Max - data.Range.Min) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, double min, double max, int nBins) => HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors) ? Factory(data, min, max, (max - min) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IHistogram histogram, IData data, List<IConvergenceCriteria> criterias) => Histograms.Histogram.Fit(histogram, data, criterias);
    }
}
