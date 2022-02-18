using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;

namespace Statistics.Distributions
{
    public class Uniform: ContinuousDistribution
    {
        #region Fields and Properties
        private double _min;
        private double _max;

        #region IDistribution Properties
        public override IDistributionEnum Type => IDistributionEnum.Uniform;
        [Stored(Name = "Min", type =typeof(double))]
        public double Min { get{return _min;} set{_min = value;} }
        [Stored(Name = "Max", type = typeof(double))]
        public double Max { get{return _max;} set{_max = value;} }
        #endregion

        #endregion

        #region Constructor
        public Uniform()
        {
            //for reflection
            Min = 0;
            Max = 1;
            SampleSize = 0;
            addRules();
        }
        public Uniform(double min, double max, int sampleSize = int.MaxValue)
        {
            Min = min;
            Max = max;
            SampleSize = sampleSize;
            addRules();
        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min <= Max;
                },
                "Min must be smaller or equal to Max.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min < Max;
                },
                "Min shouldnt equal Max.",
                ErrorLevel.Minor));
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
        public override double PDF(double x){
            if (Max == Min)
            {
                if (x == Min)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
            if(x<Min){
                return 0;
            }else if(x<= Max){
                return 1/(Max-Min);
            }else{
                return 0;
            }
        }
        public override double CDF(double x){
            if (Max == Min)
            {
                if (x >= Min)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
            if (x<Min){
                return 0;
            }else if(x<= Max){
                return (x-Min)/(Max-Min);
            }else{
                return 0;
            }
        }
        public override double InverseCDF(double p){
            return Min +((Max-Min)*p);
        }
        public override string Print(bool round = false) {
           return "Uniform(range: {Min:"+Min+", Max:"+Max+"})";
        }
        public override string Requirements(bool printNotes) => RequiredParameterization(printNotes);
        public override bool Equals(IDistribution distribution){
            if (Type==distribution.Type){
                Uniform dist = (Uniform)distribution;
                if (Min == dist.Min)
                {
                    if(Max == dist.Max){
                        if(SampleSize == dist.SampleSize){
                            return true;
                        } 
                    }
                }                
            }
            return false;
        }
        #endregion

        internal static string Print(double Min, double Max) => $"Uniform(range: {Min} to {Max})";
        internal static string RequiredParameterization(bool printNotes = false) => $"The Uniform distribution requires the following parameterization: {Parameterization()}.";
        internal static string Parameterization() => $"Uniform()";

        public override IDistribution Fit(double[] sample)
        {
            ISampleStatistics stats = new SampleStatistics(sample);
            return new Uniform(stats.Range.Min, stats.Range.Max, stats.SampleSize);
        }
        #endregion
    }
}
