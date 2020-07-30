using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides static factory methods for the construction of IFdaFunctions, usually returned 
    /// </summary>
    public static class IFdaFunctionFactory
    {      
        /// <summary>
        /// Provides a method for creation of functions implementing the <see cref="IFdaFunction"/> interface.
        /// </summary>
        /// <param name="fx"> An <see cref="IFunction"/> that forms the basis of the <see cref="IFdaFunction"/>. </param>
        /// <param name="fType"> The <see cref="IParameterEnum"/> function type. </param>
        /// <param name="label"> A title or label describing the function. </param>
        /// <param name="xUnits"> Optional parameter describing the <see cref="IFdaFunction"/> x units. If set to the default: <see cref="UnitsEnum.NotSet"/> value, then the default <see cref="UnitsEnum"/> for the specified <see cref="IParameterEnum"/> is inferred." </param>
        /// <param name="xLabel"> Optional parameter describing the <see cref="IFdaFunction"/> x ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and the <paramref name="xUnits"/>. </param>
        /// <param name="yUnits"> Optional parameter describing the <see cref="IFdaFunction"/> y units. If set to the default: <see cref="UnitsEnum.NotSet"/> value, then the default <see cref="UnitsEnum"/> for the specified <see cref="IParameterEnum"/> is inferred."</param>
        /// <param name="yLabel"> Optional parameter describing the <see cref="IFdaFunction"/> y ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and the <paramref name="yUnits"/>. </param>
        /// <returns> An object implementing the <see cref="IFdaFunction"/> interface. </returns>
        /// <remarks> If a more specific implementation is required consider requesting an <see cref="IFrequencyFunction"/> using the <see cref="IFrequencyFunctionFactory"/> or an <see cref="ITransformFunction"/> using the <see cref="ITransformFunctionFactory"/>. </remarks>
        public static IFdaFunction Factory(IFunction fx, IParameterEnum fType, string label = "", UnitsEnum xUnits = UnitsEnum.NotSet, string xLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet, string yLabel = "")
        {
            switch (fType) 
            {
                case IParameterEnum.InflowFrequency:
                case IParameterEnum.OutflowFrequency:
                case IParameterEnum.ExteriorStageFrequency:
                case IParameterEnum.InteriorStageFrequency:
                case IParameterEnum.DamageFrequency:
                    if (xUnits != UnitsEnum.NotSet && xUnits != UnitsEnum.Probability) throw new ArgumentException($"The {typeof(IFdaFunction)} cannot be constructed because the x ordinate units are set to: {xUnits.Print(false)}. The only valid selection for a {fType.ToString()} function is {UnitsEnum.Probability.ToString()}.");
                    return IFrequencyFunctionFactory.Factory(fx, fType, label, xLabel, yLabel, yUnits);
                case IParameterEnum.InflowOutflow:
                case IParameterEnum.Rating:
                case IParameterEnum.ExteriorInteriorStage:
                case IParameterEnum.InteriorStageDamage:
                    return ITransformFunctionFactory.Factory(fx, fType, label, xUnits, xLabel, yUnits, yLabel);
                default:
                    throw new NotImplementedException($"The specified parameter type: {fType.ToString()} is not supported.");
            }
        }
        /// <summary>
        /// Provides a method for creation of automatically failure functions implementing the <see cref="IFdaFunction"/> interface.
        /// </summary>
        /// <param name="top"> The top of lateral structure elevation, in exterior elevation units. The exterior water surface elevation at which failure is assumed (e.g. the probability of failure is 100 percent). </param>
        /// <param name="bottom"> The bottom of lateral structure elevation, in exterior elevation units. The exterior water surface elevation at the bottom or toe of the structure. </param>
        /// <param name="xUnits"> Optional parameter describing the elevation axis units. Defaults to: <see cref="UnitsEnum.Foot"/> if no value is provided." </param>
        /// <param name="xLabel"> Optional parameter describing the <see cref="IFdaFunction"/> elevation ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and its default units. </param>
        /// <param name="yLabel"> Optional parameter describing the <see cref="IFdaFunction"/> failure probability ordinates. In this case <see cref="UnitsEnum.Probability"/>. </param>
        /// <param name="label"> Optional parameter describing the <see cref="IFdaFunction"/>. </param>
        /// <returns> An object implementing the <see cref="IFdaFunction"/> interface. </returns>
        /// <remarks> If a more specific implementation is required consider requesting an <see cref="IFrequencyFunction"/> using the <see cref="IFrequencyFunctionFactory"/> or an <see cref="ITransformFunction"/> using the <see cref="ITransformFunctionFactory"/>. </remarks>
        public static IFdaFunction Factory(double top, double bottom = double.NaN, UnitsEnum xUnits = UnitsEnum.Foot, string xLabel = "", string yLabel = "", string label = "")
        {
            IFunction fx = IFunctionFactory.Factory(new List<ICoordinate>() { 
                ICoordinateFactory.Factory(bottom == double.NaN ? IParameterUtilities.LateralStructureElevationRange.Min : bottom, 0.0), 
                ICoordinateFactory.Factory(top, 1.0) }, InterpolationEnum.Piecewise);
            return new Conditions.Locations.LateralStructures.FailureFunction(fx, xUnits, xLabel, yLabel, label);
        }
        internal static IFdaFunction Factory(IFunction failurefx, IParameterEnum fType = IParameterEnum.LateralStructureFailure, string label = "", UnitsEnum xUnits = UnitsEnum.Foot, string xLabel = "", string yLabel = "")
        {
            if (fType != IParameterEnum.LateralStructureFailure) Factory(failurefx, fType, label, xUnits, xLabel, yUnits: UnitsEnum.NotSet, yLabel);
            return new Conditions.Locations.LateralStructures.FailureFunction(failurefx, xUnits, xLabel, yLabel, label);
        }
    } 
    
}
