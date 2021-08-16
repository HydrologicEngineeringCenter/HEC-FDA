using Model.Metrics;
using System;
using System.Collections.Generic;
using System.Text;
using Model.Metrics;

namespace Model
{
    /// <summary>
    /// Provides methods for the construction of objects implementing the <see cref="IMetric"/> interface.
    /// </summary>
    public static class IMetricFactory
    {
        /// <summary>
        /// Generates an <see cref="IMetricEnum.ExpectedAnnualDamage"/> metric.
        /// </summary>
        /// <returns> A <see cref="IMetricEnum.ExpectedAnnualDamage"/> <see cref="IMetric"/>.</returns>
        public static IMetric Factory()
        {
            return new Metric(IMetricEnum.ExpectedAnnualDamage);
        }
        /// <summary>
        /// Generates an <see cref="IMetric"/>.
        /// </summary>
        /// <param name="type"> The type of metric that is desired. </param>
        /// <param name="target"> The exceedance value to evaluate. </param>
        /// <returns> A metric that is evaluated during an <see cref="IConditionLocationYear{In, Out}.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/>. </returns>
        public static IMetric Factory(IMetricEnum type, double target)
        {
            return new Metric(type, target);
        }
    }
}
