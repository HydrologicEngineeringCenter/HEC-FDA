using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics;
using Statistics.Distributions;

namespace HEC.FDA.Model.structures
{
    public class ValueRatioWithUncertainty: Validation 
    {
        #region Fields
        private double _standardDeviationOrMin;
        private double _centralTendency;
        private double _max;
        private IDistributionEnum _distributionType;
        #endregion

        #region Constructor 
        public ValueRatioWithUncertainty()
        {
            _standardDeviationOrMin = 0;
            _centralTendency = 0;
            _max = double.MaxValue;
            _distributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        public ValueRatioWithUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMin, double centralTendency, double max = double.MaxValue)
        {
            _distributionType = distributionEnum;
            _standardDeviationOrMin = standardDeviationOrMin;
            _centralTendency = centralTendency;
            _max = max;
            AddRules();
        }
        public ValueRatioWithUncertainty(double deterministicValueRatio)
        {
            _centralTendency = deterministicValueRatio;
            _standardDeviationOrMin = 0;
            _max = 0;
            _distributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        #endregion

        #region Methods
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_distributionType), new Rule(() => _distributionType.Equals(IDistributionEnum.Normal) || _distributionType.Equals(IDistributionEnum.Uniform) || _distributionType.Equals(IDistributionEnum.Deterministic) || _distributionType.Equals(IDistributionEnum.Triangular), "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value ratio uncertainty", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_standardDeviationOrMin), new Rule(() => _standardDeviationOrMin >= 0 && _max >= 0 && _centralTendency >= 0, "Value ratio parameter values must be non-negative", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_max), new Rule(() => _max >= _standardDeviationOrMin, "The max must be larger than the minimum", ErrorLevel.Fatal));
        }
        public double Sample(double probability, bool computeIsDeterministic)
        {
            double sampledValueRatio;
            if (computeIsDeterministic)
            {
                switch (_distributionType)
                {
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new LogNormal(_centralTendency, _standardDeviationOrMin);
                        Deterministic deterministicLogNormal = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(logNormal);
                        sampledValueRatio = deterministicLogNormal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new Uniform(_standardDeviationOrMin, _max);
                        Deterministic deterministicUniform = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(uniform);
                        sampledValueRatio = deterministicUniform.InverseCDF(probability);
                        break;
                    default:
                        sampledValueRatio = _centralTendency;
                        break;
                }

            } else
            {
                switch (_distributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new Normal(_centralTendency, _standardDeviationOrMin);
                        sampledValueRatio = normal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new LogNormal(_centralTendency, _standardDeviationOrMin);
                        sampledValueRatio = logNormal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new Triangular(_standardDeviationOrMin, _centralTendency, _max);
                        sampledValueRatio = triangular.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new Uniform(_standardDeviationOrMin, _max);
                        sampledValueRatio = uniform.InverseCDF(probability);
                        break;
                    default:
                        sampledValueRatio = _centralTendency;
                        break;
                }
            }
            //do not allow for negative value ratios
            if (sampledValueRatio < 0)
            {
                sampledValueRatio = 0;
            }
            return sampledValueRatio;
        }
        #endregion
    }
}
