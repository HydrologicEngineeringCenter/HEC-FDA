using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

using Statistics.Validation;
using System.Xml.Linq;

namespace Statistics.Histograms
{
    internal abstract class Histogram : IHistogram
    {
        private readonly IBin[] _Bins;
        #region Properties
        public IBin[] Bins => (IBin[])_Bins.Clone();
        #region IDistribution Properties
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double Skewness { get; }
        public double StandardDeviation { get; }
        public IRange<double> Range { get; }
        public int SampleSize { get; }
        public double Mode { get; }
        public IDistributionEnum Type => IDistributionEnum.Histogram;
        #endregion      
        #region IMessagePublisher Properties
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion
        #endregion

        #region Constructor
        protected Histogram(IBin[] bins)
        {          
            _Bins = bins;
            var stats = ISampleStatisticsFactory.Factory(bins);
            Mean = stats.Mean;
            Median = stats.Median;
            Variance = stats.Variance;
            Skewness = stats.Skewness;
            StandardDeviation = stats.StandardDeviation;
            Range = stats.Range;
            SampleSize = stats.SampleSize;
            State = Validate(new Validation.HistogramValidator(), out IEnumerable<IMessage> msgs);
            Messages = stats.Messages.Concat(msgs);
            //IsConverged = false;
        }
        protected Histogram(IHistogram histogram, List<IConvergenceResult> convergenceResults)
        {
            _Bins = histogram.Bins;
            Mean = histogram.Mean;
            Median = histogram.Median;
            Variance = histogram.Variance;
            Skewness = histogram.Skewness;
            StandardDeviation = histogram.StandardDeviation;
            Range = histogram.Range;
            SampleSize = histogram.SampleSize;
            State = histogram.State;
            bool isConverged = true;
            List<IMessage> msgs = new List<IMessage>(histogram.Messages);
            foreach (var result in convergenceResults)
            {
                if (!result.Passed) isConverged = false;
                msgs.Add(result.TestMessage);
            }
            Messages = msgs;
        }
        #endregion

        #region Functions
        public static IHistogram Fit(IData data, int nBins = 100) => IHistogramFactory.Factory(data, nBins);
        public static IHistogram Fit(IHistogram histogram, IData sample)
        {
            // 1. Expand histogram bins to accommodate new sample data, place new data in the expended bins.
            return new HistogramBinnedData(FillHistogramBins(ExpandHistogramRange(histogram, sample.Range), sample).ToArray());
            //// 2. Get convergence results
            //List<IConvergenceResult> results = CriteriaResults(criterias, histogram, newHistogram, sample.SampleSize);
            //return new HistogramBinnedData(newHistogram);
        }
        //private static List<IConvergenceResult> CriteriaResults(IList<IConvergenceCriteria> criterias, IHistogram histogramBefore, IHistogram histogramAfter, int sampleSize)
        //{
        //    List<IConvergenceResult> results = new List<IConvergenceResult>();
        //    foreach (var criteria in criterias)
        //    {
        //        results.Add(criteria.Test(histogramBefore.InverseCDF(criteria.Quantile), histogramAfter.InverseCDF(criteria.Quantile), sampleSize, histogramAfter.SampleSize));
        //    }
        //    return results;
        //}
        /// <summary>
        /// Generates a list of quantile values for convergence testing (<seealso cref="IDistribution.InverseCDF(double)"/>). 
        /// </summary>
        /// <param name="criteria"> Convergence criteria containing the quantile values to be tested. </param>
        /// <returns> A <see cref="List{T}"/> containing the quantile values. </returns>
        private static List<double> QuantileValues(IHistogram histogram, IEnumerable<IConvergenceCriteria> criteria)
        {
            List<double> qValues = new List<double>();
            foreach (var element in criteria) qValues.Add(histogram.InverseCDF(element.Quantile));
            return qValues;
        }
        /// <summary>
        /// Builds list of <see cref="IBin"/> from a existing <see cref="IHistogram"/> expanded to fit an <see cref="IData.Range"/>.
        /// </summary>
        /// <param name="dataRange"> The range of the <see cref="IData"/> to be binned in the new <see cref="IHistogram"/>. </param>
        /// <returns> A <see cref="List{IBin}"/> in which an existing <see cref="IHistogram"/> and new <see cref="IData"/> observations can be binned. </returns>
        private static List<IBin> ExpandHistogramRange(IHistogram histogram, IRange<double> dataRange)
        {
            List<IBin> bins = new List<IBin>();
            double loRange = histogram.Range.Min - dataRange.Min;
            if (loRange > 0)
            {
                double width = histogram.Bins[0].Range.Max - histogram.Bins[0].Range.Min;
                int loBinCount = Increments(loRange, width, Math.Ceiling);
                double newMin = histogram.Range.Min - loBinCount * width;
                bins.AddRange(InitializeEmptyBins(newMin, width, loBinCount));
            }
            bins.AddRange(histogram.Bins);
            double hiRange = dataRange.Max - histogram.Range.Max;
            if (hiRange > 0)
            {
                double width = histogram.Bins[0].Range.Max - histogram.Bins[0].Range.Min;
                int hiBinCount = Increments(hiRange, width, Math.Ceiling);
                bins.AddRange(InitializeEmptyBins(histogram.Range.Max, width, hiBinCount));
            }
            return bins;
        }
        private static IBin[] FillHistogramBins(List<IBin> bins, IData data)
        {
            int i = 0, n = 0;
            foreach (double x in data.Elements)
            {
                if (x.IsOnRange(bins[i].Range.Min, bins[i].Range.Max, inclusiveMin: true, inclusiveMax: false)) n++;
                else
                {
                    if (n > 0)
                    {
                        bins[i] = new Bin(bins[i], n);
                        n = x.IsOnRange(bins[i + 1].Range.Min, bins[i + 1].Range.Max, inclusiveMin: true, inclusiveMax: false) ? 1 : 0;                        
                    }
                    while (!x.IsOnRange(bins[i + 1].Range.Min, bins[i + 1].Range.Max, inclusiveMin: true, inclusiveMax: false)) i++;
                    i++;
                }
            }
            if (n > 0) bins[i] = new Bin(bins[i], n);
            return bins.ToArray();
            //TODO: reeeeaaaaalllly need to test this.
        }

