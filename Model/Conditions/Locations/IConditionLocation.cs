using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// A collection of <see cref="IConditionLocationYearSummary"/>s for the same <see cref="ILocation"/> during different <see cref="IConditionLocationYearSummary.Year"/>s.
    /// </summary>
    public interface IConditionLocation
    {
        /// <summary>
        /// The years included in the <see cref="IConditionLocation"/>.
        /// </summary>
        IReadOnlyDictionary<int, IConditionLocationYearSummary> Years { get; }
        /// <summary>
        /// Produces a single <see cref="IConditionLocationYearRealization"/> from the <see cref="IConditionLocation"/> with each of the <see cref="Years"/> realization computed by holding all values at their means (<seealso cref="IConditionLocationYearSummary.ComputePreview"/>).
        /// </summary>
        /// <returns></returns>
        IConditionLocationRealization ComputePreview();
    }
}
