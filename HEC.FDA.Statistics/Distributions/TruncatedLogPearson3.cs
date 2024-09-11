using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;


namespace Statistics.Distributions
{
    public class TruncatedLogPearson3 : ContinuousDistribution
    {

        private double _ProbabilityMin;
        private double _ProbabilityMax;
        private bool _Constructed;

        #region Properties
        public override IDistributionEnum Type => IDistributionEnum.LogPearsonIII;
        [Stored(Name = "Mean", type = typeof(double))]
        public double Mean { get; set; }
        [Stored(Name = "St_Dev", type = typeof(double))]
        public double StandardDeviation { get; set; }
        [Stored(Name = "Skew", type = typeof(double))]
        public double Skewness { get; set; }
        [Stored(Name = "Min", type = typeof(double))]
        public double Min
        {
            get; set;
        }
        [Stored(Name = "Max", type = typeof(double))]
        public double Max
        {
            get; set;
        }
        #endregion

        #region Constructor
        public TruncatedLogPearson3()
        {
            //for reflection;
            Mean = 0.1;
            StandardDeviation = .01;
            Skewness = .01;
            SampleSize = 1;
            Min = double.NegativeInfinity;
            Max = double.PositiveInfinity;
            _ProbabilityMin = 0D;
            _ProbabilityMax = 1D;
            addRules();
            BuildFromProperties();
            _Constructed = true;

        }
        public TruncatedLogPearson3(double mean, double standardDeviation, double skew, double min, double max, Int64 sampleSize = 1)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            Skewness = skew;
            SampleSize = sampleSize;
            Min = min;
            Max = max;
            Truncated = true;
            addRules();
            BuildFromProperties();
        }

        public void BuildFromProperties()
        {
            Validate();
            if (!HasErrors)
            {
                SetProbabilityRangeAndMinAndMax(Min, Max);
            }

            _Constructed = true;
        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(StandardDeviation),
                new Rule(() => {
                    return StandardDeviation > 0;
                },
                "Standard Deviation must be greater than 0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(StandardDeviation),
                new Rule(() => {
                    return StandardDeviation < 3;
                },
                "Standard Deviation must be less than 3.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Mean),
                new Rule(() => {
                    return Mean > 0;
                },
                "Mean must be greater than 0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Mean),
                new Rule(() => {
                    return Mean < 7; //log base 10 mean annual max flow in cfs of amazon river at mouth is 6.7
                },
                "Mean must be less than 7.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Skewness),
                new Rule(() => {
                    return Skewness > -3.0;
                },
                "Skewness must be greater than -3.0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Skewness),
                new Rule(() => {
                    return Skewness < 3.0;
                },
                "Skewness must be less than 3.0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(SampleSize),
                new Rule(() => {
                    return SampleSize > 0;
                },
                "SampleSize must be greater than 0.",
                ErrorLevel.Fatal));
        }
        #endregion

        #region Functions

        private void SetProbabilityRangeAndMinAndMax(double min, double max)
        {//TODO: need to set the probability range before we get here if LP3 is not constructed 
            double pmin = 0;
            double epsilon = 1 / 1000000000d;
            double pmax = 1 - pmin;
            if (min.IsFinite() || max.IsFinite())//not entirely sure how inclusive or works with one sided truncation and the while loop below.
            {
                pmin = CDF(min);
                pmax = CDF(max);
            }
            else
            {
                pmin = .0000001;
                pmax = 1 - pmin;
                min = InverseCDF(pmin);
                max = InverseCDF(pmax);
            }
            while (!(min.IsFinite() && max.IsFinite()))
            {
                pmin += epsilon;
                pmax -= epsilon;
                if (!min.IsFinite()) min = InverseCDF(pmin);
                if (!max.IsFinite()) max = InverseCDF(pmax);
                if (pmin > 0.25)
                    throw new Exception($"The log Pearson III object is not constructable because 50% or more of its distribution returns {double.NegativeInfinity} and {double.PositiveInfinity}.");
            }
            //apparently we have done everything we need at this point.
            Max = max;
            Min = min;
            _ProbabilityMin = pmin;
            _ProbabilityMax = pmax;
            //IsConstructed = true;
        }
        #region IDistribution Functions
        public override double PDF(double x)
        {
            if (x < Min || x > Max) return double.Epsilon;
            else
            {
                PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
                return d.PDF(Math.Log10(x)) / x / Math.Log(10);

            }
        }
        public override double CDF(double x)
        {

            if (_Constructed)
            {
                if (x == Min) return _ProbabilityMin;
                if (x == Max) return _ProbabilityMax;
            }

            if (x < Min) return 0;
            if (x > Max) return 1;
            if (x > 0)
            {
                PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
                return d.CDF(Math.Log10(x));
            }
            else return 0;
        }
        public override double InverseCDF(double p)
        {
            //check if constructed
            //if not, skip probability min and max pieces

            if (Truncated && _Constructed)
            {
                p = _ProbabilityMin + (p) * (_ProbabilityMax - _ProbabilityMin);
            }
            if (!p.IsFinite())
            {
                throw new ArgumentException($"The value of specified probability parameter: {p} is invalid because it is not on the valid probability range: [0, 1].");
            }
            else // Range has been set check p against [_ProbabilityRange.Min, _ProbabilityRange.Max]
            {
                if (_Constructed) // object is constructed
                {
                    if (p <= _ProbabilityMin) return Min;
                    if (p >= _ProbabilityMax) return Max;
                }
            }
            PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
            return Math.Pow(10, d.InverseCDF(p));
        }
        #endregion

        public override bool Equals(IDistribution distribution)
        {
            if (!(distribution is TruncatedLogPearson3))
            {
                return false;
            }
            return ((TruncatedLogPearson3)distribution).Mean == Mean &&
                   ((TruncatedLogPearson3)distribution).StandardDeviation == StandardDeviation &&
                   ((TruncatedLogPearson3)distribution).Skewness == Skewness &&
                   ((TruncatedLogPearson3)distribution).SampleSize == SampleSize;
        }

        public override IDistribution Fit(double[] sample)
        {
            for (int i = 0; i < sample.Count(); i++)
            {
                sample[i] = Math.Log10(sample[i]);
            }
            ISampleStatistics stats = new SampleStatistics(sample);
            return new TruncatedLogPearson3(stats.Mean, stats.StandardDeviation, stats.Skewness, this.Min, this.Max, stats.SampleSize);
        }
        #endregion
    }
}
