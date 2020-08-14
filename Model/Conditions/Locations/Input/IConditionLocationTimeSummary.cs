using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides lean access to a <see cref="IConditionLocationTime{T}"/> descriptive properties and compute method.
    /// </summary>
    public interface IConditionLocationTimeSummary
    {
        /// <summary>
        /// The year described in the <see cref="IConditionLocationTime{T}"/>.
        /// </summary>
        int Year { get; }
        /// <summary>
        /// A location described in the <see cref="IConditionLocationTime{T}"/>.
        /// </summary>
        ILocation Location { get; }
        /// <summary>
        /// Computes a single realization of the condition at the specified location <see cref="IConditionLocationTime{T}"/>.
        /// </summary>
        /// <param name="sampleParameters"> A dictionary of parameter <see cref="IParameterEnum"/> key and sample probability <see cref="ISample"/> value pairs. <seealso cref="IConditionLocationTime{T}.SamplePacket(Random, IReadOnlyDictionary{IParameterEnum, bool})"/> </param>
        /// <returns> An <see cref="IConditionLocationTimeRealization"/> representing a single realization of the <see cref="IConditionLocationTime{T}"/> in the compute. </returns>
        IConditionLocationTimeRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters);
    }
}
