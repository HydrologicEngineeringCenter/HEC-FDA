using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides lean access to the key outputs of a realization of the <see cref="IConditionLocationYearSummary.Compute(IReadOnlyDictionary{IParameterEnum, ISample}, int)"/> method.
    /// </summary>
    public interface IConditionLocationYearRealizationSummary
    {
        /// <summary>
        /// An identifier used to locate realizations.
        /// </summary>
        int ID { get; }
        /// <summary>
        /// A realization of the <see cref="IConditionLocationYear{T}.Metrics"/> values.
        /// </summary>
        IReadOnlyDictionary<IMetric, double> Metrics { get; }
    }
}