        public bool Equals(IHistogram histogram)
        {
            if (histogram.Bins.Length != _Bins.Length) return false;
            else for (int i = 0; i < _Bins.Length; i++) if (!_Bins[i].Equals(histogram.Bins[i])) return false;
            return true;
        }
        #region Initialization Functions
        /// <summary>
        /// Provides the histogram minimum and maximum values by comparing the data range to the requested histogram range (implied by the minimum (e.g. <paramref name="min"/>) and maximum (e.g. <paramref name="max"/>) values). If the data range is broader than the requested histogram range the requested range is broadened to accomdate all data elements. 
        /// </summary>
        /// <param name="data"> Data to be binned. </param>
        /// <param name="min"> The requested histogram minimum value. </param>
        /// <param name="max"> The requested histogram maximum value. </param>
        /// <param name="msgs"> If the histogram range is expanded to accommodate data elements a message detailing this change is provided </param>
        /// <returns> A tuple with the histogram minimum and maximum values, respectively. </returns>
        protected static Tuple<double, double> GetHistogramRange(double min, double max, IData data, ref List<IMessage> msgs)
        {
            var range = new Tuple<double, double>(data.Elements.First() < min ? data.Elements.First() : min, data.Elements.Last() > max ? data.Elements.Last() + double.Epsilon : max);
            if (!(range.Item1 == min && range.Item2 == max)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The requested data range: [{min}, {max}) was modified to [{range.Item1}, {range.Item2}) in order to accomidated some data elements outside of the provided range."));
            return range;
        }
        protected static int Increments(double range, double width, Func<double, double> rounding) => range < width ? 0 : (int)rounding(range / width);
        protected static List<IBin> InitializeEmptyBins(double min, double width, int nBins)
        {
            List<IBin> bins = new List<IBin>();
            for (int i = 0; i < nBins; i++)
            {
                bins.Add(new Bin(min, min + width, 0));
                min += width;
            }
            return bins;
        }
        /// <summary>
        /// Creates histogram bins and places data into that histogram.
        /// </summary>
        /// <param name="data"> Data to be binned in the histogram. </param>
        /// <param name="min"> The histogram minimum (inclusive). </param>
        /// <param name="max"> The histogram maximum (exclusive). </param>
        /// <param name="widths"> Histogram bin widths. </param>
        /// <param name="mean"> The histogram (not data) mean. </param>
        /// <param name="samplesize"> The histogram sample size (not necessarily the original data array length). </param>
        /// <returns></returns>
        protected static List<IBin> InitializeDataBins(double min, IData data, double width)
        {
            int n = 0;
            List<IBin> bins = new List<IBin>();
            IBin nextBin = new Bin(min, min + width, 0);
            foreach (double x in data.Elements)
            {
                if (x.IsOnRange(nextBin.Range.Min, nextBin.Range.Max, inclusiveMin: true, inclusiveMax: false)) n++;
                else
                {
                    while (!x.IsOnRange(nextBin.Range.Min, nextBin.Range.Max, inclusiveMin: true, inclusiveMax: false))
                    { //or x > nextBin.Maximum
                        bins.Add(new Bin(nextBin, n));
                        n = 0;
                        nextBin = new Bin(nextBin.Range.Max, nextBin.Range.Max + width, 0);
                    }
                    n++;
                }
            }
            bins.Add(new Bin(nextBin, n));
            return bins;
        }
        #region IValidate Function
        public IMessageLevels Validate(IValidator<IHistogram> validator, out IEnumerable<IMessage> messages)
        {
            return validator.IsValid(this, out messages);
        }
        #endregion
        public static string Print(int n, int nBins, IRange<double> range) => $"Histogram(observations: {n.Print()}, bins: {nBins.Print()}, range: {range.Print(true)})";
        public static string RequiremedParameterization(bool printNotes)
        {
            string msg = $"Histograms require the following parameterization: {Parameterization()}.";
            if (printNotes) msg += RequirementNotes();
            return msg;
        }
        public static string RequirementNotes() => $"The histogram must contain 1 or more bins (only one bin is unlikely to produce the desired results).";
        public static string Parameterization() => $"Histogram(observations: [{1}, {int.MaxValue.Print()}], bins: {Bin.Parameterization()}, range: {Resources.DoubleRangeRequirements()})";
        #endregion
        #region IDistribution Functions
        public abstract double PDF(double x);
        public abstract double CDF(double x);
        public abstract double InverseCDF(double p);
        public abstract double Sample(Random r = null);
        public abstract double[] Sample(int n, Random r = null);
        public abstract IDistribution SampleDistribution(Random r);
        public string Print(bool round) => round ? Print(SampleSize, Bins.Length, Range) : $"Histogram(observations: {SampleSize}, bins: {Bins.Length}, range: {Range.Print()})";
        public string Requirements(bool printNotes) => RequiremedParameterization(printNotes);
        public bool Equals(IDistribution distribution) => distribution.Type == IDistributionEnum.Histogram ? Equals((IHistogram)distribution) : false;

        public static readonly string XML_BINS = "Bins";
        public static readonly string XML_BIN = "Bin";
        public static readonly string XML_MIN = "Min";
        public static readonly string XML_MAX = "Max";
        public static readonly string XML_MIDPOINT = "MidPoint";
        public static readonly string XML_COUNT = "Count";

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement(XML_BINS);
            foreach(IBin bin in Bins)
            {
                XElement binElem = new XElement(XML_BIN);
                binElem.SetAttributeValue(XML_MIN, bin.Range.Min);
                binElem.SetAttributeValue(XML_MAX, bin.Range.Max);
                binElem.SetAttributeValue(XML_MIDPOINT, bin.MidPoint);
                binElem.SetAttributeValue(XML_COUNT, bin.Count);

                masterElem.Add(binElem);
            }
            return masterElem;
        }
        #endregion
        #endregion
    }

