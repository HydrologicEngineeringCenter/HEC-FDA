using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;
using Utilities.Ranges;
using Statistics.Validation;
using System.Xml.Linq;

namespace Statistics.Histograms
{
    internal class Histogram 
    {

        #region Properties

        public double[] BinCounts = new double[] { }; //using double for bin counts to satisfy IData 
        public double BinWidth { get; }
        
        #region IDistribution Properties
        public double Mean { get; }
        public double Median { get; }
        public double Variance { get; }
        public double Skewness { get; }
        public double StandardDeviation { get; }
        public IRange<double> Range { get; set; } //includes min and max 
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
        //we will need to initiate the histogram, then call add observations to fill the histogram with data
        public Histogram(double binWidth, double min, double max)
        {
            BinWidth = binWidth;

            Int64 numberOfBins = Convert.ToInt64(Math.Ceiling((max - min) / binWidth));
            BinCounts = new double[numberOfBins];
            IData data = new Data(BinCounts); //This does not work because the bin counts are not the data 

            var stats = ISampleStatisticsFactory.Factory(data);
            Mean = stats.Mean;
            Median = stats.Median;
            Variance = stats.Variance;
            Skewness = stats.Skewness;
            StandardDeviation = stats.StandardDeviation;
            SampleSize = stats.SampleSize;

            Range = IRangeFactory.Factory(min, max, true, true, true, false);

            // State = Validate(new Validation.HistogramValidator(), out IEnumerable<IMessage> msgs);
            //Messages = stats.Messages.Concat(msgs);
            //IsConverged = false;
        }
        #endregion



        #region Functions
        public static void AddObservationToHistogram(Histogram histogram, IData data)
        {
            Int64 quantityAdditionalBins = 0;

            if (data.Range.Min < histogram.Range.Min)
            {   
                quantityAdditionalBins = Convert.ToInt64(Math.Ceiling((histogram.Range.Min - data.Elements.First())/histogram.BinWidth));
                double[] newBinCounts = new double[quantityAdditionalBins + histogram.BinCounts.Length];

                for (Int64 i = histogram.BinCounts.Length + quantityAdditionalBins -1; i > (quantityAdditionalBins-1); i--)
                {
                    newBinCounts[i] = histogram.BinCounts[i - quantityAdditionalBins];
                }
                histogram.BinCounts = newBinCounts;
                histogram.BinCounts[0] += 1;
                histogram.Range.Min = Convert.ToDouble(Math.Floor(data.Elements.First()));
            } else if (data.Range.Max > histogram.Range.Max)
            {
                quantityAdditionalBins = Convert.ToInt64(Math.Ceiling((data.Elements.First() - histogram.Range.Max) / histogram.BinWidth));
                double[] newBinCounts = new double[quantityAdditionalBins + histogram.BinCounts.Length];
                for (Int64 i = 0; i < histogram.BinCounts.Length; i++)
                {
                    newBinCounts[i] = histogram.BinCounts[i];
                }
                newBinCounts[histogram.BinCounts.Length + quantityAdditionalBins - 1] += 1;
                histogram.BinCounts = newBinCounts;
                histogram.Range.Max = Convert.ToDouble(Math.Ceiling(data.Elements.First()));
            } else
            {
                Int64 newObsIndex = Convert.ToInt64((data.Elements.First() - histogram.Range.Min) / histogram.BinWidth);
                histogram.BinCounts[newObsIndex] += 1;
            }
        }

        public static void AddObservationsToHistogram(Histogram histogram, IData data)
        {
            foreach (double x in data.Elements)
            {
                double[] xToSingleRowArray = new double[1] { x };
                IData obs = new Data(xToSingleRowArray);
                AddObservationToHistogram(histogram, obs);
            }
       
        }

        /// <summary>
        /// Generates a list of quantile values for convergence testing (<seealso cref="IDistribution.InverseCDF(double)"/>). 
        /// </summary>
        /// <param name="criteria"> Convergence criteria containing the quantile values to be tested. </param>
        /// <returns> A <see cref="List{T}"/> containing the quantile values. </returns>
        private static List<double> QuantileValues(Histogram histogram, IEnumerable<IConvergenceCriteria> criteria)
        {
            List<double> qValues = new List<double>();
            foreach (var element in criteria) qValues.Add(histogram.InverseCDF(element.Quantile));
            return qValues;
        }

        public bool Equals(Histogram histogram)
        {
            throw new NotImplementedException("Need to figure this one out");
        }

        private double FindBinCount(double x, bool cummulative = true)
        {
            Int64 obsIndex = Convert.ToInt64((x - Range.Min) / BinWidth);
            if (cummulative)
            {
                double sum = 0;
                for (int i = 0; i==obsIndex; i++)
                {
                    sum += BinCounts[i];
                }
                return sum;
            }
            else
            {
                return BinCounts[obsIndex];
            }

        }

        internal double GetSampleSize()
        {
            double sum = 0;
            for (Int64 i = 0; i<BinCounts.Length; i++)
            {
                sum += BinCounts[i];
            }
            return sum;
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


        #region IValidate Function
        public IMessageLevels Validate(IValidator<Histogram> validator, out IEnumerable<IMessage> messages)
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
        public double PDF(double x) => (double)FindBinCount(x, false) / (double)SampleSize;
        public double CDF(double x) => (double)FindBinCount(x) / (double)SampleSize;
        public double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The provided probability value: {p} is not on the a valid range: [0, 1]");
            else
            {
                double n = 0;
                for (int i = 0; i<BinCounts.Length; i++)
                {
                    n += BinCounts[i];
                    double pAtN = n / SampleSize;
                    if (!(pAtN < p)) return i * BinWidth + 0.5 * BinWidth;
                }
                throw new Exception($"An unexpected error occured while attempting to find the histogram bin associated with the probability value {p}.");
            }
        }
        public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        public double[] Sample(int sampleSize, Random r = null)
        {
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = Sample(r);
            return sample;
        }
        //public IDistribution SampleDistribution(Random r = null) => IDistribution.Factory(IDataFactory.Factory(Sample(SampleSize, r)), Range.Min, Range.Max, Bins.Length);
        #endregion
















        public string Print(bool round) => round ? Print(SampleSize, Bins.Length, Range) : $"Histogram(observations: {SampleSize}, bins: {Bins.Length}, range: {Range.Print()})";
        public string Requirements(bool printNotes) => RequiremedParameterization(printNotes);
        public bool Equals(IDistribution distribution) => distribution.Type == IDistributionEnum.Histogram ? Equals((Histogram)distribution) : false;

        public static readonly string XML_BINS = "Bins";
        public static readonly string XML_BIN = "Bin";
        public static readonly string XML_MIN = "Min";
        public static readonly string XML_MAX = "Max";
        public static readonly string XML_MIDPOINT = "MidPoint";
        public static readonly string XML_COUNT = "Count";

        //Will fix this after re-assessing the histogram approach 
        //public XElement WriteToXML()
        //{
        //    XElement masterElem = new XElement(XML_BINS);
        //    foreach(Int64 binCount in BinCounts) 
        //    {
        //        XElement binElem = new XElement(XML_BIN);
        //        binElem.SetAttributeValue(XML_MIN, bin.Range.Min);
        //        binElem.SetAttributeValue(XML_MAX, bin.Range.Max);
        //        binElem.SetAttributeValue(XML_MIDPOINT, bin.MidPoint);
        //        binElem.SetAttributeValue(XML_COUNT, bin.Count);

        //        masterElem.Add(binElem);
        //    }
        //    return masterElem;
        //}
        #endregion
        #endregion
    }

    
}
