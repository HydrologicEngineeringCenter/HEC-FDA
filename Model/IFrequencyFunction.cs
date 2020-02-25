using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides an interface for functions in the Fda program with frequency-based (probability) X values.
    /// </summary>
    public interface IFrequencyFunction : IFdaFunction
    {
        /// <summary>
        /// A list of <see cref="ITransformFunction"/> function types used in the Fda program that can be composed with the instance <see cref="IFrequencyFunction"/>.
        /// </summary>
        List<ImpactAreaFunctionEnum> ComposeableTypes { get; }
        //IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2);

    }
}
