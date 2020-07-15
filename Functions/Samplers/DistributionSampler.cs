using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    internal class DistributionSampler : ISampler
    {
        public bool CanSample(ICoordinatesFunction coordinatesFunction)
        {
            return (typeof(CoordinatesFunctionVariableYs).IsAssignableFrom(coordinatesFunction.GetType()));
        }

        public IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability)
        {
            if (typeof(CoordinatesFunctionVariableYs).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                return new CoordinatesFunctionConstants(ConvertCoordinatesToConstants(coordinatesFunction.Coordinates, probability), coordinatesFunction.Interpolator);
            }
            throw new ArgumentException("Could not sample the coordinates function.");
        }

        private List<ICoordinate> ConvertCoordinatesToConstants(List<ICoordinate> Coordinates, double p)
        {
            List<ICoordinate> coords = new List<ICoordinate>();
            foreach (ICoordinate coord in Coordinates)
            {
                coords.Add(new CoordinateConstants(new Constant(coord.X.Value()), new Constant(((Distribution)coord.Y).InverseCDF(p))));
            }
            return coords;
        }
    }
}
