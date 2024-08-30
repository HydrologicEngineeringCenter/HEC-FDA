using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;

namespace Statistics.Distributions
{
    public class Triangular: ContinuousDistribution
    {
        private double _min;
        private double _max;
        private double _mostlikely;
        public override IDistributionEnum Type => IDistributionEnum.Triangular;
        [Stored(Name = "Min", type =typeof(double))]
        public double Min { get{return _min;} set{_min = value;} }
        [Stored(Name = "Max", type = typeof(double))]
        public double Max { get{return _max;} set{_max = value;} }
        [Stored(Name = "MostLikely", type = typeof(double))]
        public double MostLikely{ get{return _mostlikely;} set{_mostlikely = value;} }


        #region Constructor
        public Triangular()
        {
            //for reflection
            Min = 0;
            Max = 1;
            MostLikely = .5;
            SampleSize = 0;
            addRules();
        }
        public Triangular(double min, double mode, double max, int sampleSize = 1)
        {
            Min = min;
            Max = max;
            SampleSize = sampleSize;
            MostLikely = mode;
            addRules();
        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min <= Max;
                },
                "Min must not exceed Max.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min < Max;
                },
                "Min shouldnt equal Max.",
                ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min <= MostLikely;
                },
                "Min must be smaller than or equal to MostLikely.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Min),
                new Rule(() => {
                    return Min < MostLikely;
                },
                "Min shouldnt equal MostLikely.",
                ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(Max),
                new Rule(() => {
                    return MostLikely <= Max;
                },
                "MostLikely must be smaller than or equal to Max.",
                ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(Max),
                new Rule(() => {
                    return MostLikely < Max;
                },
                "MostLikely shouldnt equal to Max.",
                ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(SampleSize),
                new Rule(() => {
                    return SampleSize > 0;
                },
                "SampleSize must be greater than 0.",
                ErrorLevel.Fatal));
        }
        #endregion
        
        public override double PDF(double x){
            if (Max == Min)
            {
                if (x == MostLikely)//this is actually kinda odd.
                {
                    return 1.0;
                }
                return 0.0;
            }
            if(x<Min){
                return 0;
            }else if(x<=MostLikely){
                return 2*(x-Min)/((Max-Min)*(MostLikely-Min));
            }else if(x<=Max){
                return 2*(Max-x)/((Max-Min)*(Max-MostLikely));
            }else{
                return 0;
            }
        }
        public override double CDF(double x){
            if (Max == Min)
            {
                if (x >= MostLikely)//this is actually kinda odd.
                {
                    return 1.0;
                }
                return 0.0;
            }
            if (x<Min){
                return 0;
            }else if (x<= MostLikely){
                return Math.Pow(x-Min,2)/((Max-Min)*(MostLikely-Min));
            }else if(x <= Max){
                return 1- Math.Pow(Max-x,2)/((Max-Min)*(Max-MostLikely));
            }else{
                return 1;
            }
        }
        public override double InverseCDF(double p){
            if (Max == Min)
            {
                return MostLikely;//this is actually kinda odd.
            }
            double a = MostLikely - Min;
            double b = Max - MostLikely;
            if (p <= 0){
                return Min;
            }else if(p<a/(Max-Min)){
                return Min + Math.Sqrt(p*(Max-Min)*a);
            }else if( p<1){
                return Max - Math.Sqrt((1-p)*(Max-Min)*b);
            }else{
                return Max;
            }
        }
        public override bool Equals(IDistribution distribution){
            if (Type==distribution.Type){
                Triangular dist = (Triangular)distribution;
                if (Min == dist.Min){
                    if(Max == dist.Max){
                        if(SampleSize == dist.SampleSize){
                            if(MostLikely== dist.MostLikely){
                                return true;
                            }
                        } 
                    }
                }                
            }
            return false;
        }
        public override IDistribution Fit(double[] sample)
        {
            ISampleStatistics stats = new SampleStatistics(sample);
            return new Triangular(stats.Min, stats.Mean, stats.Max, stats.SampleSize);
        }
    }
}
