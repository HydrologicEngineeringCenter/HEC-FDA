using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using Functions.Ordinates;
using Functions.CoordinatesFunctions;
using System.Xml.Linq;
using Utilities.Serialization;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    public static class ImpactAreaFunctionFactory
    {
        //#region CreateNew() Methods

        //public static IFrequencyFunction<double> CreateNewFrequencyFunction(ICoordinatesFunction<double, double> function, ImpactAreaFunctionEnum type)
        //{
        //    if (type == ImpactAreaFunctionEnum.InflowFrequency)
        //    {
        //        InflowFrequency<double> inflowFrequency = new InflowFrequency<double>(function);
        //        return inflowFrequency;
        //    }
        //    else return null;
        //}

        public static IFdaFunction Factory(String xmlString, ImpactAreaFunctionEnum type)
        {
            XDocument doc =  XDocument.Parse(xmlString);
            XElement functionsElem = doc.Element(SerializationConstants.FUNCTIONS);
            bool singleFunction = SerializationConstants.NOT_LINKED.Equals( functionsElem.Attribute(SerializationConstants.TYPE).Value);

            if(singleFunction)
            {
                XElement functionElement = functionsElem.Element(SerializationConstants.FUNCTION);
                InterpolationEnum interpolator = GetInterpolator(functionElement);
                List<ICoordinate> coordinates = GetCoordinates(functionElement);
                ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(coordinates, interpolator);
                return Factory(func, type);
            }
            else
            {
                //todo add the linked option
                //its linked

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

        public static IFdaFunction Factory(ICoordinatesFunction function, ImpactAreaFunctionEnum type) 
        {
            if (type == ImpactAreaFunctionEnum.InflowOutflow)
            {
                InflowOutflow inflowOutflow = new InflowOutflow(function);
                return inflowOutflow;
            }
            else if(type == ImpactAreaFunctionEnum.InflowFrequency)
            {
                InflowFrequency inflowFrequency = new InflowFrequency(function);
                return inflowFrequency;
            }
            else return null;
        }


        //public static ITransformFunction<YType> CreateNewTransformFunction<YType>(ICoordinatesFunction<double, YType> function, ImpactAreaFunctionEnum type)
        //{
        //    if (type == ImpactAreaFunctionEnum.InflowOutflow)
        //    {
        //        InflowOutflow<YType> inflowOutflow = new InflowOutflow<YType>(function);
        //        return inflowOutflow;
        //    }
        //    else return null;
        //}


        //public static IFunctionCompose CreateNew(Statistics.Distributions.LogPearsonIII logPearsonFrequencyFunction)
        //{
        //    return new InflowFrequency(new FrequencyFunction(logPearsonFrequencyFunction));
        //}
        //public static IFunctionCompose CreateNew(FrequencyFunction logPearsonFrequencyFunction)
        //{
        //    return new InflowFrequency(logPearsonFrequencyFunction);
        //}
        ///// <summary> Creates a new IFunctionCompose from a set of composed ordinates. Only intended for use by the IFunctionBase.Compose function. An exception is thrown if the inputs cannot be expressed as an IFunctionCompose computation point function. </summary>
        ///// <param name="composedOrdinates"> The set of ordinates created by the IFunctionBase.Compose function. </param>
        ///// <param name="type"> The type of computation point function that is being targeted for creation. An exception will be thrown if the target is not a frequency function type. </param>
        ///// <returns> A new IFunctionCompose computation point function, if one can be created. Otherwise an exception is thrown. </returns>
        //internal static IFunctionCompose CreateNew(IList<Tuple<double, double>> composedOrdinates, ImpactAreaFunctionEnum type)
        //{
        //    if ((int)type % 2 == 1) return ConstructImplementation(composedOrdinates, type);
        //    else throw new NotImplementedException();
        //}
        ///// <summary> Creates a new IFunctionTransform, provided that the input IFunctionBase and target computation point function type can be expressed as an IFunctionTransform. An exception is thrown if the inputs cannot be expressed as an IFunctionTransform computation point function. </summary>
        ///// <param name="function"> An IFunctionBase that can be expressed as an IFunctionTransform. </param>
        ///// <param name="ordinates"> The IFunctionBase ordinates property. </param>
        ///// <param name="type"> The computation point functions target type. An exception will be thrown if the target is not a tranform function type. </param>
        ///// <returns> A IFunctionTransform compuation point function, if one can be created. Otherwise an exception is thrown. </returns>
        //public static ITransformFunction CreateNew(Statistics.CurveIncreasing function, ImpactAreaFunctionEnum type)
        //{
        //    OrdinatesFunction ordinatesFunction = new OrdinatesFunction(function);
        //    if ((int)type % 2 == 0) return ConstructImplementation(ordinatesFunction, ordinatesFunction.Ordinates, type);
        //    else throw new NotImplementedException();
        //}
        //public static ITransformFunction CreateNew(Statistics.UncertainCurveIncreasing function, ImpactAreaFunctionEnum type)
        //{
        //    UncertainOrdinatesFunction uncertainOrdinatesFunction = new UncertainOrdinatesFunction(function);
        //    if ((int)type % 2 == 0) return ConstructImplementation(uncertainOrdinatesFunction, uncertainOrdinatesFunction.Ordinates, type);
        //    else throw new NotImplementedException();
        //}
        //public static ITransformFunction CreateNew(IFunctionBase function, IList<Tuple<double, double>> ordinates, ImpactAreaFunctionEnum type)
        //{
        //    if ((int)type % 2 == 0) return ConstructImplementation(function, function.GetOrdinates(), type);
        //    else throw new NotImplementedException();
        //}
        //internal static ImpactAreaFunctionBase CreateNew(IFunctionBase function, ImpactAreaFunctionEnum type)
        //{
        //    return ConstructImplementation(function, type, true);
        //}
        //#endregion

        //private static ImpactAreaFunctionBase ConstructImplementation(IFunctionBase function, ImpactAreaFunctionEnum type, bool constructCPFunctionBase = true)
        //{
        //    switch (type)
        //    {
        //        case ImpactAreaFunctionEnum.InflowOutflow:
        //            return new InflowOutflow(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.OutflowFrequency:
        //            return new OutflowFrequency(function);
        //        case ImpactAreaFunctionEnum.Rating:
        //            return new Rating(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.ExteriorStageFrequency:
        //            return new ExteriorStageFrequency(function);
        //        case ImpactAreaFunctionEnum.ExteriorInteriorStage:
        //            return new ExteriorInteriorStage(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.InteriorStageFrequency:
        //            return new InteriorStageFrequency(function);
        //        case ImpactAreaFunctionEnum.InteriorStageDamage:
        //            return new StageDamage(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.DamageFrequency:
        //            return new DamageFrequency(function);
        //        default:
        //            return new UnUsed(function, type);
        //    }
        //}
        //private static IFunctionCompose ConstructImplementation(IFunctionBase function, ImpactAreaFunctionEnum type)
        //{
        //    if ((int)type % 2 == 0) throw new NotImplementedException();
        //    switch (type)
        //    {
        //        case ImpactAreaFunctionEnum.OutflowFrequency:
        //            return new OutflowFrequency(function);
        //        case ImpactAreaFunctionEnum.ExteriorStageFrequency:
        //            return new ExteriorStageFrequency(function);
        //        case ImpactAreaFunctionEnum.InteriorStageFrequency:
        //            return new InteriorStageFrequency(function);
        //        case ImpactAreaFunctionEnum.DamageFrequency:
        //            return new DamageFrequency(function);
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}
        //private static IFunctionCompose ConstructImplementation(IList<Tuple<double, double>> composedOrdinates, ImpactAreaFunctionEnum type)
        //{
        //    IFunctionBase baseFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(composedOrdinates.Select(x => x.Item1).ToList(), composedOrdinates.Select(y => y.Item2).ToList(), true, false));
        //    switch (type)
        //    {
        //        case ImpactAreaFunctionEnum.OutflowFrequency:
        //            return new OutflowFrequency(baseFunction);
        //        case ImpactAreaFunctionEnum.ExteriorStageFrequency:
        //            return new ExteriorStageFrequency(baseFunction);
        //        case ImpactAreaFunctionEnum.InteriorStageFrequency:
        //            return new InteriorStageFrequency(baseFunction);
        //        case ImpactAreaFunctionEnum.DamageFrequency:
        //            return new DamageFrequency(baseFunction);
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}
        //private static ITransformFunction ConstructImplementation(IFunctionBase function, IList<Tuple<double, double>> ordinates, ImpactAreaFunctionEnum type)
        //{
        //    switch (type)
        //    {
        //        case ImpactAreaFunctionEnum.InflowOutflow:
        //            return new InflowOutflow(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.Rating:
        //            return new Rating(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.ExteriorInteriorStage:
        //            return new ExteriorInteriorStage(function, function.GetOrdinates());
        //        case ImpactAreaFunctionEnum.InteriorStageDamage:
        //            return new StageDamage(function, function.GetOrdinates());
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}   
    }
}
