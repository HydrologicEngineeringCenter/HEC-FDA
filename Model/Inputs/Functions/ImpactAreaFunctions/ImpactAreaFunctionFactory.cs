using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using System.Xml.Linq;
using Utilities.Serialization;
using Functions.CoordinatesFunctions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    public static class ImpactAreaFunctionFactory
    {
        public static IFdaFunction Factory(String xmlString, ImpactAreaFunctionEnum type)
        {
            XDocument doc = XDocument.Parse(xmlString);
            XElement functionsElem = doc.Element(SerializationConstants.FUNCTIONS);
            bool singleFunction = SerializationConstants.NOT_LINKED.Equals(functionsElem.Attribute(SerializationConstants.TYPE).Value);

            if (singleFunction)
            {
                XElement functionElement = functionsElem.Element(SerializationConstants.FUNCTION);
                ICoordinatesFunction func = CreateFunctionFromFunctionElement(functionElement);
                return Factory(func, type);
            }
            else
            {
                //its linked
                IEnumerable<XElement> functionElems = functionsElem.Elements(SerializationConstants.FUNCTION);
                List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();
                foreach (XElement elem in functionElems)
                {
                    functions.Add(CreateFunctionFromFunctionElement(elem));
                }
                List<InterpolationEnum> interpolators = GetInterpolatorsFromFunctions(functions);
                ICoordinatesFunction linkedFunc = ICoordinatesFunctionsFactory.Factory(functions, interpolators);
                return Factory(linkedFunc, type);
            }
            throw new ArgumentException("Could not convert the xml text into a function.");
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
            if(ordinates.Count() != 2)
            {
                throw new ArgumentException("An XElement did not have two ordinates. There should be one for X and one for Y.");
            }
            XElement xElem = ordinates.First();
            XElement yElem = ordinates.Last();

            return ICoordinateFactory.Factory(xElem, yElem);
        }


        //todo: i think we can get rid of the list of interpolators when making linked functions
        private static List<InterpolationEnum> GetInterpolatorsFromFunctions(List<ICoordinatesFunction> functions)
        {
            List<InterpolationEnum> interps = new List<InterpolationEnum>();
            foreach (ICoordinatesFunction func in functions)
            {
                interps.Add(func.Interpolator);
            }
            //remove the last one
            interps.RemoveAt(interps.Count - 1);
            return interps;
        }

        private static ICoordinatesFunction CreateFunctionFromFunctionElement(XElement functionElement)
        {
            InterpolationEnum interpolator = GetInterpolator(functionElement);
            List<ICoordinate> coordinates = GetCoordinates(functionElement);
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(coordinates, interpolator);
            return func;
        }

        public static ITransformFunction FactoryTransform(ICoordinatesFunction function, ImpactAreaFunctionEnum type)
        {
            if (type == ImpactAreaFunctionEnum.InflowOutflow)
            {
                InflowOutflow inflowOutflow = new InflowOutflow(function);
                return inflowOutflow;
            }
            else if (type == ImpactAreaFunctionEnum.Rating)
            {
                Rating rating = new Rating(function);
                return rating;
            }
            else if (type == ImpactAreaFunctionEnum.ExteriorInteriorStage)
            {

                ExteriorInteriorStage extInt = new ExteriorInteriorStage(function);
                return extInt;
            }
            else if (type == ImpactAreaFunctionEnum.LeveeFailure)
            {

                LeveeFailure failure = new LeveeFailure(function);
                return failure;
            }
            else if (type == ImpactAreaFunctionEnum.InteriorStageDamage)
            {

                StageDamage stageDamage = new StageDamage(function);
                return stageDamage;
            }
            else
            {
                throw new ArgumentException("Cannot create a transform function from the function type: " + type);

            }
        }

        public static IFrequencyFunction FactoryFrequency(ICoordinatesFunction function, ImpactAreaFunctionEnum type)
        {
             if (type == ImpactAreaFunctionEnum.DamageFrequency)
            {
                DamageFrequency damageFrequency = new DamageFrequency(function);
                return damageFrequency;
            }
            else if (type == ImpactAreaFunctionEnum.ExteriorStageFrequency)
            {
                ExteriorStageFrequency extStageFreq = new ExteriorStageFrequency(function);
                return extStageFreq;
            }
            else if (type == ImpactAreaFunctionEnum.InflowFrequency)
            {
                InflowFrequency inflowFrequency = new InflowFrequency(function);
                return inflowFrequency;
            }
            else if (type == ImpactAreaFunctionEnum.InteriorStageFrequency)
            {
                InteriorStageFrequency intStageFreq = new InteriorStageFrequency(function);
                return intStageFreq;
            }
            else if (type == ImpactAreaFunctionEnum.OutflowFrequency)
            {
                OutflowFrequency outflowFreq = new OutflowFrequency(function);
                return outflowFreq;
            }
            else
            {
                throw new ArgumentException("Cannot create a frequency function from the function type: " + type);

            }
        }

        public static IFdaFunction Factory(ICoordinatesFunction function, ImpactAreaFunctionEnum type)
        {
            //todo: i could delete this and just use the FactoryFrequency and the FactoryTransform above.
            if (type == ImpactAreaFunctionEnum.InflowOutflow)
            {
                InflowOutflow inflowOutflow = new InflowOutflow(function);
                return inflowOutflow;
            }
            else if (type == ImpactAreaFunctionEnum.InflowFrequency)
            {
                InflowFrequency inflowFrequency = new InflowFrequency(function);
                return inflowFrequency;
            }
            else if (type == ImpactAreaFunctionEnum.Rating)
            {
                Rating rating = new Rating(function);
                return rating;
            }
            else if (type == ImpactAreaFunctionEnum.ExteriorInteriorStage)
            {

                ExteriorInteriorStage extInt = new ExteriorInteriorStage(function);
                return extInt;
            }
            else if (type == ImpactAreaFunctionEnum.LeveeFailure)
            {

                LeveeFailure failure = new LeveeFailure(function);
                return failure;
            }
            else if (type == ImpactAreaFunctionEnum.InteriorStageDamage)
            {

                StageDamage stageDamage = new StageDamage(function);
                return stageDamage;
            }
            //todo: add more types here.
            else
            {
                throw new ArgumentException("The type of function: '" + type + "' is not currently supported.");
            }

        }
        public static IFdaFunction Factory(List<double> xs, List<double> ys, ImpactAreaFunctionEnum type)
        {
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(xs, ys);
            return Factory(func, type);
        }


    }
}
