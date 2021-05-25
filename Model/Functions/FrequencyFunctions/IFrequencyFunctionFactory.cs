using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Functions;
using Model.Functions.FrequencyFunctions;
using Utilities.Serialization;

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
        /// <param name="abbreviate"> Optional parameter describing if labels and units should be abbreviated. Set to <see langword="true"/> default. </param>
        /// <returns> An <see cref="IFrequencyFunction"/> implementing the <see cref="IFdaFunction"/> interface. </returns>
        public static IFrequencyFunction Factory(IFunction fx, IParameterEnum fType, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.NotSet, bool abbreviate = true)
        {
            label = label == "" ? fType.PrintLabel() : label;
            yUnits = yUnits == UnitsEnum.NotSet ? fType.YUnitsDefault() : yUnits;
            yLabel = yLabel == "" ? fType.PrintYLabel(yUnits, abbreviate) : yLabel;
            xLabel = xLabel == "" ? fType.PrintXLabel(fType.XUnitsDefault(), abbreviate) : xLabel;
            switch (fType)
            {                
                case IParameterEnum.InflowFrequency:
                    if (fx.DistributionType == IOrdinateEnum.LogPearsonIII) return new Functions.FrequencyFunctions.LogPearsonIII(fx, label, xLabel, yLabel, yUnits);
                    //TODO: add graphical functions.
                    else throw new NotImplementedException("Graphical Frequency Functions have not been implemented yet.");
                case IParameterEnum.OutflowFrequency:
                    return new Functions.OutflowFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.ExteriorStageFrequency:
                    return new Functions.ExteriorStageFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.InteriorStageFrequency:
                    return new Functions.InteriorStageFrequency(fx, label, xLabel, yLabel, yUnits);
                case IParameterEnum.DamageFrequency:
                    return new Functions.DamageFrequency(fx, label, xLabel, yLabel, yUnits);
                default:
                    throw new ArgumentException($"The specified parameter type: {fType} is not a frequency function.");
            }
        }

        public static ICoordinatesFunction Factory(string xmlString)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement functionsElem = doc.Element(SerializationConstants.FUNCTIONS);
            bool isLP3 = SerializationConstants.LOG_PEARSON_III.Equals(functionsElem.Attribute(SerializationConstants.TYPE).Value);

            if (isLP3)
            {
                XElement functionElement = functionsElem.Element(SerializationConstants.FUNCTION);
                ICoordinatesFunction func = CreateFunctionFromFunctionElement(functionElement);
                return func;
                //IFunction function = IFunctionFactory.Factory(func.Coordinates, func.Interpolator);
                //return Factory(function, type);
            }
            else
            {
                //its linked
                //IEnumerable<XElement> functionElems = functionsElem.Elements(SerializationConstants.FUNCTION);
                //List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();
                //foreach (XElement elem in functionElems)
                //{
                //    functions.Add(CreateFunctionFromFunctionElement(elem));
                //}
                //ICoordinatesFunction linkedFunc = ICoordinatesFunctionsFactory.Factory(functions);
                //return linkedFunc;
                //IFunction function = IFunctionFactory.Factory(linkedFunc.Coordinates, linkedFunc.Interpolator);
                //return Factory(function, type);
            }
            throw new ArgumentException("Could not convert the xml text into a function.");
        }

        private static ICoordinatesFunction CreateFunctionFromFunctionElement(XElement functionElement)
        {
            InterpolationEnum interpolator = GetInterpolator(functionElement);
            List<ICoordinate> coordinates = GetCoordinates(functionElement);
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(coordinates, interpolator);
            return func;
        }

        private static InterpolationEnum GetInterpolator(XElement functionElement)
        {
            string interp = functionElement.Attribute(SerializationConstants.INTERPOLATOR).Value;
            return (InterpolationEnum)Enum.Parse(typeof(InterpolationEnum), interp);

        }

        private static List<ICoordinate> GetCoordinates(XElement functionElement)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (XElement coordElem in functionElement.Elements(SerializationConstants.COORDINATE))
            {
                coordinates.Add(GetCoordinate(coordElem));
            }

            return coordinates;
        }

        private static ICoordinate GetCoordinate(XElement coordinateElement)
        {
            IEnumerable<XElement> ordinates = coordinateElement.Elements(SerializationConstants.ORDINATE);
            if (ordinates.Count() != 2)
            {
                throw new ArgumentException("An XElement did not have two ordinates. There should be one for X and one for Y.");
            }
            XElement xElem = ordinates.First();
            XElement yElem = ordinates.Last();

            return ICoordinateFactory.Factory(xElem, yElem);
        }

    }
}
