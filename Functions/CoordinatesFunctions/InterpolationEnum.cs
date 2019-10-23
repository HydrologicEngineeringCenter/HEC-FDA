using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    /// <summary> Describes how y values are interpolated on the domain of the function. </summary>
    public enum InterpolationEnum
    {
        // TODO: Add Cublic Spline (And Consider Polynomial Interpolation)

        /// <summary> Y values are not interpolated resulting in a discrete representation of the function. Default Interpolator. </summary>
        NoInterpolation = 0,
        /// <summary> Y values are linearly interpolated on the function domain resulting in a continous representation of the function on the range mapped to its domain https://en.wikipedia.org/wiki/Linear_interpolation. </summary>
        Linear = 1,
        /// <summary> Y values are piecewise interpolated (also known as nearest-neighbor or proximal interpolation) on the function domain resulting in a discontinous representation of the function on the range mapped to its domain https://en.wikipedia.org/wiki/Interpolation.
        Piecewise = 2,
    }
}
