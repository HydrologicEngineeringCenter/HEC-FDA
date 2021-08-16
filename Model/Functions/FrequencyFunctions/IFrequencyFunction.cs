using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    /// <summary>
    /// Provides an interface for functions in the Fda program with frequency-based (probability) X values.
    /// </summary>
    public interface IFrequencyFunction: IFdaFunction
    {
        /// <summary>
        /// A list of <see cref="ITransformFunction"/> function types used in the Fda program that can be composed with the instance <see cref="IFrequencyFunction"/>.
        /// </summary>
        List<IParameterEnum> ComposeableTypes { get; }
        /// <summary>
        /// Computes the <see cref="IFunction.TrapizoidalRiemannSum"/> approximating numerical integration. 
        /// </summary>
        /// <returns> A <see cref="double"/> precision value representing the 'area under the curve'. </returns>
        double Integrate();

        //IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2);
    }
}
