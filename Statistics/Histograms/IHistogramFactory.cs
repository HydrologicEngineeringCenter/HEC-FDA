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
        public static IHistogram Factory(IData data, double width) => HistogramValidator.IsConstructable(data.Minimum, data.Maximum, width, out IList<string> errors) ? Factory(data, data.Minimum, data.Maximum, width) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, double min, double max, double width) => HistogramValidator.IsConstructable(min, max, width, out IList<string> errors) ? new Histograms.HistogramBinnedData(data, min, max, width) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, int nBins) => HistogramValidator.IsConstructable(data.Minimum, data.Maximum, nBins, out IList<string> errors) ? Factory(data, data.Minimum, data.Maximum, (data.Maximum - data.Minimum) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);
        public static IHistogram Factory(IData data, double min, double max, int nBins) => HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors) ? Factory(data, min, max, (max - min) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);     
    }
}
