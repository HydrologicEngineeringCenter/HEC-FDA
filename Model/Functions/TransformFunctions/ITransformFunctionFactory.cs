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
        /// <returns> An <see cref="ITransformFunction"/> implementing the <see cref="IFdaFunction"/> interface. </returns>
        public static ITransformFunction Factory(IFunction fx, IParameterEnum fType, string label = "", UnitsEnum xUnits = UnitsEnum.NotSet, string xLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet, string yLabel = "")
        {
            label = label == "" ? fType.Print() : label;
            switch (fType)
            {
                case IParameterEnum.InflowOutflow:
                    xUnits = xUnits == UnitsEnum.NotSet ? UnitsEnum.CubicFootPerSecond : xUnits;
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.CubicFootPerSecond : yUnits;
                    xLabel = xLabel == "" ? $"Unregulated Flow ({xUnits.Print(true)})" : xLabel;
                    yLabel = yLabel == "" ? $"Regulated Flow ({yUnits.Print(true)})" : yLabel;
                    return new Functions.InflowOutflow(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.Rating:
                    xUnits = xUnits == UnitsEnum.NotSet ? UnitsEnum.CubicFootPerSecond : xUnits;
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : yUnits;
                    xLabel = xLabel == "" ? $"Flow ({xUnits.Print(true)})" : xLabel;
                    yLabel = yLabel == "" ? $"Water Surface Elevation ({yUnits.Print(true)})" : yLabel;
                    return new Functions.Rating(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.ExteriorInteriorStage:
                    xUnits = xUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : xUnits;
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : yUnits;
                    xLabel = xLabel == "" ? $"Exterior (In-Channel) Water Surface Elevation ({xUnits.Print(true)})" : xLabel;
                    yLabel = yLabel == "" ? $"Interior (Floodplain) Water Surface Elevation ({yUnits.Print(true)})" : yLabel;
                    return new Functions.ExteriorInteriorStage(fx, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.InteriorStageDamage:
                    xUnits = xUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : xUnits;
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Dollars : yUnits;
                    xLabel = xLabel == "" ? $"Interior (Floodplain) Water Surface Elevation ({xUnits.Print(true)})" : xLabel;
                    yLabel = yLabel == "" ? $"Flood Damage ({xUnits.Print(true)})" : yLabel;
                    return new Functions.StageDamage(fx, label, xUnits, xLabel, yUnits, yLabel);
                default:
                    throw new ArgumentException($"The specified parameter type: {fType} is not a transform function.");
            }
        }
    }
}
