using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions
{
    public static class ICoordinatesFunctionsFactory
    {
        public static ICoordinatesFunction Factory(IDistributedOrdinate distribution, InterpolationEnum interpolator)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            double epsilon = (distribution.Range.Max - distribution.Range.Min) * 0.01, y = distribution.Range.Min + epsilon;
            for (int p = 0; p < 100; p++)
            {
                ICoordinate coordinate = ICoordinateFactory.Factory(p * 0.01, distribution.InverseCDF(p * 0.01));
                while (y < coordinate.Y.Value())
                {

                }
                if (y < coordinate.Y.Value())
                {
                     
                }
                coordinates.Add(coordinate);
            }
            //TODO: This had no return value?
            return null;
        }
        
        
        public static ICoordinatesFunction Factory(List<ICoordinate> coordinates, InterpolationEnum interpolator)
        {
            if (IsConstantYValues(coordinates))
            {
                return new CoordinatesFunctionConstants(coordinates, interpolator);
            }
            else if(IsDistributedYValues(coordinates))
            {
                return new CoordinatesFunctionVariableYs(coordinates, interpolator);
            }
            //todo add the linked plots option. not sure i need to do this anymore
            else throw new ArgumentException("Could not turn the coordinates provided into a function.");
        }
        private static bool IsDistributedYValues(List<ICoordinate> coordinates)
        {
            foreach (ICoordinate coord in coordinates)
            {
                if (!coord.Y.GetType().Equals(typeof(Distribution)))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsConstantYValues(List<ICoordinate> coordinates)
        {
            foreach(ICoordinate coord in coordinates)
            {
                if(!coord.Y.GetType().Equals(typeof(Constant)))
                {
                    return false;
                }
            }
            return true;
        }

        public static ICoordinatesFunction Factory(IEnumerable<double> xs, IEnumerable<double> ys, InterpolationEnum interpolation = InterpolationEnum.None)
        {
            //are lengths the same
            if (xs.Count() == ys.Count())
            {
                List<double> Xs = xs.ToList(), Ys = ys.ToList();
                List<ICoordinate> coordinates = new List<ICoordinate>();
                for (int i = 0; i < Xs.Count; i++)
                {
                    ICoordinate coordinate = ICoordinateFactory.Factory(Xs[i], Ys[i]);
                    coordinates.Add(coordinate);
                }
                return new CoordinatesFunctionConstants(coordinates, interpolation);
            }
            else
            {
                throw new InvalidConstructorArgumentsException("The coordinate function cannot be created because the X and Y values are not the same lengths.");
            }
        }

        //public static ICoordinatesFunction<Constant, IOrdinate> Factory(ICoordinatesFunctionBase function)
        //{
        //    if (typeof(ICoordinatesFunction<Constant, Constant>).IsAssignableFrom(function.GetType()))
        //    {
        //        return new CoordinatesFunctionOrdinateYs((CoordinatesFunctionConstants)function);
        //    }
        //    else if (typeof(ICoordinatesFunction<Constant, Distribution>).IsAssignableFrom(function.GetType()))
        //    {
        //        return new CoordinatesFunctionOrdinateYs((CoordinatesFunctionVariableYs)function);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("The function was not a constant or variable y function.");
        //    }
        //}

        public static ICoordinatesFunction Factory(List<double> xs, List<Statistics.IDistribution> ys)
        {
            //are lengths the same
            if (xs.Count == ys.Count)
            {
                List<ICoordinate> coordinates = new List<ICoordinate>();
                for (int i = 0; i < xs.Count; i++)
                {
                    ICoordinate coordinate = ICoordinateFactory.Factory(xs[i], ys[i]);
                    coordinates.Add(coordinate);
                }

                return new CoordinatesFunctionVariableYs(coordinates);
            }
            else
            {
                throw new ArgumentException("X values are a different length than the Y values.");
            }
        }

        public static ICoordinatesFunction Factory(List<double> xs, List<IDistributedOrdinate> ys, InterpolationEnum interpolationEnum)
        {
            //are lengths the same
            if (xs.Count == ys.Count)
            {
                List<ICoordinate> coordinates = new List<ICoordinate>();
                for (int i = 0; i < xs.Count; i++)
                {
                    ICoordinate coordinate = ICoordinateFactory.Factory(xs[i], ys[i]);
                    coordinates.Add(coordinate);
                }

                return new CoordinatesFunctionVariableYs(coordinates,interpolationEnum);
            }
            else
            {
                throw new ArgumentException("X values are a different length than the Y values.");
            }
        }

        //public static ICoordinatesFunction Factory(List<double> xs, List<IDistribution> ys)
        //{
        //    //are lengths the same
        //    if (xs.Count == ys.Count)
        //    {
        //        List<ICoordinate> coordinates = new List<ICoordinate>();
        //        for (int i = 0; i < xs.Count; i++)
        //        {
        //            IDistributedOrdinate y = new Distribution(ys[i]);
        //            ICoordinate coordinate = ICoordinateFactory.Factory(xs[i], y);
        //            coordinates.Add(coordinate);
        //        }

        //        return new CoordinatesFunctionVariableYs(coordinates);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("X values are a different length than the Y values.");
        //    }
        //}
        /// <summary>
        /// Inserts/adds coordinates to the function based off the x value of the coordinates.
        /// Coordinates have to be of the same type as the coordinates in the function. ie (double, double) or (double, IDistribution)
        /// </summary>
        /// <param name="function"></param>
        /// <param name="additionalCoordinates"></param>
        /// <returns></returns>
        //public static ICoordinatesFunction<Constant, Constant> Factory(ICoordinatesFunction<Constant, Constant> function, List<ICoordinate<double, double>> additionalCoordinates)
        //{

        //    if (!function.GetType().Equals(typeof(CoordinatesFunctionConstants)))
        //    {
        //        throw new InvalidConstructorArgumentsException("The function was of the wrong type for this method.");
        //    }

        //    List<ICoordinate<Constant, Constant>> coordinates = new List<ICoordinate<Constant, Constant>>();

        //    //add the coordinates from the function
        //    foreach (ICoordinate<double, double> coord in function.Coordinates)
        //    {
        //        coordinates.Add(coord);
        //    }
        //    //add the coordinates to the end
        //    foreach (ICoordinate<double, double> coord in additionalCoordinates)
        //    {
        //        coordinates.Add(coord);
        //    }

        //    coordinates.Sort(delegate (ICoordinate<double, double> coord1, ICoordinate<double, double> coord2)
        //   {
        //       return coord1.X.CompareTo(coord2.X);
        //   });

        //    InterpolationEnum originalInterp = ((CoordinatesFunctionConstants)function).Interpolator;
        //    return new CoordinatesFunctionConstants(coordinates, originalInterp);
        //}

        //public static ICoordinatesFunction<Constant, Distribution> Factory(ICoordinatesFunction<double, IDistribution> function, List<ICoordinate<double, IDistribution>> additionalCoordinates)
        //{
        //    if (!function.GetType().Equals(typeof(CoordinatesFunctionVariableYs)))
        //    {
        //        throw new InvalidConstructorArgumentsException("The function was of the wrong type for this method.");
        //    }

        //    List<ICoordinate<double, IDistribution>> coordinates = new List<ICoordinate<double, IDistribution>>();

        //    //add the coordinates from the function
        //    foreach (ICoordinate<double, IDistribution> coord in function.Coordinates)
        //    {
        //        coordinates.Add(coord);
        //    }
        //    //add the coordinates to the end
        //    foreach (ICoordinate<double, IDistribution> coord in additionalCoordinates)
        //    {
        //        coordinates.Add(coord);
        //    }

        //    coordinates.Sort(delegate (ICoordinate<double, IDistribution> coord1, ICoordinate<double, IDistribution> coord2)
        //    {
        //        return coord1.X.CompareTo(coord2.X);
        //    });

        //    return new CoordinatesFunctionVariableYs(coordinates);
        //}

        /// <summary>
        /// Creates a function that links functions together. Interpolation schemes need to be passed in detailing how to interpolate
        /// between the functions.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="additionalCoordinates"></param>
        /// <returns></returns>
        public static ICoordinatesFunction Factory(List<ICoordinatesFunction> functions)
        {
            //the functions cant have any overlap. They can't have points on top of each other. 
            //sort the functions on the domain so that the xValues are increasing
            List<ICoordinatesFunction> sortedFunctions = functions.OrderBy(func => func.Domain.Min).ToList();
            //make sure there is no overlapping domains
            string error = ValidateDomains(sortedFunctions);
            if (error != null)
            {
                //todo: do what?
                throw new ArgumentException("Overlapping domains were detected. This is not allowed.");
            }

            CoordinatesFunctionLinked linkedFunction = new CoordinatesFunctionLinked(sortedFunctions);

            return linkedFunction;

            }
        /// <summary>
        /// This method assumes that the list has been sorted on the xvalues so that each subsequent function
        /// has higher x values.
        /// </summary>
        /// <param name="functions"></param>
        private static string ValidateDomains(List<ICoordinatesFunction> functions)
        {
            if (functions.Count == 0) { return "No functions were passed in."; }
            double min = functions[0].Domain.Min;
            double max = functions[0].Domain.Max;

            for(int i = 1;i<functions.Count;i++)// (ICoordinatesFunction func in functions)
            {
                double funcMin = functions[i].Domain.Min;
                double funcMax = functions[i].Domain.Max;
                //if this function's domain falls within the previously established domain
                //then this list of funcs is invalid.
                if (funcMin < max)
                {
                    return "Overlapping domain detected.";
                }
                else
                {
                    max = funcMax;
                }
            }
            return null;
        }

        //todo: finish if you have time. We don't plan on using this at this time.
        /// <summary>
        /// Takes a list of coordinates and with a yType of IOrdinate and converts and groups coordinates by their concrete
        /// IOrdinat type.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns>A list of ICoordinateFunction. It might be one or many in the list.</returns>
        //public static ICoordinatesFunction<double, IOrdinate> Factory(List<ICoordinate<double,IOrdinate>> coordinates)
        //{
        //    List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

        //    List<double> xs = new List<double>();
        //    List<double> ys = new List<double>();

        //    List<IDistribution> ysDistributed = new List<IDistribution>();

        //    bool workingOnDistributedFunction = true;
        //    if(coordinates.Count>0)
        //    {
        //        workingOnDistributedFunction = coordinates[0].Y.IsDistributed;
        //    }

        //    foreach(ICoordinate<double, IOrdinate> coord in coordinates)
        //    {
        //        if(coord.Y.IsDistributed)
        //        {
        //            if (workingOnDistributedFunction)
        //            {
        //                xs.Add(coord.X.Value());
        //                ysDistributed.Add((IDistribution)coord.Y);
        //            }
        //            else
        //            {
        //                //finish the not distributed func
        //                functions.Add(Factory(xs, ys));
        //                ClearLists(xs, ys, ysDistributed);
        //                //start the distributed func
        //                xs.Add(coord.X.Value());
        //                ysDistributed.Add((IDistribution)coord.Y);

        //            }
        //        }
        //        else
        //        {
        //            if(workingOnDistributedFunction)
        //            {
        //                //finish the distributed func
        //                functions.Add(Factory(xs, ysDistributed));
        //                ClearLists(xs, ys, ysDistributed);
        //                //start the not distributed func
        //                xs.Add(coord.X.Value());
        //                ys.Add(coord.Y.Value());
        //            }
        //            else
        //            {
        //                xs.Add(coord.X.Value());
        //                ys.Add(coord.Y.Value());
        //            }
        //        }
        //        //there will likely still be coordinates in the list at the end of the looping
        //        if(workingOnDistributedFunction)
        //        {
        //            functions.Add(Factory(xs, ysDistributed));
        //        }
        //        else
        //        {
        //            functions.Add(Factory(xs, ys));
        //        }
        //    }

        //    //now i have a list of functions
        //    if(functions.Count == 1)
        //    {
        //        //if there is only one function then return that function
        //        return functions[0];
        //    }
        //    else
        //    {
        //        //create a linked coordinates function with null interpolators that will have to get added later
        //        return Factory(functions, null);
        //    }
        //}

        //private static List<ICoordinate<double, IOrdinate>> ConvertDistributedYsToOrdinates(List<ICoordinate<double, IDistribution>> coords)
        //{
        //    List<ICoordinate<double, IOrdinate>> retval = new List<ICoordinate<double, IOrdinate>>();
        //    foreach (ICoordinate<double, IDistribution> coord in coords)
        //    {
        //        retval.Add(new CoordinateOrdinateY(coord.X, new Distribution(coord.Y)));
        //    }
        //    return retval;
        //}

        //private static List<ICoordinate<double, IOrdinate>> ConvertConstantYsToOrdinates(List<ICoordinate<double, double>> coords)
        //{
        //    List<ICoordinate<double, IOrdinate>> retval = new List<ICoordinate<double, IOrdinate>>();
        //    foreach (ICoordinate<double, double> coord in coords)
        //    {
        //        retval.Add(new CoordinateOrdinateY(coord.X, new Constant(coord.Y)));
        //    }
        //    return retval;
        //}


        //private static void ClearLists(List<double> xs, List<double> ys, List<IDistribution> ysDistributed)
        //{
        //    xs.Clear();
        //    ys.Clear();
        //    ysDistributed.Clear();
        //}

        static public ICoordinatesFunction DefaultOccTypeFunction()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<double> ys = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            return Factory(xs, ys);
        }

    }
}
