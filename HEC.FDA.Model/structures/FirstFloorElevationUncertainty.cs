using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Base.Enumerations;
using System;

namespace HEC.FDA.Model.structures
{
    public class FirstFloorElevationUncertainty: Validation 
    {
        #region Fields
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromOrFeetBelowInventoryValue;
        private IDistributionEnum _distributionType;
        #endregion

        #region Properties 
        public IDistributionEnum DistributionType { get { return _distributionType; } }
        #endregion

        #region Contructor 
        /// <summary>
        /// This constructor is used for deterministic first floor elevations only 
        /// </summary>
        public FirstFloorElevationUncertainty()
        {
            _feetAboveInventoryValue = 0;
            _standardDeviationFromOrFeetBelowInventoryValue = 0;
            _distributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        public FirstFloorElevationUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = double.MaxValue)
        {
            _distributionType = distributionEnum;
            _standardDeviationFromOrFeetBelowInventoryValue = standardDeviationOrMinimum;
            _feetAboveInventoryValue = maximum;
            AddRules();
        }
        #endregion

        #region Methods 
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_distributionType), new Rule(() => _distributionType.Equals(IDistributionEnum.Normal) || _distributionType.Equals(IDistributionEnum.Uniform) || _distributionType.Equals(IDistributionEnum.Deterministic) || _distributionType.Equals(IDistributionEnum.Triangular), "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value ratio uncertainty", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_standardDeviationFromOrFeetBelowInventoryValue), new Rule(() => _standardDeviationFromOrFeetBelowInventoryValue >= 0 && _feetAboveInventoryValue >= 0, "First floor elevation uncertainty parameters must be positive", ErrorLevel.Fatal));

        }
        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the feet to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value        
        /// /// </summary>
        /// <param name="probability"></param>
        /// <param name="computeIsDeterministic"></param>
        /// <returns></returns>
        public double Sample(double probability, bool computeIsDeterministic)
        {
            double centerOfDistribution = 0;
            double sampledFirstFloorElevationOffset;
            if (computeIsDeterministic)
            {
                if (_distributionType == IDistributionEnum.LogNormal)
                {
                    sampledFirstFloorElevationOffset = 1;
                }
                else
                {
                    sampledFirstFloorElevationOffset = centerOfDistribution;

                }
            }
            else
            {
                switch (_distributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new Normal(centerOfDistribution, _standardDeviationFromOrFeetBelowInventoryValue);
                        sampledFirstFloorElevationOffset = normal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.LogNormal:
                        sampledFirstFloorElevationOffset = Math.Exp(Normal.StandardNormalInverseCDF(probability) * _standardDeviationFromOrFeetBelowInventoryValue);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new Triangular(-_standardDeviationFromOrFeetBelowInventoryValue, centerOfDistribution, _feetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = triangular.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new Uniform(-_standardDeviationFromOrFeetBelowInventoryValue, _feetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = uniform.InverseCDF(probability);
                        break;
                    default:
                        sampledFirstFloorElevationOffset = centerOfDistribution;
                        break;
                }
            }
            return sampledFirstFloorElevationOffset;
        }
        #endregion
    }
}