    //internal class Histogram : IHistogram, IDistribution, IValidate<IHistogram>
    //{
    //    #region Properties
    //    #region IValidate Properties
    //    public bool IsValid { get; }
    //    public IEnumerable<IMessage> Messages { get; }
    //    #endregion
    //    #region IDistribution Properties
    //    public IDistributions Type => IDistributions.Histogram;
    //    public double Mean { get; }
    //    public double Median { get; }
    //    public double StandardDeviation { get; }
    //    public double Variance { get; }
    //    public double Skewness { get; }
    //    public double Minimum { get; }
    //    public double Maximum { get; }
    //    public int SampleSize { get; }
    //    #endregion
    //    public IBin[] Bins { get; }
    //    #endregion

    //    #region Construtors
    //    /* Constructors:
    //     *      a. min, max, nBins
    //     *      b. min, max, nBins, data
    //     *      b. min, max, widths
    //     *      >c. min, max, widths, data
    //     *      >d. sample, bool - beta
    //     *      >e. histogram, sample, bool - beta
    //     */
    //    /// <summary>
    //    /// Constructor for histogram with empty bins, built with a min, max and requested number of bins.
    //    /// An InvalidConstructorArguementException is thrown if the provided parameters cannot be used to construct a histogram.
    //    /// </summary>
    //    /// <param name="min"> The requested histogram minimum value (inclusive). </param>
    //    /// <param name="max"> The requested histogram maximum value (exclusive). </param>
    //    /// <param name="nBins"> The requested number of histograms, from which a bin width is infered. </param>
    //    internal Histogram(double min, double max, int nBins)
    //    {
    //        if (!HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //        double widths = (max - min) / nBins;
    //        new Histogram(min, max, widths);
    //    }
    //    /// <summary>
    //    /// Constructor for histogram with empty bins, built with a min, max and requested bin width.
    //    /// An InvalidConstructorArguementException is thrown if the provided parameters cannot be used to construct a histogram.
    //    /// </summary>
    //    /// <param name="min"> The requested histogram minimum value (inclusive). </param>
    //    /// <param name="max"> The requested histogram maximum value (exclusive). </param>
    //    /// <param name="widths"> The requested histogram bin widths from which the number of bins is infered. </param>
    //    internal Histogram(double min, double max, double widths)
    //    {
    //        if (!HistogramValidator.IsConstructable(min, max, widths, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //        else
    //        {
    //            var emptyStats = new SummaryStatistics();
    //            Bins = InitializeBins(min, widths, nBinsOnRange(max - min, widths)).ToArray();
    //            Minimum = min;
    //            Maximum = max;
    //            Mean = emptyStats.Mean;
    //            Variance = emptyStats.Variance;
    //            Skewness = emptyStats.Skewness;
    //            SampleSize = emptyStats.SampleSize;
    //            StandardDeviation = emptyStats.StandardDeviation;
    //            IsValid = Validate(new HistogramValidator(), out IEnumerable<IMessage> messages);
    //            Messages = messages;
    //        }
    //    }
    //    internal Histogram(IEnumerable<double> data, int nBins)
    //    {
    //        // 1. Data preparation
    //        Data orderedData = new Data(data);
    //        //if this is invalid cannot construct.
    //    }

    //    private IList<IBin> InitializeBins(double min, double width, int nBins)
    //    {
    //        IList<IBin> bins = new List<IBin>();
    //        for (int i = 0; i < nBins; i++)
    //        {
    //            bins[i] = new Bin(min, min + width, 0);
    //            min += width;
    //        }
    //        return bins;
    //    }

    //    /// <summary>
    //    /// Constructor for histogram built with the requested <paramref name="widths"/> and provided set of <paramref name="data"/>, a scaled beta distribution is fit to the provided <paramref name="data"/> to provide an esitmated minimum and maximum histogram value. 
    //    /// </summary>
    //    /// <param name="data"> The data to be binned. </param>
    //    /// <param name="widths"> The requested bin widths. This may be narrowed by the scaled beta distribution minimum and maximum values </param>
    //    //internal Histogram(IEnumerable<double> data, double widths)
    //    //{
    //    //    if (!HistogramValidator.IsConstructable(data, widths, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //    //    else
    //    //    {
    //    //        //TODO: What happens is if the data contains non-finite values
    //    //        Distributions.Beta4Parameters beta = Distributions.Beta4Parameters.Fit(data);
    //    //        int nBins = nBinsOnRange(beta.Maximum - beta.Minimum, widths);
    //    //        new Histogram(min: beta.Minimum, max: beta.Maximum, nBins, data);
    //    //    }
    //    //}
    //    /// <summary>
    //    /// Constructor for histogram built on a specified [<paramref name="min"/>, <paramref name="max"/>) range with a specified number of <see cref="IBin"/>s with the provided set of <paramref name="data"/>.
    //    /// An InvalidConstructorArguementException is thrown if the provided parameters cannot be used to construct the requested histogram.
    //    /// </summary>
    //    /// <param name="min"> The requested histogram minimum value (inclusive). </param>
    //    /// <param name="max"> The requested histogram maximum value (exclusive). </param>
    //    /// <param name="nBins"> The requested number of histogram bins from which the bin widths are infered. </param>
    //    /// <param name="data"> The data to be binned in the histogram. </param>
    //    internal Histogram(double min, double max, int nBins, IEnumerable<double> data)
    //    {
    //        if (!HistogramValidator.IsConstructable(min, max, nBins, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //        else
    //        {
    //            if (data.IsNullOrEmpty()) new Histogram(min, max, (max - min) / nBins);
    //            else
    //            {
    //                var dataToBin = OrderFiniteSample(data, out IMessage nonFiniteElementMessage);
    //                Tuple<double, double> range = HistogramRange(dataToBin, out IMessage changedRangeMessage, min, max);
    //                IEnumerable<IMessage> dataMessages = DataMessages(nonFiniteElementMessage, changedRangeMessage);
    //                Bins = BinData(dataToBin, range.Item1, range.Item2, (range.Item2 - range.Item1) / nBins, out double mean, out int n).ToArray();
    //                Mean = mean;
    //                Minimum = range.Item1;
    //                Maximum = range.Item2;
    //                SampleSize = n;
    //                var dispersion = SecondAndThirdMoments(Bins, mean);
    //                Variance = dispersion.Item1;
    //                Skewness = dispersion.Item2;
    //                StandardDeviation = Math.Sqrt(Variance);
    //                IsValid = Validate(new Validation.HistogramValidator(), out IEnumerable<IMessage> messages);
    //                Messages = dataMessages.Concat(messages);
    //            }
    //        }
    //    }


