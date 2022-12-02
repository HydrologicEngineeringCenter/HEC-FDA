using Statistics;
using Statistics.Distributions;

namespace HEC.FDA.Model.structures
{
    public class FirstFloorElevationUncertainty
    {
        #region Fields
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromOrFeetBelowInventoryValue;
        private IDistributionEnum _distributionType;
        #endregion

        #region Contructor 
        public FirstFloorElevationUncertainty()
        {
            _feetAboveInventoryValue = 0;
            _standardDeviationFromOrFeetBelowInventoryValue = 0;
            _distributionType = IDistributionEnum.Deterministic;
        }
        public FirstFloorElevationUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = 0)
        {
            _distributionType = distributionEnum;
            _standardDeviationFromOrFeetBelowInventoryValue = standardDeviationOrMinimum;
            _feetAboveInventoryValue = maximum;
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
