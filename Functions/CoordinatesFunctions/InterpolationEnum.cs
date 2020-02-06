

namespace Functions.CoordinatesFunctions
{
    /// <summary> Describes how y values are interpolated on the domain of the function. </summary>
    public enum InterpolationEnum
    {
        // TODO: Add Cublic Spline (And Consider Polynomial Interpolation)

        /// <summary> Y values are not interpolated resulting in a discrete representation of the function. Default Interpolator. </summary>
        None = 0,
        /// <summary> Y values are linearly interpolated on the function domain resulting in a continuous representation of the function on the range mapped to its domain https://en.wikipedia.org/wiki/Linear_interpolation. </summary>
        Linear = 1,
        /// <summary> Y values are piecewise interpolated (also known as nearest-neighbor or proximal interpolation) on the function domain resulting in a discontinuous representation of the function on the range mapped to its domain https://en.wikipedia.org/wiki/Interpolation.
        Piecewise = 2,
        /// <summary> The function is the CDF of a statistical distribution. </summary>
        Statistical = 3,
        /// <summary> The specified function coordinates are  </summary>
        NaturalCubicSpline = 4,
    }

    

}
