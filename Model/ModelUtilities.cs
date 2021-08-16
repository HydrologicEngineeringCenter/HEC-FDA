using Functions;
using Model.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    ///// <summary>
    ///// Provides static variables and functions for use in the model.
    ///// </summary>
    //internal static class ModelUtilities
    //{
    //    /// <summary>
    //    /// The acceptable range ground elevations. 
    //    /// </summary>
    //    /// <remarks>
    //    /// The min is set to the lowest earth ground elevation: -1,355 feet, the Dead Sea Depression.
    //    /// The max is set to the highest earth ground elevation: 29,035 feet, the summit of Mount Everest.
    //    /// </remarks>
    //    internal static IRange<double> GroundElevationRange = IRangeFactory.Factory(-1365d, 29035);
    //    /// <summary>
    //    /// The acceptable range of elevations for lateral structures in feet.
    //    /// </summary>
    //    /// <remarks> 
    //    /// The min is set to the lowest earth ground elevation: -1,355 feet (the Dead Sea Depression). 
    //    /// The max is set to the highest earth ground elevation 29,035 feet (the summit of Mount Everest) plus the elevation of the highest dam on earth: 1001 feet (the Jinping-I Dam).  
    //    /// </remarks>
    //    internal static IRange<double> LateralStructureElevationRange = IRangeFactory.Factory(-1365d, GroundElevationRange.Max + 1001d); 
    
    //    internal static IFunction Sample(this IFdaFunction fx, double nonexceedanceProbability) => ((FdaFunctionBase)fx)._Function.Sample(nonexceedanceProbability);
    //}
}
