﻿using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Base.Enumerations;
using System;

namespace HEC.FDA.Model.structures
{
    public class FirstFloorElevationUncertainty: Validation 
    {
        #region Fields
        private readonly double _FeetAboveInventoryValue;
        private readonly double _StandardDeviationFromOrFeetBelowInventoryValue;
        private readonly IDistributionEnum _DistributionType;
        private double[] _RandomNumbers;
        #endregion

        #region Properties 
        public IDistributionEnum DistributionType { get { return _DistributionType; } }
        #endregion

        #region Contructor 
        /// <summary>
        /// This constructor is used for deterministic first floor elevations only 
        /// </summary>
        public FirstFloorElevationUncertainty()
        {
            _FeetAboveInventoryValue = 0;
            _StandardDeviationFromOrFeetBelowInventoryValue = 0;
            _DistributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        public FirstFloorElevationUncertainty(IDistributionEnum distributionEnum, double standardDeviationOrMinimum, double maximum = double.MaxValue)
        {
            _DistributionType = distributionEnum;
            _StandardDeviationFromOrFeetBelowInventoryValue = standardDeviationOrMinimum;
            _FeetAboveInventoryValue = maximum;
            AddRules();
        }
        #endregion

        #region Methods 
        internal void GenerateRandomNumbers(int seed, long size)
        {
            Random random = new Random(seed);
            double[] randos = new double[size];
            for (int i = 0; i < size; i++)
            {
                randos[i] = random.NextDouble();
            }
            _RandomNumbers = randos;
        }
        private void AddRules()
        {
            AddSinglePropertyRule(nameof(_DistributionType), new Rule(() => _DistributionType.Equals(IDistributionEnum.Normal) || _DistributionType.Equals(IDistributionEnum.Uniform) || _DistributionType.Equals(IDistributionEnum.Deterministic) || _DistributionType.Equals(IDistributionEnum.Triangular), "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value ratio uncertainty", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_StandardDeviationFromOrFeetBelowInventoryValue), new Rule(() => _StandardDeviationFromOrFeetBelowInventoryValue >= 0 && _FeetAboveInventoryValue >= 0, "First floor elevation uncertainty parameters must be positive", ErrorLevel.Fatal));

        }
        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the feet to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value        
        /// /// </summary>
        /// <param name="probability"></param>
        /// <param name="computeIsDeterministic"></param>
        /// <returns></returns>
        public double Sample(double probability)
        {
            double centerOfDistribution = 0;
            double sampledFirstFloorElevationOffset;

                switch (_DistributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new(centerOfDistribution, _StandardDeviationFromOrFeetBelowInventoryValue);
                        sampledFirstFloorElevationOffset = normal.InverseCDF(probability);
                        break;
                    case IDistributionEnum.LogNormal:
                        sampledFirstFloorElevationOffset = Math.Exp(Normal.StandardNormalInverseCDF(probability) * _StandardDeviationFromOrFeetBelowInventoryValue);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new(-_StandardDeviationFromOrFeetBelowInventoryValue, centerOfDistribution, _FeetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = triangular.InverseCDF(probability);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(-_StandardDeviationFromOrFeetBelowInventoryValue, _FeetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = uniform.InverseCDF(probability);
                        break;
                    default:
                        sampledFirstFloorElevationOffset = centerOfDistribution;
                        break;
                }
            return sampledFirstFloorElevationOffset;
        }

        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the feet to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value        
        /// All sampling methods include a computeIsDeterministic argument that bypasses the iteration number for the retrieval of the deterministic representation of the variable 
        /// </summary>
        /// <param name="iteration"></param> If this method is called during a full compute with uncertainty, random numbers need to have been previously generated, and the correct random number will be pulled for the given iteration number
        /// <param name="computeIsDeterministic"></param> If the method is instead called during a deterministic compute, the iteration number will be bypassed and the deterministic representation will be returned
        /// <returns></returns> the first floor elevation offset is returned as a double
        public double Sample(long iteration, bool computeIsDeterministic)
        {

            double centerOfDistribution = 0;
            double sampledFirstFloorElevationOffset;
            if (computeIsDeterministic)
            {
                if (_DistributionType == IDistributionEnum.LogNormal)
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
                if (_RandomNumbers.Length == 0)
                {
                    throw new Exception("Random numbers by iteration have not yet been generated for this compute but the software attempted to sample value uncertainty by iteration.");
                }
                if (iteration < 0 || iteration >= _RandomNumbers.Length)
                {
                    throw new Exception("The iteration at which value uncertainty was attempted to be sampled is beyond the quantity of random numbers sampled. There is a significant conflict between the stage-damage convergence criteria and the quantity of iterations being computed.");
                }
                switch (_DistributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new(centerOfDistribution, _StandardDeviationFromOrFeetBelowInventoryValue);
                        sampledFirstFloorElevationOffset = normal.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.LogNormal:
                        sampledFirstFloorElevationOffset = Math.Exp(Normal.StandardNormalInverseCDF(_RandomNumbers[iteration]) * _StandardDeviationFromOrFeetBelowInventoryValue);
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new(-_StandardDeviationFromOrFeetBelowInventoryValue, centerOfDistribution, _FeetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = triangular.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(-_StandardDeviationFromOrFeetBelowInventoryValue, _FeetAboveInventoryValue);
                        sampledFirstFloorElevationOffset = uniform.InverseCDF(_RandomNumbers[iteration]);
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
