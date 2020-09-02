using System;
using Functions;
using Functions.Ordinates;
using Model.Functions;


namespace Model
{
    internal sealed class Metric: IMetric
    {
        #region Properties
        public double Ordinate { get; } = 0;
        public IMetricEnum Type { get; } = IMetricEnum.NotSet;
        public IParameterEnum TargetFunction { get; }
        #endregion

        #region Constructors
        internal Metric (IMetricEnum type, double exceedanceTarget = 0)
        {
            if (type == IMetricEnum.NotSet) throw new ArgumentException("The desired type of metric must be set.");
            if (type != IMetricEnum.ExpectedAnnualDamage && double.IsNaN(exceedanceTarget) || double.IsInfinity(exceedanceTarget)) throw new ArgumentException("A computable target value must be provided.");
            Type = type;
            Ordinate = exceedanceTarget;
            TargetFunction = GetTargetFunction();
        }
        internal Metric ()
        {
            Type = IMetricEnum.ExpectedAnnualDamage;
            TargetFunction = GetTargetFunction();
        }
        #endregion

        #region Methods
        private IParameterEnum GetTargetFunction()
        {
            switch (Type)
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
            if (Type == IMetricEnum.ExpectedAnnualDamage)
            {
                return sampledFreqFunc.TrapizoidalRiemannSum();
                //return frequencyFunction.Integrate();
            }
            else
            {
                return sampledFreqFunc.InverseF(new Constant(Ordinate)).Value();
                //return frequencyFunction.GetXFromY(ExceedanceTarget);
            }
        }
        #endregion
    }
}
