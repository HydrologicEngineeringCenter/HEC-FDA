using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics
{
    public class SampleStatistics : ISampleStatistics
    {
        private double _mean;
        private double _sampleVariance;
        private double _min;
        private double _max;
        private double _median;
        private double _skew;
        private double _kurtosis;
        private Int64 _n;

        public double Mean
        {
            get { return _mean; }
        }
        public double Median
        {
            get { return _median; }
        }
        public double Variance
        {
            get
            {
                return _sampleVariance * (double)((double)(_n - 1) / (double)_n);
            }
        }
        public double StandardDeviation
        {
            get
            {
                return Math.Pow(Variance, 0.5);
            }
        }
        public double Skewness
        {
            get { return _skew; }
        }
        public double Kurtosis
        {
            get { return _kurtosis; }
        }
        public Utilities.IRange<double> Range { get; }
        public int SampleSize
        {
            get { return (int)_n; }
        }
        public SampleStatistics(double[] data)
        {
            InitalizeStats(data);
            Range = Utilities.IRangeFactory.Factory(_min, _max);
        }
        internal void InitalizeStats(IEnumerable<double> observations)
        {
            foreach (double observation in observations)
            {
                if (_n == 0)
                {
                    _max = observation;
                    _min = observation;
                    _mean = observation;
                    _sampleVariance = 0;
                    _n = 1;
                }
                else
                {
                    if (observation > _max) _max = observation;
                    if (observation < _min) _min = observation;
                    _n += 1;
                    double obsminusmean = observation - _mean;
                    double tmpMean = _mean + ((obsminusmean) / (double)_n);
                    _sampleVariance = ((((double)(_n - 2) / (double)(_n - 1)) * _sampleVariance) + ((obsminusmean)*(obsminusmean)) / (double)_n);
                    _mean = tmpMean;
                }
            }

            double s = System.Math.Pow(_sampleVariance * (double)((double)(_n - 1) / (double)_n), 0.5);
            double SkewSums = 0;
            //double ksums = 0;
            double midpoint = ((double)_n - 1) / 2;
            bool noRounding = false;
            if (System.Math.Floor(midpoint) == midpoint)
            {
                noRounding = true;
            }
            else
            {
                midpoint = System.Math.Floor(midpoint);
            }
            Array.Sort(observations.ToArray());
            int val = 0;
            bool firstpass = true;
            foreach (double observation in observations)
            {
                if (midpoint == val)
                {
                    if (noRounding)
                    {
                        _median = observation;
                    }
                    else
                    {
                        if (firstpass)
                        {
                            midpoint += 1;
                            _median += observation;
                            firstpass = false;
                        }
                        else
                        {
                            _median += observation;
                            _median /= 2.0;
                        }
                    }

                }
                double obsminusmean = observation - _mean;
                SkewSums = SkewSums + (obsminusmean*obsminusmean*obsminusmean);
                //ksums = ksums + System.Math.Pow(((observation - _mean) / s), 4.0);
                val++;
            }
            double nd = (double)_n;
           // ksums = ksums * (nd * (nd + 1.0)) / ((nd - 1.0) * (nd - 2.0) * (nd - 3.0));
            _skew = ((nd) * SkewSums) / ((nd - 1.0) * (nd - 2.0) * (s*s*s));
            //_kurtosis = (ksums) - ((3.0 * (System.Math.Pow((nd - 1.0), 2.0))) / ((nd - 2.0) * (nd - 3.0)));
        }
        public IMessageLevels Validate(Utilities.IValidator<ISampleStatistics> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
    }
}
