using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ComputePoint
{
    /// <summary>
    /// A computational point that represents a location at a point in time.
    /// </summary>
    public interface IComputePoint
    {
        int Year { get; }
        
        
        
        /// <summary>
        /// The function used to initialize a compute.
        /// </summary>
        IFrequencyFunction EntryPoint { get; }
        /// <summary>
        /// The functions used for composition in the compute.
        /// </summary>
        IEnumerable<ITransformFunction> TransformFunctions { get; }
        
    }
}
