using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;

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
        public static IFdaFunction Factory(IParameterEnum fType, ICoordinatesFunction fx, string label = "", UnitsEnum xUnits = UnitsEnum.NotSet, string xLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet, string yLabel = "")
        {
            switch (fType) 
            {
                case IParameterEnum.InflowFrequency:
                case IParameterEnum.OutflowFrequency:
                case IParameterEnum.ExteriorStageFrequency:
                case IParameterEnum.InteriorStageFrequency:
                case IParameterEnum.DamageFrequency:
                    if (xUnits != UnitsEnum.NotSet && xUnits != UnitsEnum.Probability) throw new ArgumentException($"The {typeof(IFdaFunction)} cannot be constructed because the x ordinate units are set to: {xUnits.Print(false)}. The only valid selection for a {fType.ToString()} function is {UnitsEnum.Probability.ToString()}.");
                    if(typeof(IFunction).IsAssignableFrom(fx.GetType()))
                    {
                        return IFrequencyFunctionFactory.Factory((IFunction)fx, fType, label, xLabel, yLabel, yUnits);
                    }
                    else
                    {
                        throw new ArgumentException($"A {fType} was requested which requires an {typeof(IFunction)} {nameof(fx)} parameter but a {fx.GetType()} was found.");
                    }
                case IParameterEnum.LateralStructureFailure:
                case IParameterEnum.InflowOutflow:
                case IParameterEnum.Rating:
                case IParameterEnum.ExteriorInteriorStage:
                case IParameterEnum.InteriorStageDamage:
                    return ITransformFunctionFactory.Factory(fx, fType, label, xUnits, xLabel, yUnits, yLabel);
                case IParameterEnum.YearExteriorStageAEP:
                case IParameterEnum.YearInteriorStageAEP:
                case IParameterEnum.YearDamageAEP:
                case IParameterEnum.YearEAD:
                case IParameterEnum.YearEquavalentAnnualDamages:
                    return new Functions.MetricYear(fx, fType, label, xLabel, yLabel, yUnits);
                default:
                    throw new NotImplementedException($"The specified parameter type: {fType.ToString()} is not supported.");
            }
        }        
    } 
    
}
