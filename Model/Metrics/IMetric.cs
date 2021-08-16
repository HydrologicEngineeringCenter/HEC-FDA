using Functions;

namespace Model
{
    /// <summary>
    /// Provides a public interface for computable metrics.
    /// </summary>
    public interface IMetric: IParameterOrdinate
    {
        #region Properties
        ///// <summary>
        ///// The enumerable metric type.
        ///// </summary>
        //IParameterEnum ParameterType { get; }
        ///// <summary>
        ///// The exceedance probability value for the metric. 
        ///// </summary>
        ///// <remarks> Note: this parameter is not used for <see cref="IMetricEnum.ExpectedAnnualDamage"/>. </remarks>
        //IOrdinate Ordinate { get; }
        /// <summary>
        /// The target function for the metric.
        /// </summary>
        /// <remarks> Note: this is set internally. </remarks>
        IParameterEnum TargetFunction { get; }
        #endregion
        /// <summary>
        /// Computes the metric value for the target <see cref="IFrequencyFunction"/>.
        /// </summary>
        /// <param name="frequencyFunction"> The <see cref="TargetFunction"/> instance to evaluate. </param>
        /// <param name="p"> A sample probability for <see cref="IFrequencyFunction"/>s with distributed Y ordinate values. </param>
        /// <returns></returns>
        double Compute(IFrequencyFunction frequencyFunction, double p = 0.50);
    }
}
