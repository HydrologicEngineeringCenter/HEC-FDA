using System;
using System.Linq;
using Utilities;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Statistics.Histograms
{
    public class Histogram: IHistogram
    {
        #region Fields
        private Int32[] _BinCounts = new Int32[] { };
        private double _SampleMean;
        private double _SampleVariance;
        private double _Min;
        private double _Max;
        private double _SampleMin;
        private double _SampleMax;
        private int _N;
        private double _BinWidth;
        private bool _Converged = false;
        private int _ConvergedIterations = int.MinValue;
        private bool _ConvergedOnMax = false;
        private ConvergenceCriteria _ConvergenceCriteria;
        private bool _minHasNotBeenSet = false;
        private const string _type = "Histogram";
        #endregion
        #region Properties

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
        public int ConvergedIteration
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
        public Int32[] BinCounts{
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
                return _SampleVariance*(double)((double)(_N-1)/(double)_N);
            }
        }
        public double StandardDeviation { 
            get {
                return Math.Pow(Variance, 0.5);
            } 
        }
        public int SampleSize {
            get{
                return _N;
            }
            private set{
                _N = value;
            }
        }
        #endregion      
        #region Constructor
        public Histogram()
        {
            _BinWidth = 1; //TODO this hard-coded value is a hack 
            _minHasNotBeenSet = true;
            _ConvergenceCriteria = new ConvergenceCriteria();
        }
        public Histogram(double min, double binWidth)
        {
            _BinWidth = binWidth;
            Min = min;
            Max = Min + _BinWidth;
            int numberOfBins = 1;
            _BinCounts = new Int32[numberOfBins];
            _ConvergenceCriteria = new ConvergenceCriteria();
        }
        public Histogram(double binWidth)
        {
            _BinWidth = binWidth;
            _minHasNotBeenSet = true;
            _ConvergenceCriteria = new ConvergenceCriteria();
        }
        public Histogram(double min, double binWidth, ConvergenceCriteria _c)
        {
            _BinWidth = binWidth;
            Min = min;
            Max = Min + _BinWidth;
            int numberOfBins = 1;
            _BinCounts = new Int32[numberOfBins];
            _ConvergenceCriteria = _c;
        }
        private Histogram(double min, double max, double binWidth, int sampleSize, Int32[] binCounts, ConvergenceCriteria convergenceCriteria)
        {
            Min = min;
            Max = max;
            _BinWidth = binWidth;
            _BinCounts = binCounts;
            _ConvergenceCriteria = convergenceCriteria;
            _N = sampleSize;
        }
        #endregion
        #region Functions
        public double Skewness()
        {
            double deviation = 0, deviation2 = 0, deviation3 = 0;
            if (_N == 0)
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
            if (_N == 0)
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
            return sum / _N;
        }
        public double HistogramVariance()
        {
            if (_N == 0)
            {
                return double.NaN;
            }
            if (_N == 1)
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
            double variance = deviation2 / (_N - 1);
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
        public void AddObservationToHistogram(double observation, int index = 0) //TODO index is a hack
        {   
            if (_N == 0){
                _SampleMax = observation;
                _SampleMin = observation;
                _SampleMean = observation;
                _SampleVariance = 0;
                if (_minHasNotBeenSet)
                {
                    Min = observation;
                    Max = observation + _BinWidth;
                    _BinCounts = new int[] { 0};
                }
                _N = 1;
            }else{
                if (observation>_SampleMax) _SampleMax = observation;
                if (observation<_SampleMin) _SampleMin = observation;
                _N +=1;
                double tmpMean = _SampleMean +((observation -_SampleMean)/(double)_N);
                _SampleVariance = ((((double)(_N-2)/(double)(_N-1))*_SampleVariance)+(Math.Pow(observation-_SampleMean,2))/(double)_N);
                _SampleMean = tmpMean;
            }
            int quantityAdditionalBins = 0;
            if (observation < _Min)
            {   
                quantityAdditionalBins = Convert.ToInt32(Math.Ceiling((_Min - observation)/_BinWidth));
                Int32[] newBinCounts = new Int32[quantityAdditionalBins + _BinCounts.Length];

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
                Int32[] newBinCounts = new Int32[quantityAdditionalBins + _BinCounts.Length];
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
                    Int32[] newBinCounts = new Int32[quantityAdditionalBins + _BinCounts.Length];
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
        private double FindBinCount(double x, bool cumulative = true)
        {
            int obsIndex = Convert.ToInt32(Math.Floor((x - Min) / _BinWidth));
            if (cumulative)
            {
                double sum = 0;
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
            if (_N == 0)
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
            if (_N == 0)
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
                if (_N == 0)
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
        public void AddHistograms(List<IHistogram> histograms)
        {
            //if histograms.count == 0, then nothing happens, as you would expect
            //adding 0 histograms to *this* histogram changes nothing 
            if (histograms.Count > 0)
            {
                ConvergenceCriteria convergenceCriteria = histograms[0].ConvergenceCriteria;
                double min = 0;
                double max = 0;
                int sampleSize = 0;
                foreach (Histogram histogramToAdd in histograms)
                {
                    double newMin = Math.Min(min, histogramToAdd.Min);
                    min = newMin;
                    double newMax = Math.Max(max, histogramToAdd.Max);
                    max = newMax;
                    int newSampleSize = Math.Max(sampleSize, (int)histogramToAdd.SampleSize);
                    sampleSize = newSampleSize;
                }
                double range = max - min;
                double binQuantity = 1 + 3.322 * Math.Log(sampleSize); //sturges rule 
                double binWidth = range / sampleSize;
                Histogram histogram = new Histogram(min, binWidth, convergenceCriteria);
                int seed = 1234;
                Random random = new Random(seed);
                for (int i = 0; i < sampleSize; i++)
                {
                    double summedValue = 0;
                    foreach (Histogram histogramToSample in histograms)
                    {
                        double value = histogramToSample.InverseCDF(random.NextDouble());
                        summedValue += value;
                    }
                    double thisValue = this.InverseCDF(random.NextDouble());
                    summedValue += thisValue;
                    histogram.AddObservationToHistogram(summedValue, i);
                }
                _BinCounts = histogram._BinCounts;
                _SampleMean = histogram._SampleMean;
                _SampleVariance = histogram._SampleVariance;
                _Min = histogram._Min;
                _Max = histogram._Max;
                _SampleMin = histogram._SampleMin;
                _SampleMax = histogram._SampleMax;
                _N = histogram._N;
                _BinWidth = histogram._BinWidth;
                _Converged = histogram._Converged;
                _ConvergedIterations = histogram._ConvergedIterations;
                _ConvergedOnMax = histogram._ConvergedOnMax;
                _ConvergenceCriteria = histogram._ConvergenceCriteria;
            }
        }
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("Histogram");
            masterElem.SetAttributeValue("Min", Min);
            masterElem.SetAttributeValue("Max", Max);
            masterElem.SetAttributeValue("Bin_Width", _BinWidth);
            masterElem.SetAttributeValue("Ordinate_Count", SampleSize);
            masterElem.SetAttributeValue("Bin_Quantity", _BinCounts.Length);
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
            string sampleSizeString = element.Attribute("Ordinate_Count").Value;
            int sampleSize = Convert.ToInt32(sampleSizeString);
            string binQuantityString = element.Attribute("Bin_Quantity").Value;
            int binQuantity = Convert.ToInt32(binQuantityString);
            Int32[] binCounts = new Int32[binQuantity];
            for (int i = 0; i < binQuantity; i++)
            {
                binCounts[i] = Convert.ToInt32(element.Attribute($"Bin_Counts_{i}").Value);
            }
            ConvergenceCriteria convergenceCriteria = ConvergenceCriteria.ReadFromXML(element.Element("Convergence_Criteria"));
            return new Histogram(min, max, binWidth, sampleSize, binCounts, convergenceCriteria);
        }
        public bool TestForConvergence(double upperq, double lowerq)
        {
            if (_Converged) { return true; }
            if (_N< _ConvergenceCriteria.MinIterations) { return false; }
            if (_N >= _ConvergenceCriteria.MaxIterations) {
                _Converged = true;
                _ConvergedIterations = _N;
                _ConvergedOnMax = true;
                return true;
            }
            double qval = InverseCDF(lowerq);
            double qslope = PDF(qval);
            double variance = (lowerq * (1 - lowerq)) / (((double)_N) * qslope * qslope);
            bool lower = false;
            double lower_comparison = Math.Abs(_ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if (lower_comparison <= (_ConvergenceCriteria.Tolerance *.5)){ lower = true; }
            qval = InverseCDF(upperq);
            qslope = PDF(qval);
            variance = (upperq * (1 - upperq)) / (((double)_N) * qslope * qslope);
            bool upper = false;
            double upper_comparison = Math.Abs(_ConvergenceCriteria.ZAlpha * Math.Sqrt(variance) / qval);
            if ( upper_comparison <= (_ConvergenceCriteria.Tolerance *.5)) { upper = true; }
            if (lower)
            {
                _Converged = true;
                _ConvergedIterations = _N;
            }
            if (upper)
            {
                _Converged = true;
                _ConvergedIterations = _N;
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
            bool sampleSizesAreEqual = _N.Equals(histogramToCompare.SampleSize);
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
        public int EstimateIterationsRemaining(double upperq, double lowerq)
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
            int upperestimate = _ConvergenceCriteria.MaxIterations;
            if (ufxp > 0.0 & uxp != 0)
            {
                upperestimate = Math.Abs((int)Math.Ceiling(val * (Math.Pow((uz2 / (uxp * _ConvergenceCriteria.Tolerance * ufxp)), 2.0))));
            }
            double lp = lowerq;
            double lval = lp * (1 - lp);
            double lz2 = 2 * _ConvergenceCriteria.ZAlpha;
            double lxp = InverseCDF(lp);
            double lfxp = PDF(lxp);
            int lowerestimate = _ConvergenceCriteria.MaxIterations;
            if (lfxp > 0.0 & uxp != 0)
            {
                lowerestimate = Math.Abs((int)Math.Ceiling(val * (Math.Pow((lz2 / (lxp * _ConvergenceCriteria.Tolerance * lfxp)), 2.0))));
            }
            int biggestGuess = Math.Max(upperestimate, lowerestimate);
            int remainingIters = _ConvergenceCriteria.MaxIterations - _N;
            return Convert.ToInt32(Math.Min(remainingIters, biggestGuess));
        }

        #endregion
    }
}
