using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.Model.structures
{
    public class FirstFloorElevationUncertainty: Validation 
    {
        #region Fields
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromOrFeetBelowInventoryValue;
        private IDistributionEnum _distributionType;
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
        public double Sample(double inventoriedFirstFloorElevation, double probability, bool computeIsDeterministic)
        {
            double sampledFirstFloorElevation;
            if (computeIsDeterministic)
            {
                sampledFirstFloorElevation = inventoriedFirstFloorElevation;
            } else
            {
                switch (_distributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new Normal(inventoriedFirstFloorElevation, _standardDeviationFromOrFeetBelowInventoryValue);
                        sampledFirstFloorElevation = normal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.LogNormal:
                        LogNormal logNormal = new LogNormal(inventoriedFirstFloorElevation, _standardDeviationFromOrFeetBelowInventoryValue);
                        sampledFirstFloorElevation = logNormal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new Triangular(inventoriedFirstFloorElevation - _standardDeviationFromOrFeetBelowInventoryValue, inventoriedFirstFloorElevation, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                        sampledFirstFloorElevation = triangular.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new Uniform(inventoriedFirstFloorElevation - _standardDeviationFromOrFeetBelowInventoryValue, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                        sampledFirstFloorElevation = uniform.InverseCDF(probability);
                        break;
                    default:
                        sampledFirstFloorElevation = inventoriedFirstFloorElevation;
                        break;
                }
            }
            return sampledFirstFloorElevation;
        }
        #endregion
    }
}
