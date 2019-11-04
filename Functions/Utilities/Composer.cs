using Functions.CoordinatesFunctions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Utilities
{
    public static class Composer
    {

        //public static ICoordinatesFunction<double, double> Compose(ICoordinatesFunction<double, double> function1,
        //    ICoordinatesFunction<double, double> function2)
        //{





        //    ICoordinatesFunction<double, double> retval = new CoordinatesFunctions.CoordinatesFunctionConstants();
        //    return retval;
        //}

        #region Compose()
        private static ICoordinatesFunction<double, double> Compose(ICoordinatesFunction<double, double> func1, 
            ICoordinatesFunction<double, double> func2)
        {
            // Advance F Ordinate index until F[i].y >= G[0].x 
            int i = FirstX(func1,func2), I = func1.Coordinates.Count; // - 1;
            if (i == I) throw new InvalidOperationException(NoOverlapMessage(func1, func2));
            // Advance G Ordinate index until G[j].x >= F[0].y - then move back to j - 1.
            int j = FirstZ(func1, func2), J = func2.Coordinates.Count; // - 1;
            if (j == J) throw new InvalidOperationException(NoOverlapMessage(func1, func2));

            List<ICoordinate<double, double>> fog = new List<ICoordinate<double, double>>();
            while (!IsComplete(i, I, j, J,func1, func2)) // InOverlapping Portion
            {
                if (func1.Coordinates[i].Y == func2.Coordinates[j].X) //Matching ordinate
                {
                    fog.Add(ICoordinateFactory.Factory(new Constant(func1.Coordinates[i].X).Value(), new Constant(func2.Coordinates[j].Y).Value()));
                    i++;
                    j++;
                }
                else // Mismatching ordinate
                {
                    if (func1.Coordinates[i].Y < func2.Coordinates[j].X) // An X should be added and Z interpolated
                    {
                        // Add new ordinate to FoG if G allows interpolation between ordinates
                        if (!(func2.Interpolator == InterpolationEnum.NoInterpolation))
                            fog.Add(ICoordinateFactory.Factory(new Constant(func1.Coordinates[i].X).Value(), new Constant(func2.F(func1.Coordinates[i].Y)).Value()));
                        i++;
                    }
                    else // A Z should be added and X interpolated
                    {
                        // Add new ordinate to FoG if F allows Interpolation between ordinates
                        if (!(func1.Interpolator == InterpolationEnum.NoInterpolation))
                            fog.Add(ICoordinateFactory.Factory(new Constant(InverseF(func2.Coordinates[j].X, i - 1, func1)).Value(), new Constant(func2.Coordinates[j].Y).Value()));
                        j++;
                    }
                }
            }
            // Past overlapping area or at end of both functions
            return IFunctionFactory.Factory(fog, func2.Interpolator);
        }

        private static double InverseF(double y, int i, ICoordinatesFunction<double, double> function)
        {
            // TODO: IsFinite() IsNaN() check
            if (!Utilities.Validation.IsFinite(y)) throw new ArgumentOutOfRangeException(string.Format("The specified y value: {0} is not finite.", y));
            // TODO: OnRange()  check  - so this works with decreasing functions.
            if (!IsOnRange(y, function)) throw new ArgumentOutOfRangeException(string.Format("The specified y values: {0} is invalid because it is not on the domain of the inverse coordinates function [{1}, {2}] (e.g. range of coordinates function).",
                y, function.Coordinates[0].Y, function.Coordinates[function.Coordinates.Count - 1].Y));
            if (y < function.Coordinates[i].Y || y > function.Coordinates[i + 1].Y) throw new ArgumentException(
                string.Format("The InverseF operation could not be completed because the specified y: {0} is not on the implicitly defined range: [{1}, {2}].",
                y, function.Coordinates[i].Y, function.Coordinates[i + 1].Y));
            if (function.Coordinates[i + 1].Y == y) return function.Coordinates[i + 1].Y;
            else return 0;// InverseInterpolationFunction(i, y);
        }

        private static bool IsOnRange(double yValue, ICoordinatesFunction<double, double> function)
        {
            //todo: john, can we be positive that the coordinates are in order here?
            bool retval = false;
            if (function.Coordinates.Count > 0)
            {
                double min = GetMinRange(function);
                double max = GetMaxRange(function);
                retval = yValue >= min && yValue <= max;
            }
            return retval;
        }
        private static double GetMinRange(ICoordinatesFunction<double, double> function)
        {
            return function.Coordinates[0].Y;
        }
        private static double GetMaxRange(ICoordinatesFunction<double, double> function)
        {
            return function.Coordinates[function.Coordinates.Count - 1].Y;
        }
        private static int FirstX(ICoordinatesFunction<double, double> func1,
            ICoordinatesFunction<double, double> func2)
        {
            int i = 0, I = func1.Coordinates.Count;
            while (func1.Coordinates[i].Y < func2.Coordinates[0].X)
            {
                i++;
                if (i == I) break;
            }
            return i;
        }
        private static int FirstZ(ICoordinatesFunction<double, double> func1,
            ICoordinatesFunction<double, double> func2)
        {
            int j = 0, J = func2.Coordinates.Count; //- 1;
            while (func2.Coordinates[j].X < func1.Coordinates[0].Y)
            {
                j++;
                if (j == J) break;
            }
            return j;
        }
        private static bool IsComplete(int i, int I, int j, int J, ICoordinatesFunction<double, double> func1,
            ICoordinatesFunction<double, double> func2)
        {
            return (IsFinalIndex(i, I, j, J) || IsXOffOverlap(i, J,func1, func2) || IsZOffOverlap(I, j,func1, func2)) ? true : false;
        }
        private static bool IsXOffOverlap(int i, int J, ICoordinatesFunction<double, double> func1,
            ICoordinatesFunction<double, double> func2)
        {
            return func1.Coordinates[i].Y > func2.Coordinates[J - 1].X ? true : false;
        }
        private static bool IsZOffOverlap(int I, int j, ICoordinatesFunction<double, double> func1,
            ICoordinatesFunction<double, double> func2)
        {
            return func1.Coordinates[I - 1].Y < func2.Coordinates[j].X ? true : false;
        }
        private static bool IsFinalIndex(int i, int I, int j, int J) => (i == I || j == J) ? true : false;
        private static string NoOverlapMessage(ICoordinatesFunction<double, double> func1, ICoordinatesFunction<double, double> func2)
        {
            return string.Format("The functional composition operation could not be performed. The range of F: [{0}, {1}] in the composition equation F(G(x)) does not overlap the domain of G: [{2}, {3}].",
                GetMinRange(func1), GetMaxRange(func1), func2.Domain.Item1, func2.Domain.Item2);
        }
        #endregion

    }
}
