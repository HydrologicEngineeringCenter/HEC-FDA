using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// An interface for the <see cref="IConditionLocationYearRealizationSummary"/>s produced during the compute of an <see cref="IConditionLocation"/>.
    /// </summary>
    public interface IConditionLocationRealization
    {
        /// <summary>
        /// A <see cref="IDictionary{TKey, TValue}"/> of <see cref="int"/> year keys and those years <see cref="IConditionLocationYearRealizationSummary"/> values.
        /// </summary>
        IReadOnlyDictionary<int, IConditionLocationYearRealizationSummary> Years { get; }
        /// <summary>
        /// Creates an <see cref="IFdaFunction"/> with each <see cref="Years"/> <see cref="IMetric"/> values, produced by the <see cref="IMetric.Compute(IFrequencyFunction, double)"/> method during the <see cref="IConditionLocationYearSummary"/>.
        /// </summary>
        /// <param name="metric"> The <see cref="IMetric"/> to be included in the <see cref="IFdaFunction"/>. </param>
        /// <param name="interpolation"> The method of interpolation between <see cref="Years"/>' <see cref="IMetric.Compute(IFrequencyFunction, double)"/> values in the resulting <see cref="IFdaFunction"/>. <see cref="InterpolationEnum.Linear"/> is used by default (<seealso cref="ICoordinatesFunction.Interpolator"/>). </param>
        /// <returns> An <see cref="IFdaFunction"/> with years in the x axis and <see cref="IMetric.Compute(IFrequencyFunction, double)"/> values on the y axis. </returns>
        IFdaFunction GetMetricYearFunction(IMetric metric, InterpolationEnum interpolation = InterpolationEnum.Linear);
        /// <summary>
        /// Produces an <see cref="IFdaFunction"/> using the <see cref="GetMetricYearFunction(IMetric, InterpolationEnum)"/> function, with discounted <see cref="IParameterEnum.EAD"/> metric values.
        /// </summary>
        /// <param name="discountRate"> The interest rate: [0, 1] used to discount <see cref="IParameterEnum.EAD"/> values. </param>
        /// <param name="interpolation"> The interpolation scheme used to compute values between <see cref="Years"/> in the <see cref="IFdaFunction"/>. <see cref="InterpolationEnum.Linear"/> is used by default (<seealso cref="ICoordinatesFunction.Interpolator"/>). </param>
        /// <param name="metric"> The <see cref="IParameterEnum.EAD"/> <see cref="IMetric"/> to be computed. </param>
        /// <returns> An <see cref="IFdaFunction"/> discounted <see cref="IParameterEnum.EAD"/> by analysis year. </returns>
        IFdaFunction GetEquivalentAnnualDamagesFunction(double discountRate, InterpolationEnum interpolation = InterpolationEnum.Linear, IMetric metric = null);
        /// <summary>
        /// Produces an <see cref="IFdaFunction"/> using the <see cref="GetMetricYearFunction(IMetric, InterpolationEnum)"/> function, with discounted <see cref="IParameterEnum.EAD"/> metric values.
        /// </summary>
        /// <param name="discountRate"> The interest rate: [0, 1] used to discount <see cref="IParameterEnum.EAD"/> values. </param>
        /// <param name="EadFx"> The <see cref="IParameterEnum.YearEAD"/> function containing <see cref="IParameterEnum.EAD"/> values by analysis year, which will be discounted to produce <see cref="IParameterEnum.EquivalentAnnualDamages"/>. </param>
        /// <returns> An <see cref="IFdaFunction"/> discounted <see cref="IParameterEnum.EAD"/> by analysis year. </returns>
        IFdaFunction GetEquivalentAnnualDamagesFunction(IFdaFunction EadFx, double discountRate);
        /// <summary>
        /// Sums together discounted <see cref="IParameterEnum.EAD"/> values across the years in the <see cref="IConditionLocationRealization"/> and to produce an <see cref="IParameterEnum.EquivalentAnnualDamages"/> estimate.
        /// </summary>
        /// <param name="EquivalentAnnualDamagesFx"> A function containing discounted <see cref="IParameterEnum.EAD"/> values. </param>
        /// <returns> A estimate of <see cref="IParameterEnum.EquivalentAnnualDamages"/>. </returns>
        double GetEquivalentAnnualDamages(IFdaFunction EquivalentAnnualDamagesFx);
    }
}
