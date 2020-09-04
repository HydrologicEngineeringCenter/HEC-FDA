using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides lean access to a <see cref="IConditionLocationYear{T}"/> descriptive properties and compute method.
    /// </summary>
    public interface IConditionLocationYearSummary
    {
        /// <summary>
        /// A label describing the condition. Concatenates the year and location by default.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// The year described in the <see cref="IConditionLocationYear{T}"/>.
        /// </summary>
        int Year { get; }
        /// <summary>
        /// A location described in the <see cref="IConditionLocationYear{T}"/>.
        /// </summary>
        ILocation Location { get; }
              
        /// <summary>
        /// Provides a <see cref="IDictionary{TKey, TValue}"/> of the compute parameters as <see cref="IParameter"/> keys with <see cref="bool"/> values set to <see langword="true"/> if the parameter is distributed, and <see langword="false"/> if it is constant.
        /// </summary>
        IReadOnlyDictionary<IParameterEnum, bool> Parameters { get; }

        /// <summary>
        /// Produces a compute with the parameters held at their mean values.
        /// </summary>
        /// <returns> A realization of the compute with the parameters held to their mean values. </returns>
        IConditionLocationYearRealization ComputePreview();
        /// <summary>
        /// Computes a single realization of the condition at the specified location <see cref="IConditionLocationYear{T}"/>.
        /// </summary>
        /// <param name="sampleParameters"> A dictionary of parameter <see cref="IParameterEnum"/> key and sample probability <see cref="ISample"/> value pairs. <seealso cref="IConditionLocationYearSummary.SampleParametersPacket(Random, IReadOnlyDictionary{IParameterEnum, bool})"/> </param>
        /// <param name="id"> An optional ID used to identify specific realizations at a later time, set to -1 by default. </param>
        /// <returns> An <see cref="IConditionLocationYearRealization"/> representing a single realization of the <see cref="IConditionLocationYear{T}"/> in the compute. </returns>
        IConditionLocationYearRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters, int id = -1);
        /// <summary>
        /// Generates a set of sampling probabilities for the specified set of <see cref="IConditionLocationYear{T}"/> parameters. 
        /// </summary>
        /// <param name="rng"> A random number generator. If one is not provided a new one is generated using the clock time value as a seed. </param>
        /// <param name="parameters"> A <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="IConditionLocationYear{T}"/> parameters as <see cref="IParameterEnum"/> keys and a <see cref="bool"/> value: set to <see langword="true"/> if the parameter should be randomly sampled, <see langword="false"/> otherwise. <seealso cref="IConditionLocationYearSummary.Parameters"/> </param>
        /// <returns> An <see cref="IDictionary{TKey, TValue}"/> containing the compute parameter <see cref="IParameterEnum"/> keys and <see cref="ISample"/> values containing a <see cref="bool"/> value describing if the parameter is being randomly sampled and a <see cref="double"/> precision sample probability (set to the median if the <see cref="bool"/> is <see langword="false"/>, indicating that the parameter is not being sampled. </returns>
        IReadOnlyDictionary<IParameterEnum, ISample> SampleParametersPacket(Random rng = null, IReadOnlyDictionary<IParameterEnum, bool> parameters = null);
    }
}
