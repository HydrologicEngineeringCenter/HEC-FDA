using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public interface IFunction : ICoordinatesFunction
    {
        /// <summary>
        /// <see langword="true"/> if the inverse of the <see cref="IFunction"/> is also a function (e.g. attempts to compute F(y) = x from F(x) = y).
        /// </summary>
        bool IsInvertible { get; }
        /// <summary>
        /// Provides the minimum and maximum Y values for the <see cref="IFunction"/>.
        /// </summary>
        Utilities.IRange<double> Range { get; }
        /// <summary>
        /// Approximates the <see cref="IFunction"/> integral. For more information see: https://en.wikipedia.org/wiki/Trapezoidal_rule.
        /// </summary>
        /// <returns> The area under the xy-plane bounded by the <see cref="IFunction"/> <see cref="ICoordinatesFunction.Domain"/>. </returns>
        /// <remarks> In the case that the <see cref="IFunction"/> <see cref="ICoordinatesFunction.Domain"/> describes the probability of the <see cref="IFunction.Range"/> values occurrence what is produced is a probability weighted expected value for the <see cref="IFunction"/>. </remarks>
        double TrapizoidalRiemannSum();
        /// <summary>
        /// Performs functional composition, producing a new <see cref="IFunction"/> h(x) such that h(x) = g(f(x)) where g(*) is the <paramref name="g"/> parameter. For more information see: https://en.wikipedia.org/wiki/Function_composition. 
        /// </summary>
        /// <param name="g"> An <see cref="IFunction"/> containing the Y values for the composed (e.g. new) <see cref="IFunction"/>. </param>
        /// <returns> A new <see cref="IFunction"/> containing the instance variable <see cref="ICoordinate.X"/> values and the <paramref name="g"/> parameter's <see cref="ICoordinate.Y"/> values. </returns>
        /// <remarks> The Compose() function requires that the instance function <see cref="IFunction.Range"/> (e.g. Y values) are sorted in the same ascending or descending order as the <paramref name="g"/> parameter function domain (e.g. X values). </remarks>
        /// <example> Suppose we have two functions f(x) with the coordinates: {(0, 1), (3, 4)}, and g(x) with the coordinates: { (0, 0), (1, 10), (2, 20), (3, 30)} with linear interpolation between the coordinates. 
        /// Then the composed function h(x) = g(f(x)) will contain the coordinates: { (0, 10), (1, 20), (2, 30) } with linear interpolation between the coordinates. 
        /// Note that the f(x) coordinate: (3, 4) is dropped because the y value: 4 is not on the domain: [0, 3] of g(x). Similarly, the g(x) coordinate (0, 0) is dropped because the x value: 0 is not on the range: [1, 4] of f(x). 
        /// </example>
        IFunction Compose(IFunction g);
        
    }
}
