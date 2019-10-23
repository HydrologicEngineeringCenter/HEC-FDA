using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Functions
{
    public static class ICoordinateFunctionsFactory
    {

        //public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates, InterpolationEnum interpolation = InterpolationEnum.NoInterpolation)
        //{
        //    return new CoordinatesFunctions.CoordinatesFunctionConstants(coordinates, interpolation);
        //}

        public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(List<double> xs, List<double> ys, InterpolationEnum interpolation = InterpolationEnum.NoInterpolation)
        {
            //are lengths the same
            if (xs.Count == ys.Count)
            {
                ImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates = ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>();

                for (int i = 0; i < xs.Count; i++)
                {
                    ICoordinate<IOrdinate, IOrdinate> coordinate = ICoordinateFactory.Factory(xs[i], ys[i]);
                    coordinates.Add(coordinate);
                }
                return new CoordinatesFunctions.CoordinatesFunctionConstants(coordinates, interpolation);
            }
            else
            {
                throw new ArgumentException("X values are a different length than the Y values.");
            }
        }

        public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(List<double> xs, List<Statistics.IDistribution> ys)
        {
            //are lengths the same
            if (xs.Count == ys.Count)
            {
                ImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates = ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>();
                for (int i = 0; i < xs.Count; i++)
                {
                    ICoordinate<IOrdinate, IOrdinate> coordinate = ICoordinateFactory.Factory(xs[i], ys[i]);
                    coordinates.Add(coordinate);
                }

                return new CoordinatesFunctions.CoordinatesFunctionVariableYs(coordinates);
            }
            else
            {
                throw new ArgumentException("X values are a different length than the Y values.");
            }
        }

        /// <summary>
        /// Inserts/adds coordinates to the function based off the x value of the coordinates.
        /// Coordinates have to be of the same type as the coordinates in the function. ie (double, double) or (double, IDistribution)
        /// </summary>
        /// <param name="function"></param>
        /// <param name="additionalCoordinates"></param>
        /// <returns></returns>
        public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(ICoordinatesFunction<IOrdinate, IOrdinate> function, List<ICoordinate<IOrdinate, IOrdinate>> additionalCoordinates)
        {
            bool isFunctionDistributed = false;
            //the new coordinates need to be the same type as the function
            if (function.GetType().Equals(typeof(CoordinatesFunctionVariableYs)))
            {
                isFunctionDistributed = true;
                //then the ys need to be idistribution and the xs need to be double
                foreach (ICoordinate<IOrdinate, IOrdinate> coord in additionalCoordinates)
                {
                    if (coord.X.IsDistributed || !coord.Y.IsDistributed)
                    {
                        throw new ArgumentException("Additional coordinates were not of the same type as the function.");
                    }
                }
            }
            else if (function.GetType().Equals(typeof(CoordinatesFunctionConstants)))
            {
                //then the xs and ys need to be double
                foreach (ICoordinate<IOrdinate, IOrdinate> coord in additionalCoordinates)
                {
                    if (coord.X.IsDistributed || coord.Y.IsDistributed)
                    {
                        throw new ArgumentException("Additional coordinates were not of the same type as the function.");
                    }
                }
            }


            ImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates = ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>();

            //cant i just add them all to a list and then sort the list?


            //add the coordinates from the function
            foreach (ICoordinate<IOrdinate, IOrdinate> coord in function.Coordinates)
            {
                coordinates.Add(coord);
            }
            //add the coordinates to the end
            foreach (ICoordinate<IOrdinate, IOrdinate> coord in additionalCoordinates)
            {
                coordinates.Add(coord);
            }

            ImmutableList<ICoordinate<IOrdinate, IOrdinate>> sortedList = coordinates.Sort(delegate (ICoordinate<IOrdinate, IOrdinate> coord1, ICoordinate<IOrdinate, IOrdinate> coord2)
            {
                return coord1.X.Value().CompareTo(coord2.X.Value());// x.Total.CompareTo(y.Total);
            });

            if (isFunctionDistributed)
            {
                return new CoordinatesFunctionVariableYs(coordinates);
            }
            else
            {
                InterpolationEnum originalInterp = ((CoordinatesFunctionConstants)function).Interpolation;
                return new CoordinatesFunctionConstants(coordinates, originalInterp);
            }

        }

        /// <summary>
        /// Creates a function that links functions together. Interpolation schemes need to be passed in detailing how to interpolate
        /// between the functions.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="additionalCoordinates"></param>
        /// <returns></returns>
        public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(List<ICoordinatesFunction<IOrdinate, IOrdinate>> functions, List<InterpolationEnum> interpolators)
        {
            //the functions cant have any overlap. They can't have points on top of each other. 
            //the interpolators should be functions.count -1
            //for example: func1, interp1, func2, interp2, func3
            bool needsInterpolators = interpolators == null;
            //sort the functions on the domain so that the xValues are increasing
            List<ICoordinatesFunction<IOrdinate, IOrdinate>> sortedFunctions = functions.OrderBy(func => func.Domain.Item1).ToList();
            //make sure there is no overlapping domains
            string error = ValidateDomains(sortedFunctions);
            if(error != null)
            {
                //todo: do what?
            }

            LinkedCoordinatesFunction linkedFunction = new LinkedCoordinatesFunction(sortedFunctions, interpolators);

            return linkedFunction;

        }
        /// <summary>
        /// This method assumes that the list has been sorted on the xvalues so that each subsequent function
        /// has higher x values.
        /// </summary>
        /// <param name="functions"></param>
        private static string ValidateDomains(List<ICoordinatesFunction<IOrdinate, IOrdinate>> functions)
        {
            if(functions.Count == 0) { return "No functions were passed in."; }
            double min = functions[0].Domain.Item1;
            double max = functions[0].Domain.Item2;

            foreach(ICoordinatesFunction<IOrdinate, IOrdinate> func in functions)
            {
                double funcMin = func.Domain.Item1;
                double funcMax = func.Domain.Item2;
                //if this function's domain falls within the previously established domain
                //then this list of funcs is invalid.
                if(funcMin < max)
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

        public static ICoordinatesFunction<IOrdinate, IOrdinate> Factory(List<ICoordinate<IOrdinate,IOrdinate>> coordinates)
        {
            List<ICoordinatesFunction<IOrdinate, IOrdinate>> functions = new List<ICoordinatesFunction<IOrdinate, IOrdinate>>();

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            List<IDistribution> ysDistributed = new List<IDistribution>();

            bool workingOnDistributedFunction = true;
            if(coordinates.Count>0)
            {
                workingOnDistributedFunction = coordinates[0].Y.IsDistributed;
            }

            foreach(ICoordinate<IOrdinate, IOrdinate> coord in coordinates)
            {
                if(coord.Y.IsDistributed)
                {
                    if (workingOnDistributedFunction)
                    {
                        xs.Add(coord.X.Value());
                        ysDistributed.Add((IDistribution)coord.Y);
                    }
                    else
                    {
                        //finish the not distributed func
                        functions.Add(Factory(xs, ys));
                        ClearLists(xs, ys, ysDistributed);
                        //start the distributed func
                        xs.Add(coord.X.Value());
                        ysDistributed.Add((IDistribution)coord.Y);

                    }
                }
                else
                {
                    if(workingOnDistributedFunction)
                    {
                        //finish the distributed func
                        functions.Add(Factory(xs, ysDistributed));
                        ClearLists(xs, ys, ysDistributed);
                        //start the not distributed func
                        xs.Add(coord.X.Value());
                        ys.Add(coord.Y.Value());
                    }
                    else
                    {
                        xs.Add(coord.X.Value());
                        ys.Add(coord.Y.Value());
                    }
                }
                //there will likely still be coordinates in the list at the end of the looping
                if(workingOnDistributedFunction)
                {
                    functions.Add(Factory(xs, ysDistributed));
                }
                else
                {
                    functions.Add(Factory(xs, ys));
                }
            }

            //now i have a list of functions
            if(functions.Count == 1)
            {
                //if there is only one function then return that function
                return functions[0];
            }
            else
            {
                //create a linked coordinates function with null interpolators that will have to get added later
                return Factory(functions, null);
            }
        }

        private static void ClearLists(List<double> xs, List<double> ys, List<IDistribution> ysDistributed)
        {
            xs.Clear();
            ys.Clear();
            ysDistributed.Clear();
        }


    }
}
