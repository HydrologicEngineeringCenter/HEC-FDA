using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FunctionsTests.CoordinatesFunctions
{
    public static class CoordinateFunctionsTestData
    {
        //I can call the other two test class and use there theorydata
        public static List<ICoordinatesFunction<IOrdinate, IOrdinate>> GoodFunctions()
        {
            List<ICoordinatesFunction<IOrdinate, IOrdinate>> funcs = new List<ICoordinatesFunction<IOrdinate, IOrdinate>>();
            CoordinatesFunctionConstants func1 =
                new CoordinatesFunctionConstants(ScalarCoordinates(new double[] { 0, 1, 2, 3, 4 }, new double[] { 0, 1, 2, 3, 4 }));
            funcs.Add(func1);
            return funcs;
        }

        public static IImmutableList<ICoordinate<IOrdinate, IOrdinate>> ScalarCoordinates(double[] xs, double[] ys)
        {
            if(xs.Length != ys.Length)
            {
                throw new ArgumentException("xValues were a different length than the yValues.");
            }

            List<ICoordinate<IOrdinate, IOrdinate>> coords = new List<ICoordinate<IOrdinate, IOrdinate>>();
            for(int i =0;i<xs.Length; i++)
            {
                coords.Add((ICoordinate<IOrdinate, IOrdinate>)new CoordinateConstants(xs[i], ys[i]));
            }
            
            return ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(coords.ToArray());
        }
    }
}
