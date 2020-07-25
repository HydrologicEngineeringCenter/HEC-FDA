using System;
using Functions;
using Functions.Ordinates;
using Model.Functions;


namespace Model
{
    public class Metric: IMetric
    {
        #region Properties
        public double ExceedanceTarget { get; } = 0;
        public MetricEnum Type { get; } = MetricEnum.NotSet;
        public IParameterEnum TargetFunction { get; }
        #endregion

        #region Constructors
        public Metric (MetricEnum type, double exceedanceTarget = 0)
        {
            if (type == MetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != MetricEnum.ExpectedAnnualDamage && double.IsNaN(exceedanceTarget) || double.IsInfinity(exceedanceTarget)) throw new ArgumentException("A computable target value must be provided.");
            Type = type;
            ExceedanceTarget = exceedanceTarget;
            TargetFunction = GetTargetFunction();
        }
        public Metric ()
        {
            Type = MetricEnum.Damages;
            TargetFunction = GetTargetFunction();
        }
        #endregion

        #region Methods
        private IParameterEnum GetTargetFunction()
        {
            switch (Type)
            {
                case MetricEnum.ExteriorStage:
                    return IParameterEnum.ExteriorStageFrequency;
                case MetricEnum.InteriorStage:
                    return IParameterEnum.InteriorStageFrequency;
                case MetricEnum.Damages:
                    return IParameterEnum.DamageFrequency;
                case MetricEnum.ExpectedAnnualDamage:
                    return IParameterEnum.DamageFrequency;
                default:
                    throw new InvalidOperationException("The application could not set a valid target function when calling Metric.TargetFunction()");
            }
        }
        public double Compute(IFrequencyFunction frequencyFunction, double probability)
        {
            IParameterEnum targetFunction = TargetFunction;
            if (frequencyFunction.ParameterType != targetFunction)
            {
                throw new ArgumentException(string.Format("A {0} metric cannot be computed from the provided {1} function. Provide a {2} function and try again.", Type, frequencyFunction.ParameterType, targetFunction));
            }

            IFunction sampledFreqFunc = Sampler.Sample(((FdaFunctionBase)frequencyFunction)._Function, probability);
            if (Type == MetricEnum.ExpectedAnnualDamage)
            {
                return sampledFreqFunc.TrapizoidalRiemannSum();
                //return frequencyFunction.Integrate();
            }
            else
            {
                return sampledFreqFunc.InverseF(new Constant(ExceedanceTarget)).Value();
                //return frequencyFunction.GetXFromY(ExceedanceTarget);
            }
        }
        #endregion
    }
}
