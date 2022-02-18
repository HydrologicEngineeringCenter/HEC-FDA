using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Linq;

namespace Statistics.Distributions
{
    public class LogNormal : ContinuousDistribution
    {      
        #region Fields and Properties
        private double _mean;
        private double _standardDeviation;


        #region IDistribution Properties
        public override IDistributionEnum Type => IDistributionEnum.Normal;
        [Stored(Name = "Mean", type = typeof(double))]
         public double Mean { get{return _mean;} set{_mean = value;} }
        [Stored(Name = "Standard_Deviation", type = typeof(double))]
        public double StandardDeviation { get{return _standardDeviation;} set{_standardDeviation = value;} }
        #endregion
        #endregion

        #region Constructor
        public LogNormal()
        {
            //for reflection;
            Mean = 0;
            StandardDeviation = 1.0;
            addRules();
        }
        public LogNormal(double mean, double sd, int sampleSize = int.MaxValue)
        {
            Mean = mean;
            StandardDeviation = sd;
            SampleSize = sampleSize;
            addRules();
        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(StandardDeviation),
                new Rule(() => {
                    return StandardDeviation >= 0;
                },
                "Standard Deviation must be greater than or equal to 0.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(StandardDeviation),
                new Rule(() => {
                    return StandardDeviation > 0;
                },
                "Standard Deviation shouldnt equal 0.",
                ErrorLevel.Minor));
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
        public override double PDF(double x){
            Normal sn = new Normal();
            return sn.PDF(Math.Log(x));
        }
        public override double CDF(double x){
            Normal sn = new Normal();
            return sn.CDF(Math.Log(x));
        }
        public override double InverseCDF(double p)
        {
            if (p <= 0) return 0;
            if (p >= 1) return double.PositiveInfinity;
            Normal sn = new Normal();
            return Math.Exp(Mean+Normal.StandardNormalInverseCDF(p)*StandardDeviation);
        }
        public override string Print(bool round = false) => round ? Print(Mean, StandardDeviation, SampleSize) : $"LogNormal(mean: {Mean}, sd: {StandardDeviation}, sample size: {SampleSize})";
        public override string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public override bool Equals(IDistribution distribution) => string.Compare(Print(), distribution.Print()) == 0;
        #endregion

        internal static string Print(double mean, double sd, int n) => $"LogNormal(mean: {mean}, sd: {sd}, sample size: {n})";
        public static string RequiredParameterization(bool printNotes)
        {
            string msg = $"The Log Normal distribution requires the following parameterization: {Parameterization()}.";
            if (printNotes) msg += $" {RequirementNotes()}";
            return msg;
        }
        private static string Parameterization() => $"LogNormal(mean: [{double.MinValue}, {double.MaxValue}], sd: [0, {double.MaxValue}], sample size: > 0)";
        private static string RequirementNotes() => $"The parameters should reflect the log-scale random number values.";

        public override IDistribution Fit(double[] sample)
        {
            for (int i = 0; i < sample.Count(); i++)
            {
                sample[i] = Math.Log10(sample[i]);
            }
            ISampleStatistics stats = new SampleStatistics(sample);
            return new LogNormal(stats.Mean, stats.StandardDeviation, stats.SampleSize);
        }   
        #endregion
    }
}
