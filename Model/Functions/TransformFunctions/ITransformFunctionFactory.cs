using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Provides static factory methods for the creation of <see cref="IFdaFunction"/>s implementing the <see cref="ITransformFunction"/> interface.
    /// </summary>
    public static class ITransformFunctionFactory
    {
        /// <summary>
        /// Provides a method for creation of functions implementing the <see cref="ITransformFunction"/> interface.
        /// </summary>
        /// <param name="fx"> An <see cref="IFunction"/> that forms the basis of the <see cref="ITransformFunction"/>. </param>
        /// <param name="fType"> The <see cref="IParameterEnum"/> function type. </param>
        /// <param name="label"> Optional parameter describing the <see cref="IFdaFunction"/>. </param>
        /// <param name="xUnits"> Optional parameter describing the <see cref="IFdaFunction"/> x units. If set to the default: <see cref="UnitsEnum.NotSet"/> value, then the default <see cref="UnitsEnum"/> for the specified <see cref="IParameterEnum"/> is inferred." </param>
        /// <param name="xLabel"> Optional parameter describing the <see cref="IFdaFunction"/> x ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and the <paramref name="xUnits"/>. </param>
        /// <param name="yUnits"> Optional parameter describing the <see cref="IFdaFunction"/> y units. If set to the default: <see cref="UnitsEnum.NotSet"/> value, then the default <see cref="UnitsEnum"/> for the specified <see cref="IParameterEnum"/> is inferred."</param>
        /// <param name="yLabel"> Optional parameter describing the <see cref="IFdaFunction"/> y ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and the <paramref name="yUnits"/>. </param>
        /// <param name="abbreviate"> Optional parameter describing if labels and units should be abbreviated. Set to <see langword="true"/> default. </param>
        /// <returns> An <see cref="ITransformFunction"/> implementing the <see cref="IFdaFunction"/> interface. </returns>
        public static ITransformFunction Factory(IFunction fx, IParameterEnum fType, string label = "", UnitsEnum xUnits = UnitsEnum.NotSet, string xLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet, string yLabel = "", bool abbreviate = true)
        {
            label = label == "" ? fType.PrintLabel(abbreviate: abbreviate) : label;
            xUnits = xUnits == UnitsEnum.NotSet ? fType.XUnitsDefault() : xUnits;
            yUnits = yUnits == UnitsEnum.NotSet ? fType.YUnitsDefault() : yUnits;
            xLabel = xLabel == "" ? fType.PrintXLabel(xUnits, abbreviate) : xLabel;
            yLabel = yLabel == "" ? fType.PrintYLabel(yUnits, abbreviate) : yLabel;
            switch (fType)
            {
                case IParameterEnum.InflowOutflow:                   
                    return new Functions.InflowOutflow(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.Rating:
                    return new Functions.Rating(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.ExteriorInteriorStage:
                    return new Functions.ExteriorInteriorStage(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.InteriorStageDamage:
                    return new Functions.StageDamage(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.LateralStructureFailure:
                    return new Functions.FailureFunction(fx, xUnits, xLabel, yLabel, label);
                default:
                    throw new ArgumentException($"The specified parameter type: {fType} is not a transform function.");
            }
        }       
        internal static ITransformFunction Factory(double top, UnitsEnum xUnits = UnitsEnum.NotSet, string label = "", string xLabel = "", string yLabel = "", bool abbreviate = true)
        {
            var type = IParameterEnum.LateralStructureFailure;
            label = label == "" ? type.PrintLabel(abbreviate: abbreviate) : label;
            xUnits = xUnits == UnitsEnum.NotSet ? type.XUnitsDefault() : xUnits;
            xLabel = xLabel == "" ? type.PrintXLabel(xUnits, abbreviate) : xLabel;
            yLabel = yLabel == "" ? type.PrintYLabel(type.YUnitsDefault(), abbreviate) : yLabel;
            
            IFunction fx = IFunctionFactory.Factory(new List<ICoordinate>() {
                ICoordinateFactory.Factory(IParameterUtilities.LateralStructureElevationRange.Min, 0.0),
                ICoordinateFactory.Factory(top, 1.0) }, InterpolationEnum.Piecewise);
            return new Functions.FailureFunction(fx, xUnits, xLabel, yLabel, label);
        }
    }
}
