using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model 
{ 

    /// <summary>
    /// Provides a factory for the construction of objects implementing the <see cref="ILateralStructure"/> interface (<seealso cref="ILateralStructure"/>).
    /// </summary>
    public static class ILateralStructureFactory
    {
        
        /// <summary>
        /// Constructs an object implementing the <see cref="ILateralStructure"/> interface containing only a top of lateral structure elevation (<seealso cref="IParameterEnum.LateralStructureElevation"/>) and an auto-generated <see cref="IParameterEnum.LateralStructureFailure"/> (<seealso cref="ITransformFunctionFactory.Factory(double, UnitsEnum, string, string, string)"/>).
        /// </summary>
        /// <param name="top"> The top of lateral structure elevation (<seealso cref="ILateralStructure.TopElevation"/>). </param>
        /// <param name="units"> The length unit of measurement for the lateral structure height, set to <see cref="UnitsEnum.Foot"/> by default. </param>
        /// <param name="label"> A <see cref="string"/> label describing the lateral structure. </param>
        /// <returns> A new object implementing the <see cref="ILateralStructure"/> interface. </returns>
        public static ILateralStructure Factory(double top, UnitsEnum units = UnitsEnum.Foot, string label = "")
        {
            ITransformFunction fx = ITransformFunctionFactory.Factory(top, units, label);
            return new Model.Parameters.LateralStructures.LateralStructure(IElevationFactory.Factory(top, units, IParameterEnum.ExteriorElevation), fx, units, label);
        }
        /// <summary>
        /// Constructs an object implementing the <see cref="ILateralStructure"/> interface containing a user specified top of lateral structure elevation (<seealso cref="IParameterEnum.LateralStructureElevation"/>) and <see cref="ITransformFunction"/> lateral structure failure function (<seealso cref="IParameterEnum.LateralStructureFailure"/>).
        /// </summary>
        /// <param name="top"> The top of lateral structure elevation (<seealso cref="ILateralStructure.TopElevation"/>). </param>
        /// <param name="failureFx"> A <see cref="ITransformFunction"/> describing the probability of failure as a function of the water surface elevation (<see cref="IParameterEnum.LateralStructureFailure"/>). </param>
        /// <param name="units"> The length unit of measurement for the lateral structure height, set to <see cref="UnitsEnum.Foot"/> by default. </param>
        /// <param name="label"> A <see cref="string"/> label describing the lateral structure. </param>
        /// <returns> A new object implementing the <see cref="ILateralStructure"/> interface. </returns>
        public static ILateralStructure Factory(double top, ITransformFunction failureFx, UnitsEnum units = UnitsEnum.Foot, string label = "")
        {
            return new Model.Parameters.LateralStructures.LateralStructure(IParameterFactory.Factory(top, IParameterEnum.LateralStructureElevation, units, label), failureFx, units, label);
        }
    }
}