    //    //internal Histogram(IHistogram histogram, IEnumerable<double> data)
    //    //{
    //    //    if (data.IsNullOrEmpty()) new Histogram(histogram, true);
    //    //    else
    //    //    {
    //    //        var orderedData = OrderFiniteSample(data, out IMessage dataMessage);
    //    //        Tuple<double, double> range = HistogramRange(orderedData, out IMessage rangeMessage);
    //    //        IEnumerable<IMessage> dataMessages = DataMessages(dataMessage, rangeMessage);

    //    //        //remove conditional logic stuff.
    //    //        //int nLoBins = range.Item1 < histogram.Minimum ? AddNBins(histogram.Minimum - range.Item1, histogram.BinWidths) : 0;
    //    //        //int nHiBins = range.Item2 > histogram.Maximum ? AddNBins(range.Item2 - histogram.Maximum, histogram.BinWidths) : 0;
    //    //    }

    //    //}
    //    private bool IsConstructable(IHistogram histogram, IEnumerable<double> data, out IList<string> errors)
    //    {
    //        errors = new List<string>();
    //        throw new NotImplementedException();
    //    }
    //    //private Histogram(IHistogram histogram, bool emptySampleMessage)
    //    //{
    //    //    Bins = histogram.Bins;
    //    //    Mean = histogram.Mean;
    //    //    Minimum = histogram.Minimum;
    //    //    Maximum = histogram.Maximum;
    //    //    Variance = histogram.Variance;
    //    //    Skewness = histogram.Skewness;
    //    //    SampleSize = histogram.SampleSize;
    //    //    StandardDeviation = histogram.StandardDeviation;
    //    //    IsValid = histogram.IsValid;
    //    //    var msgs = Messages.ToList();
    //    //    msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, "$An empty or null dataset was provided, returning the original histogram with this message."));
    //    //    Messages = msgs;
    //    //}
    //    /// <summary>
    //    /// Provides an ordered copy of specified <paramref name="data"/> with any non-finite elements removed.
    //    /// </summary>
    //    /// <param name="data"> The data to be binned in the histogram. </param>
    //    /// <param name="message"> An <see cref="IMessage"/> specifying if any non-finite data was removed from the <paramref name="data"/> data. </param>
    //    /// <returns> An ordered enumerable of doubles without any <see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/> or <see cref="double.PositiveInfinity"/> items. </returns>
    //    private IOrderedEnumerable<double> OrderFiniteSample(IEnumerable<double> data, out IMessage message)
    //    {
    //        List<double> finite = new List<double>();
    //        foreach (double x in data) if (x.IsFinite()) finite.Add(x);
    //        message = data.Count() == finite.Count ? null : IMessageFactory.Factory(IMessageLevels.Message, "One or more non-finite elements in the sample were exclueded from this histogram.");
    //        return finite.OrderBy(i => i);
    //    }
    //    /// <summary>
    //    /// Overrides the specified histogram minimum and maximum range, if the smallest or largest data elements falls outside of this range.
    //    /// </summary>
    //    /// <param name="orderedData"> The data to be binned in the histogram. </param>
    //    /// <param name="message"> A message describing new histogram range if the specified range is altered by the data values. </param>
    //    /// <param name="min"> The specified histogram minimum. </param>
    //    /// <param name="max"> The specified histogram maximum. </param>
    //    /// <returns></returns>
    //    private Tuple<double, double> HistogramRange(IOrderedEnumerable<double> orderedData, out IMessage message, double min = double.NegativeInfinity, double max = double.NegativeInfinity)
    //    {
    //        double smallest = orderedData.First() < min ? orderedData.First() : min, largest = orderedData.Last() > max ? orderedData.Last() : max;
    //        if (smallest < min || largest > max) message = IMessageFactory.Factory(IMessageLevels.Message, $"The specified histogram range [{min}, {max}] was changed to [{smallest}, {largest}].");
    //        else message = null;
    //        return new Tuple<double, double>(smallest, largest);
    //    }      
    //    /// <summary>
    //    /// Adds any data messages to an enumerable of <see cref="IMessage"/>.
    //    /// </summary>
    //    /// <param name="nonFiniteElementMessage"></param>
    //    /// <param name="changedRangeMessage"></param>
    //    /// <returns></returns>
    //    private IEnumerable<IMessage> DataMessages(IMessage nonFiniteElementMessage = null, IMessage changedRangeMessage = null)
    //    {
    //        List<IMessage> msgs = new List<IMessage>();
    //        if (!changedRangeMessage.IsNull()) msgs.Add(changedRangeMessage);
    //        if (!nonFiniteElementMessage.IsNull()) msgs.Add(nonFiniteElementMessage);
    //        return msgs;
    //    }

    //    private int nBelowMinBins(double newMin, double oldMin, double widths) => nBinsOnRange(oldMin - newMin, widths);
    //    private int nAboveMaxBins(double newMax, double oldMax, double widths) => nBinsOnRange(newMax - oldMax, widths);
    //    private int nBinsOnRange(double range, double widths) => range > 0 ? (int)Math.Ceiling(range / widths) : 0;

