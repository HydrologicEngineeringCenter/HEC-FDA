using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics;
using Statistics.Distributions;
using HEC.MVVMFramework.Model.Messaging;
using System;

namespace HEC.FDA.Model.structures
{
    public class ValueUncertainty : ValidationErrorLogger
    {
        #region Fields
        private readonly double _PercentOfInventoryValueStandardDeviationOrMin;
        private readonly double _PercentOfInventoryValueMax;
        private readonly IDistributionEnum _DistributionType;
        private double[] _RandomNumbers;

        #endregion

        #region Properties 
        public IDistributionEnum DistributionType { get { return _DistributionType; } }
        #endregion 
        #region Constructor 
        public ValueUncertainty()
        {
            _PercentOfInventoryValueMax = 100;
            _PercentOfInventoryValueStandardDeviationOrMin = 0;
            _DistributionType = IDistributionEnum.Deterministic;
            AddRules();
        }
        public ValueUncertainty(IDistributionEnum distributionType, double percentOfInventoryValueStandardDeviationOrMin, double percentOfInventoryMax = 100)
        {
            _DistributionType = distributionType;
            _PercentOfInventoryValueStandardDeviationOrMin = percentOfInventoryValueStandardDeviationOrMin;
            _PercentOfInventoryValueMax = percentOfInventoryMax;
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
            AddSinglePropertyRule(nameof(_DistributionType), new Rule(() => _DistributionType.Equals(IDistributionEnum.Normal) || _DistributionType.Equals(IDistributionEnum.Uniform) || _DistributionType.Equals(IDistributionEnum.Deterministic) || _DistributionType.Equals(IDistributionEnum.Triangular), "Only Deterministic, Normal, Triangular, and Uniform distributions can be used for value uncertainty", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_PercentOfInventoryValueStandardDeviationOrMin), new Rule(() => _PercentOfInventoryValueStandardDeviationOrMin >= 0, "The percent of inventory value must be greaeter than or equal to zero.", ErrorLevel.Fatal));
            AddSinglePropertyRule(nameof(_PercentOfInventoryValueMax), new Rule(() => _PercentOfInventoryValueMax >= 100, "The max percent of the inventory value must be greater than or equal to 100", ErrorLevel.Fatal));
        }

        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the percent of inventory value to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value
        /// </summary>
        /// <param name="probability"></param> cumulative probability at which to return a quantile of the value uncertainty 
        /// <returns></returns>        
        public double Sample(double probability)
        {
            double centerOfDistribution = 100;
            double sampledValueOffset;
            switch (_DistributionType)
            {
                case IDistributionEnum.Normal:
                    Normal normal = new(centerOfDistribution/100, (_PercentOfInventoryValueStandardDeviationOrMin/100));
                    sampledValueOffset = normal.InverseCDF(probability);
                    break;

                case IDistributionEnum.LogNormal:
                    sampledValueOffset = Math.Exp(Normal.StandardNormalInverseCDF(probability)* (_PercentOfInventoryValueStandardDeviationOrMin/100));
                    break;
                case IDistributionEnum.Triangular:
                    Triangular triangular = new(_PercentOfInventoryValueStandardDeviationOrMin/100, centerOfDistribution/100, _PercentOfInventoryValueMax/100);
                    sampledValueOffset = triangular.InverseCDF(probability);
                    break;
                case IDistributionEnum.Uniform:
                    Uniform uniform = new(_PercentOfInventoryValueStandardDeviationOrMin/100, _PercentOfInventoryValueMax/100);
                    sampledValueOffset = uniform.InverseCDF(probability);
                    break;
                default:
                    sampledValueOffset = centerOfDistribution/100;
                    break;
            }
            if (sampledValueOffset < 0)
            {
                sampledValueOffset = 0;
            }
            return sampledValueOffset;
        }

        /// <summary>
        /// The use of this method will depend on the type of distribution. 
        /// If Normal, Triangular, or Uniform, the value returned is the percent of inventory value to add or subtract from the inventoried value
        /// If log normal, then the return value will need to be multiplied by the inventoried value
        /// </summary>
        /// <param name="iteration"></param> The iteration is used to pull the correct random number from a pre-generated list of random numbers for a compute with uncertainty
        /// <param name="computeIsDeterministic"></param> Flag compute is deterministic if not running uncertainty and the measure of central tendancy will be used for input distributions.
        /// <returns></returns>        
        public double Sample(long iteration, bool computeIsDeterministic)
        {
            double centerOfDistribution = 100;
            double sampledValueOffset;
            if (computeIsDeterministic)
            {
                sampledValueOffset = centerOfDistribution / 100;
            }
            else
            {
                if(_RandomNumbers.Length == 0)
                {
                    throw new Exception("Random numbers by iteration have not yet been generated for this compute but the software attempted to sample value uncertainty by iteration.");
                }
                if(iteration < 0 || iteration >= _RandomNumbers.Length)
                {
                    throw new Exception("The iteration at which value uncertainty was attempted to be sampled is beyond the quantity of random numbers sampled. There is a significant conflict between the stage-damage convergence criteria and the quantity of iterations being computed.");
                }
                switch (_DistributionType)
                {
                    case IDistributionEnum.Normal:
                        Normal normal = new(centerOfDistribution / 100, (_PercentOfInventoryValueStandardDeviationOrMin / 100));
                        sampledValueOffset = normal.InverseCDF(_RandomNumbers[iteration]);
                        break;

                    case IDistributionEnum.LogNormal:
                        sampledValueOffset = Math.Exp(Normal.StandardNormalInverseCDF(_RandomNumbers[iteration]) * (_PercentOfInventoryValueStandardDeviationOrMin / 100));
                        break;
                    case IDistributionEnum.Triangular:
                        Triangular triangular = new(_PercentOfInventoryValueStandardDeviationOrMin / 100, centerOfDistribution / 100, _PercentOfInventoryValueMax / 100);
                        sampledValueOffset = triangular.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    case IDistributionEnum.Uniform:
                        Uniform uniform = new(_PercentOfInventoryValueStandardDeviationOrMin / 100, _PercentOfInventoryValueMax / 100);
                        sampledValueOffset = uniform.InverseCDF(_RandomNumbers[iteration]);
                        break;
                    default:
                        sampledValueOffset = centerOfDistribution / 100;
                        break;
                }
            }
            if (sampledValueOffset < 0)
            {
                sampledValueOffset = 0;
            }
            return sampledValueOffset;
        }
        #endregion

    }
}
