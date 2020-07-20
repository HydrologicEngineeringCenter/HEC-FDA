using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    /// <summary>
    /// Provides static methods for creation of <see cref="IFdaFunction"/>s implementing the <see cref="IFrequencyFunction"/> interface.
    /// </summary>
    /// <seealso cref="IFunction"/>
    public static class IFrequencyFunctionFactory
    {
        /// <summary>
        /// Provides a method for creation of functions implementing the <see cref="IFrequencyFunction"/> interface.
        /// </summary>
        /// <param name="fx"> An <see cref="IFunction"/> that forms the basis of the <see cref="ITransformFunction"/>. </param>
        /// <param name="fType"> The <see cref="IParameterEnum"/> function type. </param>
        /// <param name="label"> Optional parameter describing the <see cref="IFdaFunction"/>.</param>
        /// <param name="xLabel"> Optional parameter describing the <see cref="IFdaFunction"/> x ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value. </param>
        /// <param name="yUnits"> Optional parameter describing the <see cref="IFdaFunction"/> y units. If set to the default: <see cref="UnitsEnum.NotSet"/> value, then the default <see cref="UnitsEnum"/> for the specified <see cref="IParameterEnum"/> is inferred."</param>
        /// <param name="yLabel"> Optional parameter describing the <see cref="IFdaFunction"/> y ordinates. If not set a default value is inferred based on the specified <see cref="IParameterEnum"/> value and the <paramref name="yUnits"/>. </param>
        /// <returns> An <see cref="IFrequencyFunction"/> implementing the <see cref="IFdaFunction"/> interface. </returns>
        public static IFrequencyFunction Factory(IFunction fx, IParameterEnum fType, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet)
        {
            label = label == "" ? fType.Print() : label;
            switch (fType)
            {
                
                case IParameterEnum.InflowFrequency:
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.CubicFootPerSecond : yUnits;
                    xLabel = xLabel == "" ? $"Annual Exceedance Probability" : xLabel;
                    yLabel = yLabel == "" ? $"Unregulated Flow ({yUnits.Print(true)})" : yLabel;
                    return new Functions.InflowFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.OutflowFrequency:
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.CubicFootPerSecond : yUnits;
                    xLabel = xLabel == "" ? $"Annual Exceedance Probability" : xLabel;
                    yLabel = yLabel == "" ? $"Regulated Flow ({yUnits.Print(true)})" : yLabel;
                    return new Functions.OutflowFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.ExteriorStageFrequency:
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : yUnits;
                    xLabel = xLabel == "" ? $"Annual Exceedance Probability" : xLabel;
                    yLabel = yLabel == "" ? $"Exterior (In-channel) Water Surface Elevation ({yUnits.Print(true)})" : yLabel;
                    return new Functions.ExteriorStageFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.InteriorStageFrequency:
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : yUnits;
                    xLabel = xLabel == "" ? $"Annual Exceedance Probability" : xLabel;
                    yLabel = yLabel == "" ? $"Interior (Floodplain) Water Surface Elevation ({yUnits.Print(true)})" : yLabel;
                    return new Functions.InteriorStageFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.DamageFrequency:
                    yUnits = yUnits == UnitsEnum.NotSet ? UnitsEnum.Foot : yUnits;
                    xLabel = xLabel == "" ? $"Annual Exceedance Probability" : xLabel;
                    yLabel = yLabel == "" ? $"Flood Damage ({yUnits.Print(true)})" : yLabel;
                    return new Functions.DamageFrequency(fx, label, xLabel, yLabel, yUnits);
                default:
                    throw new ArgumentException($"The specified parameter type: {fType} is not a frequency function.");
            }
        }
    }
}
