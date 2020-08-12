using System;
using Functions;
using Functions.Ordinates;
using Model.Functions;

using Utilities;

namespace Model.Metrics
{
    internal sealed class Metric: IMetric
    {
        #region Properties
        public IMetricEnum Type { get; }
        public double ExceedanceTarget { get; }  
        public IParameterEnum TargetFunction { get; }
        #endregion

        #region Constructors
        public Metric (IMetricEnum type, double exceedanceTarget = 0)
        {
            if (type == IMetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != IMetricEnum.ExpectedAnnualDamage && exceedanceTarget.IsFinite()) throw new ArgumentOutOfRangeException("A computable target value must be provided.");
            Type = type;
            ExceedanceTarget = exceedanceTarget;
            TargetFunction = SetTargetFunction(type);
        }
        #endregion

        #region Methods
        private IParameterEnum SetTargetFunction(IMetricEnum type)
        {
            switch (type)
            {
                case IMetricEnum.ExteriorStage:
                    return IParameterEnum.ExteriorStageFrequency;
                case IMetricEnum.InteriorStage:
                    return IParameterEnum.InteriorStageFrequency;
                case IMetricEnum.Damages:
                    return IParameterEnum.DamageFrequency;
                case IMetricEnum.ExpectedAnnualDamage:
                    return IParameterEnum.DamageFrequency;
                default:
                    throw new InvalidOperationException($"The specified metric type: {type.ToString()} was not successfully matched with a valid target function.");
            }
        }
        public double Compute(IFrequencyFunction frequencyFx, double p = 0.50)
        {
            if (frequencyFx.ParameterType == TargetFunction)
            {
                IFrequencyFunction fx = frequencyFx.IsConstant ? frequencyFx : frequencyFx.Sample(p);
                if (Type == IMetricEnum.ExpectedAnnualDamage) return fx.Integrate();
                else return fx.InverseF(IOrdinateFactory.Factory(ExceedanceTarget)).Value();
            }
            else throw new ArgumentException($"The {Type} metric cannot be computed with the provided {frequencyFx.ParameterType} function, a {TargetFunction} function is required.");
        }
        #endregion
    }
}
