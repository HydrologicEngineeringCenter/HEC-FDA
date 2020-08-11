using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    /// <summary>
    /// An interface for lateral structures such as levees which impede the flow of water from river channel to the floodplain (<seealso cref="IParameterEnum.LateralStructure"/>). 
    /// </summary>
    public interface ILateralStructure: IParameter
    {
        /// <summary>
        /// The elevation of the top of the lateral structure (<seealso cref="IParameterEnum.LateralStructureElevation"/>).
        /// </summary>
        IParameterOrdinate TopElevation { get; }
        /// <summary>
        /// An <see cref="ITransformFunction"/> (<seealso cref="IParameterEnum.LateralStructureFailure"/>) describing the probability of lateral failure (<seealso cref="IParameterEnum.FailureProbability"/>) as a function of the in-channel water surface elevation (<seealso cref="IParameterEnum.ExteriorElevation"/>).
        /// </summary>
        ITransformFunction FailureFunction { get; }
    }
}
