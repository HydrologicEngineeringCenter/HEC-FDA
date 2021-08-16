using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public class DistributionSampler : ISampler
    {
        public bool CanSample(ICoordinatesFunction coordinatesFunction)
        {
            bool canSample = false;
            if(typeof(CoordinatesFunctionVariableYs).IsAssignableFrom(coordinatesFunction.GetType()) ||
                typeof(DistributionFunction).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                canSample = true;
            }
            return (canSample);
        }

        public IFunction Sample(ICoordinatesFunction coordinatesFunction, double probability)
        {
            if (typeof(CoordinatesFunctionVariableYs).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                return new CoordinatesFunctionConstants(ConvertCoordinatesToConstants(coordinatesFunction.Coordinates, probability), coordinatesFunction.Interpolator);
            }
            else if(typeof(DistributionFunction).IsAssignableFrom(coordinatesFunction.GetType()))
            {
                if (coordinatesFunction.DistributionType == IOrdinateEnum.LogPearsonIII)
                {
                    IDistributedOrdinate distOrdinate = ((DistributionFunction)coordinatesFunction)._Distribution;
                    int sampleSize = distOrdinate.SampleSize;
                    int seed = (int)Math.Round(probability * 1000.0);
                    Random rng = new Random(seed);
                    List<double> flowValues = new List<double>();
                    for(int i = 0;i<sampleSize;i++)
                    {
                        flowValues.Add(distOrdinate.InverseCDF(rng.NextDouble()));
                    }
                    IDistribution lp3 = IDistributionFactory.FactoryFitLogPearsonIII(flowValues);
                    return IFunctionFactory.Factory(lp3);
                }
                else
                {
                    //todo: need to deal with any non LP3 distributions
                    return new CoordinatesFunctionConstants(coordinatesFunction.Coordinates, InterpolationEnum.Linear);
                }
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