    //    /// <summary>
    //    /// Provides the initial set of histogram bins containing no sample data.
    //    /// </summary>
    //    /// <param name="min"> The minimum value for the set of bins (inclusive). </param>
    //    /// <param name="max"> The maximum value for the set of bins (exclusive). </param>
    //    /// <param name="widths"> The bin widths. </param>
    //    /// <returns> A list of bins containing no observations. </returns>
    //    private List<IBin> InitializeBins(double min, double max, double widths)
    //    {
    //        List<IBin> bins = new List<IBin>();
    //        while (min < max)
    //        {
    //            if (min + widths <= max) bins.Add(new Bin(min, min + widths, 0));
    //            min += widths;
    //        }
    //        return bins;
    //    }
    //    /// <summary>
    //    /// Creates histogram bins and places data into that histogram.
    //    /// </summary>
    //    /// <param name="data"> Data to be binned in the histogram. </param>
    //    /// <param name="min"> The histogram minimum (inclusive). </param>
    //    /// <param name="max"> The histogram maximum (exclusive). </param>
    //    /// <param name="widths"> Histogram bin widths. </param>
    //    /// <param name="mean"> The histogram (not data) mean. </param>
    //    /// <param name="samplesize"> The histogram sample size (not necessarily the original data array length). </param>
    //    /// <returns></returns>
    //    private List<IBin> BinData(IOrderedEnumerable<double> data, double min, double max, double widths, out double mean, out int samplesize)
    //    {
    //        // 1. Initialize bin data
    //        List<double> orderedData = data.ToList();
    //        // 2. Initialize with empty bins below sample min.
    //        List<IBin> bins = InitializeBins(min: min, max: orderedData[0], widths);
    //        // 3. Add sample data to bins.
    //        samplesize = 0;
    //        int n = 0, i = bins.Count == 0 ? 0 : bins.Count - 1;
    //        double binmin = bins.Count == 0 ? min: bins[i].Minimum, binmax = binmin + widths, sum = 0;
    //        foreach (double x in orderedData)
    //        {
    //            // criteria for binning: binmin <= x < binmax
    //            if (!x.IsOnRange(binmin, binmax, inclusiveMin: true, inclusiveMax: false))
    //            {
    //                // generate bin
    //                IBin bin = new Bin(binmin, binmax, n);
    //                sum += bin.MidPoint * n;
    //                bins.Add(bin);
    //                // range for next bin
    //                binmin = binmax;
    //                binmax = binmin + widths;
    //            }
    //            n++;
    //            samplesize++;
    //        }
    //        IBin lastDataBin = new Bin(binmin, binmax, n);
    //        sum += lastDataBin.MidPoint * n;
    //        mean = sum / samplesize;
    //        bins.Add(lastDataBin);                    
    //        // 4. Add empty bins above sample max 
    //        bins.AddRange(FinalizeBins(binmax, max, widths));
    //        return bins;
    //    }
    //    private List<IBin> FinalizeBins(double min, double max, double widths)
    //    {
    //        double binmax = min + widths;
    //        List<IBin> bins = new List<IBin>();
    //        while (!(binmax > max))
    //        {
    //            bins.Add(new Bin(min, binmax, 0));
    //            min = binmax;
    //            binmax = min + widths;
    //        }
    //        return bins;
    //    }
    //    private Tuple<double, double> SecondAndThirdMoments(IBin[] bins, double mean)
    //    {
    //        double deviations = 0, variance = 0, skewness = 0;
    //        foreach (IBin bin in bins)
    //        {
    //            deviations = (bin.MidPoint - mean) * bin.Count;
    //            variance = Math.Pow(deviations, 2);
    //            skewness = Math.Pow(deviations, 3);
    //        }
    //        return new Tuple<double, double>(variance, skewness);
    //    }
    //    #endregion


    //    private int BinIntF(double x, List<IBin> bins)
    //    {
    //        if (x < bins[0].Minimum) return -1;
    //        for (int i = 0; i < bins.Count; i++)
    //        {
    //            if (x < bins[i].Maximum) return i;
    //        }
    //        return bins.Count;
    //    }

    //    //TODO: Add convergence
    //    //TODO: Keep bins in accending order
    //    //TODO: Fit to Beta4Parameter distribution
    //    //TODO: Construct with bin widths (instead of nBins)

    //    #region Old Properties
    //    #region IValidate Properties
    //    //public bool IsValid { get; }
    //    //public IEnumerable<IMessage> Messages { get; }
    //    #endregion
    //    #region IDistribution Properties
    //    //public IDistributions Type => IDistributions.Histogram;
    //    //public double Mean { get; }
    //    //public double Median { get; }
    //    //public double StandardDeviation { get; }
    //    //public double Variance { get; }
    //    //public double Skewness { get; }
    //    //public double Minimum { get; }
    //    //public double Maximum { get; }
    //    //public int SampleSize { get; }
    //    #endregion
    //    //public IBin[] Bins { get; }
    //    #endregion
    //    #region Old Constructors
    //    /// <summary>
    //    /// Histogram constructor for initial sample.
    //    /// </summary>
    //    /// <param name="sample"> Data to place in histogram. </param>
    //    /// <param name="nBins"> The number of desired bins. </param>
    //    //public Histogram(IEnumerable<double> sample, int nBins = 100)
    //    //{
    //    //    if (!IsConstructable(sample, nBins, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
    //    //    else
    //    //    {

    //    //    }
    //    //    if (IsConstructable(sample, nBins, out _))
    //    //    {
    //    //        //1. Bin data also collect sample size and histogram mean.
    //    //        Bins = BinData(sample, nBins, out int n, out double mu);
    //    //        SampleSize = n; Mean = mu;
    //    //        //2. Collect histogram measures of dispersion on second pass.
    //    //        Minimum = Bins[0].Minimum; Maximum = Bins[Bins.Length - 1].Maximum;
    //    //        var dispersionTuple = HistogramMedianAndDeviations(Bins, Mean, SampleSize);
    //    //        Median = dispersionTuple.Item1;  Variance = dispersionTuple.Item2 / (SampleSize - 1);
    //    //        Skewness = dispersionTuple.Item3 / (SampleSize - 1); StandardDeviation = Math.Sqrt(Variance);
    //    //    }
    //    //    else throw new ArgumentException();
    //    //}
    //    //private bool IsConstructable(IEnumerable<double> sample, int nBins, out IList<string> errors)
    //    //{
    //    //    errors = new List<string>();
    //    //    if (nBins < 1) errors.Add($"The requested number of bins: {nBins} is invalid, at least one bin must be requested.");
    //    //    if (sample.IsNullOrEmpty()) errors.Add("The provided sample data is invalid because it is null or empty.");
    //    //    else
    //    //    {
    //    //        SummaryStatistics stats = new SummaryStatistics(sample);
    //    //        if (stats.Variance == 0) errors.Add("The provided sample data is invalid in the current constructor because it lacks variance and therefore cannot be used to generate histogram bin widths.");
    //    //    }
    //    //    return errors.Any();
    //    //}

