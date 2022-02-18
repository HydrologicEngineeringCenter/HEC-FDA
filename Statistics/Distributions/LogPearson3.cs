using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Linq;

namespace Statistics.Distributions
{
    public class LogPearson3: ContinuousDistribution
    {
        private double _twoDividedBySkew = 0;
        private double _skewDividedBySix = 0;
        private double _skew;

        #region Properties
        public override IDistributionEnum Type => IDistributionEnum.LogPearsonIII;
        [Stored(Name = "Mean", type = typeof(double))]
        public double Mean { get; set; }
        [Stored(Name = "St_Dev", type = typeof(double))]
        public double StandardDeviation { get; set; }
        [Stored(Name = "Skew", type = typeof(double))]
        public double Skewness {
            get
            {
                return _skew;
            }
            set
            {
                _skew = value;
                if(_skew != 0)
                {
                    _skewDividedBySix = _skew / 6;
                    _twoDividedBySkew = 2 / _skew;
                }
                else
                {
                    _skewDividedBySix = 0;
                    _twoDividedBySkew = 0;
                }
            }
        }
        #endregion

        #region Constructor
        public LogPearson3()
        {
            //for reflection;
            Mean = 0.1;
            StandardDeviation = .01;
            Skewness = .01;
            SampleSize = 1;
            addRules();
            
        }
        public LogPearson3(double mean, double standardDeviation, double skew, int sampleSize = int.MaxValue)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            Skewness = skew;
            SampleSize = sampleSize;
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

        #region IDistribution Functions
        public override double PDF(double x)
        {
            PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
            return d.PDF(Math.Log10(x))/x/Math.Log(10);        
        }
        public override double CDF(double x)
        {
            if (x > 0)
            {
                PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
                return d.CDF(Math.Log10(x));
            }
            else return 0;
        }
        public override double InverseCDF(double p)
        {

            if (p <= 0)
            {
                p = 0.000000000001;
            }
            if (p >= 1)
            {
                p = 0.999999999999;
            };

            //PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
            //return Math.Pow(10, d.InverseCDF(p));
            if (Skewness == 0)
            {
                //Normal zeroSkewNorm = new Normal(Mean, StandardDeviation);
                double logflow = (Normal.StandardNormalInverseCDF(p)*StandardDeviation) + Mean;
                return Math.Pow(10, logflow);
            }
            else
            {
                //Normal sn = new Normal();
                double z = Normal.StandardNormalInverseCDF(p);
                double whfactor = (z - _skewDividedBySix) * Skewness / 6.0 + 1;
                double k = (_twoDividedBySkew) * ((whfactor*whfactor*whfactor) - 1); //pemdas says you cant substitute for the divide in that other instance... so dont do it!
                double logflow = Mean + (k * StandardDeviation);
                return Math.Pow(10, logflow);
            }
        }
        public static double FastInverseCDF(double mean, double sd, double skew, double skewdividedbysix, double twodividedbyskew, double p)
        {

            if (p <= 0)
            {
                p = 0.000000000001;
            }
            if (p >= 1)
            {
                p = 0.999999999999;
            };

            //PearsonIII d = new PearsonIII(Mean, StandardDeviation, Skewness, SampleSize);
            //return Math.Pow(10, d.InverseCDF(p));
            if (skew == 0)
            {
                //Normal zeroSkewNorm = new Normal(Mean, StandardDeviation);
                double logflow = (Normal.StandardNormalInverseCDF(p) * sd) + mean;
                return Math.Pow(10, logflow);
            }
            else
            {

                //Normal sn = new Normal();
                double z = Normal.StandardNormalInverseCDF(p);
                double whfactor = (z - skewdividedbysix) * skew / 6.0 + 1;
                double k = (twodividedbyskew) * ((whfactor * whfactor * whfactor) - 1); //pemdas says you cant substitute for the divide in that other instance... so dont do it!
                double logflow = mean + (k * sd);
                return Math.Pow(10, logflow);
            }
        }
        public override string Print(bool round = false) => round ? Print(Mean, StandardDeviation, Skewness, SampleSize) : $"log PearsonIII(mean: {Mean}, sd: {StandardDeviation}, skew: {Skewness}, sample size: {SampleSize})";
        public override string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public override bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print(), StringComparison.InvariantCultureIgnoreCase) == 0 ? true : false;
        #endregion

        internal static string Print(double mean, double sd, double skew, int n) => $"log PearsonIII(mean: {mean}, sd: {sd}, skew: {skew}, sample size: {n})";
        internal static string RequiredParameterization(bool printNotes = true)
        {
            string s = $"The log PearsonIII distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) s += RequirementNotes();
            return s;
        }
        internal static string Parameterization() => $"log PearsonIII(mean: (0, {Math.Log10(double.MaxValue)}], sd: (0, {Math.Log10(double.MaxValue)}], skew: [{(Math.Log10(double.MaxValue) * -1)}, {Math.Log10(double.MaxValue)}], sample size: > 0)";
        internal static string RequirementNotes() => $"The distribution parameters are computed from log base 10 random numbers (e.g. the log Pearson III distribution is a distribution of log base 10 Pearson III distributed random values). Therefore the mean and standard deviation parameters must be positive finite numbers and while a large range of numbers are acceptable a relative small rate will produce meaningful results.";

        public override IDistribution Fit(double[] sample)
        {
            for(int i = 0; i<sample.Count(); i++) {
                sample[i] = Math.Log10(sample[i]);
            }
            ISampleStatistics stats = new SampleStatistics(sample);
            return new LogPearson3(stats.Mean, stats.StandardDeviation, stats.Skewness, stats.SampleSize);
        }
        #endregion
    }
}
