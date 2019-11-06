using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public class ConstantSampler : ISampler
    {
        public bool CanSample(ICoordinatesFunction<IOrdinate, IOrdinate> coordinatesFunction)
        {
            return typeof(ICoordinatesFunction<double, double>).IsAssignableFrom(coordinatesFunction.GetType());
            //return (typeof(XType) == typeof(double) && typeof(YType) == typeof(double));
        }

        public IFunction Sample(ICoordinatesFunction<IOrdinate, IOrdinate> coordinatesFunction)
        {
            if(typeof(ICoordinatesFunction<double, double>).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                return new CoordinatesFunctions.CoordinatesFunctionConstants(((ICoordinatesFunction<Constant, Constant>)coordinatesFunction).Coordinates);
                 
            }
            throw new ArgumentException("Could not sample the coordinates function.");
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
    }
}
