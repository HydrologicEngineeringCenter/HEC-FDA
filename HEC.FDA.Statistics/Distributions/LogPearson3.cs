using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Linq;

namespace Statistics.Distributions
{
    public class LogPearson3: ContinuousDistribution
    {
        public static double[] _RequiredExceedanceProbabilitiesForBootstrapping = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 
            0.65000, 0.60000,0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000,
            0.25000, 0.24000,0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 
            0.14500, 0.14000,0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000,
            0.06500, 0.06000,0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 
            0.04500, 0.04400,0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000,
            0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 
            0.01650, 0.01600,0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 
            0.00850, 0.00800,0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195,
            0.00190, 0.00185,0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 
            0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 
            0.00030, 0.00025,0.00020, 0.00015, 0.00010 };

        private double _twoDividedBySkew = 0;
        private double _skewDividedBySix = 0;
        private double _skew;
        private bool _successfullyLoggedData = true;

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
        [Stored(Name = "IsNull", type = typeof(bool))]
        public bool IsNull { get; set;  }
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
            IsNull = true;
            
        }
        /// <summary>
        /// LP# Dist
        /// Default sample size is 1. 
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <param name="skew"></param>
        /// <param name="sampleSize"></param>
        public LogPearson3(double mean, double standardDeviation, double skew, int sampleSize = 1)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            Skewness = skew;
            SampleSize = sampleSize;
            addRules();
            IsNull = false;
            
        }
        private LogPearson3(bool successfullyLoggedData)
        {
            //for reflection;
            Mean = 0.1;
            StandardDeviation = .01;
            Skewness = .01;
            SampleSize = 1;
            _successfullyLoggedData = successfullyLoggedData;
            addRules();
            IsNull = true;

        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(_successfullyLoggedData),
                new Rule(() =>
                {
                    return _successfullyLoggedData == true;
                },
                "Input flow values cannot be negative",
                ErrorLevel.Severe));
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
        public override bool Equals(IDistribution distribution)
        {
            if (!(distribution is LogPearson3))
            {
                return false;
            }
            return ((LogPearson3)distribution).Mean == Mean &&
                   ((LogPearson3)distribution).StandardDeviation == StandardDeviation &&
                   ((LogPearson3)distribution).Skewness == Skewness &&
                   ((LogPearson3)distribution).SampleSize == SampleSize;
        }
        #endregion

        internal static string Print(double mean, double sd, double skew, Int64 n) => $"log PearsonIII(mean: {mean}, sd: {sd}, skew: {skew}, sample size: {n})";
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

            for(int i = 0; i<sample.Count(); i++) 
            {   
                if (sample[i] <= 0)
                {
                    return new LogPearson3(successfullyLoggedData: false);
                }
                sample[i] = Math.Log10(sample[i]);
            }
            ISampleStatistics stats = new SampleStatistics(sample);
            return new LogPearson3(stats.Mean, stats.StandardDeviation, stats.Skewness, stats.SampleSize);
        }
        #endregion
    }
}
