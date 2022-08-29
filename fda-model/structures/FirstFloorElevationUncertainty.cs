using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Distributions;

namespace structures
{
    public class FirstFloorElevationUncertainty
{
        #region Fields
        private double _feetBelowInventoryValue;
        private double _feetAboveInventoryValue;
        private double _standardDeviationFromInventoryValue;
        private IDistributionEnum _distributionType;
        #endregion

        #region Contructor 
        public FirstFloorElevationUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = 0)
        {
            _distributionType = distributionEnum;
            if(_distributionType == IDistributionEnum.Normal || _distributionType == IDistributionEnum.LogNormal)
            {
                _standardDeviationFromInventoryValue = standardDeviationOrMinimum;
            }
            if(_distributionType == IDistributionEnum.Triangular || _distributionType == IDistributionEnum.Uniform)
            {
                _feetBelowInventoryValue = standardDeviationOrMinimum;
                _feetAboveInventoryValue = maximum;
            }
        }

        public double SampleFirstFloorElevation(double inventoriedFirstFloorElevation, double randomProbability)
        {
            double sampledFirstFloorElevation;
            switch(_distributionType)
            {
                case IDistributionEnum.Normal:
                    Normal normal = new Normal(inventoriedFirstFloorElevation, _standardDeviationFromInventoryValue);
                    sampledFirstFloorElevation = normal.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.LogNormal:
                    LogNormal logNormal = new LogNormal(inventoriedFirstFloorElevation, _standardDeviationFromInventoryValue);
                    sampledFirstFloorElevation = logNormal.InverseCDF(randomProbability);
                    break ;
                case IDistributionEnum.Triangular:
                    Triangular triangular = new Triangular(inventoriedFirstFloorElevation - _feetBelowInventoryValue, inventoriedFirstFloorElevation, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                    sampledFirstFloorElevation = triangular.InverseCDF(randomProbability);
                    break;
                case IDistributionEnum.Uniform: 
                    Uniform uniform = new Uniform(inventoriedFirstFloorElevation - _feetBelowInventoryValue, inventoriedFirstFloorElevation + _feetAboveInventoryValue);
                    sampledFirstFloorElevation = uniform.InverseCDF(randomProbability);
                    break;
                default:
                    sampledFirstFloorElevation = inventoriedFirstFloorElevation;
                    break;
            }
            return sampledFirstFloorElevation;
        }
        #endregion
    }
}