    //    //public Histogram(IEnumerable<double> sample, double width)
    //    //{
    //    //    throw new NotImplementedException();    
    //    //}
    //    //private bool IsConstructable(IEnumerable<double> sample, double width, out IList<string> errors)
    //    //{
    //    //    errors = new List<string>();
    //    //    if (!width.IsFinite() || width < 0) errors.Add($"The requested bin width: {width} is invalid because it is not a positive finite value.");
    //    //    return errors.Any();
    //    //}

    //    //private bool IsConstructable(double width, int nBins, out IList<string> errors)
    //    //{
    //    //    errors = new List<string>();
    //    //    if (!width.IsFinite() || width < 0) errors.Add($"The requested bin width: {width} is invalid because it is not a positive finite value.");
    //    //    if (nBins < 1) errors.Add($"The requested number of bins: {nBins} is invalid, at least one bin must be requested.");
    //    //    return errors.Any();
    //    //}

    //    /// <summary>
    //    /// Histogram constructor for adding sample to existing histogram.
    //    /// </summary>
    //    /// <param name="oldBins"> The pre-existing histogram bins. </param>
    //    /// <param name="sample"> The sample data to be added to the histogram. </param>
    //    private Histogram(IBin[] oldBins, IEnumerable<double> sample)
    //    {
    //        Bins = BinData(oldBins, sample, out int sampleSize, out double mean);
    //        SampleSize = sampleSize;
    //        Mean = mean;

    //        var medianAndDeviations = HistogramMedianAndDeviations(Bins, Mean, SampleSize);
    //        // Item1: Median, Item2: Squared Deviations, Item3: Cubed Deviations
    //        Median = medianAndDeviations.Item1;
    //        Variance = medianAndDeviations.Item2 / (SampleSize - 1);
    //        StandardDeviation = Math.Sqrt(Variance);
    //        Skewness = medianAndDeviations.Item3 / (SampleSize - 1);
    //        Minimum = Bins[0].Minimum;
    //        Maximum = Bins[Bins.Length - 1].Maximum;
    //    }
    //    #endregion

    //    #region Functions
    //    #region Initialization Functions
    //    #region Data Binning, Histogram Mean and Sample Size
    //    /// <summary>
    //    /// Bins a sample of data with no pre-existing bins, counts data and provides mean of the histogram.
    //    /// </summary>
    //    /// <param name="sample"> The sample of data to be bined. </param>
    //    /// <param name="nBins"> The number of bins to to create. </param>
    //    /// <param name="sampleSize"> Provides the number of observations in the histogram. </param>
    //    /// <param name="mean"> Provides the mean of the histogram (note: not the mean of the sample). </param>
    //    /// <returns> The binned sample data. </returns>
    //    private IBin[] BinData(IEnumerable<double> sample, int nBins, out int sampleSize, out double mean)
    //    {
    //        sampleSize = 0;
    //        Bin[] bins = new Bin[nBins];
    //        int binIndex = 0, binCount = 0;


    //        IEnumerable<double> data = sample.OrderBy(i => i);
    //        SummaryStatistics stats = new SummaryStatistics(data);
    //        // Bin width, Bin min, Bin max, sum of sample data values
    //        double width = (stats.Maximum - stats.Minimum) / (nBins - 1), min = data.Min() - width / 2, max = min + width, sum = 0;
    //        foreach (var x in data)
    //        {
    //            // x is greater than bin max, so create new bin(s) until x can be binned.
    //            while (!(x < max))
    //            {
    //                bins[binIndex] = new Bin(min, max, binCount);
    //                sum += bins[binIndex].MidPoint * bins[binIndex].Count;
    //                // Reset bin count, bin min, bin max, increment bin index
    //                binIndex++;
    //                binCount = 0;
    //                min = max;
    //                max = min + width;
    //            }
    //            // x is in existing bin increase count of current bin
    //            binCount++;
    //            sampleSize++;
    //        }
    //        mean = sum / sampleSize;
    //        return bins;
    //    }
    //    /// <summary>
    //    /// Bins a sample of data by adding it to pre-existing histogram bins, counts data and provides mean of the new histogram.
    //    /// </summary>
    //    /// <param name="oldBins"> The pre-exising histogram bins. </param>
    //    /// <param name="sample"> The sample of data to be added to the histogram. </param>
    //    /// <param name="sampleSize"> Provides the number of observations in the histogram. </param>
    //    /// <param name="mean"> Provides the mean of the histogram. </param>
    //    /// <returns> The binned data containing the sample, pre-existing bins and any new bins created to accomidate the sample. </returns>
    //    private IBin[] BinData(IBin[] oldBins, IEnumerable<double> sample, out int sampleSize, out double mean)
    //    {
    //        double[] data = sample.OrderBy(x => x).ToArray();
    //        // n = obs to add to bin, N = total hist obs, i = sample idx, sum = sum of hist obs
    //        List<IBin> bins = BinDataBelowHistogramMin(data, oldBins[0], out int i, out double sum);
    //        int binCount = 0, N = i + 1, binIndex = bins.Count - 1, oldBinIndex = 0;
    //        // add data observations to pre-existing histogram bins
    //        while (i < data.Length && oldBinIndex < oldBins.Length)
    //        {

