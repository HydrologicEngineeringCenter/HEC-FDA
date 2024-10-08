﻿using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Linq;
using Utilities;

namespace Statistics.Distributions
{
    public class TruncatedLogNormal : ContinuousDistribution
    {
        #region Fields and Properties
        private double _mean;
        private double _standardDeviation;
        private double _min;
        private double _max;
        private double _ProbabilityMin;
        private double _ProbabilityMax;

        #region IDistribution Properties
        public override IDistributionEnum Type => IDistributionEnum.Normal;
        [Stored(Name = "Mean", type = typeof(double))]
        public double Mean { get { return _mean; } set { _mean = value; } }
        [Stored(Name = "Standard_Deviation", type = typeof(double))]
        public double StandardDeviation { get { return _standardDeviation; } set { _standardDeviation = value; } }
        [Stored(Name = "Min", type = typeof(double))]
        public double Min { get { return _min; } set { _min = value; } }
        [Stored(Name = "Max", type = typeof(double))]
        public double Max { get { return _max; } set { _max = value; } }
        #endregion


        #endregion

        #region Constructor
        public TruncatedLogNormal()
        {
            //for reflection;
            Mean = 0;
            StandardDeviation = 1.0;
            _ProbabilityMin = 0.0;
            _ProbabilityMax = 1.0;
            Min = InverseCDF(0.0000000000001);
            Max = InverseCDF(1 - 0.0000000000001);

            addRules();
        }
        public TruncatedLogNormal(double mean, double sd, double minValue, double maxValue, int sampleSize = 1)
        {
            Mean = mean;
            StandardDeviation = sd;
            SampleSize = sampleSize;
            Min = minValue;
            Max = maxValue;
            Truncated = true;
            _ProbabilityMin = 0.0;
            _ProbabilityMax = 1.0;
            addRules();

        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(StandardDeviation),
                new Rule(() => {
                    return StandardDeviation > 0;
                },
                "Standard Deviation must be greater than 0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Mean),
                new Rule(() => {
                    return Mean > 0;
                },
                "Mean must be greater than 0.",
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
        #region IDistribution
        public override double PDF(double x)
        {
            Normal sn = new Normal();
            return sn.PDF(Math.Log(x));
        }
        public override double CDF(double x)
        {
            Normal sn = new Normal();
            return sn.CDF(Math.Log(x));
        }
        public override double InverseCDF(double p)
        {
            if (Truncated)
            {
                p = _ProbabilityMin + (p) * (_ProbabilityMax - _ProbabilityMin);
            }
            if (p <= _ProbabilityMin) return Min;
            if (p >= _ProbabilityMax) return Max;
            //Normal sn = new Normal();
            return Math.Exp(Mean + Normal.StandardNormalInverseCDF(p) * StandardDeviation);
        }

        public override bool Equals(IDistribution distribution)
        {
            if (!(distribution is TruncatedLogNormal))
            {
                return false;
            }
            return ((TruncatedLogNormal)distribution).Mean == Mean &&
                   ((TruncatedLogNormal)distribution).StandardDeviation == StandardDeviation &&
                   ((TruncatedLogNormal)distribution).SampleSize == SampleSize;
        }
        #endregion

        public override IDistribution Fit(double[] sample)
        {
            for (int i = 0; i < sample.Count(); i++)
            {
                sample[i] = Math.Log10(sample[i]);
            }
            ISampleStatistics stats = new SampleStatistics(sample);
            return new TruncatedLogNormal(stats.Mean, stats.StandardDeviation, this.Min, this.Max, stats.SampleSize);
        }
        #endregion
    }
}

