using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;

namespace Statistics.Distributions
{
    internal class TruncatedNormal : Normal
    {
        //TODO: Sample
        #region Fields and Propertiesj
        private double _mean;
        private double _standardDeviation;
        private double _min;
        private double _max;
        internal IRange<double> _ProbabilityRange;

        #region IDistribution Properties
        public override IDistributionEnum Type => IDistributionEnum.TruncatedNormal;
        [Stored(Name = "Mean", type = typeof(double))]
        public new double Mean { get { return _mean; } set { _mean = value; } }
        [Stored(Name = "Standard_Deviation", type = typeof(double))]
        public new double StandardDeviation { get { return _standardDeviation; } set { _standardDeviation = value; } }
        [Stored(Name = "Min", type = typeof(double))]
        public double Min { get { return _min; } set { _min = value; } }
        [Stored(Name = "Max", type = typeof(double))]
        public double Max { get { return _max; } set { _max = value; } }
        #endregion

        #endregion

        #region Constructor
        public TruncatedNormal()
        {
            //for reflection;
            Mean = 0;
            StandardDeviation = 1.0;

            //if (!Validation.NormalValidator.IsConstructable(Mean, StandardDeviation, SampleSize, out string msg)) throw new Utilities.InvalidConstructorArgumentsException(msg);
            _ProbabilityRange = IRangeFactory.Factory(0.0, 1.0);
            Min = InverseCDF(0.0000000000001);
            Max = InverseCDF(1 - 0.0000000000001);
            //State = Validate(new Validation.NormalValidator(), out IEnumerable<Utilities.IMessage> msgs);
            //Messages = msgs;
            addRules();
        }
        public TruncatedNormal(double mean, double sd, double minValue, double maxValue, int sampleSize = int.MaxValue)
        {
            Mean = mean;
            StandardDeviation = sd;
            SampleSize = sampleSize;
            Min = minValue;
            Max = maxValue;
            Truncated = true;
            //if (!Validation.NormalValidator.IsConstructable(Mean, StandardDeviation, SampleSize, out string msg)) throw new Utilities.InvalidConstructorArgumentsException(msg);
            _ProbabilityRange = FiniteRange(Min, Max);
            // State = Validate(new Validation.NormalValidator(), out IEnumerable<Utilities.IMessage> msgs);
            //Messages = msgs;
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
            AddSinglePropertyRule(nameof(SampleSize),
                new Rule(() => {
                    return SampleSize > 0;
                },
                "SampleSize must be greater than 0.",
                ErrorLevel.Fatal));
        }
        #endregion

        #region Functions
        private IRange<double> FiniteRange(double min, double max)
        {
            double pmin = 0;
            double pmax = 1 - pmin;
            if (min.IsFinite() || max.IsFinite())
            {
                pmin = CDF(min);
                pmax = CDF(max);
            }
            return IRangeFactory.Factory(pmin, pmax);
        }

        #region IDistribution Functions
        public override double PDF(double x)
        {
            return Math.Exp(-(x - Mean) * (x - Mean) / (2.0 * StandardDeviation * StandardDeviation)) / (Math.Sqrt(2.0 * Math.PI) * StandardDeviation);
        }
        public override double CDF(double x)
        {
            if (x == Double.PositiveInfinity)
            {
                return 1.0;
            }
            else if (x == Double.NegativeInfinity)
            {
                return 0.0;
            }
            else if (x >= Mean)
            {
                return 0.5 * (1.0 + SpecialFunctions.regIncompleteGamma(0.5, (x - Mean) * (x - Mean) / (2.0 * StandardDeviation * StandardDeviation)));
            }
            else
            {
                return 0.5 * (1.0 - SpecialFunctions.regIncompleteGamma(0.5, (x - Mean) * (x - Mean) / (2.0 * StandardDeviation * StandardDeviation)));
            }
        }
        public override double InverseCDF(double p)
        {
            if (Truncated)
            {
                //https://en.wikipedia.org/wiki/Truncated_normal_distribution
                p = _ProbabilityRange.Min + (p) * (_ProbabilityRange.Max - _ProbabilityRange.Min);
            }
            if (p <= _ProbabilityRange.Min) return Min;
            if (p >= _ProbabilityRange.Max) return Max;
            return Mean + Normal.StandardNormalInverseCDF(p)*StandardDeviation;
            //return invCDFNewton(p, Mean, 1e-10, 100);
        }


        public override string Print(bool round = false) => round ? Print(Mean, StandardDeviation, SampleSize) : $"Normal(mean: {Mean}, sd: {StandardDeviation}, sample size: {SampleSize})";
        public override string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public override bool Equals(IDistribution distribution)
        {
            if (Type == distribution.Type)
            {
                TruncatedNormal dist = (TruncatedNormal)distribution;
                if (SampleSize == dist.SampleSize)
                {
                    if (Mean == dist.Mean)
                    {
                        if (StandardDeviation == dist.StandardDeviation)
                        {
                            if (Truncated)
                            {
                                if (Truncated == dist.Truncated)
                                {
                                    if (Min == dist.Min)
                                    {
                                        if (Max == dist.Max)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        internal new static string Print(double mean, double sd, int n) => $"TruncatedNormal(mean: {mean.Print()}, sd: {sd.Print()}, sample size: {n.Print()})";
        public new static string RequiredParameterization(bool printNotes = false) => $"The TruncatedNormal distribution requires the following parameterization: {Parameterization()}.";
        internal new static string Parameterization() => $"TruncatedNormal(mean: [{double.MinValue.Print()}, {double.MaxValue.Print()}], sd: [{double.MinValue.Print()}, {double.MaxValue.Print()}], sample size: > 0)";
        public override IDistribution Fit(double[] sample)
        {
            ISampleStatistics stats = new SampleStatistics(sample);
            return new TruncatedNormal(stats.Mean, stats.StandardDeviation, this.Min, this.Max, stats.SampleSize);
        }

        /**
 * @param p = probability between 0 and 1
 * @return a value corresponding to the inverse of 
 *         cumulative probability distribution
 *         found using the Newton method
 *         This method is not guarantee to converge
 */
        private double invCDFNewton(double p, double valGuess, double tolP, int maxIter)
        {
            double x = valGuess;
            double testY = CDF(x) - p;
            for (int i = 0; i < maxIter; i++)
            {

                double dfdx = PDF(x);
                if (Double.MinValue > Math.Abs(dfdx))
                {
                    //this is a minimum or maximum. Can't get any closer
                    return x;
                }

                x = x - testY / dfdx;
                testY = CDF(x) - p;
                if (Math.Abs(testY) <= tolP)
                {
                    return x;
                }
            }
            return Double.NaN;
        }
        #endregion
    }
}