    //            if (data[i] < oldBins[oldBinIndex].Minimum) throw new Exception($"An unexpected error has occured. The sample data value {data[i]} is not on the expected bin range [{oldBins[oldBinIndex].Minimum}, {oldBins[oldBinIndex].Maximum}).");
    //            if (data[i] < oldBins[oldBinIndex].Maximum) binCount++; // current observation is 'in bin'.
    //            else // current observation is 'off bin' create new bin for last n 'in bin' observations.
    //            {
    //                bins.Add(new Bin(oldBins[oldBinIndex], binCount));
    //                binCount = 0;
    //                binIndex++;
    //                oldBinIndex++;
    //                N += bins[binIndex].Count;
    //                sum += bins[binIndex].MidPoint * bins[binIndex].Count;
    //            }
    //            i++;
    //        }
    //        if (oldBinIndex != oldBins.Length) bins.AddRange(BinHistogramAboveSampleMax(oldBins, oldBinIndex, ref N, ref sum));
    //        if (i != data.Length) bins.AddRange(BinDataAboveHistogramMax(data, bins[binIndex], i, ref N, ref sum));
    //        sampleSize = N;
    //        mean = sum / sampleSize;
    //        return bins.ToArray();
    //    }
    //    /// <summary>
    //    /// Bins sample data with values below pre-existing histogam minimum value.
    //    /// </summary>
    //    /// <param name="sortedData"> Sample data to bin sorted in accending order. </param>
    //    /// <param name="firstBin"> Bin from pre-existing histogram containing its minimum value. </param>
    //    /// <param name="i"> Returns the index of the next sample data value to bin. </param>
    //    /// <param name="sum"> Returns the sum of the bins below the pre-existing histogram minimum value. </param>
    //    /// <returns> Bins of sample data with valuse below the pre-existing histogram mimimum value. </returns>
    //    private List<IBin> BinDataBelowHistogramMin(double[] sortedData, IBin firstBin, out int i, out double sum)
    //    {
    //        i = 0; // position in data
    //        sum = 0; // sum of new bins observations
    //        int binIndex = 0, binCount = 0;
    //        List<IBin> newBins = new List<IBin>();
    //        double width = firstBin.Maximum - firstBin.Minimum, binMin = sortedData.Min() - width / 2, binMax = binMin + width;
    //        // WHILE data value is less than existing historgram minimum
    //        while (sortedData[i] < firstBin.Minimum)
    //        {
    //            if (sortedData[i] < binMax) binCount++;
    //            else
    //            {
    //                newBins.Add(new Bin(binMin, binMax, binCount));
    //                sum += newBins[binIndex].MidPoint * newBins[binIndex].Count;
    //                binIndex++;
    //                binCount = 0;
    //                binMin = binMax;
    //                binMax = binMin + width;
    //            }
    //            i++;
    //        }
    //        return newBins;
    //    }
    //    /// <summary>
    //    /// Bins sample data wiht values above pre-existing histrogram maximum value.
    //    /// </summary>
    //    /// <param name="sortedData"> Sample data to bin sorted in accending order. </param>
    //    /// <param name="lastBin"> Bin from pre-existing histogram containing its maximum value. </param>
    //    /// <param name="i"> Returns the index </param>
    //    /// <param name="sum"></param>
    //    /// <returns></returns>
    //    private List<IBin> BinDataAboveHistogramMax(double[] sortedData, IBin lastBin, int i, ref int sampleSize, ref double sum)
    //    {
    //        int binIndex = 0, binCount = 0;
    //        List<IBin> newBins = new List<IBin>();
    //        double width = lastBin.Maximum - lastBin.Minimum, binMin = lastBin.Maximum, binMax = binMin + width;
    //        while (i < sortedData.Length)
    //        {
    //            if (sortedData[i] < binMax) binCount++;
    //            else
    //            {
    //                newBins.Add(new Bin(binMin, binMax, binCount));
    //                sampleSize += newBins[binIndex].Count; //this is infered in the BinDataBelowHistogramMin method.
    //                sum += newBins[binIndex].MidPoint * newBins[binIndex].Count;
    //                binIndex++;
    //                binCount = 0;
    //                binMin = binMax;
    //                binMax = binMin + width;
    //            }
    //            i++;
    //        }
    //        return newBins;
    //    }
    //    /// <summary>
    //    /// Adds pre-existing histogram bins above the sample data maximum value to the new histogram bins.
    //    /// </summary>
    //    /// <param name="oldBins"> Pre-existing histogram bins. </param>
    //    /// <param name="oldBinIndex"> Index of next pre-existing histogram bin to add to new histogram bins. </param>
    //    /// <param name="sampleSize"> Provides running count of histogram observations. </param>
    //    /// <param name="sum"> Provides running sum of histogram observations values. </param>
    //    /// <returns> List of pre-existing histrogram bins to add to new histrogram bins. </returns>
    //    private List<IBin> BinHistogramAboveSampleMax(IBin[] oldBins, int oldBinIndex, ref int sampleSize, ref double sum)
    //    {
    //        List<IBin> bins = new List<IBin>();
    //        while (oldBinIndex < oldBins.Length)
    //        {
    //            bins.Add(oldBins[oldBinIndex]);
    //            sampleSize += bins[oldBinIndex + 1].Count;
    //            sum += bins[oldBinIndex + 1].MidPoint * bins[oldBinIndex + 1].Count;
    //            oldBinIndex++;
    //        }
    //        return bins;
    //    }
    //    #endregion
    //    #region Median and Measures of Histogram Dispersion
    //    /// <summary>
    //    /// Provides median, variance and skewness of histogram (note: not the median, variance and skew of the sample data).
    //    /// </summary>
    //    /// <param name="bins"> The histogram bins. </param>
    //    /// <param name="mean"> The mean of the histogram. </param>
    //    /// <param name="N"> The number of items binned in the histogram. </param>
    //    /// <returns> A Tuple with 3 items:
    //    /// Item1: the median value of the histogram,
    //    /// Item2: the squared deviations of the histogram,
    //    /// Item3: the cubed deviations of the histogram.
    //    /// </returns>
    //    private Tuple<double, double, double> HistogramMedianAndDeviations(IBin[] bins, double mean, int N)
    //    {

