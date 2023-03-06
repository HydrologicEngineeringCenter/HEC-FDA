using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;

namespace Statistics.Distributions
{
    public class Deterministic : ContinuousDistribution
    {
        #region IDistribution Properties
        public override IDistributionEnum Type => IDistributionEnum.Deterministic;
        #endregion
        [Stored(Name = "Value", type = typeof(double))]
        public double Value { get; set; }
        #region constructor
        public Deterministic(double x)
        {
            Value = x;
            addRules();
        }
        public Deterministic()
        {
            addRules();
        }
        private void addRules()
        {
            //AddSinglePropertyRule(nameof(Value),
            //    new Rule(() => {
            //        return Value!=null;
            //    },
            //    "On Deterministic Distributions Value cannot be Null.",
            //    ErrorLevel.Fatal));
        }
        #endregion

        #region functions
        public override double CDF(double x)
        {
            if (x >= Value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(IDistribution distribution)
        {
            if (Type==distribution.Type){
                Deterministic dist = (Deterministic)distribution;
                if (Value == dist.Value)
                {
                    return true;
                }                
            }
            return false;
        }

        public override double InverseCDF(double p)
        {
            return Value;
        }

        public override double PDF(double x)
        {
            if (x == Value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override string Print(bool round = false)
        {
            return $"The Value is {Value}";
        }

        public override string Requirements(bool printNotes)
        {
            return "A value is required";
        }
        public override IDistribution Fit(double[] data)
        {
            ISampleStatistics stats = new SampleStatistics(data);
            return new Deterministic(stats.Mean);
        }
        #endregion
    }
}