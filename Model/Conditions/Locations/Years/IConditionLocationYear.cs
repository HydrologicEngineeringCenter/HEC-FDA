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
    public interface IConditionLocationYear<T>: IConditionLocationYearSummary
    {        
       
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
        T LateralStructure { get; }
        /// <summary>
        /// A list of performance metrics to report on during the compute.
        /// </summary>
        IOrderedEnumerable<IMetric> Metrics { get; }

        /// <summary>
        /// Provides an list of constant compute parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IParameterEnum> ConstantParameters();           
    }
}

 

