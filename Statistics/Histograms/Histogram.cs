using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;
using Utilities.Ranges;
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
            return new HistogramBinnedData(FillHistogramBins(histogram, sample).ToArray());
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
        private static void ExpandHistogramRange(IHistogram histogram, IRange<double> dataRange)
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
            //bins.AddRange(histogram.Bins);
            double hiRange = dataRange.Max - histogram.Range.Max;
            if (hiRange > 0)
            {
                double width = histogram.Bins[0].Range.Max - histogram.Bins[0].Range.Min;
                int hiBinCount = Increments(hiRange, width, Math.Ceiling);
                bins.AddRange(InitializeEmptyBins(histogram.Range.Max, width, hiBinCount));
            }
            //we now need to add this list of empty bins to the existing histogram
            //BUT HOW!!
            //histogram.bins is an array and arrays are immutable 
            //create 
            histogram.Bins.Append<IBin>((IBin)bins);
        }
        private static IBin[] FillHistogramBins(IHistogram histogram, IData data)
        {
            IHistogram histogramForAddingObservations = histogram;
            foreach (double x in data.Elements)
            {
                double[] xToSingleRowArray = new double[1] { x };
                IData obs = new Data(xToSingleRowArray);
                //there is a problem of scope. on each iteration, we need the previously augmented histograms
                AddObservationToHistogram(histogramForAddingObservations, obs);
            }
            return histogramForAddingObservations.Bins;
        }

        private static void AddObservationToHistogram(IHistogram histogram, IData data)
        {

            if (data.Range.Min < histogram.Range.Min)
            {
                IRange<double> range = IRangeFactory.Factory(data.Range.Min, histogram.Range.Max);
                ExpandHistogramRange(histogram, range);
            }
            if (data.Range.Max > histogram.Range.Max)
            {
                IRange<double> range = IRangeFactory.Factory(histogram.Range.Min, data.Range.Max);
                ExpandHistogramRange(histogram, range);
            }
            double binWidth = histogram.Bins[0].Range.Max - histogram.Bins[0].Range.Min;
            double index = (data.Elements.FirstOrDefault() - histogram.Range.Min)/binWidth;
            int indexRounded = (int)Convert.ToInt64(Math.Floor(index));
            if (indexRounded < 0)
            {
                throw new ArgumentException("The computed index is less than zero");
            } else if (indexRounded > histogram.Bins.Length) {
                throw new ArgumentException("The computed index is greater than the bin quantity");
            } else
            {
                histogram.Bins[indexRounded] = new Bin(histogram.Bins[indexRounded], 1);
            }
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
        //does rounding use conventional criteria, round up, or round down?
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

    
}
