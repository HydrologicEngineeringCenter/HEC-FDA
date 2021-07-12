using System;
using System.Collections.Generic;

using Utilities;

using Statistics.Validation;
using Statistics.Histograms;
using System.Xml.Linq;

namespace Statistics
{
    /// <summary>
    /// Provides factory methods for the construction of objects implementing the <see cref="IHistogram"/> interface.
    /// </summary>
    public static class IHistogramFactory
    {
        /// <summary>
        /// Constructs an <see cref="IHistogram"/> with <paramref name="nBins"/> empty bins on the range specified by the <paramref name="min"/> and <paramref name="max"/> arguments.
        /// </summary>
        /// <param name="min"> The inclusive <see cref="IHistogram"/> minimum value (<seealso cref="IDistribution.Range"/>). </param>
        /// <param name="max"> The <b>exclusive</b> <see cref="IHistogram"/> maximum value (<seealso cref="IDistribution.Range"/>) </param>
        /// <param name="nBins"> The desired number of <see cref="IHistogram.Bins"/> between the specified <paramref name="min"/> and <paramref name="max"/> values. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface with no binned data. </returns>
        public static IHistogram Factory(double min, double max, int nBins) => HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors) ? Factory(min, max, (max - min) / (double)nBins) : throw new InvalidConstructorArgumentsException(errors);
        /// <summary>
        /// Constructs an <see cref="IHistogram"/> with empty bins on the range specified by the <paramref name="min"/> and <paramref name="max"/> arguments.
        /// </summary>
        /// <param name="min"> The inclusive <see cref="IHistogram"/> minimum value (<seealso cref="IDistribution.Range"/>). </param>
        /// <param name="max"> The <b>exclusive</b> <see cref="IHistogram"/> maximum value (<seealso cref="IDistribution.Range"/>) </param>
        /// <param name="width"> The desired width of each <see cref="IBin"/> between the specified <paramref name="min"/> and <paramref name="max"/> values. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface with no binned data. </returns>
        public static IHistogram Factory(double min, double max, double width) => HistogramValidator.IsConstructable(min, max, width, out IList<string> errors) ? new Histograms.HistogramNoData(min, max, width) : throw new InvalidConstructorArgumentsException(errors);
        /// <summary>
        /// Constructs a histogram with provided <paramref name="data"/>. Bins are constructed with the specified <paramref name="width"/> on the <see cref="IData.Range"/>.
        /// </summary>
        /// <param name="data"> The <see cref="IData.Elements"/> to be binned (<seealso cref="IData"/>. </param>
        /// <param name="width"> The desired <see cref="IBin"/> width. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface filled with the provided <paramref name="data"/>. </returns>
        public static IHistogram Factory(IData data, double width) => data.IsNull() ? throw new ArgumentNullException(nameof(data)) : HistogramValidator.IsConstructable(data.Range.Min, data.Range.Max, width, out IList<string> errors) ? Factory(data, data.Range.Min, data.Range.Max, width) : throw new InvalidConstructorArgumentsException(errors);
        /// <summary>
        /// Constructs a histogram with provided <paramref name="data"/>. Bins are constructed with the specified <paramref name="width"/> on the range of the <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="data"> The <see cref="IData.Elements"/> to be binned (<seealso cref="IData"/>. </param>
        /// <param name="min"> The inclusive <see cref="IHistogram"/> minimum value (<seealso cref="IDistribution.Range"/>). </param>
        /// <param name="max"> The <b>exclusive</b> <see cref="IHistogram"/> maximum value (<seealso cref="IDistribution.Range"/>) </param>
        /// <param name="width"> The desired <see cref="IBin"/> width. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface filled with the provided <paramref name="data"/>. </returns>
        public static IHistogram Factory(IData data, double min, double max, double width) => HistogramValidator.IsConstructable(min, max, width, out IList<string> errors) ? new Histograms.HistogramBinnedData(data, min, max, width) : throw new InvalidConstructorArgumentsException(errors);
        /// <summary>
        /// Constructs a histogram with provided <paramref name="data"/>. The specified <paramref name="nBins"/> bins are constructed on the <paramref name="data"/> <see cref="IData.Range"/>.
        /// </summary>
        /// <param name="data"> The <see cref="IData.Elements"/> to be binned (<seealso cref="IData"/>. </param>
        /// <param name="nBins"> The desired number of <see cref="IBin"/>s on the <see cref="IData.Range"/> generated by the <paramref name="data"/>. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface filled with the provided <paramref name="data"/>. </returns>
        public static IHistogram Factory(IData data, int nBins)
        {

            if(data.IsNull())
            {
                throw new ArgumentNullException(nameof(data));
            }
            else 
            {
                if(data.Range.Min == data.Range.Max)
                {
                    double increment = data.Range.Min * .1;
                    double max = data.Range.Min + increment;
                    double width = (max - data.Range.Min) / (double)nBins;
                    return Factory(data, data.Range.Min, max, width);
                }
                if (HistogramValidator.IsConstructable(data.Range.Min, data.Range.Max, nBins, out IList<string> errors))
                {
                    return Factory(data, data.Range.Min, data.Range.Max, (data.Range.Max - data.Range.Min) / (double)nBins);
                }
                else
                {
                    throw new InvalidConstructorArgumentsException(errors);
                }
            }
            
        }

