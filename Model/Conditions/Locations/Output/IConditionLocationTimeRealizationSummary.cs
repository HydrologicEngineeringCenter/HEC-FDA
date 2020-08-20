using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides lean access to the key outputs of a realization of the <see cref="IConditionLocationTimeSummary.Compute(IReadOnlyDictionary{IParameterEnum, ISample})"/> method.
    /// </summary>
    public interface IConditionLocationTimeRealizationSummary
    {
        /// <summary>
        /// An identifier used to locate realizations.
        /// </summary>
        int ID { get; }
        /// <summary>
        /// A realization of the <see cref="IConditionLocationTime{T}.Metrics"/> values.
        /// </summary>
        IReadOnlyDictionary<IMetric, double> Metrics { get; }
    }
}
