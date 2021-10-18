using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using Utilities.Ranges;
using Statistics.Validation;
using System.Xml.Linq;

namespace Statistics.Histograms
{//TODO: REMOVE Idistribution
    public class Histogram : IDistribution
    {

        #region Properties

        public double[] BinCounts = new double[] { }; //using double for bin counts to satisfy IData 
        public double BinWidth { get; }
        public double Min { get; set; }
        public double Max { get; set; }
        
        #region IDistribution Properties
        public double Mean { get {
                return GetMean();
            } 
        }
        public double Median { get {
                return GetMedian();
            } 
        }
        public double Variance { get {
                return GetVariance();
            } 
        }
        public double Skewness { get {
                return GetSkewness();
            } 
        }
        public double StandardDeviation { get {
                return Math.Pow(Variance, 0.5);
            } 
        }
        public IRange<double> Range { get; set; } //includes min and max 
        public int SampleSize { get {
                return GetSampleSize();
            }
        }
        public double Mode { get; }
        
        public IDistributionEnum Type => IDistributionEnum.Histogram;
        #endregion      
        #region IMessagePublisher Properties
        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion
        #endregion

        #region Constructor
        public Histogram(IData data, double binWidth)
        {
            BinWidth = binWidth;
            Min = Math.Floor(data.Range.Min); //this need not be a integer - it just needs to be the nearest bin start - a function of bin width.
            Int64 numberOfBins = Convert.ToInt64(Math.Ceiling((data.Range.Max - Min) / binWidth)); 
            Max = Min + (numberOfBins * binWidth);
            BinCounts = new double[numberOfBins];
            AddObservationsToHistogram(data);
            Range = GetRange(Min, Max);

        }
        #endregion

        internal int GetBinQuantity()
        {
            int binQuantity = 0;
            double value = (Min - Max) / BinWidth;
            if (value == Convert.ToInt32(value))
            {
                binQuantity = Convert.ToInt32(value) + 1;
            } else
            {
                binQuantity = Convert.ToInt32(Math.Ceiling(value));
            }
            return binQuantity;
        }

        internal int GetSampleSize()
        {
            double sum = 0;
            for (Int64 i = 0; i < BinCounts.Length; i++)
            {
                sum += BinCounts[i];
            }
            return Convert.ToInt32(sum);
        }

        internal IRange<double> GetRange(double min, double max)
        {
            var range = IRangeFactory.Factory(min, max, true, true, true, false);
            return range;
        }
        internal double GetMean()
        {           
            double sum = 0;
            double min = Min;
                for (int i = 0; i < BinCounts.Length; i++)
                {
                    sum += (min + (i * BinWidth) + (0.5 * BinWidth)) * BinCounts[i];
                }
            double mean = SampleSize > 0 ? sum / SampleSize : double.NaN;
            return mean;
        }


        internal double GetMedian()
        {
            double median = InverseCDF(0.5);
            return median;

        }

        internal double GetVariance()
        {
            double deviation = 0, deviation2 = 0;

            for (int i = 0; i < BinCounts.Length; i++)
            {
                double midpoint = Min + (i * BinWidth) + (0.5 * BinWidth);

                deviation = midpoint - Mean;
                deviation2 += deviation * deviation;

            }
            double variance = SampleSize > 1 ? deviation2 / (SampleSize - 1) : 0;
            return variance;
        }

        internal double GetSkewness()
        {
            double deviation = 0, deviation2 = 0, deviation3 = 0;

            for (int i = 0; i < BinCounts.Length; i++)
            {
                double midpoint = Min + (i * BinWidth) + (0.5 * BinWidth);

                deviation += midpoint - Mean;
                deviation2 += deviation * deviation;
                deviation3 += deviation2 * deviation;

            }
            double skewness = SampleSize > 2 ? deviation3 / SampleSize / Math.Pow(Variance, 3 / 2) : 0;
            return skewness;
        }



        #region Functions
        public void AddObservationToHistogram(IData data)
        {   
            Int64 quantityAdditionalBins = 0;

            if (data.Range.Min < Min)
            {   
                quantityAdditionalBins = Convert.ToInt64(Math.Ceiling((Min - data.Elements.First())/BinWidth));
                double[] newBinCounts = new double[quantityAdditionalBins + BinCounts.Length];

                for (Int64 i = BinCounts.Length + quantityAdditionalBins -1; i > (quantityAdditionalBins-1); i--)
                {
                    newBinCounts[i] = BinCounts[i - quantityAdditionalBins];
                }
                BinCounts = newBinCounts;
                BinCounts[0] += 1;
                double newMin = Min - (quantityAdditionalBins * BinWidth);
                double max = Max;
                Min = newMin;
                Range = IRangeFactory.Factory(newMin, max, true, true, true, false);
            } else if (data.Range.Max > Max)
            {
                quantityAdditionalBins = Convert.ToInt64(Math.Ceiling((data.Elements.First() - Max+BinWidth) / BinWidth));
                double[] newBinCounts = new double[quantityAdditionalBins + BinCounts.Length];
                for (Int64 i = 0; i < BinCounts.Length; i++)
                {
                    newBinCounts[i] = BinCounts[i];
                }
                newBinCounts[BinCounts.Length + quantityAdditionalBins-1] += 1;
                BinCounts = newBinCounts;
                double newMax = Min + (BinCounts.Length * BinWidth); //is this right?
                Max = newMax;
                double min = Min;
                Range = IRangeFactory.Factory(min, newMax, true, true, true, false);
            } else
            {
                Int64 newObsIndex = Convert.ToInt64(Math.Floor((data.Elements.First() - Min) / BinWidth));
                if (data.Elements.First() == Max)
                {
                    quantityAdditionalBins = 1;
                    double[] newBinCounts = new double[quantityAdditionalBins + BinCounts.Length];
                    for (Int64 i = 0; i < BinCounts.Length; i++)
                    {
                        newBinCounts[i] = BinCounts[i];
                    }
                    BinCounts = newBinCounts;
                    double newMax = Min + (BinCounts.Length * BinWidth);//double check
                    Max = newMax;
                }
                BinCounts[newObsIndex] += 1;
            }
        }

