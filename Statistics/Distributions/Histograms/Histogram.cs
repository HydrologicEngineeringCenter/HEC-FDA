using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utilities;
using Utilities.Serialization;

namespace Statistics.Histograms
{
    internal class Histogram : IHistogram, IDistribution, IValidate<IHistogram>
    {
        internal Histogram(double min, double max, double widths)
        {
            if (!IsConstructable(min, max, widths, out IList<string> messages)) throw new InvalidConstructorArgumentsException(messages);
            else
            {
                Bins = InitializeEmptyBins(min, max, widths, out SummaryStatistics stats);
                Minimum = min;
                Maximum = max;
                Mean = stats.Mean;
                Variance = stats.Variance;
                Skewness = stats.Skewness;
                SampleSize = stats.SampleSize;
                StandardDeviation = stats.StandardDeviation;
                IsValid = Validate(new Validation.HistogramValidator(), out IEnumerable<IMessage> errors);
                Errors = errors;
            }
        }
        private bool IsConstructable(double min, double max, double binwidths, out IList<string> messages)
        {
            messages = new List<string>();
            if (!binwidths.IsFinite() || !(binwidths > 0)) messages.Add($"The requested {typeof(Histogram)} cannot be constructed because the requested bin width: {binwidths} is not a positive finite value.");
            if (!min.IsFinite() || !max.IsFinite() || Utilities.Validate.IsRange(min, max)) messages.Add($"The requested {typeof(Histogram)} cannot be constructed because the range: [{min}, {max}) that was provided is invalid.");
            if (max - min < binwidths) messages.Add($"The requested {typeof(Histogram)} cannot be constructed because the request bin width: {binwidths} is less than the requested {typeof(Histogram)} range: [{min}, {max}).");
            return messages.Any();
        }
        private IBin[] InitializeEmptyBins(double min, double max, double widths, out SummaryStatistics stats)
        {
            List<IBin> bins = new List<IBin>();
            while (min < max)
            {
                bins.Add(new Bin(min, min + widths, 0));
                min += widths;
            }
            stats = new SummaryStatistics();
            return bins.ToArray();
        } 

        
        
        
        
        
        
        //TODO: Add convergence
        //TODO: Keep bins in accending order
        //TODO: Fit to Beta4Parameter distribution
        //TODO: Construct with bin widths (instead of nBins)

