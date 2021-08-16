using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public class LinkedFunctionsSampler : ISampler
    {
        public bool CanSample(ICoordinatesFunction coordinatesFunction)
        {
            return (typeof(CoordinatesFunctionLinked).IsAssignableFrom(coordinatesFunction.GetType()));
        }

        public IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability)
        {
            /* 2 options for this method:
             *      (1) Loop over the coordinates, convert them to constants, then create a constant coordinates function
             *      (2) Loop over the functions, sample them to get a list of constant coordinate functions, then use the coordinates from them to create a constant coordinates function
             */
            //i could either loop over all coordinates and convert to constant and then create a constant coord func
            //or i could loop over all the funcitons and sample them to get a list of constant coord funcs and then
            //get all their coords to create a constant coord func.
            //todo: john what is the interpolation enum for a linked coordinates function. It has so many, one for each func and one in between each funcs.
            return new CoordinatesFunctionConstants(ConvertCoordinatesToConstants(coordinatesFunction.Coordinates, probability));
        }
        private List<ICoordinate> ConvertCoordinatesToConstants(List<ICoordinate> Coordinates, double p)
        {
            List<ICoordinate> coords = new List<ICoordinate>();
            foreach (ICoordinate coord in Coordinates)
            {
                if (coord.X.GetType() == typeof(Constant))
                {
                    if (coord.Y.GetType() == typeof(Distribution))
                    {
                        coords.Add(new CoordinateConstants(new Constant(coord.X.Value()), new Constant(((Distribution)coord.Y).InverseCDF(p))));

                    }
                    else if (coord.Y.GetType() == typeof(Constant))
                    {
                        coords.Add(new CoordinateConstants(new Constant(coord.X.Value()), new Constant(coord.Y.Value())));
                    }
                    else
                    {
                        throw new ArgumentException("LinkedFunctionsSampler encountered a Y value that was of an unknown type.");
                    }
                }
                else
                {
                    throw new ArgumentException("At least one coordinate did not have a constant X value.");
                }


            }
            return coords;
        }

    }
}