        public void AddObservationsToHistogram(IData data)
        {
            foreach (double x in data.Elements)
            {
                double[] xToSingleRowArray = new double[1] { x };
                IData obs = new Data(xToSingleRowArray);
                AddObservationToHistogram(obs);
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

        private double FindBinCount(double x, bool cumulative = true)
        {
            Int64 obsIndex = Convert.ToInt64(Math.Floor((x - Min) / BinWidth));
            if (cumulative)
            {
                double sum = 0;
                for (int i = 0; i<obsIndex+1; i++)
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
        #endregion
        #region IDistribution Functions 
        public double PDF(double x)
        {
            double nAtX = Convert.ToDouble(FindBinCount(x, false));
            double n = Convert.ToDouble(SampleSize);
            return nAtX/n;
        }
        public double CDF(double x)
        {
            double nAtX = Convert.ToDouble(FindBinCount(x));
            double n = Convert.ToDouble(SampleSize);
            return nAtX / n;
        }
        public double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The provided probability value: {p} is not on the a valid range: [0, 1]");
            else
            {
                if (p==0)
                {
                    return Min;
                }
                if (p==1)
                {
                    return Max;
                }
                Int64 numobs = Convert.ToInt64(SampleSize * p);
                if (p <= 0.5)
                {
                    Int64 index = 0;
                    double obs = BinCounts[index];
                    double cobs = obs;
                    while (cobs < numobs)
                    {
                        index++;
                        obs = BinCounts[index];
                        cobs += obs;

                    }
                    double fraction = (cobs - numobs) / obs;
                    double binOffSet = Convert.ToDouble(index + 1);
                    return Min + BinWidth * binOffSet - BinWidth * fraction;
                } else
                {
                    Int64 index = BinCounts.Length - 1;
                    double obs = BinCounts[index];
                    double cobs = SampleSize - obs;
                    while (cobs > numobs)
                    {
                        index--;
                        obs = BinCounts[index];
                        cobs -= obs;
                    }
                    double fraction = (numobs - cobs) / obs;
                    double binOffSet = Convert.ToDouble(BinCounts.Length - index);
                    return Max - BinWidth * binOffSet + BinWidth * fraction;
                }
                
            }
        }
        public double Sample(Random r = null) => InverseCDF(r == null ? new Random().NextDouble() : r.NextDouble());
        public double[] Sample(int sampleSize, Random r = null)
        {
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sampleSize; i++) sample[i] = Sample(r);
            return sample;
        }
        public string Requirements(bool printNotes)
        {
            return "Histogram requirements consist of a min, max, bin width, and some data.";
        }
        public Histogram Fit(IEnumerable<double> sample, int nBins)
        {
            double min = sample.Min();
            double max = sample.Max();
            double binWidth = (min - max) / nBins;

            IData data = new Data(sample);
            Histogram histogram = new Histogram(data, binWidth);
  
            return histogram;

        }
        #endregion















 
        public string Print(bool round) => round ? Print(SampleSize, BinCounts.Length, Range) : $"Histogram(observations: {SampleSize}, bins: {BinCounts.Length}, range: {Range.Print()})";
        public bool Equals(IDistribution distribution) => distribution.Type == IDistributionEnum.Histogram ? Equals((Histogram)distribution) : false;

        public static readonly string XML_BINS = "Bins";
        public static readonly string XML_BIN = "Bin";
        public static readonly string XML_MIN = "Inclusive Min";
        public static readonly string XML_MAX = "Exclusive Max";
        public static readonly string XML_MIDPOINT = "MidPoint";
        public static readonly string XML_COUNT = "Count";
        //TODO: write test on WriteToXML and ReadToXML
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement(XML_BINS);
            for (Int64 i=0; i<BinCounts.Length; i++)
            {
                XElement binElem = new XElement(XML_BIN);
                binElem.SetAttributeValue(XML_MIN, i*BinWidth);
                binElem.SetAttributeValue(XML_MAX, (i+1)*BinWidth);
                binElem.SetAttributeValue(XML_MIDPOINT, (i+0.5)*BinWidth);
                binElem.SetAttributeValue(XML_COUNT, BinCounts[i]);

                masterElem.Add(binElem);
            }
            return masterElem;
        }
        //TODO: implement ReadFromXML
        public static Histogram ReadFromXML(string histogramXMLString)
        {
            throw new NotImplementedException();
        }
        #endregion

    }

    
}
