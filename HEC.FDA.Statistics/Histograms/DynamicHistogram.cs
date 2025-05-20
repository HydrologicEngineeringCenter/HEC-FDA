using System;
using System.Linq;
using Utilities;
using System.Xml.Linq;
using System.Collections.Generic;
using Statistics.Distributions;

namespace Statistics.Histograms
{
    public class DynamicHistogram : IHistogram
    {
        #region Fields
        private double _SampleVariance;
        private bool _minHasNotBeenSet = false;
        private bool _HistogramShutDown = false;
        #endregion
        #region Properties
        public bool HistogramIsZeroValued
        {
            get
            {
                return IsZeroValued();
            }
        }
        public bool HistogramIsSingleValued
        {
            get
            {
                return IsSingleValued();
            }
        }
        internal double SampleMax { get; private set; }
        internal double SampleMin { get; private set; }
        internal bool ConvergedOnMax { get; private set; } = false;

        public bool IsConverged { get; private set; } = false;
        public Int64 ConvergedIteration { get; private set; } = int.MinValue;
        public double BinWidth { get; private set; }
        public ConvergenceCriteria ConvergenceCriteria { get; }
        public Int64[] BinCounts { get; private set; } = Array.Empty<long>();
        public double Min { get; private set; }
        public double Max { get; set; }
        public double Mean { get; private set; } = 10;
        public double Variance
        {
            get
            {
                return _SampleVariance * (double)((double)(SampleSize - 1) / (double)SampleSize);
            }
        }
        public double StandardDeviation
        {
            get
            {
                return Math.Pow(Variance, 0.5);
            }
        }
        public Int64 SampleSize { get; private set; }

