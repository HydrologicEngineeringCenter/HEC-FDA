using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public class ConstantSampler : ISampler
    {
        //I think the function will always be an OrdinateY function.
        public bool CanSample(ICoordinatesFunction coordinatesFunction)
        {
            return (typeof(CoordinatesFunctionConstants).IsAssignableFrom(coordinatesFunction.GetType()));
        }

        public IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability)
        {

            if (typeof(CoordinatesFunctionConstants).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                return (CoordinatesFunctionConstants)coordinatesFunction;
            }
            throw new ArgumentException("Could not sample the coordinates function.");

            //if (typeof(CoordinatesFunctionOrdinateYs).IsAssignableFrom(coordinatesFunction.GetType()))
            //{
            //    return ConvertOrdinateYsToConstant((CoordinatesFunctionOrdinateYs)coordinatesFunction);
            //}

            ////the can sample should have been checked before this method is called but i am being overly cautious
            //if (CanSample(coordinatesFunction))
            //{
            //    //now that we know that it is <double, double> cast all coordinates into new list
            //    List<ICoordinate<double, double>> constantCoords = new List<ICoordinate<double, double>>();
            //    foreach (ICoordinate<XType, YType> coord in coordinatesFunction.Coordinates)
            //    {
            //        constantCoords.Add((ICoordinate<double, double>)coord);
            //    }

            //    //return a new constant function
            //    return new CoordinatesFunctions.CoordinatesFunctionConstants(constantCoords);
            //}
            //throw new ArgumentException("Could not sample the coordinates function because the x and/or y values were of the wrong type.");
        }

        //private CoordinatesFunctionConstants ConvertOrdinateYsToConstant(CoordinatesFunctionOrdinateYs func)
        //{
        //    List<ICoordinate<Constant, Constant>> coords = new List<ICoordinate<Constant, Constant>>();
        //    foreach(ICoordinate<Constant, IOrdinate> coord in func.Coordinates)
        //    {
        //        coords.Add(new Coordinates.CoordinateConstants(coord.X, new Constant(coord.Y.Value())));
        //    }
        //    return new CoordinatesFunctionConstants(coords, func.Interpolator);
        //}

    }
}
