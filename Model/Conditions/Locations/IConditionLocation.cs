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
    public interface IConditionLocation
    {        
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
        /// An optional lateral structure (i.e. levee) parameter for the location.
        /// </summary>
        ILateralStructure LateralStructure { get; }
        /// <summary>
        /// A list of performance metrics to report on during the compute.
        /// </summary>
        IOrderedEnumerable<IMetric> Metrics { get; }
        /// <summary>
        /// Provides a <see cref="IDictionary{TKey, TValue}"/> of the compute parameters as <see cref="IParameter"/> keys with <see cref="bool"/> values set to <see langword="true"/> if the parameter is distributed, and <see langword="false"/> if it is constant.
        /// </summary>
        IDictionary<IParameterEnum, bool> Parameters { get; }
        /// <summary>
        /// Provides an list of constant compute parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IParameterEnum> ConstantParameters();
        /// <summary>
        /// Generates a set of sampling probabilities for the specified set of <see cref="IConditionLocation"/> parameters. 
        /// </summary>
        /// <param name="rng"> A random number generator. If one is not provided a new one is generated using the clock time value as a seed. </param>
        /// <param name="sampleParameters"> A <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="IConditionLocation"/> parameters as <see cref="IParameterEnum"/> keys and a <see cref="bool"/> value: set to <see langword="true"/> if the parameter should be randomly sampled, <see langword="false"/> otherwise. <seealso cref="IConditionLocation.Parameters"/> </param>
        /// <returns> An <see cref="IDictionary{TKey, TValue}"/> containing the compute parameter <see cref="IParameterEnum"/> keys and <see cref="ISampleRecord"/> values containing a <see cref="bool"/> value describing if the parameter is being randomly sampled and a <see cref="double"/> precision sample probability (set to the median if the <see cref="bool"/> is <see langword="false"/>, indicating that the parameter is not being sampled. </returns>
        IDictionary<IParameterEnum, ISampleRecord> SamplePacket(Random rng = null, IDictionary<IParameterEnum, bool> sampleParameters = null);
        /// <summary>
        /// Computes a single realization of the condition at the specified location <see cref="IConditionLocation"/>.
        /// </summary>
        /// <param name="parameterSamplePs"> A dictionary of parameter <see cref="IParameterEnum"/> key and sample probability <see cref="ISampleRecord"/> value pairs. <seealso cref="IConditionLocation.SamplePacket(Random, IDictionary{IParameterEnum, bool})"/> </param>
        /// <returns> An <see cref="IConditionLocationRealization"/> representing a single realization of the <see cref="IConditionLocation"/> in the compute. </returns>
        IConditionLocationRealization Compute(IDictionary<IParameterEnum, ISampleRecord> parameterSamplePs);
    }
}

 