    //        var medianData = MedianData(N);
    //        int n = 0, medianIndex = medianData.Item2;
    //        double deviations2 = 0, deviations3 = 0, median = double.NaN;
    //        for (int i = 0; i < bins.Length; i++)
    //        {
    //            n += bins[i].Count;
    //            deviations2 += Math.Pow(bins[i].MidPoint - mean, 2) * bins[i].Count;
    //            deviations3 += Math.Pow(bins[i].MidPoint - mean, 3) * bins[i].Count;
    //            if (!(n < medianIndex)) median = ComputeMedian(medianData, bins, i);
    //        }
    //        return new Tuple<double, double, double>(median, deviations2, deviations3);
    //    }
    //    /// <summary>
    //    /// Calculates the median index in an ordered set of N values.
    //    /// </summary>
    //    /// <param name="N"> The number of values in the ordered set. </param>
    //    /// <returns> A Tuple containing two items. 
    //    /// Item1: <see langword="true"/> if the median must be interpolated between 2 values, <see langword="false"/> otherwise. 
    //    /// Item2: the integer index of the median value, or lesser of the 2 indexes across which the median value must be interpolated. 
    //    /// </returns>
    //    private Tuple<bool, int> MedianData(int N)
    //    {
    //        double midPointIndex = N / 2;
    //        int medianIndex = (int)Math.Floor(midPointIndex);
    //        // returned Tuple
    //        //      Item 1: true if the median must be interpolated false othewise, 
    //        //      Item2: index of median or lesser of two indexes across which it must be interpolated.
    //        return new Tuple<bool, int>(midPointIndex - medianIndex > 0 ? true : false, medianIndex);
    //    }
    //    private double ComputeMedian(Tuple<bool, int> medianData, IBin[] bins, int binIndex)
    //    {
    //        bool interpolateMedian = medianData.Item1;
    //        if (!interpolateMedian) return bins[binIndex].MidPoint;
    //        else return (bins[binIndex].MidPoint + bins[binIndex + 1].MidPoint) / 2;
    //    }
    //    #endregion
    //    #endregion
    //    #region Validation Functions
    //    public bool Validate(IValidator<IHistogram> validator, out IEnumerable<IMessage> errors)
    //    {
    //        return validator.IsValid(this, out errors);
    //    }
    //    #endregion
    //    #region IDistribution Functions
    //    public double PDF(double x)
    //    {
    //        if (!x.IsOnRange(min: Bins[0].Minimum, max: Bins[Bins.Length-1].Maximum, inclusiveMin: true, inclusiveMax: false)) return 0;
    //        else return (double)FindBin(x, false) / SampleSize;
    //    }
    //    public double CDF(double x)
    //    {
    //        if (!x.IsOnRange(min: Bins[0].Minimum, max: Bins[Bins.Length - 1].Maximum, inclusiveMin: true, inclusiveMax: false)) return x < Bins[0].Minimum ? 0 : 1;
    //        else return (double)FindBin(x) / SampleSize;
    //    }
    //    public double InverseCDF(double p)
    //    {
    //        int n = 0;
    //        for (int i = 0; i < Bins.Length; i++)
    //        {
    //            n += Bins[i].Count; // running count of observations in previously looped bins
    //            double P = (double)n / SampleSize; //running count of portion of data in looped bins
    //            if (!(P < p)) return Bins[i].MidPoint; //TODO: Add interpolation option?
    //        }
    //        throw new ArgumentOutOfRangeException($"The provided value: {p} is not in the valid range of [0, 1].");
    //    }
    //    private int FindBin(double x, bool returnCummulativeObservations = true)
    //    {
    //        int n = 0;
    //        for (int i = 0; i < Bins.Length; i++)
    //        {
    //            n += Bins[i].Count;
    //            if (x.IsOnRange(min: Bins[i].Minimum, max: Bins[i].Maximum, inclusiveMin: true, inclusiveMax: false)) return returnCummulativeObservations ? n : Bins[i].Count;
    //        }
    //        throw new ArgumentOutOfRangeException($"The provided value: {x} is not in the range of the histogram bins.");
    //    }

    //    public double Sample() => InverseCDF(new Random().NextDouble());
    //    public double[] Sample(Random numberGenerator) => Sample(SampleSize, numberGenerator);
    //    public double[] Sample(int sampleSize, Random numberGenerator = null)
    //    {
    //        if (numberGenerator == null) numberGenerator = new Random();
    //        double[] sample = new double[sampleSize];
    //        for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
    //        return sample;
    //    }
    //    public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(numberGenerator), Bins.Length);
    //    public string Print() => $"Histogram(bins: {Bins.Length}, sample size: {SampleSize}, range: [{Minimum}, {Maximum}].";
    //    public bool Equals(IDistribution distribution)
    //    {
    //        if (!(distribution.Type == IDistributions.Histogram)) return false;
    //        if (!(distribution.SampleSize == SampleSize)) return false;
    //        return Equals((IHistogram)distribution);       
    //    }
    //    #endregion
    //    //#region IOrdinate Functions
    //    //public double GetValue(double sampleProbability = 0.50) => InverseCDF(sampleProbability);
    //    //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IHistogram) ? Equals((IDistribution)ordinate) : false;
    //    //#endregion
    //    #region IHistogram Functions
    //    public IHistogram AddSample(IEnumerable<double> sample) => new Histogram(Bins, sample);
    //    public bool Equals(IHistogram histogram)
    //    {
    //        if (!(Bins.Length == histogram.Bins.Length)) return false;
    //        for (int i = 0; i < Bins.Length; i++) if (!Bins[i].Equals(histogram.Bins[i])) return false;
    //        return true;
    //    }
    //    #endregion

    //    public static Histogram Fit(IEnumerable<double> sample, int nBins = 100)
    //    {
    //        return new Histogram(sample, nBins);
    //    }     
    //    #endregion
    //}
}