        public IDistributionEnum Type
        {
            get
            {
                return IDistributionEnum.IHistogram;
            }
        }
        //TODO: We can do more on this if we actually implement truncation. Until then, it is accurate to return false. 
        public bool Truncated
        {
            get
            {
                return false;
            }
        }
        #endregion
        #region Constructor
        /// <summary>
        /// This histogram is an ARBITRARY histogram and should not be used to collect data
        /// </summary>
        public DynamicHistogram()
        {
            BinWidth = 1;
            _minHasNotBeenSet = true;
            ConvergenceCriteria = new ConvergenceCriteria();
            for (int i = 0; i < 10; i++)
            {
                AddObservationToHistogram(0);
            }
        }
        public DynamicHistogram(double min, double binWidth, ConvergenceCriteria convergenceCriteria)
        {
            BinWidth = binWidth;
            Min = min;
            Max = Min + BinWidth;
            int numberOfBins = 1;
            BinCounts = new Int64[numberOfBins];
            ConvergenceCriteria = convergenceCriteria;
        }
        public DynamicHistogram(double binWidth, ConvergenceCriteria convergenceCriteria)
        {
            BinWidth = binWidth;
            _minHasNotBeenSet = true;
            ConvergenceCriteria = convergenceCriteria;
        }
        public DynamicHistogram(List<double> dataList, ConvergenceCriteria convergenceCriteria)
        {
            double[] data = dataList.ToArray();
            ConvergenceCriteria = convergenceCriteria;
            Min = data.Min();
            Max = data.Max();
            int quantityOfBins = (int)Math.Ceiling(1 + 3.322 * Math.Log10(data.Length));
            double range = Max - Min;
            if (range == 0)
            {
                BinWidth = 1;

            }
            else
            {
                BinWidth = range / quantityOfBins;
            }
            BinCounts = new long[quantityOfBins];
            AddObservationsToHistogram(data);
        }
        private DynamicHistogram(double min, double max, double binWidth, Int64 sampleSize, Int64[] binCounts, ConvergenceCriteria convergenceCriteria)
        {
            Min = min;
            Max = max;
            BinWidth = binWidth;
            BinCounts = binCounts;
            ConvergenceCriteria = convergenceCriteria;
            SampleSize = sampleSize;
        }
        #endregion
        #region Functions
        public double Skewness()
        {
            double deviation = 0, deviation2 = 0, deviation3 = 0;
            if (SampleSize == 0)
            {
                return double.NaN;
            }
            if (Min == (Max - BinWidth))
            {
                return 0.0;
            }
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


        public void ForceDeQueue()
        {
            //do nothing
            //HACK
        }

        public double HistogramVariance()
        {
            if (SampleSize == 0)
            {
                return double.NaN;
            }
            if (SampleSize == 1)
            {
                return 0.0;
            }
            if (Min == (Max - BinWidth))
            {
                return 0.0;
            }

            double deviation2 = 0;
            for (int i = 0; i < BinCounts.Length; i++)
            {
                double midpoint = Min + (i * BinWidth) + (0.5 * BinWidth);

                double deviation = midpoint - Mean;
                deviation2 += deviation * deviation;

            }
            double variance = deviation2 / (SampleSize - 1);
            return variance;
        }
        public double HistogramStandardDeviation()
        {
            return Math.Sqrt(HistogramVariance());
        }
        /// <summary>
        /// The only argument that should be used in this function is the observation value
        /// A hacky solution was used here so that Histogram and ThreadsafeInlineHIstogram match 
        /// and the interface would work 
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="index"></param>
        public void AddObservationToHistogram(double observation, Int64 index = 0) //TODO index is a hack
        {
            if (_HistogramShutDown)
            {
                //do nothing because the histogram is shut down 
            }
            else
            {

                if (SampleSize == 0)
                {
                    SampleMax = observation;
                    SampleMin = observation;
                    Mean = observation;
                    _SampleVariance = 0;
                    if (_minHasNotBeenSet)
                    {
                        Min = observation;
                        Max = observation + BinWidth;
                        BinCounts = new Int64[] { 0 };
                    }
                    SampleSize = 1;
                }
                else
                {
                    if (observation > SampleMax) SampleMax = observation;
                    if (observation < SampleMin) SampleMin = observation;
                    SampleSize += 1;
                    double tmpMean = Mean + ((observation - Mean) / (double)SampleSize);
                    _SampleVariance = ((((double)(SampleSize - 2) / (double)(SampleSize - 1)) * _SampleVariance) + (Math.Pow(observation - Mean, 2)) / (double)SampleSize);
                    Mean = tmpMean;
                }
                int quantityAdditionalBins;
                if (observation < Min)
                {
                    quantityAdditionalBins = Convert.ToInt32(Math.Ceiling((Min - observation) / BinWidth));
                    if (observation == 0)
                    {
                        ResetToZeroMin(quantityAdditionalBins);
                    }
                    else
                    {
                        double newMin = Min - (quantityAdditionalBins * BinWidth);
                        if (observation > 0 && newMin < 0)
                        {
                            ResetToZeroMin(quantityAdditionalBins);
                        }
                        else
                        {
                            Int64[] newBinCounts = new Int64[quantityAdditionalBins + BinCounts.Length];

                            for (int i = BinCounts.Length + quantityAdditionalBins - 1; i > (quantityAdditionalBins - 1); i--)
                            {
                                newBinCounts[i] = BinCounts[i - quantityAdditionalBins];
                            }
                            BinCounts = newBinCounts;
                            BinCounts[0] += 1;
                            Min = newMin;
                        }
                    }
                }
                else if (observation > Max)
                {
                    quantityAdditionalBins = Convert.ToInt32(Math.Ceiling((observation - Max + BinWidth) / BinWidth));
                    Int64[] newBinCounts = new Int64[quantityAdditionalBins + BinCounts.Length];
                    for (int i = 0; i < BinCounts.Length; i++)
                    {
                        newBinCounts[i] = BinCounts[i];
                    }
                    newBinCounts[BinCounts.Length + quantityAdditionalBins - 1] += 1;
                    BinCounts = newBinCounts;
                    double newMax = Min + (BinCounts.Length * BinWidth); //is this right?
                    Max = newMax;
                }
                else
                {
                    Int64 newObsIndex = 0;
                    if (observation != Min)
                    {
                        newObsIndex = Convert.ToInt64(Math.Floor((observation - Min) / BinWidth));
                    }
                    if (observation == Max)
                    {
                        quantityAdditionalBins = 1;
                        Int64[] newBinCounts = new Int64[quantityAdditionalBins + BinCounts.Length];
                        for (int i = 0; i < BinCounts.Length; i++)
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
        }
        public void AddObservationsToHistogram(double[] data)
        {
            bool sampleSizeIsBigEnough = SampleSize > 1000;
            if (sampleSizeIsBigEnough && HistogramIsZeroValued)
            {
                ShutHistogramDown();
            }
            else
            {
                foreach (double x in data)
                {
                    AddObservationToHistogram(x);
                }
                if (BinCounts.Length > 2000)
                {
                    double divisor = BinCounts.Length / 500;
                    ResizeHistogram(divisor);
                }
            }
        }

        private void ResizeHistogram(double divisor)
        {
            BinWidth = BinWidth * divisor;
            int newBinCount = Convert.ToInt32(Math.Ceiling(BinCounts.Length / divisor));
            long[] newBins = new long[newBinCount];
            for (int i = 0; i < BinCounts.Length; i++)
            {
                int newBin = Convert.ToInt32(Math.Floor(i / divisor));
                newBins[newBin] += BinCounts[i];
            }
            Max = Min + newBinCount * BinWidth;
            BinCounts = newBins;
        }

        private void ResetToZeroMin(int quantityAdditionalBins)
        {
            double newMin = 0;
            int originalBinsLength = BinCounts.Length;
            long[] newBins = new long[quantityAdditionalBins + originalBinsLength];
            newBins[0] += 1;

            double lowerFraction = (newMin + quantityAdditionalBins * BinWidth - Min) / BinWidth;
            double upperFraction = 1 - lowerFraction;

            for (int i = 0; i < originalBinsLength; i++)
            {
                newBins[i + quantityAdditionalBins - 1] += Convert.ToInt64(lowerFraction * BinCounts[i]);
                newBins[i + quantityAdditionalBins] += Convert.ToInt64(upperFraction * BinCounts[i]);
            }

            Min = newMin;
            BinCounts = newBins;
            Max = Min + BinCounts.Length * BinWidth;
        }

        private void ShutHistogramDown()
        {
            _HistogramShutDown = true;
            BinCounts = new long[] { 1 };
            Min = 0;
            BinWidth = 1;
            SampleMax = 0;
            Mean = 0;
            _SampleVariance = 0;
            SampleMin = 0;
            SampleSize = 1;
            IsConverged = true;
            ConvergedOnMax = false;
        }

        public Int64 FindBinCount(double x, bool cumulative = true)
        {
            if (x > Max)
            {
                if (cumulative)
                {
                    return SampleSize;
                }
                else
                {
                    return 0;
                }
            }
            if (x < Min)
            {
                return 0;
            }
            int obsIndex = Convert.ToInt32(Math.Floor((x - Min) / BinWidth));
            if (obsIndex == BinCounts.Length)
            {
                obsIndex -= 1;
            }
            if (cumulative)
            {
                Int64 sum = 0;
                for (int i = 0; i < obsIndex + 1; i++)
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
        public double PDF(double x)
        {
            if (SampleSize == 0)
            {
                return double.NaN;
            }
            if (Min == (Max - BinWidth))
            {
                if (x > Min)
                {
                    if (x <= Max)
                    {
                        return 1.0;
                    }
                }
                return 0.0;
            }
            double nAtX = Convert.ToDouble(FindBinCount(x, false));
            double n = Convert.ToDouble(SampleSize);
            return nAtX / n;
        }
        public double CDF(double x)
        {
            if (SampleSize == 0)
            {
                return double.NaN;
            }
            if (Min == (Max - BinWidth))
            {
                if (x > Min)
                {
                    if (x <= Max)
                    {
                        return (Max - x) / (Max - Min);
                    }
                    else
                    {
                        return 1.0;
                    }
                }
                return 0.0;
            }
            double nAtX = Convert.ToDouble(FindBinCount(x));
            double n = Convert.ToDouble(SampleSize);
            return nAtX / n;
        }
        public double InverseCDF(double p)
        {
            if (!p.IsOnRange(0, 1)) throw new ArgumentOutOfRangeException($"The provided probability value: {p} is not on the a valid range: [0, 1]");
            else
            {
                if (HistogramIsZeroValued)
                {
                    return 0.0;
                }
                if (HistogramIsSingleValued)
                {
                    return Mean;
                }
                if (SampleSize == 0)
                {
                    return double.NaN;
                }
                if (Min == (Max - BinWidth))
                {
                    return Min + (BinWidth * p);
                }
                if (p == 0)
                {
                    return Min;
                }
                if (p == 1)
                {
                    return Max;
                }
                int numobs = Convert.ToInt32(SampleSize * p);
                if (p <= 0.5)
                {
                    int index = 0;
                    double obs = BinCounts[index];
                    double cobs = obs;
                    while (cobs < numobs)
                    {
                        index++;
                        obs = BinCounts[index];
                        cobs += obs;

                    }
                    double fraction;
                    if (obs == 0)
                    {
                        fraction = .5;
                    }
                    else
                    {
                        fraction = (cobs - numobs) / obs;
                    }
                    double binOffSet = Convert.ToDouble(index + 1);
                    return Min + BinWidth * binOffSet - BinWidth * fraction;
                }
                else
                {
                    int index = BinCounts.Length - 1;
                    double obs = BinCounts[index];
                    double cobs = SampleSize - obs;
                    while (cobs > numobs)
                    {
                        index--;
                        obs = BinCounts[index];
                        cobs -= obs;
                    }
                    double fraction;
                    if (obs == 0)
                    {
                        fraction = .5;
                    }
                    else
                    {
                        fraction = (numobs - cobs) / obs;
                    }
                    double binOffSet = Convert.ToDouble(BinCounts.Length - index);
                    return Max - BinWidth * binOffSet + BinWidth * fraction;
                }

            }
        }


        public static Empirical ConvertToEmpiricalDistribution(IHistogram histogram)
        {
            int probabilitySteps = 2500;
            double[] cumulativeProbabilities = new double[probabilitySteps];
            double[] invCDS = new double[probabilitySteps];
            for (int i = 0; i < probabilitySteps; i++)
            {
                double probabilityStep = (i + 0.5) / probabilitySteps;
                cumulativeProbabilities[i] = probabilityStep;
                invCDS[i] = histogram.InverseCDF(probabilityStep);
            }
            return new Empirical(cumulativeProbabilities, invCDS);
        }

        public XElement ToXML()
        {
            XElement masterElem = new("Histogram");
            masterElem.SetAttributeValue("Min", Min);
            masterElem.SetAttributeValue("Max", Max);
            masterElem.SetAttributeValue("Bin_Width", BinWidth);
            masterElem.SetAttributeValue("Sample_Size", SampleSize);
            masterElem.SetAttributeValue("Sample_Mean", Mean);
            masterElem.SetAttributeValue("Sample_Variance", _SampleVariance);
            masterElem.SetAttributeValue("Sample_Min", SampleMin);
            masterElem.SetAttributeValue("Sample_Max", SampleMax);
            masterElem.SetAttributeValue("Bin_Quantity", BinCounts.Length);
            masterElem.SetAttributeValue("Converged", IsConverged);
            masterElem.SetAttributeValue("Converged_Iterations", ConvergedIteration);
            masterElem.SetAttributeValue("Converged_On_Max", ConvergedOnMax);
            masterElem.SetAttributeValue("Min_Not_Set", _minHasNotBeenSet);

            string binCounts = string.Join(",", BinCounts.Select(n => n.ToString()).ToArray());
            XElement binElem = new("Bin_Counts");
            binElem.SetAttributeValue("Bin_Count", binCounts);
            masterElem.Add(binElem);

            XElement convergenceCriteriaElement = ConvergenceCriteria.WriteToXML();
            convergenceCriteriaElement.Name = "Convergence_Criteria";
            masterElem.Add(convergenceCriteriaElement);
            return masterElem;
        }

        public static DynamicHistogram ReadFromXML(XElement element)
        {
            string minString = element.Attribute("Min")?.Value;
            double min = Convert.ToDouble(minString);
            string maxString = element.Attribute("Max").Value;
            double max = Convert.ToDouble(maxString);
            string binWidthString = element.Attribute("Bin_Width").Value;
            double binWidth = Convert.ToDouble(binWidthString);
            string sampleSizeString = element.Attribute("Sample_Size").Value;
            Int64 sampleSize = Convert.ToInt64(sampleSizeString);
            string binQuantityString = element.Attribute("Bin_Quantity").Value;
            int binQuantity = Convert.ToInt32(binQuantityString);
            Int64[] binCounts = new Int64[binQuantity];

            XElement binCountsElement = element.Element("Bin_Counts");
            if (binCountsElement != null)
            {
                string binCountString = binCountsElement.Attribute("Bin_Count").Value;
                if (binCountString != null && binCountString.Length > 0)
                {
                    binCounts = binCountString.Split(',').Select(Int64.Parse).ToArray();
                }
            }
            else
            {
                for (int i = 0; i < binQuantity; i++)
                {
                    binCounts[i] = Convert.ToInt64(element.Attribute($"Bin_Counts_{i}").Value);
                }
            }

            ConvergenceCriteria convergenceCriteria = ConvergenceCriteria.ReadFromXML(element.Element("Convergence_Criteria"));
            string sampleMeanString = element.Attribute("Sample_Mean").Value;
            double sampleMean = Convert.ToDouble(sampleMeanString);
            string sampleVarianceString = element.Attribute("Sample_Variance").Value;
            double sampleVariance = Convert.ToDouble(sampleVarianceString);
            string sampleMinString = element.Attribute("Sample_Min").Value;
            double sampleMin = Convert.ToDouble(sampleMinString);
            string sampleMaxString = element.Attribute("Sample_Max").Value;
            double sampleMax = Convert.ToDouble(sampleMaxString);
            string convergedString = element.Attribute("Converged").Value;
            bool converged = Convert.ToBoolean(convergedString);
            string convergedIterationsString = element.Attribute("Converged_Iterations").Value;
            int convergedIterations = Convert.ToInt32(convergedIterationsString);
            string convergedOnMaxString = element.Attribute("Converged_On_Max").Value;
            bool convergedOnMax = Convert.ToBoolean(convergedOnMaxString);
            //as long as we have a min, the min is set
            bool minNotSet = false;
            DynamicHistogram histogram = new(min, max, binWidth, sampleSize, binCounts, convergenceCriteria)
            {
                Mean = sampleMean,
                _SampleVariance = sampleVariance,
                SampleMin = sampleMin,
                SampleMax = sampleMax,
                IsConverged = converged,
                ConvergedIteration = convergedIterations,
                ConvergedOnMax = convergedOnMax,
                _minHasNotBeenSet = minNotSet
            };
            return histogram;
        }
        public bool IsHistogramConverged(double upperq, double lowerq)
        {
            if (IsConverged) { return true; }
            if (SampleSize < ConvergenceCriteria.MinIterations) { return false; }
            if (SampleSize >= ConvergenceCriteria.MaxIterations)
            {
                IsConverged = true;
                ConvergedIteration = SampleSize;
                ConvergedOnMax = true;
                return true;
            }
            if (SampleSize >= ConvergenceCriteria.MinIterations && Min == Max)
            {
                return true;
            }
            double qval = InverseCDF(lowerq);
            double qslope = PDF(qval);
            double variance = (lowerq * (1 - lowerq)) / (((double)SampleSize) * qslope * qslope);
            bool lower = false;
            double lower_comparison = Math.Abs(ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if (lower_comparison <= (ConvergenceCriteria.Tolerance * .5)) { lower = true; }
            qval = InverseCDF(upperq);
            qslope = PDF(qval);
            variance = (upperq * (1 - upperq)) / (((double)SampleSize) * qslope * qslope);
            bool upper = false;
            double upper_comparison = Math.Abs(ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if (upper_comparison <= (ConvergenceCriteria.Tolerance * .5)) { upper = true; }
            if (lower)
            {
                IsConverged = true;
                ConvergedIteration = SampleSize;
            }
            if (upper)
            {
                IsConverged = true;
                ConvergedIteration = SampleSize;
            }
            return IsConverged;
        }
        public bool Equals(IDistribution distribution)
        {

            if (distribution == null)
            {
                return false;
            }
            if (distribution.Type != IDistributionEnum.IHistogram)
            {
                return false;
            }
            IHistogram histogramToCompare = (IHistogram)distribution;
            bool convergenceCriteriaAreEqual = ConvergenceCriteria.Equals(histogramToCompare.ConvergenceCriteria);
            if (!convergenceCriteriaAreEqual)
            {
                return false;
            }
            for (int i = 0; i < BinCounts.Length; i++)
            {
                bool binCountsAreEqual = BinCounts[i].Equals(histogramToCompare.BinCounts[i]);
                if (!binCountsAreEqual)
                {
                    return false;
                }
            }
            bool minAreEqual = Min.Equals(histogramToCompare.Min);
            if (!minAreEqual)
            {
                return false;
            }
            bool binWidthsAreEqual = BinWidth.Equals(histogramToCompare.BinWidth);
            if (!binWidthsAreEqual)
            {
                return false;
            }
            bool sampleSizesAreEqual = SampleSize.Equals(histogramToCompare.SampleSize);
            if (!sampleSizesAreEqual)
            {
                return false;
            }
            bool maxesAreEqual = Max.Equals(histogramToCompare.Max);
            if (!maxesAreEqual)
            {
                return false;
            }
            bool bothConverged = IsConverged.Equals(histogramToCompare.IsConverged);
            if (!bothConverged)
            {
                return false;
            }
            bool convergedIterationsAreEqual = ConvergedIteration.Equals(histogramToCompare.ConvergedIteration);
            if (!convergedIterationsAreEqual)
            {
                return false;
            }
            return true;

        }
        public Int64 EstimateIterationsRemaining(double upperq, double lowerq)
        {
            //TODO: WHAT DO THE BELOW VARIABLES EVEN MEAN??????????
            //PLEASE PROVIDE VARIABLE NAMES IN ENGLISH thank you so much 
            //until then this remains gobbledygook 
            if (IsConverged) return 0;
            double up = upperq;
            double val = up * (1 - up);
            double uz2 = 2 * ConvergenceCriteria.ZAlpha;
            double uxp = InverseCDF(up);
            double ufxp = PDF(uxp);
            Int64 upperestimate = ConvergenceCriteria.MaxIterations;
            if (ufxp > 0.0 & uxp != 0)
            {
                double estimate = Math.Ceiling(val * (Math.Pow((uz2 / (uxp * ConvergenceCriteria.Tolerance * ufxp)), 2.0)));
                if (estimate > int.MaxValue - 1)
                {
                    upperestimate = int.MaxValue - 1;
                }
                else
                {
                    upperestimate = Math.Abs((Int64)estimate);
                }
            }
            double lp = lowerq;
            double lval = lp * (1 - lp);
            double lz2 = 2 * ConvergenceCriteria.ZAlpha;
            double lxp = InverseCDF(lp);
            double lfxp = PDF(lxp);
            Int64 lowerestimate = ConvergenceCriteria.MaxIterations;
            if (lfxp > 0.0 & uxp != 0)
            {
                double estimate = Math.Ceiling(lval * (Math.Pow((lz2 / (lxp * ConvergenceCriteria.Tolerance * lfxp)), 2.0)));
                if (estimate > int.MaxValue - 1)
                {
                    lowerestimate = int.MaxValue - 1;
                }
                else
                {
                    lowerestimate = Math.Abs((Int64)estimate);
                }
            }
            Int64 biggestGuess = Math.Max(upperestimate, lowerestimate);
            Int64 remainingIters = ConvergenceCriteria.MaxIterations - SampleSize;
            return Convert.ToInt64(Math.Min(remainingIters, biggestGuess));
        }

        public string Print(bool round = false)
        {
            string histogram = $"This histogram consists of the following bin starts and bin counts:" + Environment.NewLine;
            for (int i = 0; i < BinCounts.Length; i++)
            {
                histogram += $"Bin Start: {Min + BinWidth * i}, Bin Count: {BinCounts[i]}" + Environment.NewLine;
            }
            return histogram;
        }

        public string Requirements(bool printNotes)
        {
            string message = "The histogram minimally requires a bin width or a list of observations and convergence criteria.";
            return message;
        }

        public IDistribution Sample(double[] packetOfRandomNumbers)
        {
            if (packetOfRandomNumbers.Length < SampleSize) throw new ArgumentException($"The parametric bootstrap sample cannot be constructed using the {Print(true)} distribution. It requires at least {SampleSize} random value but only {packetOfRandomNumbers.Length} were provided.");
            double[] samples = new double[SampleSize];
            for (int i = 0; i < SampleSize; i++) samples[i] = this.InverseCDF(packetOfRandomNumbers[i]);
            return this.Fit(samples);
        }

        public IDistribution Fit(double[] data)
        {
            return new DynamicHistogram(data.ToList(), this.ConvergenceCriteria);
        }

        private bool IsZeroValued()
        {
            bool isZeroValued = false;
            bool meanIsZero = Mean == 0;
            bool standardDeviationIsZero = StandardDeviation == 0;
            if (meanIsZero && standardDeviationIsZero)
            {
                isZeroValued = true;
            }
            return isZeroValued;

        }

        private bool IsSingleValued()
        {
            bool isSingleValued = false;
            if (BinCounts[0] == SampleSize)
            {
                isSingleValued = true;
            }
            return isSingleValued;
        }
        #endregion
    }
}