        #region Properties
        #region IValidate Properties
        public bool IsValid { get; }
        public IEnumerable<IMessage> Errors { get; }
        #endregion
        #region IDistribution Properties
        public IDistributions Type => IDistributions.Histogram;
        public double Mean { get; }
        public double Median { get; }
        public double StandardDeviation { get; }
        public double Variance { get; }
        public double Skewness { get; }
        public double Minimum { get; }
        public double Maximum { get; }
        public int SampleSize { get; }
        #endregion
        public IBin[] Bins { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Histogram constructor for initial sample.
        /// </summary>
        /// <param name="sample"> Data to place in histogram. </param>
        /// <param name="nBins"> The number of desired bins. </param>
        public Histogram(IEnumerable<double> sample, int nBins = 100)
        {
            if (!IsConstructable(sample, nBins, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
            else
            {

            }
            if (IsConstructable(sample, nBins, out _))
            {
                //1. Bin data also collect sample size and histogram mean.
                Bins = BinData(sample, nBins, out int n, out double mu);
                SampleSize = n; Mean = mu;
                //2. Collect histogram measures of dispersion on second pass.
                Minimum = Bins[0].Minimum; Maximum = Bins[Bins.Length - 1].Maximum;
                var dispersionTuple = HistogramMedianAndDeviations(Bins, Mean, SampleSize);
                Median = dispersionTuple.Item1;  Variance = dispersionTuple.Item2 / (SampleSize - 1);
                Skewness = dispersionTuple.Item3 / (SampleSize - 1); StandardDeviation = Math.Sqrt(Variance);
            }
            else throw new ArgumentException();
        }
        private bool IsConstructable(IEnumerable<double> sample, int nBins, out IList<string> errors)
        {
            errors = new List<string>();
            if (nBins < 1) errors.Add($"The requested number of bins: {nBins} is invalid, at least one bin must be requested.");
            if (sample.IsNullOrEmpty()) errors.Add("The provided sample data is invalid because it is null or empty.");
            else
            {
                SummaryStatistics stats = new SummaryStatistics(sample);
                if (stats.Variance == 0) errors.Add("The provided sample data is invalid in the current constructor because it lacks variance and therefore cannot be used to generate histogram bin widths.");
            }
            return errors.Any();
        }

        public Histogram(IEnumerable<double> sample, double width)
        {
            throw new NotImplementedException();    
        }
        private bool IsConstructable(IEnumerable<double> sample, double width, out IList<string> errors)
        {
            errors = new List<string>();
            if (!width.IsFinite() || width < 0) errors.Add($"The requested bin width: {width} is invalid because it is not a positive finite value.");
            return errors.Any();
        }

        public Histogram(double width, int nBins)
        {
            throw new NotImplementedException();
        }
        private bool IsConstructable(double width, int nBins, out IList<string> errors)
        {
            errors = new List<string>();
            if (!width.IsFinite() || width < 0) errors.Add($"The requested bin width: {width} is invalid because it is not a positive finite value.");
            if (nBins < 1) errors.Add($"The requested number of bins: {nBins} is invalid, at least one bin must be requested.");
            return errors.Any();
        }

        /// <summary>
        /// Histogram constructor for adding sample to existing histogram.
        /// </summary>
        /// <param name="oldBins"> The pre-existing histogram bins. </param>
        /// <param name="sample"> The sample data to be added to the histogram. </param>
        private Histogram(IBin[] oldBins, IEnumerable<double> sample)
        {
            Bins = BinData(oldBins, sample, out int sampleSize, out double mean);
            SampleSize = sampleSize;
            Mean = mean;

            var medianAndDeviations = HistogramMedianAndDeviations(Bins, Mean, SampleSize);
            // Item1: Median, Item2: Squared Deviations, Item3: Cubed Deviations
            Median = medianAndDeviations.Item1;
            Variance = medianAndDeviations.Item2 / (SampleSize - 1);
            StandardDeviation = Math.Sqrt(Variance);
            Skewness = medianAndDeviations.Item3 / (SampleSize - 1);
            Minimum = Bins[0].Minimum;
            Maximum = Bins[Bins.Length - 1].Maximum;
        }
        #endregion

        #region Functions
        #region Initialization Functions
        #region Data Binning, Histogram Mean and Sample Size
        /// <summary>
        /// Bins a sample of data with no pre-existing bins, counts data and provides mean of the histogram.
        /// </summary>
        /// <param name="sample"> The sample of data to be bined. </param>
        /// <param name="nBins"> The number of bins to to create. </param>
        /// <param name="sampleSize"> Provides the number of observations in the histogram. </param>
        /// <param name="mean"> Provides the mean of the histogram (note: not the mean of the sample). </param>
        /// <returns> The binned sample data. </returns>
        private IBin[] BinData(IEnumerable<double> sample, int nBins, out int sampleSize, out double mean)
        {
            sampleSize = 0;
            Bin[] bins = new Bin[nBins];
            int binIndex = 0, binCount = 0;
            

            IEnumerable<double> data = sample.OrderBy(i => i);
            SummaryStatistics stats = new SummaryStatistics(data);
            // Bin width, Bin min, Bin max, sum of sample data values
            double width = (stats.Maximum - stats.Minimum) / (nBins - 1), min = data.Min() - width / 2, max = min + width, sum = 0;
            foreach (var x in data)
            {
                // x is greater than bin max, so create new bin(s) until x can be binned.
                while (!(x < max))
                {
                    bins[binIndex] = new Bin(min, max, binCount);
                    sum += bins[binIndex].MidPoint * bins[binIndex].Count;
                    // Reset bin count, bin min, bin max, increment bin index
                    binIndex++;
                    binCount = 0;
                    min = max;
                    max = min + width;
                }
                // x is in existing bin increase count of current bin
                binCount++;
                sampleSize++;
            }
            mean = sum / sampleSize;
            return bins;
        }
        /// <summary>
        /// Bins a sample of data by adding it to pre-existing histogram bins, counts data and provides mean of the new histogram.
        /// </summary>
        /// <param name="oldBins"> The pre-exising histogram bins. </param>
        /// <param name="sample"> The sample of data to be added to the histogram. </param>
        /// <param name="sampleSize"> Provides the number of observations in the histogram. </param>
        /// <param name="mean"> Provides the mean of the histogram. </param>
        /// <returns> The binned data containing the sample, pre-existing bins and any new bins created to accomidate the sample. </returns>
        private IBin[] BinData(IBin[] oldBins, IEnumerable<double> sample, out int sampleSize, out double mean)
        {
            double[] data = sample.OrderBy(x => x).ToArray();
            // n = obs to add to bin, N = total hist obs, i = sample idx, sum = sum of hist obs
            List<IBin> bins = BinDataBelowHistogramMin(data, oldBins[0], out int i, out double sum);
            int binCount = 0, N = i + 1, binIndex = bins.Count - 1, oldBinIndex = 0;
            // add data observations to pre-existing histogram bins
            while (i < data.Length && oldBinIndex < oldBins.Length)
            {

                if (data[i] < oldBins[oldBinIndex].Minimum) throw new Exception($"An unexpected error has occured. The sample data value {data[i]} is not on the expected bin range [{oldBins[oldBinIndex].Minimum}, {oldBins[oldBinIndex].Maximum}).");
                if (data[i] < oldBins[oldBinIndex].Maximum) binCount++; // current observation is 'in bin'.
                else // current observation is 'off bin' create new bin for last n 'in bin' observations.
                {
                    bins.Add(new Bin(oldBins[oldBinIndex], binCount));
                    binCount = 0;
                    binIndex++;
                    oldBinIndex++;
                    N += bins[binIndex].Count;
                    sum += bins[binIndex].MidPoint * bins[binIndex].Count;
                }
                i++;
            }
            if (oldBinIndex != oldBins.Length) bins.AddRange(BinHistogramAboveSampleMax(oldBins, oldBinIndex, ref N, ref sum));
            if (i != data.Length) bins.AddRange(BinDataAboveHistogramMax(data, bins[binIndex], i, ref N, ref sum));
            sampleSize = N;
            mean = sum / sampleSize;
            return bins.ToArray();
        }
        /// <summary>
        /// Bins sample data with values below pre-existing histogam minimum value.
        /// </summary>
        /// <param name="sortedData"> Sample data to bin sorted in accending order. </param>
        /// <param name="firstBin"> Bin from pre-existing histogram containing its minimum value. </param>
        /// <param name="i"> Returns the index of the next sample data value to bin. </param>
        /// <param name="sum"> Returns the sum of the bins below the pre-existing histogram minimum value. </param>
        /// <returns> Bins of sample data with valuse below the pre-existing histogram mimimum value. </returns>
        private List<IBin> BinDataBelowHistogramMin(double[] sortedData, IBin firstBin, out int i, out double sum)
        {
            i = 0; // position in data
            sum = 0; // sum of new bins observations
            int binIndex = 0, binCount = 0;
            List<IBin> newBins = new List<IBin>();
            double width = firstBin.Maximum - firstBin.Minimum, binMin = sortedData.Min() - width / 2, binMax = binMin + width;
            // WHILE data value is less than existing historgram minimum
            while (sortedData[i] < firstBin.Minimum)
            {
                if (sortedData[i] < binMax) binCount++;
                else
                {
                    newBins.Add(new Bin(binMin, binMax, binCount));
                    sum += newBins[binIndex].MidPoint * newBins[binIndex].Count;
                    binIndex++;
                    binCount = 0;
                    binMin = binMax;
                    binMax = binMin + width;
                }
                i++;
            }
            return newBins;
        }
        /// <summary>
        /// Bins sample data wiht values above pre-existing histrogram maximum value.
        /// </summary>
        /// <param name="sortedData"> Sample data to bin sorted in accending order. </param>
        /// <param name="lastBin"> Bin from pre-existing histogram containing its maximum value. </param>
        /// <param name="i"> Returns the index </param>
        /// <param name="sum"></param>
        /// <returns></returns>
        private List<IBin> BinDataAboveHistogramMax(double[] sortedData, IBin lastBin, int i, ref int sampleSize, ref double sum)
        {
            int binIndex = 0, binCount = 0;
            List<IBin> newBins = new List<IBin>();
            double width = lastBin.Maximum - lastBin.Minimum, binMin = lastBin.Maximum, binMax = binMin + width;
            while (i < sortedData.Length)
            {
                if (sortedData[i] < binMax) binCount++;
                else
                {
                    newBins.Add(new Bin(binMin, binMax, binCount));
                    sampleSize += newBins[binIndex].Count; //this is infered in the BinDataBelowHistogramMin method.
                    sum += newBins[binIndex].MidPoint * newBins[binIndex].Count;
                    binIndex++;
                    binCount = 0;
                    binMin = binMax;
                    binMax = binMin + width;
                }
                i++;
            }
            return newBins;
        }
        /// <summary>
        /// Adds pre-existing histogram bins above the sample data maximum value to the new histogram bins.
        /// </summary>
        /// <param name="oldBins"> Pre-existing histogram bins. </param>
        /// <param name="oldBinIndex"> Index of next pre-existing histogram bin to add to new histogram bins. </param>
        /// <param name="sampleSize"> Provides running count of histogram observations. </param>
        /// <param name="sum"> Provides running sum of histogram observations values. </param>
        /// <returns> List of pre-existing histrogram bins to add to new histrogram bins. </returns>
        private List<IBin> BinHistogramAboveSampleMax(IBin[] oldBins, int oldBinIndex, ref int sampleSize, ref double sum)
        {
            List<IBin> bins = new List<IBin>();
            while (oldBinIndex < oldBins.Length)
            {
                bins.Add(oldBins[oldBinIndex]);
                sampleSize += bins[oldBinIndex + 1].Count;
                sum += bins[oldBinIndex + 1].MidPoint * bins[oldBinIndex + 1].Count;
                oldBinIndex++;
            }
            return bins;
        }
        #endregion
        #region Median and Measures of Histogram Dispersion
        /// <summary>
        /// Provides median, variance and skewness of histogram (note: not the median, variance and skew of the sample data).
        /// </summary>
        /// <param name="bins"> The histogram bins. </param>
        /// <param name="mean"> The mean of the histogram. </param>
        /// <param name="N"> The number of items binned in the histogram. </param>
        /// <returns> A Tuple with 3 items:
        /// Item1: the median value of the histogram,
        /// Item2: the squared deviations of the histogram,
        /// Item3: the cubed deviations of the histogram.
        /// </returns>
        private Tuple<double, double, double> HistogramMedianAndDeviations(IBin[] bins, double mean, int N)
        {

            var medianData = MedianData(N);
            int n = 0, medianIndex = medianData.Item2;
            double deviations2 = 0, deviations3 = 0, median = double.NaN;
            for (int i = 0; i < bins.Length; i++)
            {
                n += bins[i].Count;
                deviations2 += Math.Pow(bins[i].MidPoint - mean, 2) * bins[i].Count;
                deviations3 += Math.Pow(bins[i].MidPoint - mean, 3) * bins[i].Count;
                if (!(n < medianIndex)) median = ComputeMedian(medianData, bins, i);
            }
            return new Tuple<double, double, double>(median, deviations2, deviations3);
        }
        /// <summary>
        /// Calculates the median index in an ordered set of N values.
        /// </summary>
        /// <param name="N"> The number of values in the ordered set. </param>
        /// <returns> A Tuple containing two items. 
        /// Item1: <see langword="true"/> if the median must be interpolated between 2 values, <see langword="false"/> otherwise. 
        /// Item2: the integer index of the median value, or lesser of the 2 indexes across which the median value must be interpolated. 
        /// </returns>
        private Tuple<bool, int> MedianData(int N)
        {
            double midPointIndex = N / 2;
            int medianIndex = (int)Math.Floor(midPointIndex);
            // returned Tuple
            //      Item 1: true if the median must be interpolated false othewise, 
            //      Item2: index of median or lesser of two indexes across which it must be interpolated.
            return new Tuple<bool, int>(midPointIndex - medianIndex > 0 ? true : false, medianIndex);
        }
        private double ComputeMedian(Tuple<bool, int> medianData, IBin[] bins, int binIndex)
        {
            bool interpolateMedian = medianData.Item1;
            if (!interpolateMedian) return bins[binIndex].MidPoint;
            else return (bins[binIndex].MidPoint + bins[binIndex + 1].MidPoint) / 2;
        }
        #endregion
        #endregion
        #region Validation Functions
        public bool Validate(IValidator<IHistogram> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion
        #region IDistribution Functions
        public double PDF(double x)
        {
            if (!x.IsOnRange(min: Bins[0].Minimum, max: Bins[Bins.Length-1].Maximum, inclusiveMin: true, inclusiveMax: false)) return 0;
            else return (double)FindBin(x, false) / SampleSize;
        }
        public double CDF(double x)
        {
            if (!x.IsOnRange(min: Bins[0].Minimum, max: Bins[Bins.Length - 1].Maximum, inclusiveMin: true, inclusiveMax: false)) return x < Bins[0].Minimum ? 0 : 1;
            else return (double)FindBin(x) / SampleSize;
        }
        public double InverseCDF(double p)
        {
            int n = 0;
            for (int i = 0; i < Bins.Length; i++)
            {
                n += Bins[i].Count; // running count of observations in previously looped bins
                double P = (double)n / SampleSize; //running count of portion of data in looped bins
                if (!(P < p)) return Bins[i].MidPoint; //TODO: Add interpolation option?
            }
            throw new ArgumentOutOfRangeException($"The provided value: {p} is not in the valid range of [0, 1].");
        }
        private int FindBin(double x, bool returnCummulativeObservations = true)
        {
            int n = 0;
            for (int i = 0; i < Bins.Length; i++)
            {
                n += Bins[i].Count;
                if (x.IsOnRange(min: Bins[i].Minimum, max: Bins[i].Maximum, inclusiveMin: true, inclusiveMax: false)) return returnCummulativeObservations ? n : Bins[i].Count;
            }
            throw new ArgumentOutOfRangeException($"The provided value: {x} is not in the range of the histogram bins.");
        }

        public double Sample() => InverseCDF(new Random().NextDouble());
        public double[] Sample(Random numberGenerator) => Sample(SampleSize, numberGenerator);
        public double[] Sample(int sampleSize, Random numberGenerator = null)
        {
            if (numberGenerator == null) numberGenerator = new Random();
            double[] sample = new double[sampleSize];
            for (int i = 0; i < sample.Length; i++) sample[i] = InverseCDF(numberGenerator.NextDouble());
            return sample;
        }
        public IDistribution SampleDistribution(Random numberGenerator = null) => Fit(Sample(numberGenerator), Bins.Length);
        public string Print() => $"Histogram(bins: {Bins.Length}, sample size: {SampleSize}, range: [{Minimum}, {Maximum}].";
        public bool Equals(IDistribution distribution)
        {
            if (!(distribution.Type == IDistributions.Histogram)) return false;
            if (!(distribution.SampleSize == SampleSize)) return false;
            return Equals((IHistogram)distribution);       
        }
        #endregion
        //#region IOrdinate Functions
        //public double GetValue(double sampleProbability = 0.50) => InverseCDF(sampleProbability);
        //public bool Equals<T>(IOrdinate<T> ordinate) => ordinate.OrdinateType == typeof(IHistogram) ? Equals((IDistribution)ordinate) : false;
        //#endregion
        #region IHistogram Functions
        public IHistogram AddSample(IEnumerable<double> sample) => new Histogram(Bins, sample);
        public bool Equals(IHistogram histogram)
        {
            if (!(Bins.Length == histogram.Bins.Length)) return false;
            for (int i = 0; i < Bins.Length; i++) if (!Bins[i].Equals(histogram.Bins[i])) return false;
            return true;
        }
        #endregion

        public static Histogram Fit(IEnumerable<double> sample, int nBins = 100)
        {
            return new Histogram(sample, nBins);
        }

       

        public string WriteToXML()
        {
            throw new NotImplementedException();
        }

        XElement ISerializeToXML<IDistribution>.WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
