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
        /// Provides a <see cref="bool"/> for each compute parameter set to <see langword="true"/> if it is distributed, <see langword="false"/> if it is constant.
        /// </summary>
        /// <returns> An <see cref="IDictionary{TKey, TValue}"/> with <see cref="IParameterEnum"/> key values and <see cref="bool"/> values set to <see langword="false"/> if the <see cref="IParameter.IsConstant"/> and <see langword="true"/> if it is distributed. </returns>
        IDictionary<IParameterEnum, bool> ParameterIsVariablePairs();

        IConditionLocationRealization Compute(IDictionary<IParameterEnum, ISampleRecord> parameterSamplePs);
    }
}

 

