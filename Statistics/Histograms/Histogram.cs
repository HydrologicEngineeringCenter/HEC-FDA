using System;
using System.Linq;
using Utilities;
using System.Xml.Linq;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Interfaces;

namespace Statistics.Histograms
{
    public class Histogram: IHistogram, IReportMessage
    {
        #region Fields
        private bool _HistogramIsZeroValued = false;
        private Int64[] _BinCounts = new Int64[] { };
        private double _SampleMean = 10;
        private double _SampleVariance;
        private double _Min;
        private double _Max;
        private double _SampleMin;
        private double _SampleMax;
        private Int64 _SampleSize;
        private double _BinWidth;
        private bool _Converged = false;
        private Int64 _ConvergedIterations = int.MinValue;
        private bool _ConvergedOnMax = false;
        private ConvergenceCriteria _ConvergenceCriteria;
        private bool _minHasNotBeenSet = false;
        private const string _type = "Histogram";
        #endregion
        #region Properties
        public event MessageReportedEventHandler MessageReport;
        public bool HistogramIsZeroValued
        {
            get
            {
                return _HistogramIsZeroValued;
            }
            set
            {
                _HistogramIsZeroValued = value;
            }
        }
        public bool HistogramIsSingleValued
        {
            get
            {
                if(_BinCounts.Length == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        internal double SampleMax
        {
            get
            {
                return _SampleMax;
            }
        }
        internal double SampleMin
        {
            get
            {
                return _SampleMin;
            }
        }
        internal bool ConvergedOnMax
        {
            get
            {
                return _ConvergedOnMax;
            }
        }

        public string MyType
        {
            get
            {
                return _type;
            }
        }
        public bool IsConverged
        {
            get
            {
                return _Converged;
            }
        }
        public Int64 ConvergedIteration
        {
            get
            {
                return _ConvergedIterations;
            }
        }
        public double BinWidth{
            get{
                return _BinWidth;
            }
        }
        public ConvergenceCriteria ConvergenceCriteria
        {
            get
            {
                return _ConvergenceCriteria;
            }
        }
        public Int64[] BinCounts{
            get{
                return _BinCounts;
            }
        }
        public double Min {
            get{
                return _Min;
            }
            private set{
                _Min = value;
            }
        }
        public double Max {
            get{
                return _Max;
            }
            set{
                _Max = value;
            }
        }
        public double Mean {
            get{
                return _SampleMean;
            }
            private set{
                _SampleMean = value;
            }
        }
        public double Variance {
            get{
                return _SampleVariance*(double)((double)(_SampleSize-1)/(double)_SampleSize);
            }
        }
        public double StandardDeviation { 
            get {
                return Math.Pow(Variance, 0.5);
            } 
        }
        public Int64 SampleSize {
            get{
                return _SampleSize;
            }
            private set{
                _SampleSize = value;
            }
        }
        #endregion      
        #region Constructor
        public Histogram()
        {
            _BinWidth = 1; //TODO this hard-coded value is a hack 
            _minHasNotBeenSet = true;
            _ConvergenceCriteria = new ConvergenceCriteria();
            _HistogramIsZeroValued = true;
            for (int i = 0; i < 10; i++)
            {
                AddObservationToHistogram(0);
            }
        }
        public Histogram(double min, double binWidth)
        {
            _BinWidth = binWidth;
            Min = min;
            Max = Min + _BinWidth;
            int numberOfBins = 1;
            _BinCounts = new Int64[numberOfBins];
            _ConvergenceCriteria = new ConvergenceCriteria();
        }
        public Histogram(double binWidth)
        {
            _BinWidth = binWidth;
            _minHasNotBeenSet = true;
            _ConvergenceCriteria = new ConvergenceCriteria();
        }
        public Histogram(double min, double binWidth, ConvergenceCriteria convergenceCriteria)
        {
            _BinWidth = binWidth;
            Min = min;
            Max = Min + _BinWidth;
            int numberOfBins = 1;
            _BinCounts = new Int64[numberOfBins];
            _ConvergenceCriteria = convergenceCriteria;
        }
        public Histogram(double[] data, ConvergenceCriteria convergenceCriteria)
        {
            _ConvergenceCriteria = convergenceCriteria;
            Min = data.Min();
            Max = data.Max();
            int quantityOfBins = (int)Math.Ceiling(1 + 3.322 * Math.Log10(data.Length));
            double range = Max - Min;
             _BinWidth = range / quantityOfBins;
            AddObservationsToHistogram(data);
        }
        private Histogram(double min, double max, double binWidth, Int64 sampleSize, Int64[] binCounts, ConvergenceCriteria convergenceCriteria)
        {
            Min = min;
            Max = max;
            _BinWidth = binWidth;
            _BinCounts = binCounts;
            _ConvergenceCriteria = convergenceCriteria;
            _SampleSize = sampleSize;
        }
        #endregion
        #region Functions
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public double Skewness()
        {
            double deviation = 0, deviation2 = 0, deviation3 = 0;
            if (_SampleSize == 0)
            {
                return double.NaN;
            }
            if (_Min == (_Max-_BinWidth))
            {
                return 0.0;
            }
            for (int i = 0; i < _BinCounts.Length; i++)
            {
                double midpoint = Min + (i * _BinWidth) + (0.5 * _BinWidth);

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
        public double HistogramMean()
        {           
            if (_SampleSize == 0)
            {
                return double.NaN;
            }
            if (_Min == (_Max - _BinWidth))
            {
                return _Max + (.5*_BinWidth);
            }
            double sum = 0;
            for (int i = 0; i < BinCounts.Length; i++)
            {
                sum += (_Min + (i * _BinWidth) + (0.5 * _BinWidth)) * _BinCounts[i];
            }
            return sum / _SampleSize;
        }
        public double HistogramVariance()
        {
            if (_SampleSize == 0)
            {
                return double.NaN;
            }
            if (_SampleSize == 1)
            {
                return 0.0;
            }
            if (_Min == (_Max - _BinWidth))
            {
                return 0.0;
            }
            double deviation = 0, deviation2 = 0;
            for (int i = 0; i < BinCounts.Length; i++)
            {
                double midpoint = _Min + (i * _BinWidth) + (0.5 * _BinWidth);

                deviation = midpoint - _SampleMean;
                deviation2 += deviation * deviation;

            }
            double variance = deviation2 / (_SampleSize - 1);
            return variance;
        }
        public double HistogramStandardDeviation(){
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
            if (_SampleSize == 0){
                _SampleMax = observation;
                _SampleMin = observation;
                _SampleMean = observation;
                _SampleVariance = 0;
                if (_minHasNotBeenSet)
                {
                    Min = observation;
                    Max = observation + _BinWidth;
                    _BinCounts = new Int64[] { 0};
                }
                _SampleSize = 1;
            }else{
                if (observation>_SampleMax) _SampleMax = observation;
                if (observation<_SampleMin) _SampleMin = observation;
                _SampleSize +=1;
                double tmpMean = _SampleMean +((observation -_SampleMean)/(double)_SampleSize);
                _SampleVariance = ((((double)(_SampleSize-2)/(double)(_SampleSize-1))*_SampleVariance)+(Math.Pow(observation-_SampleMean,2))/(double)_SampleSize);
                _SampleMean = tmpMean;
            }
            int quantityAdditionalBins = 0;
            if (observation < _Min)
            {   
                quantityAdditionalBins = Convert.ToInt32(Math.Ceiling((_Min - observation)/_BinWidth));
                Int64[] newBinCounts = new Int64[quantityAdditionalBins + _BinCounts.Length];

                for (int i = _BinCounts.Length + quantityAdditionalBins -1; i > (quantityAdditionalBins-1); i--)
                {
                    newBinCounts[i] = _BinCounts[i - quantityAdditionalBins];
                }
                _BinCounts = newBinCounts;
                _BinCounts[0] += 1;
                double newMin = _Min - (quantityAdditionalBins * _BinWidth);
                double max = _Max;
                Min = newMin;
            } else if (observation > _Max)
            {
                quantityAdditionalBins = Convert.ToInt32(Math.Ceiling((observation - _Max+_BinWidth) / _BinWidth));
                Int64[] newBinCounts = new Int64[quantityAdditionalBins + _BinCounts.Length];
                for (int i = 0; i < _BinCounts.Length; i++)
                {
                    newBinCounts[i] = _BinCounts[i];
                }
                newBinCounts[_BinCounts.Length + quantityAdditionalBins-1] += 1;
                _BinCounts = newBinCounts;
                double newMax = Min + (_BinCounts.Length * _BinWidth); //is this right?
                Max = newMax;
            } else
            {
                int newObsIndex = 0;
                if (observation != _Min)
                {
                    newObsIndex = Convert.ToInt32(Math.Floor((observation - _Min) / _BinWidth));
                }
                if (observation == _Max)
                {
                    quantityAdditionalBins = 1;
                    Int64[] newBinCounts = new Int64[quantityAdditionalBins + _BinCounts.Length];
                    for (int i = 0; i < _BinCounts.Length; i++)
                    {
                        newBinCounts[i] = _BinCounts[i];
                    }
                    _BinCounts = newBinCounts;
                    double newMax = _Min + (_BinCounts.Length * _BinWidth);//double check
                    Max = newMax;
                }
                _BinCounts[newObsIndex] += 1;
            }
        }
        public void AddObservationsToHistogram(double[] data)
        {
            foreach (double x in data)
            {
                AddObservationToHistogram(x);
            }
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
            int obsIndex = Convert.ToInt32(Math.Floor((x - Min) / _BinWidth));
            if(obsIndex == _BinCounts.Length)
            {
                obsIndex -= 1;
            }
            if (cumulative)
            {
                Int64 sum = 0;
                for (int i = 0; i<obsIndex+1; i++)
                {
                    sum += _BinCounts[i];
                }
                return sum;
            }
            else
            {
                return _BinCounts[obsIndex];
            }

        }
        public double PDF(double x)
        {
            if (_SampleSize == 0)
            {
                return double.NaN;
            }
            if (_Min == (_Max-_BinWidth))
            {
                if (x > _Min)
                {
                    if (x <= _Max)
                    {
                        return 1.0;
                    }
                }
                return 0.0;
            }
            double nAtX = Convert.ToDouble(FindBinCount(x, false));
            double n = Convert.ToDouble(SampleSize);
            return nAtX/n;
        }
        public double CDF(double x)
        {
            if (_SampleSize == 0)
            {
                return double.NaN;
            }
            if (_Min == (_Max - _BinWidth))
            {
                if (x > _Min)
                {
                    if (x <= _Max)
                    {
                        return (_Max - x) / (_Max - _Min);
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
                if (_HistogramIsZeroValued)
                {
                    return 0.0;
                }
                if (_SampleSize == 0)
                {
                    return double.NaN;
                }
                if (_Min == (_Max - _BinWidth))
                {
                    return _Min + (_BinWidth * p);
                }
                if (p==0)
                {
                    return Min;
                }
                if (p==1)
                {
                    return Max;
                }
                int numobs = Convert.ToInt32(SampleSize * p);
                if (p <= 0.5)
                {
                    int index = 0;
                    double obs = _BinCounts[index];
                    double cobs = obs;
                    while (cobs < numobs)
                    {
                        index++;
                        obs = _BinCounts[index];
                        cobs += obs;

                    }
                    double fraction = 0.0;
                    if (obs == 0)
                    {
                        fraction = .5;
                    }
                    else
                    {
                        fraction = (cobs - numobs) / obs;
                    }
                    double binOffSet = Convert.ToDouble(index + 1);
                    return Min + _BinWidth * binOffSet - _BinWidth * fraction;
                } else
                {
                    int index = _BinCounts.Length - 1;
                    double obs = _BinCounts[index];
                    double cobs = SampleSize - obs;
                    while (cobs > numobs)
                    {
                        index--;
                        obs = _BinCounts[index];
                        cobs -= obs;
                    }
                    double fraction = 0.0;
                    if (obs == 0)
                    {
                        fraction = .5;
                    }
                    else
                    {
                        fraction = (numobs - cobs) / obs;
                    }
                    double binOffSet = Convert.ToDouble(_BinCounts.Length - index);
                    return Max - _BinWidth * binOffSet + _BinWidth * fraction;
                }
                
            }
        }
        public static IHistogram AddHistograms(List<IHistogram> histograms)
        {
            IHistogram aggregatedHistogram;

            if (histograms.Count > 0)
            {
                ConvergenceCriteria convergenceCriteria = histograms[0].ConvergenceCriteria;
                double min = 0;
                double max = 0;
                int binQuantity = 0;
                double binWidth = 0;
                foreach (IHistogram histogramToAdd in histograms)
                {
                    min += histogramToAdd.Min;
                    max += histogramToAdd.Max;
                    binQuantity = Math.Max(binQuantity, histogramToAdd.BinCounts.Length);
                    binWidth += histogramToAdd.BinWidth;
                }
                binWidth = binWidth / histograms.Count; //use the average of the binWidths 
                aggregatedHistogram = new Histogram(min, binWidth, convergenceCriteria);
                //walk across the probability domain of each histogram at equal probability intervals 
                for (int i = 0; i < binQuantity; i++)
                {
                    double probabilityStep = (i + 0.5) / binQuantity; //binQuantity determines the number of probability steps ... this may be too small
                    double summedValue = 0;
                    Int64 summedBinCount = 0;

                    foreach (IHistogram histogramToSample in histograms)
                    {
                        histogramToSample.ForceDeQueue();
                        double sampledValue = histogramToSample.InverseCDF(probabilityStep); //what is the value of each histogram at the given probability step
                        summedValue += sampledValue; //sum those values 
                        summedBinCount += histogramToSample.FindBinCount(sampledValue, false); //sum their frequencies 
                    }
                    for (int j = 0; j < summedBinCount; j++)
                    {//this is a coarse approximation, there is probably a more granular way of doing this 
                        aggregatedHistogram.AddObservationToHistogram(summedValue, j); // add the summed value to a new histogram x times where x is the sum of frequencies 
                    } 
                }
            }
            else
            {
                aggregatedHistogram = new Histogram(0,1);
            }
            return aggregatedHistogram;
        }
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("Histogram");
            masterElem.SetAttributeValue("Min", _Min);
            masterElem.SetAttributeValue("Max", _Max);
            masterElem.SetAttributeValue("Bin_Width", _BinWidth);
            masterElem.SetAttributeValue("Sample_Size", SampleSize);
            masterElem.SetAttributeValue("Sample_Mean", _SampleMean);
            masterElem.SetAttributeValue("Sample_Variance", _SampleVariance);
            masterElem.SetAttributeValue("Sample_Min", _SampleMin);
            masterElem.SetAttributeValue("Sample_Max", _SampleMax);
            masterElem.SetAttributeValue("Bin_Quantity", _BinCounts.Length);
            masterElem.SetAttributeValue("Converged", _Converged);
            masterElem.SetAttributeValue("Converged_Iterations", _ConvergedIterations);
            masterElem.SetAttributeValue("Converged_On_Max", _ConvergedOnMax);
            masterElem.SetAttributeValue("Min_Not_Set", _minHasNotBeenSet);
            for (int i = 0; i < _BinCounts.Length; i++)
            {
                masterElem.SetAttributeValue($"Bin_Counts_{i}", _BinCounts[i]);
            }
            XElement convergenceCriteriaElement = _ConvergenceCriteria.WriteToXML();
            convergenceCriteriaElement.Name = "Convergence_Criteria";
            masterElem.Add(convergenceCriteriaElement);
            return masterElem;
        }
            public static Histogram ReadFromXML(XElement element)
        {
            string minString = element.Attribute("Min").Value;
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
            for (int i = 0; i < binQuantity; i++)
            {
                binCounts[i] = Convert.ToInt64(element.Attribute($"Bin_Counts_{i}").Value);
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
            string minNotSetString = element.Attribute("Min_Not_Set").Value;
            bool minNotSet = Convert.ToBoolean(minNotSetString);
            Histogram histogram = new Histogram(min, max, binWidth, sampleSize, binCounts, convergenceCriteria);
            histogram._SampleMean = sampleMean;
            histogram._SampleVariance = sampleVariance;
            histogram._SampleMin = sampleMin;
            histogram._SampleMax = sampleMax;
            histogram._Converged = converged;
            histogram._ConvergedIterations = convergedIterations;
            histogram._ConvergedOnMax = convergedOnMax;
            histogram._minHasNotBeenSet = minNotSet;
            return histogram;
        }
        public bool IsHistogramConverged(double upperq, double lowerq)
        {
            if (_Converged) { return true; }
            if (_SampleSize< _ConvergenceCriteria.MinIterations) { return false; }
            if (_SampleSize >= _ConvergenceCriteria.MaxIterations) {
                _Converged = true;
                _ConvergedIterations = _SampleSize;
                _ConvergedOnMax = true;
                return true;
            }
            double qval = InverseCDF(lowerq);
            double qslope = PDF(qval);
            double variance = (lowerq * (1 - lowerq)) / (((double)_SampleSize) * qslope * qslope);
            bool lower = false;
            double lower_comparison = Math.Abs(_ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if (lower_comparison <= (_ConvergenceCriteria.Tolerance *.5)){ lower = true; }
            qval = InverseCDF(upperq);
            qslope = PDF(qval);
            variance = (upperq * (1 - upperq)) / (((double)_SampleSize) * qslope * qslope);
            bool upper = false;
            double upper_comparison = Math.Abs(_ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if ( upper_comparison <= (_ConvergenceCriteria.Tolerance *.5)) { upper = true; }
            if (lower)
            {
                _Converged = true;
                _ConvergedIterations = _SampleSize;
            }
            if (upper)
            {
                _Converged = true;
                _ConvergedIterations = _SampleSize;
            }
            return _Converged;
        }
        public bool Equals(IHistogram histogramToCompare)
        {
            bool convergenceCriteriaAreEqual = _ConvergenceCriteria.Equals(histogramToCompare.ConvergenceCriteria);
            if (!convergenceCriteriaAreEqual)
            {
                return false;
            }
            for (int i = 0; i < _BinCounts.Length; i++)
            {
                bool binCountsAreEqual = _BinCounts[i].Equals(histogramToCompare.BinCounts[i]);
                if (!binCountsAreEqual)
                {
                    return false;
                }
            }
            bool minAreEqual = _Min.Equals(histogramToCompare.Min);
            if (!minAreEqual)
            {
                return false;
            }
            bool binWidthsAreEqual = _BinWidth.Equals(histogramToCompare.BinWidth);
            if (!binWidthsAreEqual)
            {
                return false;
            }
            bool sampleSizesAreEqual = _SampleSize.Equals(histogramToCompare.SampleSize);
            if (!sampleSizesAreEqual)
            {
                return false;
            }
            bool maxesAreEqual = _Max.Equals(histogramToCompare.Max);
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
            if (_Converged) return 0;
            double up = upperq;
            double val = up * (1 - up);
            double uz2 = 2 * _ConvergenceCriteria.ZAlpha;
            double uxp = InverseCDF(up);
            double ufxp = PDF(uxp);
            Int64 upperestimate = _ConvergenceCriteria.MaxIterations;
            if (ufxp > 0.0 & uxp != 0)
            {
                double estimate = Math.Ceiling(val * (Math.Pow((uz2 / (uxp * _ConvergenceCriteria.Tolerance * ufxp)), 2.0)));
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
            double lz2 = 2 * _ConvergenceCriteria.ZAlpha;
            double lxp = InverseCDF(lp);
            double lfxp = PDF(lxp);
            Int64 lowerestimate = _ConvergenceCriteria.MaxIterations;
            if (lfxp > 0.0 & uxp != 0)
            {
                double estimate = Math.Ceiling(lval * (Math.Pow((lz2 / (lxp * _ConvergenceCriteria.Tolerance * lfxp)), 2.0)));
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
            Int64 remainingIters = _ConvergenceCriteria.MaxIterations - _SampleSize;
            return Convert.ToInt64(Math.Min(remainingIters, biggestGuess));
        }

        #endregion
    }
}