        /// <summary>
        /// Constructs a histogram with provided <paramref name="data"/>. <paramref name="nBins"/> <see cref="IBin"/>s constructed on the specified range provided by the <paramref name="min"/> and <paramref name="max"/> values. 
        /// </summary>
        /// <param name="data"> The <see cref="IData.Elements"/> to be binned. </param>
        /// <param name="min"> The inclusive <see cref="IHistogram"/> minimum value (<seealso cref="IDistribution.Range"/>). </param>
        /// <param name="max"> The <b>exclusive</b> <see cref="IHistogram"/> maximum value (<seealso cref="IDistribution.Range"/>) </param>
        /// <param name="nBins"> The desired number of <see cref="IBin"/>s on the range provided by the <paramref name="min"/> and <paramref name="max"/> values. </param>
        /// <returns> A new object implementing the <see cref="IHistogram"/> interface filled with the provided <paramref name="data"/>.</returns>
        public static IHistogram Factory(IData data, double min, double max, int nBins)
        {
            if (HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors))
            {
                return Factory(data, min, max, (max - min) / (double)nBins);
            }
            else
            {
                throw new InvalidConstructorArgumentsException(errors);
            }
        }

        /// <summary>
        /// Constructs a new <see cref="IHistogram"/> by adding the <paramref name="data"/> to an existing <see cref="IHistogram"/>.
        /// </summary>
        /// <param name="histogram"> The existing histogram to which the <paramref name="data"/> will be added. </param>
        /// <param name="data"> The <see cref="IData.Elements"/> to be added to the existing <paramref name="histogram"/> object. </param>
        /// <returns> A new <see cref="IHistogram"/> containing the <paramref name="histogram"/> and <paramref name="data"/> elements. </returns>
        public static IHistogram Factory(IHistogram histogram, IData data) => data.IsNull() ? throw new ArgumentNullException(nameof(data)) : Histograms.Histogram.Fit(histogram, data);

        public static IHistogram Factory(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement binsElement = doc.Element(Histogram.XML_BINS);

            List<IBin> bins = new List<IBin>();
            foreach(XElement binElem in binsElement.Elements(Histogram.XML_BIN))
            {
                double min = Convert.ToDouble( binElem.Attribute(Histogram.XML_MIN).Value);
                double max = Convert.ToDouble(binElem.Attribute(Histogram.XML_MAX).Value);
                int count = Convert.ToInt32(binElem.Attribute(Histogram.XML_COUNT).Value);

                bins.Add( IBinFactory.Factory(min, max, count));
            }

            return new HistogramBinnedData(bins.ToArray());
        }

    }
}
