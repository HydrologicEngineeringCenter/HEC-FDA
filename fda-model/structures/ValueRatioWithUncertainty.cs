using Statistics;
using Statistics.Distributions;

namespace HEC.FDA.Model.structures
{
    public class ValueRatioWithUncertainty
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
            _max = 0;
            _distributionType = IDistributionEnum.Deterministic;
        }
        public ValueRatioWithUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMin, double centralTendency, double max = 0)
        {
            _distributionType = distributionEnum;
            _standardDeviationOrMin = standardDeviationOrMin;
            _centralTendency = centralTendency;
            _max = max;
        }
        public ValueRatioWithUncertainty(double deterministicValueRatio)
        {
            _centralTendency = deterministicValueRatio;
            _standardDeviationOrMin = 0;
            _max = 0;
            _distributionType = IDistributionEnum.Deterministic;
        }
        #endregion

        #region Methods
        public double Sample(double probability)
        {
            double sampledValueRatio;
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
