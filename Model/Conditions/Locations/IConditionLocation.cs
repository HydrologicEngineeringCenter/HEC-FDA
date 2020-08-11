using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Model
{
    /// <summary>
    /// Represents the <see cref="ICondition"/> at a specific location.
    /// </summary>
    public interface IConditionLocation<In, Out>
    {        
        /// <summary>
        /// A label describing the condition. Concatenates the year and location by default.
        /// </summary>
        string Label { get; }
        /// <summary>
        /// The year associated with the condition.
        /// </summary>
        int Year { get; }
        /// <summary>
        /// Location of the computation.
        /// </summary>
        ILocation Location { get; }
        
        /// <summary>
        /// The function used to initialize a compute.
        /// </summary>
        IFrequencyFunction EntryPoint { get; }
        /// <summary>
        /// The functions used for composition in the compute.
        /// </summary>
        IOrderedEnumerable<ITransformFunction> TransformFunctions { get; }
        /// <summary>
        /// An optional lateral structure (i.e. levee) parameter for the location. If one is NOT present a string "No lateral structure" is returned.
        /// </summary>
        In LateralStructure { get; }
        /// <summary>
        /// A list of performance metrics to report on during the compute.
        /// </summary>
        IOrderedEnumerable<IMetric> Metrics { get; }
        /// <summary>
        /// Provides a <see cref="IDictionary{TKey, TValue}"/> of the compute parameters as <see cref="IParameter"/> keys with <see cref="bool"/> values set to <see langword="true"/> if the parameter is distributed, and <see langword="false"/> if it is constant.
        /// </summary>
        IReadOnlyDictionary<IParameterEnum, bool> Parameters { get; }
        /// <summary>
        /// Provides an list of constant compute parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IParameterEnum> ConstantParameters();
        /// <summary>
        /// Generates a set of sampling probabilities for the specified set of <see cref="IConditionLocation{In, Out}"/> parameters. 
        /// </summary>
        /// <param name="rng"> A random number generator. If one is not provided a new one is generated using the clock time value as a seed. </param>
        /// <param name="sampleParameters"> A <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="IConditionLocation{In, Out}"/> parameters as <see cref="IParameterEnum"/> keys and a <see cref="bool"/> value: set to <see langword="true"/> if the parameter should be randomly sampled, <see langword="false"/> otherwise. <seealso cref="IConditionLocation{In, Out}.Parameters"/> </param>
        /// <returns> An <see cref="IDictionary{TKey, TValue}"/> containing the compute parameter <see cref="IParameterEnum"/> keys and <see cref="ISample"/> values containing a <see cref="bool"/> value describing if the parameter is being randomly sampled and a <see cref="double"/> precision sample probability (set to the median if the <see cref="bool"/> is <see langword="false"/>, indicating that the parameter is not being sampled. </returns>
        IReadOnlyDictionary<IParameterEnum, ISample> SamplePacket(Random rng = null, IReadOnlyDictionary<IParameterEnum, bool> sampleParameters = null);
        /// <summary>
        /// Produces a compute with the parameters held at their mean values.
        /// </summary>
        /// <returns> A realization of the compute with the parameters held to their mean values. </returns>
        IConditionLocationRealization<string> ComputePreview();
        /// <summary>
        /// Computes a single realization of the condition at the specified location <see cref="IConditionLocation{In, Out}"/>.
        /// </summary>
        /// <param name="parameterSamplePs"> A dictionary of parameter <see cref="IParameterEnum"/> key and sample probability <see cref="ISample"/> value pairs. <seealso cref="IConditionLocation{In, Out}.SamplePacket(Random, IDictionary{IParameterEnum, bool})"/> </param>
        /// <returns> An <see cref="IConditionLocationRealization{T}"/> representing a single realization of the <see cref="IConditionLocation{In, Out}"/> in the compute. </returns>
        IConditionLocationRealization<Out> Compute(IReadOnlyDictionary<IParameterEnum, ISample> parameterSamplePs);
    }
}

 

